using UnityEngine;

public class CustomTrackableEventHandlerHome : DefaultTrackableEventHandler
{
    bool isScanned = false;
    [SerializeField] PlacementScript placeScript = null;

    #region OVERRIDED_METHODS

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
        if (!isScanned)
        {
            isScanned = true;
            placeScript.EnableButton();
        }
    }

    protected override void OnTrackingLost()
    {
        //base.OnTrackingLost();
    }

    #endregion
}
