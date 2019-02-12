using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEyes : MonoBehaviour {
    public GameObject eyesClosed;
    public float keepClosed = 0.2f;
    public float keepOpenMin = 4;
    public float keepOpenMax = 15;

    private float lastClosed;
    private float lastOpened;

    private float waitUntilClose;

	void Start () {
        openEyes();
	}
	
	void Update () {
        float timeSinceLastOpened = Time.time - lastOpened;

        if (timeSinceLastOpened > waitUntilClose && !eyesClosed.activeSelf)
        {
            // close eyes
            lastClosed = Time.time;
            eyesClosed.SetActive(true);
        }

        if (Time.time - lastClosed > keepClosed && eyesClosed.activeSelf)
        {
            // open eyes
            openEyes();
        }
	}

    private void openEyes()
    {
        lastOpened = Time.time;

        int closeAgainImmediately = Random.Range(0, 3); // with 25% probability, blink again immediately
        if (closeAgainImmediately != 0)
        {
            waitUntilClose = Random.Range(keepOpenMin, keepOpenMax);
        } else
        {
            waitUntilClose = Random.Range(0.1f, 0.5f);
        }

        eyesClosed.SetActive(false);
    }
}
