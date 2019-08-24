﻿using UnityEngine;

public class CustomTrackableEventHandler : DefaultTrackableEventHandler
{
    bool isScanned = false;
    [SerializeField] PlacementScript placeScript = null;
    [SerializeField] GameObject objectToActivate = null;

    #region OVERRIDED_METHODS
    private void Awake()
    {
        objectToActivate.SetActive(false);
    }

    override protected void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnTrackingFound()
    {
       // base.OnTrackingFound();

        objectToActivate.SetActive(true);
    }

    protected override void OnTrackingLost()
    {
        //base.OnTrackingLost();
    }

    #endregion
}
