using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace AlienMaker
{
    public class Manager : Singleton<Manager>
    {
        public enum Type
        {
            Horn,
            Arm,
            Torso,
            Leg,
        }

        // List of all available parts, assigned in Unity Editor
        public Part[] parts;

        public Animator liloJump;

        public Menu menuUI;

        public GameObject taskUI;

        public KeyCode menuKey = KeyCode.M;

        public KeyCode taskKey = KeyCode.T;

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

        private int connectedPartsMax = 0;

        public bool currentSetupFinished = false;



        /* Adds a new part to the list of currently tracked parts */
        public void registerTrackedTarget(Part part)
        {
            trackedParts.Add(part);
            printTrackedObjects();
            connectionStateChanged();
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
        }

        /* Checks whether the currently connected parts are a solution */
        private bool isSolution()
        {
            Debug.Log("Tracked: " + trackedParts.Count);
            foreach(Part t in trackedParts)
            {
                Debug.Log(t.name);
            }

            connectedParts = new HashSet<Part>();
            int countTorso = 0;
            int countHorns = 0;
            int countArms = 0;
            int countLegs = 0;

            // Add torso and all parts connected to torso to list of connected parts
            foreach (Part part in trackedParts)
            {
                if (part.typ == Type.Torso)
                {
                    Debug.Log("Torso in solution");
                    connectedParts.Add(part);
                    countTorso++;

                    foreach (Part connected in part.connectedParts)
                    {
                        connectedParts.Add(connected);
                        if (connected.typ.Equals(Type.Horn))
                        {
                            Debug.Log("Horns in solution");
                            countHorns++;
                        }
                        if (connected.typ.Equals(Type.Arm))
                        {
                            Debug.Log("Arms in solution");
                            countArms++;
                        }
                        if (connected.typ.Equals(Type.Leg))
                        {
                            Debug.Log("Legs in solution");
                            countLegs++;
                        }
                    }
                }
            }
            Debug.Log("Conntected: " + connectedParts.Count);
            foreach (Part c in connectedParts)
            {
                Debug.Log(c.name);
            }
            foreach (Part p in parts)
            {
                p.showColored(connectedParts.Contains(p));
            }

            if (connectedParts.Count > connectedPartsMax)
            {
                SoundManager.Instance.playSpitze();
                connectedPartsMax = connectedParts.Count;
            }

            // Check if connected parts match current task
            bool isSolution = true;
            if (countTorso != currentTask.torso ||
                countHorns != currentTask.horns ||
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
            Debug.Log("Next task: " + (finishedTasks + 1) + " / " + setup.tasks.Count);
            if (setup.tasks.Count > finishedTasks)
            {
                currentTask = setup.tasks[finishedTasks];

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

                SoundManager.Instance.playFile("Sounds/Tasks/" + currentTask.audio);

                Debug.Log("Next Task: " + currentTask.ToString());
                Debug.Log(parts.Length + " active parts");
            } else
            {
                currentSetupFinished = true;
            }
        }

        /* Loads a game setup with the given id and starts the first task from it */
        public void startGame(int id)
        {
            connectedPartsMax = 0;
            currentSetupFinished = false;
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

            Debug.Log(setup.ToString() + " now runnung");

            menuUI.gameObject.SetActive(false);
            taskUI.gameObject.SetActive(false);
            finishedTasks = 0;
            liloJump.Play(0);
            StartCoroutine(Example());
        }

        private IEnumerator Example()
        {
            yield return new WaitForSeconds(1);
            SoundManager.Instance.playFile("Sounds/" + setup.audio);
            yield return new WaitForSeconds(24);
            nextTask();
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

        public void Update()
        {
            Screen.SetResolution(1920, 1080, true);
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (currentSetupFinished) {
                    currentSetupFinished = false;
                    menuUI.gameObject.SetActive(true);
                    taskUI.gameObject.SetActive(true);
                }
            }
        }
    }
}
