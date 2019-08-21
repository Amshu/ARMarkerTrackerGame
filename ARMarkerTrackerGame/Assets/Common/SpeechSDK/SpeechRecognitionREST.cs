using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System;

public class SpeechRecognitionREST : MonoBehaviour
{

    string subscriptionKey = "b681a6431d8e4234a84f417fac5c8922";
    string token;

    [SerializeField] Text responseText = null;
    [SerializeField] Text feedback = null;
    [SerializeField] AudioClip soundClip = null;

    // length of any recording sent. 10s is the limit
    int recordDuration = 5;

    static bool TrustCertificate(object sender, X509Certificate x509Certificate,
        X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // assuming all certificates are accepted
        return true;
    }

    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
    }

    // Not really needed, but useful to test access to service. 
    public void Authentication()
    {
        // Unity webforms do not handle the certificates required for https servers
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create
            ("https://westus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1");
        request.ContentType = "application/x-www-form-urlencoded";
        request.Method = "POST";
        request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;
        request.ContentLength = 0;

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Debug.Log("Token received: " + responseString);

        token = responseString;

        responseText.text = token;
    }

    public void SpeechToText(byte[] wavData)
    {
        responseText.text = "Sending Audio";
        // Send the request to the service
        string fetchUri = "https://westus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fetchUri + "?language=en-US&format=detailed");
        request.ContentType = "application/x-www-form-urlencoded";
        request.Method = "POST";
        request.ContentType = "audio/wav; codecs=audio/pcm; samplerate=16000";
        request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;

        // Read a wav file off the filesystem and send that to the service. Note that the paths
        // used may not working when packaged for a mobile platform. This is useful for testing
        // under controlled and repeatable conditions (same input file each time). 

        Stream rs = request.GetRequestStream();
        //FileStream fileStream = new FileStream(Application.dataPath + "/SpeechRecognition/Sound/untitled.wav", FileMode.Open, FileAccess. Read);

        //byte[] buffer = new byte[4096];
        //int bytesRead = 0;
        //while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
        //    rs.Write(buffer, 0, bytesRead);
        //}
        //fileStream.Close();
        //rs.Close();

        //Stream rs = request.GetRequestStream();
        rs.Write(wavData, 0, wavData.Length);
        rs.Close();

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //responseText.text = "Response from service: " + responseString;

        Debug.Log("Response from service: " + responseString);
        string result = resultParser(responseString);
        responseText.text = "Your Answer: " + result;

        if(result.IndexOf("Avada cadabra") != -1)
        {
            feedback.text = "Your enemies lie in dust!";
            feedback.color = Color.green;
            feedback.gameObject.SetActive(true);
            StartCoroutine(PlaySound());
        }
        else
        {
            feedback.text = "Try Again";
            feedback.color = Color.red;
            feedback.gameObject.SetActive(true);
        }
    }

    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(5);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.Play();
    }

    private IEnumerator recordAudio()
    {
        // Set the microphone recording. Service requires 16 kHz sampling.  
        AudioClip audio = Microphone.Start(null, false, recordDuration, 16000);
        yield return new WaitForSeconds(recordDuration);
        Microphone.End(null);

        // Play the recording back, to validate it was recorded correctly. 
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.Play();

        // Convert it to a wav file, and upload to the service. 
        byte[] wavData = ConvertToWav(audio);
        SpeechToText(wavData);
    }

    public void Trigger()
    {
        //Authentication ();
        StartCoroutine(recordAudio());
    }

    // Remaining functions adapted from: https://gist.github.com/darktable /2317063 
    const int HEADER_SIZE = 44;
    static byte[] ConvertToWav(AudioClip clip)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);
        Int16[] intData = new Int16[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        Byte[] bytesData = new Byte[HEADER_SIZE + samples.Length * 2];
        //bytesData array is twice the size of 
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16
        WriteHeader(bytesData, clip);

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, HEADER_SIZE + i * 2);
        }

        return bytesData;
    }

    static void WriteHeader(byte[] bytesData, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        riff.CopyTo(bytesData, 0);
        Byte[] chunkSize = BitConverter.GetBytes(HEADER_SIZE + clip.samples * 2 - 8);
        chunkSize.CopyTo(bytesData, 4);
        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        wave.CopyTo(bytesData, 8);
        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fmt.CopyTo(bytesData, 12);
        Byte[] subChunk1 = BitConverter.GetBytes(16);
        subChunk1.CopyTo(bytesData, 16);

        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        audioFormat.CopyTo(bytesData, 20);
        Byte[] numChannels = BitConverter.GetBytes(channels);
        numChannels.CopyTo(bytesData, 22);
        Byte[] sampleRate = BitConverter.GetBytes(hz);
        sampleRate.CopyTo(bytesData, 24);
        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
        // sampleRate * bytesPerSample*number of channels, here 44100*2*2 
        byteRate.CopyTo(bytesData, 28);

        UInt16 blockAlign = (ushort)(channels * 2);
        BitConverter.GetBytes(blockAlign).CopyTo(bytesData, 32);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        bitsPerSample.CopyTo(bytesData, 34);
        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        datastring.CopyTo(bytesData, 36);
        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        subChunk2.CopyTo(bytesData, 40);
    }
    
    string resultParser(string input)
    {
        // String to be searched - "Display":"
        string wordToBeFound = "Display";

        // Get location of the word
        int startLoc = input.IndexOf(wordToBeFound);
        // Check if word exists in the input
        if (startLoc != -1)
        {
            // Offset by three spaces to ignore quotations and colon and get the whole string
            string temp = input.Substring(startLoc + 10);
            Debug.Log("------First cut: " + temp);

            // Check for the next location of '}'
            int endLoc = temp.IndexOf("}");

            // Cut the string to get the required string
            string result = temp.Substring(0, endLoc-2);
            Debug.Log("------Result: " + result);

            return result;
        }
        return "Error";
    }
}