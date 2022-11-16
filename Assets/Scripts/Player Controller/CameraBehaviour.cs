using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// obsolete -- we started doing some stuff with this and then ended up switching to the cinemachine
/// </summary>

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] public GameObject player; // find via script later
    [SerializeField] private Transform lookAt;
    [SerializeField] private Transform camPos;

    [SerializeField] private Vector3 camOffset;


    private void LateUpdate()
    {
        CamControl();
    }

    private void CamControl()
    {
        //transform.position = player.transform.position + camOffset;
        //transform.position = camPos.position;
        //transform.LookAt(lookAt);
    }
}
