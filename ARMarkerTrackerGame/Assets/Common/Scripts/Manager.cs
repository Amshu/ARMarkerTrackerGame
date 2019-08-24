using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // Creating a singleton
    private static Manager _instance;
    public static Manager Instance { get { return _instance; } }

    private void Awake()
    {
        // Singleton Code
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        // Listen to dragon events
        Dragon.eventDragon += EHandler_Dragon;
    }

    // State Machine Variables
    private static EManager _CurrState = 0;
    private void SetState(EManager state)
    {
        if (state == _CurrState)
            return;

        HandleStateChange(state);
    }
    private void HandleStateChange(EManager changedState)
    {
        EManager oldState = _CurrState;
        EManager newState = changedState;

        switch (changedState)
        {
            case EManager.DragonSpawn:
                Debug.Log("Manager state -> DragonSpawn");
                
            break;

            case EManager.DragonReady:
                Debug.Log("Manager state -> DragonReady");
            break;
        }
    }

    private bool Timer(float delayTime)
    {
        float timer = delayTime;
        while (timer > 0)
        {
            //Debug.Log(timer);
            timer -= Time.deltaTime;
        }
        return true;
    }

    private void EHandler_Dragon(EDragon dragonState)
    {
        SetState((EManager)dragonState);
    }
}
