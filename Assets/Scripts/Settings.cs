using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AlienMaker
{
    public class Settings : Singleton<Settings>
    {
        [Serializable]
        public class GameSetup
        {
            public string name;
            public int id;
            public string mode;
            public List<Task> tasks;

            public override string ToString()
            {
                return name + " (" + id + "), mode: " + mode;
            }
        }

        [Serializable]
        public class Task
        {
            public int order;
            public string group;
            public string dataset;
            public int heads;
            public int arms;
            public int legs;
            public int torso;

            public override string ToString()
            {
                return "Order: " + order + ", Group: " + group + ", Dataset: " + dataset + ", Heads: " + heads + ", Arms: " + arms + ", Legs: " + legs + ", Torso: " + torso;
            }
        }

        public List<GameSetup> setups = new List<GameSetup>();

        // Use this for initialization
        void Start()
        {
            string path = "";
#if UNITY_ANDROID	
            path = path = "jar:file://" + Application.dataPath + "!/assets";
#endif
#if UNITY_EDITOR
            path = Application.streamingAssetsPath + "/ad";
#endif
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] info = dir.GetFiles("*.json");
                foreach (FileInfo f in info)
                {
                    string text = File.ReadAllText(f.FullName);
                    GameSetup setup = JsonUtility.FromJson<GameSetup>(text);
                    setups.Add(setup);
                }
            }
            catch (Exception e) {
                Debug.Log("Path does not exist, taking hardcoded example");
                string text = "{\"id\":0,\"name\":\"Testaufgabe\",\"tasks\":[{\"mode\":\"task\",\"oder\":1,\"dataset\":\"alien-maker\",\"heads\":0,\"arms\":1,\"legs\":1,\"torso\":1}]}";
                GameSetup setup = JsonUtility.FromJson<GameSetup>(text);
                setups.Add(setup);
            }
            

            Menu.Instance.initializeButtons();
        }
    }
}
