using UnityEngine;

public class CustomTrackableEventHandler : DefaultTrackableEventHandler
{
    bool isScanned = false;
    [SerializeField] PlacementScript placeScript = null;
    [SerializeField] GameObject objectToActivate = null;

    #region OVERRIDED_METHODS

    override protected void Start()
    {
        base.Start();

        objectToActivate.SetActive(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnTrackingFound()
    {
        //base.OnTrackingFound();

        objectToActivate.SetActive(true);

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
