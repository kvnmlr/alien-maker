using System.Collections;
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

        private HashSet<Part> trackedParts = new HashSet<Part>();

        public void registerTrackedTarget(Part part)
        {
            trackedParts.Add(part);
            printTrackedObjects();
        }

        public void unregisterTrackedTarget(Part part)
        {
            trackedParts.Remove(part);
            printTrackedObjects();
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
        }

        public bool isFinished()
        {
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

                    foreach (Part connected in part.connectedParts)
                    {
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

        private void printTrackedObjects()
        {
            Debug.Log("Tracked: ");
            foreach (Part p in trackedParts)
            {
                Debug.Log(p.gameObject.name);
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            

        }
    }
}
