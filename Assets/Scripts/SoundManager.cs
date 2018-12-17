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
        public AudioClip tap;

        AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void playOpen()
        {
            audioSource.PlayOneShot(open, 0.7F);
        }

        public void playSuccess()
        {
            audioSource.PlayOneShot(success, 0.7F);
        }

        public void playError()
        {
            audioSource.PlayOneShot(error, 0.7F);
        }

        public void playTap()
        {
            audioSource.PlayOneShot(tap, 0.7F);
        }
    }
}