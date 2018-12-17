using UnityEngine;
using Vuforia;

namespace AlienMaker
{
    public class CustomTrackingBehaviour : MonoBehaviour, ITrackableEventHandler
    {
        protected TrackableBehaviour mTrackableBehaviour;

        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        void OnDestroy()
        {
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.UnregisterTrackableEventHandler(this);
            }
        }

        public void OnTrackableStateChanged(
            TrackableBehaviour.Status previousStatus,
            TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
                Manager.Instance.registerTrackedTarget(gameObject.GetComponent<Part>());

            }
            else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                     newStatus == TrackableBehaviour.Status.NOT_FOUND)
            {
                Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
                Manager.Instance.unregisterTrackedTarget(gameObject.GetComponent<Part>());
            }
            else
            {
                // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
                // Vuforia is starting, but tracking has not been lost or found yet
                // Call OnTrackingLost() to hide the augmentations
                // OnTrackingLost();
            }
        }
    }
}
