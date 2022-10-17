using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject player; // find via script later
    private Transform camPos;

    private float mouseX, mouseY;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float maxRotation;
    [SerializeField] private float minRotation;
    private float verticalRotation;

    void Start()
    {
        //camPos = player.GetComponent<Movement>().CamPos();
        //gameObject.transform.position = camPos.position;
        verticalRotation = 0;
    }

    void Update()
    {
        /*
        // calculate rotation:
        mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;
        mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minRotation, maxRotation);

        // rotate with mouse:
        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        //player.transform.Rotate(Vector3.up, mouseX);
        */
        //gameObject.transform.position = camPos.position;
    }
}
