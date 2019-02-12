using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlienMaker
{
    public class Part : MonoBehaviour
    {
        public GameObject connected;
        public string group;
        public Manager.Type typ;
        public Manager.Type[] connectables;
        public HashSet<Part> connectedParts = new HashSet<Part>();
        public float height;

        [Serializable]
        public struct ColoredMonster
        {
            public string color;
            public GameObject image;
        }
        public ColoredMonster[] colored;
        public GameObject coloredPart;

        void OnTriggerEnter(Collider col)
        {
            Debug.Log("Colider Enter " + col.gameObject.GetComponent<Part>().typ);

            Part connectable = col.gameObject.GetComponent<Part>();

            int pos = Array.IndexOf(connectables, connectable.typ);
            if (pos > -1)
            {
                connectedParts.Add(connectable);
                connectionStateChanged();
            }
        }

        void OnTriggerExit(Collider col)
        {
            Debug.Log("Colider Exit " + col.gameObject.GetComponent<Part>().typ);
            Part connectable = col.gameObject.GetComponent<Part>();

            int pos = Array.IndexOf(connectables, connectable.typ);
            if (pos > -1)
            {
                SoundManager.Instance.playError();
                connectedParts.Remove(col.gameObject.GetComponent<Part>());
                connectionStateChanged();
            }
        }

        private void connectionStateChanged()
        {
            Manager.Instance.connectionStateChanged();
            if (connected != null)
            {
                if (connectedParts.Count > 0)
                {
                    connected.SetActive(true);
                }
                else
                {
                    connected.SetActive(false);
                }
            }
        }

        public void showColored(bool active)
        {
            if (coloredPart != null)
            {
                coloredPart.SetActive(active);
            }
        }

        void Start()
        {
            height = height * 0.01f;
            transform.localScale = new Vector3(height, height, height);
            for (int i = 0; i < colored.Length; ++i)
            {
                {
                    colored[i].image.SetActive(false);
                }
            }

            int color = UnityEngine.Random.Range(0, colored.Length - 1);
            coloredPart = colored[color].image;


            connectionStateChanged();
        }

        void Update()
        {}
    }
}
