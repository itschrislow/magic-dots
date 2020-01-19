using NRKernal;
using UnityEngine;
using System.Collections;

public class Visualizer : MonoBehaviour
{
    // The TrackingImage to visualize.
    public NRTrackableImage pageFlower;

    // A model for the page to place when an image is detected.
    public GameObject sceneFlower;

    // Update is called once per frame
    void Update()
    {
        // if image tracker not found
        if (pageFlower == null || pageFlower.GetTrackingState() != TrackingState.Tracking)
        {
            // do not display scene
            sceneFlower.SetActive(false);
            return;
        }

        // if found, set position
        var center = pageFlower.GetCenterPose();
        transform.position = center.position;
        transform.rotation = center.rotation;

        // display scene
        sceneFlower.SetActive(true);
    }
}
