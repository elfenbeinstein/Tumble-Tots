using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperStartDelay : MonoBehaviour
{
    [SerializeField] float startAnimDelay;
    [SerializeField] float playbackSpeed = 1;
    [SerializeField] Animator anim;

    private void Start()
    {
        anim.speed = playbackSpeed;
    }

    private void Update()
    {
        startAnimDelay -= Time.deltaTime;

        if (startAnimDelay <= 0)
        {
            anim.SetTrigger("Start");
            enabled = false;
        }
    }
}
