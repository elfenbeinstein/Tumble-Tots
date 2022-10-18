using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject player; // find via script later
    [SerializeField] private Transform lookAt;
    [SerializeField] private Transform camPos;

    [SerializeField] private Vector3 camOffset;

    private void LateUpdate()
    {
        CamControl();
    }

    private void CamControl()
    {
        transform.position = player.transform.position + camOffset;
        //transform.position = camPos.position;
        //transform.LookAt(lookAt);
    }
}
