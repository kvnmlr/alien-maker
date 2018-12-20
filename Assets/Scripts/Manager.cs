using System.Collections.Generic;
using UnityEngine;
using Vuforia;

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

        // List of all available parts, assigned in Unity Editor
        public Part[] parts;

        // List of parts that are needed for the current task
        private List<Part> activeParts = new List<Part>();

        // List of all parts currently tracked by Vuforia
        private HashSet<Part> trackedParts = new HashSet<Part>();

        // List of all parts connected to the torso and the torso itself
        private HashSet<Part> connectedParts = new HashSet<Part>();

        // Game setup as selected from the main menu
        private Settings.GameSetup setup;

        // Currently active task from the current game setup
        private Settings.Task currentTask;

        // Number of finished tasks in the current task
        private int finishedTasks = 0;



        /* Adds a new part to the list of currently tracked parts */
        public void registerTrackedTarget(Part part)
        {
            trackedParts.Add(part);
            printTrackedObjects();
        }

        /* Removes a part from the list of currently tracked parts */
        public void unregisterTrackedTarget(Part part)
        {
            trackedParts.Remove(part);
            printTrackedObjects();
            connectionStateChanged();
        }

        /* Callback when two parts are successfully connected */
        public void connectionStateChanged()
        {
            if (setup == null || currentTask == null)
            {
                return;
            }

            if (isSolution())
            {
                Debug.Log("Finished");
                finishedTasks++;
                nextTask();
            }
            else
            {
                Debug.Log("Not yet finished");
            }

            // Mark the parts that are successfully connected in the UI
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

        /* Checks whether the currently connected parts are a solution */
        private bool isSolution()
        {
            connectedParts = new HashSet<Part>();
            int countTorso = 0;
            int countHeads = 0;
            int countArms = 0;
            int countLegs = 0;

            // Add torso and all parts connected to torso to list of connected parts
            foreach (Part part in trackedParts)
            {
                if (part.typ == Type.Torso)
                {
                    connectedParts.Add(part);
                    countTorso++;

                    foreach (Part connected in part.connectedParts)
                    {
                        connectedParts.Add(connected);
                        if (connected.typ.Equals(Type.Head))
                        {
                            countHeads++;
                        }
                        if (connected.typ.Equals(Type.LeftArm) || connected.typ.Equals(Type.RightLeg))
                        {
                            countArms++;
                        }
                        if (connected.typ.Equals(Type.LeftLeg) || connected.typ.Equals(Type.RightLeg))
                        {
                            countLegs++;
                        }
                    }
                }
            }

            // Check if connected parts match current task
            bool isSolution = true;
            if (countTorso != currentTask.torso ||
                countHeads != currentTask.heads ||
                countArms != currentTask.arms ||
                countLegs != currentTask.legs)
            {
                isSolution = false;
            }

            return isSolution;
        }

        /* Starts the next unfinished task from the current setup */
        private void nextTask()
        {
            if (setup.tasks.Count > finishedTasks)
            {
                currentTask = setup.tasks[finishedTasks];
                string group = currentTask.group;

                // Reset the list of parts needed for the current task
                activeParts = new List<Part>();
                foreach (Part part in parts)
                {
                    if (part.group.Equals(group))
                    {
                        activeParts.Add(part);
                    }
                }

                // Disable all Vuforia datasets except the one needed for the current task
                string dataset = currentTask.dataset;
                ObjectTracker imageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
                foreach (DataSet ds in imageTracker.GetDataSets())
                {
                    // determine whether the dataset is alreay active
                    bool isActive = false;
                    foreach (DataSet other in imageTracker.GetActiveDataSets())
                    {
                        if (ds.Equals(other))
                        {
                            isActive = true;
                        }
                    }

                    if (ds.Path.Contains(dataset) && !isActive)
                    {
                        imageTracker.ActivateDataSet(ds);
                        Debug.Log(ds.Path + " activated");
                    }
                    else if (!ds.Path.Contains(dataset) && isActive)
                    {
                        imageTracker.DeactivateDataSet(ds);
                        Debug.Log(ds.Path + " deactivated");
                    }
                }
            }

            Debug.Log(activeParts.Count + " active parts");
        }

        /* Loads a game setup with the given id and starts the first task from it */
        public void startGame(int id)
        {
            this.setup = null;
            foreach (Settings.GameSetup s in Settings.Instance.setups)
            {
                if (s.id == id)
                {
                    this.setup = s;
                }
            }

            if (this.setup == null)
            {
                Debug.Log("Could not find game setup with id " + id);
                return;
            }

            finishedTasks = 0;
            nextTask();
            Debug.Log(setup.name + " now runnung");
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
    }
}
