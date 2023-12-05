using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class AudioManager_StateMachine : MonoBehaviour
{

    Animator animator;
    AudioSource audioSource;


    [SerializeField]
    AudioClip[] idleAudios;

    [SerializeField]
    AudioClip[] walkAudios;

    [SerializeField]
    AudioClip[] runAudios;

    [SerializeField]
    AudioClip[] attackAudios;

    [SerializeField]
    AudioClip[] deathAudios;


    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }


    // Start is called before the first frame update
    void Start()
    {
        if (animator != null)
        {
            IdleAudio idleAudio = animator.GetBehaviour<IdleAudio>();
            SetAudioBehavior(idleAudio, idleAudios);

            WalkAudio walkAudio = animator.GetBehaviour<WalkAudio>();
            SetAudioBehavior(walkAudio, walkAudios);

            RunAudio runAudio = animator.GetBehaviour<RunAudio>();
            SetAudioBehavior(runAudio, runAudios);

            AttackAudio attackAudio = animator.GetBehaviour<AttackAudio>();
            SetAudioBehavior(attackAudio, attackAudios);

            DeathAudio deathAudio = animator.GetBehaviour<DeathAudio>();
            SetAudioBehavior(deathAudio, deathAudios);
        }

        void SetAudioBehavior(AudioBase audio, AudioClip[] audioClips)
        {
            if (audio != null)
            {
                audio.AudioSource = audioSource;
                audio.Audios = audioClips;
            }
        }
    }
}
