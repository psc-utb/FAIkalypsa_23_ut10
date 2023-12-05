using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioBase : StateMachineBehaviour
{

    AudioClip[] audios;
    public AudioClip[] Audios { get => audios; set => audios = value; }

    public AudioSource AudioSource { get; set; }


    public AudioClip GetRandomAudioClip()
    {
        if (Audios != null && Audios.Length > 0)
        {
            int index = Random.Range(0, Audios.Length);
            return Audios[index];
        }

        return null;
    }


    public void PlayRandomAudioClip()
    {
        if (Audios != null && AudioSource != null)
        {
            AudioClip audioClip = GetRandomAudioClip();
            if (audioClip != null)
            {
                AudioSource.clip = audioClip;
                AudioSource.Play();
            }
        }
    }
}
