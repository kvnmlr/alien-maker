
using System;
using UnityEngine;

public class SpeakByVolume : MonoBehaviour {
    public GameObject mouthClosed;
    public GameObject mouthOpen;
    public float mouthSpeed = 500;
    public AudioSource source;
    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;
    private float clipLoudness;
    private float loudnessThreshold = 0.01f;
    private float[] clipSampleData;
    private bool speak = true;
    private float timeSinceSpeechStart;

    #region Unity Methods
    void Start()
    {
        mouthOpen.SetActive(false);
        mouthClosed.SetActive(true);
        clipSampleData = new float[sampleDataLength];
    }

    void Update()
    {
        if (source.clip == null)
        {
            return;
        }
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;

            // read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip
            try
            {
                source.clip.GetData(clipSampleData, source.timeSamples);
            } catch (Exception)
            {
                return;
            }
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength;
        }


        float deltaTime = Time.time * 1000 - timeSinceSpeechStart;

        float swithSpeed = mouthSpeed * (mouthClosed.activeSelf ? UnityEngine.Random.Range(0.2f, 0.5f) : UnityEngine.Random.Range(0.5f, 2f));

        if (deltaTime > swithSpeed)
        {
            if (clipLoudness > loudnessThreshold)
            {
                timeSinceSpeechStart = Time.time * 1000;
                if (mouthClosed.activeSelf)
                {
                    mouthClosed.SetActive(false);
                    mouthOpen.SetActive(true);
                } else
                {
                    mouthClosed.SetActive(true);
                    mouthOpen.SetActive(false);
                }
            } else
            {
                mouthClosed.SetActive(true);
                mouthOpen.SetActive(false);
            }
        } 
    }
    #endregion
}
