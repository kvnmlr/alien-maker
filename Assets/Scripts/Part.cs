using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlienMaker
{
    public class Part : MonoBehaviour
    {
        public GameObject connected;
        public Text text;
        public string group;
        public Manager.Type typ;
        public Manager.Type[] connectables;
        public HashSet<Part> connectedParts = new HashSet<Part>();

        void OnTriggerEnter(Collider col)
        {
            Part connectable = col.gameObject.GetComponent<Part>();

            int pos = Array.IndexOf(connectables, connectable.typ);
            if (pos > -1)
            {
                SoundManager.Instance.playSuccess();
                connectedParts.Add(connectable);
                connectionStateChanged();
            }
        }

        void OnTriggerExit(Collider col)
        {
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

        void Start()
        {
            connectionStateChanged();
        }

        void Update()
        {}
    }
}
