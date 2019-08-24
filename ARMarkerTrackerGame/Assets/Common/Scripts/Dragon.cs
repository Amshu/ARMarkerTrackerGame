using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Dragon : MonoBehaviour
{
    #region Setting event delegate for manager

    public delegate void DragonEvents(EDragon state); // Declare delegate
    public static event DragonEvents eventDragon; // Create event

    private static void RaiseDragonEvent(EDragon i)
    {
        if (eventDragon == null)
        {
            Debug.Log("No Listeners for DragonEvents");
            return;
        }    
        eventDragon(i);
    }

    #endregion

    [SerializeField] Camera ArCam = null;

    // Platform Variables
    [Space][Space]
    [SerializeField] Vector3 _platOffset = Vector3.zero;
    [SerializeField] GameObject _platButton = null;
    
    [SerializeField] List<GameObject> PlatformsJumpedOnBefore = null;
    [SerializeField] GameObject CurrentLocation = null;

    // Parabola Controller members
    [Space][Space]
    [SerializeField] float _paraSpeed = 0f;
    [SerializeField] bool _paraActive = false;
    [SerializeField] ParabolaController _paracont = null;
    [SerializeField] GameObject _paraStart = null;
    [SerializeField] GameObject _paraEnd = null;
    [SerializeField] GameObject _paraMedian = null;
    [SerializeField] Vector3 _paraYOffset = Vector3.zero;
    
    //UI
    [Space][Space]
    [SerializeField] TextMesh _txtSpeech = null;
    [SerializeField] Text _txtWin = null;

    //Sound
    [Space]
    [Space]
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] AudioClip _audioJump = null;
    [SerializeField] AudioClip _audioTalk = null;
    [SerializeField] AudioClip _audioFall = null;
    [SerializeField] AudioClip _audioWin = null;

    private void Start()
    {
        _paracont.StopFollow();
        audioSource.clip = _audioFall;
        _txtWin.gameObject.SetActive(false);
        audioSource.Play();
        //_platButton.SetActive(false);
    }

    private void OnEnable()
    {
        RaiseDragonEvent(EDragon.Enabled);
    }

    public void DragonReady()
    {
        Debug.Log("DragonReady event triggering");
        RaiseDragonEvent(EDragon.Ready);

        StartCoroutine(Talk(0));
    }

    private void Update()
    {
        if (_paraActive)
        {
            transform.LookAt(_paraStart.transform);

            if (_paracont.Animation == true && _paraActive)
            {
                Debug.Log("------------------------Parabola Finished");
                _paraActive = false;
                _paracont.StopFollow();
            }
        }
        else
        {
            transform.LookAt(ArCam.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggered");
        // If it is the same location
        if (CurrentLocation == other.gameObject)
            return;

        // If not reset CurrentLocation
        CurrentLocation = other.gameObject;

        if (other.transform.tag == "Respawn")
        {
            Debug.Log("OnTriggered Finish");

            if(TeleportToPlatform(other.gameObject))
                StartCoroutine(Talk(11));
        }

        if(other.transform.tag == "Finish")
        {
            Debug.Log("OnTriggered Platform");
            ReachFinish(other.transform.position);
        }
    }

    private bool TeleportToPlatform(GameObject nextPlatform)
    {
        if (PlatformsJumpedOnBefore.Contains(nextPlatform))
        {
            Debug.Log("--------------Platform been before");
            return false;
        }

        Debug.Log("--------------new platform");
        PlatformsJumpedOnBefore.Add(nextPlatform);


        /*
        // Change positions of parabola script gameobjects.
        // Start
        _paraStart.transform.position = nextPlatform.transform.position + _platOffset;
        //_paraStart.transform.rotation = nextPlatform.transform.rotation;
        //End
        _paraEnd.transform.position = transform.position;
        //_paraEnd.transform.rotation = transform.rotation;
        // Median
        _paraMedian.transform.position = ((_paraStart.transform.position 
                                    + _paraEnd.transform.position) / 2) + _paraYOffset;

        _paracont.Speed = _paraSpeed;
        _paracont.FollowParabola();  
        */

        transform.position = nextPlatform.transform.position + _platOffset;

        return true;
    }

    void ReachFinish(Vector3 FinishLocation)
    {
        Debug.Log("OnTriggered Finish Function");
        this.transform.position += new Vector3(0f, 2f, 0f);

        audioSource.clip = _audioWin;
        audioSource.Play();
        _txtWin.gameObject.SetActive(true);
        _platButton.SetActive(false);
        _platButton.SetActive(false);
    }

    IEnumerator Talk(int state)
    {
        Debug.Log("Talk coroutine working");
        switch (state)
        {
            case 0:
                audioSource.clip = _audioTalk;
                Debug.Log("Talk coroutine working - case 1");
                yield return new WaitForSeconds(2.0f);
                audioSource.Play();
                _txtSpeech.text = "Take me home";
                yield return new WaitForSeconds(2.0f);
                _txtSpeech.text = "Need Platforms";
                yield return new WaitForSeconds(3.0f);
                _txtSpeech.text = "";
                _txtSpeech.gameObject.transform.parent.gameObject.SetActive(false);
                break;

            case 11:
                Debug.Log("Jumping to platform");
                audioSource.clip = _audioJump;
                audioSource.Play();
                break;
        }
    }
}
