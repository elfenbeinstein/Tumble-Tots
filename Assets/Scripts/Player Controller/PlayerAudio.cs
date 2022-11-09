using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip jump;
    [SerializeField] [Range(0, 1 )] private float jumpVolume;

    [SerializeField] private AudioClip dash;
    [SerializeField] [Range(0, 1)] private float dashVolume;

    [SerializeField] private AudioClip shoot;
    [SerializeField] [Range(0, 1)] private float shootVolume;

    [SerializeField] private AudioClip death;
    [SerializeField] [Range(0, 1)] private float deathVolume;

    [SerializeField] private AudioClip goal;
    [SerializeField] [Range(0, 1)] private float goalVolume;

    [SerializeField] private AudioClip start;
    [SerializeField] [Range(0, 1)] private float startVolume;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        EventSystem.Instance.AddEventListener("AUDIO", AudioListener);
    }

    private void OnDestroy()
    {
        EventSystem.Instance.RemoveEventListener("AUDIO", AudioListener);
    }

    private void AudioListener(string eventName, object param)
    {
        if (eventName == "jump") PlayJump();
        else if (eventName == "dash") PlayDash();
        else if (eventName == "shoot") PlayShoot();
        else if (eventName == "death") PlayDeath();
        else if (eventName == "start") PlayStart();
        else if (eventName == "goal") PlayGoal();
    }

    [ContextMenu("jump")]
    private void PlayJump()
    {
        audioSource.PlayOneShot(jump, jumpVolume);
    }

    [ContextMenu("dash")]
    private void PlayDash()
    {
        audioSource.PlayOneShot(dash, dashVolume);
    }

    [ContextMenu("shoot")]
    private void PlayShoot()
    {
        audioSource.PlayOneShot(shoot, shootVolume);
    }

    [ContextMenu("death")]
    private void PlayDeath()
    {
        audioSource.PlayOneShot(death, deathVolume);
    }

    [ContextMenu("goal")]
    private void PlayGoal()
    {
        audioSource.PlayOneShot(goal, goalVolume);
    }

    [ContextMenu("start")]
    private void PlayStart()
    {
        audioSource.PlayOneShot(start, startVolume);
    }
}
