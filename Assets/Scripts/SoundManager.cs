using UnityEngine;
using System.Collections;

namespace AlienMaker
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : Singleton<SoundManager>
    {
        public AudioClip open;
        public AudioClip error;
        public AudioClip success;
        public AudioClip[] spitze;


        AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void playFile(string filename)
        {
            Debug.Log(filename);
            AudioClip clip = (AudioClip)Resources.Load(filename);
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void playOpen()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(open, 0.7F);
            }
        }

        public void playSuccess()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(success, 0.7F);
            }
        }

        public void playError()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(error, 0.7F);
            }
        }

        public void playSpitze()
        {
            AudioClip clip = spitze[(int)Mathf.Floor(Random.Range(0, spitze.Length - 1))];
            if (!audioSource.isPlaying)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}