using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementScript : MonoBehaviour
{
    [SerializeField] float Cooldown = 0.0f;

    [SerializeField] GameObject Platform;
    [SerializeField] float SpawnOffset;
    [SerializeField] Button PlaceButton;

    [SerializeField] bool IsButtonEnabled;

    private void Start()
    {
        PlaceButton.enabled = false;
        PlaceButton.image.color = Color.red;
    }

    // Update is called once per frame
    public void ButtonDown()
    {
        Place();
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(Cooldown);
        // TODO: Implement cooldown functionality
    }

    void Place()
    {
        Vector3 PlaceLoc = transform.position + (transform.forward * SpawnOffset);
        GameObject Parent = GameObject.FindGameObjectWithTag("Platforms");
        Instantiate(Platform, PlaceLoc, Quaternion.identity, Parent.transform);
    }

    public void EnableButton()
    {
        if (IsButtonEnabled)
            return;

        PlaceButton.enabled = true;
        PlaceButton.image.color = Color.green;
    }
}
