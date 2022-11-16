using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagInScene : MonoBehaviour
{
    GameObject[] objectToRotate;
    Transform cameraToLookAt;

    void Start()
    {
        objectToRotate = GameObject.FindGameObjectsWithTag("PlayerTag");
    }

    void Update()
    {
        if (cameraToLookAt == null)
        {

            return;
        }
        foreach (GameObject tag in objectToRotate)
        {
            Vector3 targetDirection = cameraToLookAt.position - tag.transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 360, 0.0f);
            tag.transform.rotation = Quaternion.LookRotation(-1 * newDirection);
        }
    }

    private void FindCamera()
    {

    }
}
