using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioDelayBase : AudioBase
{
    float minDelay = 1;
    public float MinDelay
    {
        get
        {
            return minDelay;
        }
        set
        {
            if (value <= maxDelay)
            {
                minDelay = value;
            }
            else
            {
                Debug.LogWarning($"Wrong Setting in {this.GetType()}: minimum delay cannot be greater than maximum delay.");
            }
        }
    }

    float maxDelay = 5;
    public float MaxDelay
    {
        get
        {
            return maxDelay;
        }
        set
        {
            if (value >= minDelay)
            {
                maxDelay = value;
            }
            else
            {
                Debug.LogWarning($"Wrong Setting in {this.GetType()}: maximum delay cannot be lower than minimum delay.");
            }
        }
    }


    float delayBetweenPlaying;
    float currentDelayBetweenPlaying;
    public void PlayRandomAudioClipWithDelay(float minDelay, float maxDelay)
    {
        if (AudioSource.isPlaying == false)
        {
            if (currentDelayBetweenPlaying > delayBetweenPlaying)
            {
                PlayRandomAudioClip();
                delayBetweenPlaying = Random.Range(minDelay, maxDelay);
                currentDelayBetweenPlaying = 0;
            }
            else
            {
                currentDelayBetweenPlaying += Time.deltaTime;
            }
        }
    }
}
