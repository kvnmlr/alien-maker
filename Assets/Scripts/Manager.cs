using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

namespace AlienMaker
{
    [Serializable]
    public class IMGUR
    {
        public Data data; 
    }

    [Serializable]
    public class Data
    {
        public string id;
    }

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
        public GameObject arCamera;
        public GameObject regularCamera;
        public GameObject QRplane;
        public GameObject QRarea;
        public GameObject download;
        public GameObject ScreenshotCamera;
        public GameObject linkText;

        public Part[] parts;

        public Animator liloJump;

        public Animation monsterFly;

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
            //printTrackedObjects();
            connectionStateChanged();
        }

        /* Removes a part from the list of currently tracked parts */
        public void unregisterTrackedTarget(Part part)
        {
            trackedParts.Remove(part);
            //printTrackedObjects();
            connectionStateChanged();
        }

        /* Callback when two parts are successfully connected */
        public void connectionStateChanged()
        {
            Debug.Log("connectionStateChanged");
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
        }

        /* Checks whether the currently connected parts are a solution */
        private bool isSolution()
        {
            if (!arCamera.activeSelf)
            {
                Debug.Log("AR Camera is not active, not checking for solution");
                return false;
            }
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

            Debug.Log("Count Legs: " + countLegs);

            // Check if connected parts match current task
            bool isSolution = true;

            if (currentTask.mode.Equals("task"))
            {
                if ((countTorso != currentTask.torso && currentTask.torso > 0) ||
                (countHorns != currentTask.horns && currentTask.horns > 0) ||
                (countArms != currentTask.arms && currentTask.arms > 0) ||
                (countLegs != currentTask.legs && currentTask.legs > 0))
                {
                    Debug.Log(countTorso + "," + countHorns + "," + countArms + "," + countLegs + ",");
                    Debug.Log(currentTask.torso + "," + currentTask.horns + "," + currentTask.arms + "," + currentTask.legs + ",");
                    isSolution = false;
                }
            } else
            {
                if ((countTorso < currentTask.torso && currentTask.torso > 0) ||
                (countHorns < currentTask.horns && currentTask.horns > 0) ||
                (countArms < currentTask.arms && currentTask.arms > 0) ||
                (countLegs < currentTask.legs && currentTask.legs > 0))
                {
                    Debug.Log(countTorso + "," + countHorns + "," + countArms + "," + countLegs + ",");
                    Debug.Log(currentTask.torso + "," + currentTask.horns + "," + currentTask.arms + "," + currentTask.legs + ",");
                    isSolution = false;
                }
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
                connectionStateChanged(); 
            } else
            {
                StartCoroutine(SetupFinished());
            }
        }


        /* Loads a game setup with the given id and starts the first task from it */
        public void startGame(int id)
        {
            regularCamera.SetActive(false);
            this.setup = null;
            currentTask = null;
            connectedPartsMax = 0;
            currentSetupFinished = false;
            arCamera.SetActive(false);

            foreach (Part p in parts) {
                p.gameObject.SetActive(true);
            }

            var rendererComponents = GetComponentsInChildren<Renderer>(true);
            var colliderComponents = GetComponentsInChildren<Collider>(true);
            var canvasComponents = GetComponentsInChildren<Canvas>(true);

            // Enable rendering:
            foreach (var component in rendererComponents)
                component.enabled = false;

            // Enable colliders:
            foreach (var component in colliderComponents)
                component.enabled = false;

            // Enable canvas':
            foreach (var component in canvasComponents)
                component.enabled = false;
            
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


            finishedTasks = 0;
            liloJump.Play(0);
            StartCoroutine(Example());
        }

        private IEnumerator Example()
        {
            yield return new WaitForSeconds(1);
            trackedParts = new HashSet<Part>();
            connectedParts = new HashSet<Part>();
            yield return new WaitForSeconds(1);
            menuUI.gameObject.SetActive(false);
            taskUI.gameObject.SetActive(false);
            download.gameObject.SetActive(false);
            ScreenshotCamera.SetActive(false);
            download.gameObject.SetActive(false);
            arCamera.SetActive(true);
            yield return new WaitForSeconds(3);

            SoundManager.Instance.playFile("Sounds/" + setup.audio);
            yield return new WaitForSeconds(3);
            nextTask();
        }

        private IEnumerator SetupFinished()
        {
            yield return new WaitForSeconds(5);
            Debug.Log("Current setup is finished!");
            currentSetupFinished = true;

            arCamera.SetActive(false);
            trackedParts = new HashSet<Part>();
            regularCamera.SetActive(true);

            var rendererComponents = GetComponentsInChildren<Renderer>(true);

            // Enable rendering:
            foreach (var component in rendererComponents)
                component.enabled = true;

            float i = 0;
            Debug.Log("CON P: " + connectedParts.Count);
            foreach (Part part in parts)
            {
                if (connectedParts.Contains(part))
                {
                    i += 0.3f;
                    Debug.Log(i);
                    if (part.typ == Type.Arm)
                    {
                        Animation anim = part.transform.GetChild(0).GetChild(0).GetComponent<Animation>();
                        StartCoroutine(WaveAnimation(anim, i));
                    }
                }
                else
                {
                    part.gameObject.SetActive(false);
                }

            }
            if (i > 0)
            {
                StartCoroutine(FlyAnimation(i + 4.5f));
            }
            else
            {
                StartCoroutine(FlyAnimation(0));
            }
        }

        private IEnumerator WaveAnimation(Animation anim, float wait)
        {
            yield return new WaitForSeconds(wait);
            anim.Play();
        }

        private IEnumerator FlyAnimation(float wait)
        {
            yield return new WaitForSeconds(wait);
            monsterFly.Play();
            yield return new WaitForSeconds(4.01f);
            regularCamera.SetActive(false);
            ScreenshotCamera.SetActive(true);
            QRarea.SetActive(true);
            foreach (Part p in parts)
            {
                p.showColored(false);
            }
            yield return new WaitForSeconds(0.1f);
            takeScreenshot();
        }

        private string takeScreenshot()
        {
            Debug.Log("Take Screenshot");

            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int currentTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            string filename = Application.persistentDataPath + "/" + currentTime + ".png";
            //ScreenCapture.CaptureScreenshot(filename);

            Debug.Log("Screen Width : " + Screen.width);
            int width = Screen.width;
            int height = Screen.height;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            // Read screen contents into the texture
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            StartCoroutine(uploadScreenshot(System.Convert.ToBase64String(tex.EncodeToJPG(100))));

            return filename;
        }

        IEnumerator uploadScreenshot(string bytes)
        {
            Debug.Log("Upload Screenshot");

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Client-ID 2d8ec31a4c08811");

            WWWForm form = new WWWForm();
            form.AddField("image", bytes);
            byte[] rawData = form.data;

            string url = "https://api.imgur.com/3/image";

            WWW www = new WWW(url, rawData, headers);
            yield return www;

            Debug.Log("WWW Result: ");
            Debug.Log(www.text);

            IMGUR myObject = JsonUtility.FromJson<IMGUR>(www.text);

            string id = myObject.data.id;
            string imageURL = "https://imgur.com/" + id;
            Debug.Log("url: " + imageURL);

            StartCoroutine(GetTexture(imageURL, id));

        }

        IEnumerator GetTexture(string imageURL, string id)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://api.qrserver.com/v1/create-qr-code/?color=000&margin=10&size=150x150&data=" + imageURL);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                QRplane.GetComponent<Renderer>().material.mainTexture = myTexture;
                linkText.GetComponent<TextMesh>().text = "https://imgur.com/" + id;

                foreach (Part p in parts)
                {
                    p.showColored(true);
                }


                ScreenshotCamera.SetActive(false);
                regularCamera.SetActive(true);
                download.SetActive(true);
            }
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
