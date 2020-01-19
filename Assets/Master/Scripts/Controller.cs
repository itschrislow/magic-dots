namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Controller : MonoBehaviour
    {
        // A prefab for visualizing an TrackingImage.
        public Visualizer flower1;
        public Visualizer plane2;

        // The overlay containing the fit to scan user guide.
        public GameObject FitToScanOverlay;

        private Dictionary<int, Visualizer> m_Visualizers = new Dictionary<int, Visualizer>();

        private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

        // Update is called once per frame
        void Update()
        {
#if !UNITY_EDITOR
            // Check that motion tracking is tracking.
            if (NRFrame.SessionStatus != SessionState.Tracking)
            {
                return;
            }
#endif
            // Get updated augmented images for this frame.
            NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

            // Create visualizers and anchors for updated augmented images that are tracking and do not previously
            // have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempTrackingImages)
            {
                Visualizer visualizer = null;
                switch (image.GetDataBaseIndex()) {
                    case 0:
                        m_Visualizers.TryGetValue(image.GetDataBaseIndex(), out visualizer);
                        if (image.GetTrackingState() == TrackingState.Tracking && visualizer == null)
                        {
                            // based on which image is detected, display correct prefab
                            // Create an anchor to ensure that NRSDK keeps tracking this augmented image.
                            // Is img tracking persistant? will prefab vanish if cannot see book
                            visualizer = (Visualizer)Instantiate(flower1, image.GetCenterPose().position, image.GetCenterPose().rotation);
                            visualizer.pageFlower = image;
                            visualizer.transform.parent = transform;
                            m_Visualizers.Add(image.GetDataBaseIndex(), visualizer);

                            NRDebugger.Log("Found flower1.jpg!");

                        }
                        else if (image.GetTrackingState() == TrackingState.Stopped && visualizer != null)
                        {
                            m_Visualizers.Remove(image.GetDataBaseIndex());
                            Destroy(visualizer.gameObject);
                        }
                        break;
                    case 1:
                        m_Visualizers.TryGetValue(image.GetDataBaseIndex(), out visualizer);
                        if (image.GetTrackingState() == TrackingState.Tracking && visualizer == null)
                        {
                            // based on which image is detected, display correct prefab
                            // Create an anchor to ensure that NRSDK keeps tracking this augmented image.
                            // Is img tracking persistant? will prefab vanish if cannot see book
                            visualizer = (Visualizer)Instantiate(plane2, image.GetCenterPose().position, image.GetCenterPose().rotation);
                            visualizer.pageFlower = image;
                            visualizer.transform.parent = transform;
                            m_Visualizers.Add(image.GetDataBaseIndex(), visualizer);

                            NRDebugger.Log("Found plane2.jpg!");

                        }
                        else if (image.GetTrackingState() == TrackingState.Stopped && visualizer != null)
                        {
                            m_Visualizers.Remove(image.GetDataBaseIndex());
                            Destroy(visualizer.gameObject);
                        }
                        break;
                    default:
                        break;
                }
                FitToScanOverlay.SetActive(false);
            }
        }
    }
}
