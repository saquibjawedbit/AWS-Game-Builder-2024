using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GoSystem
{
    [GBehaviourAttributeAttribute("Audio Speaker", false)]
    public class AudioSpeaker : GoSystemsBehaviour
    {
        public AudioClip[] Clip;
        bool lockInput;
        void Play(AudioClip audioSource)
        {

            if (gameObject.GetComponent<AudioSource>() != null)
            {
                if (gameObject.GetComponent<AudioSource>().clip != audioSource)
                {
                    gameObject.GetComponent<AudioSource>().clip = audioSource;

                    gameObject.GetComponent<AudioSource>().Play();
                }
            }
            else
            {
                gameObject.GetComponent<AudioSource>().clip = audioSource;

                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        public void Stop()
        {
            gameObject.GetComponent<AudioSource>().Stop();
           
        }

        public void IndexClip(int Index )
        {
                Play(Clip[Index]);
        }

        public void ApplyLoop(bool Loop)
        {
            gameObject.GetComponent<AudioSource>().loop = Loop;
        }

    }
  
}