using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    [SerializeField] Vector3 PlatformOffset;
    [SerializeField] Vector3 FinishOffset;

    [SerializeField] GameObject CurrentLocation;

    [SerializeField] GameObject WinTxt;

    // Start is called before the first frame update
    void Start()
    {
        WinTxt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            TeleportToPlatform(other.transform.position);
        }

        if(other.transform.tag == "Finish")
        {
            Debug.Log("OnTriggered Platform");
            ReachFinish(other.transform.position);
        }
    }

    void TeleportToPlatform(Vector3 NextLocation)
    {
        transform.position = NextLocation + PlatformOffset;
    }

    void ReachFinish(Vector3 FinishLocation)
    {
        Debug.Log("OnTriggered Finish Function");
        transform.position = FinishLocation + FinishOffset;

        WinTxt.SetActive(true);

        //TODO: Disable collision checks once finish is reached
    }
}
