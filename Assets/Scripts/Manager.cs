using System.Collections.Generic;
using UnityEngine;

namespace AlienMaker
{
    public class Manager : Singleton<Manager>
    {
        public enum Type
        {
            Head,
            LeftArm,
            RightArm,
            Torso,
            LeftLeg,
            RightLeg,
        }
        public Part[] parts;

        private HashSet<Part> trackedParts = new HashSet<Part>();
        private HashSet<Part> connectedParts = new HashSet<Part>();

        public void registerTrackedTarget(Part part)
        {
            trackedParts.Add(part);
            printTrackedObjects();
        }

        public void unregisterTrackedTarget(Part part)
        {
            trackedParts.Remove(part);
            printTrackedObjects();
            connectionStateChanged();
        }

        public void connectionStateChanged()
        {
            if (isFinished())
            {
                Debug.Log("Finished");
            } else
            {
                Debug.Log("Not yet finished");
            }

            string filename = takeScreenshot();
            uploadScreenshot(filename);

            foreach (Part p in parts)
            {
                if (p.text != null)
                {
                    p.text.color = new Color(1, 1, 1);
                }
            }

            foreach (Part p in connectedParts)
            {
                if (p.text != null)
                {
                    p.text.color = new Color(0, 1, 0);
                }
            }
        }

        private bool isFinished()
        {
            connectedParts = new HashSet<Part>();

            // Check if the torso is tracked
            foreach (Part part in trackedParts)
            {
                if (part.typ == Type.Torso)
                {
                    // Torso is visible, check connected parts
                    if (part.connectedParts.Count < 5)
                    {
                        // return false;
                    }

                    connectedParts.Add(part);

                    foreach (Part connected in part.connectedParts)
                    {
                        connectedParts.Add(connected);

                        if (connected.typ.Equals(Type.Head))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        private string takeScreenshot()
        {
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int currentTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            string filename = currentTime + ".png";
            ScreenCapture.CaptureScreenshot(filename);
            return filename;
        }

        private bool uploadScreenshot(string filename)
        {
            return true;
        }

        private void printTrackedObjects()
        {
            Debug.Log("Tracked: ");
            foreach (Part p in trackedParts)
            {
                Debug.Log(p.gameObject.name);
            }
        }

        void Start()
        {}

        void Update()
        {}
    }
}
