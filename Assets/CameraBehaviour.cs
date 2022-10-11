using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 camOffset;
    [SerializeField] private Transform player; // find via script later

    [Space]
    [Header("Mouse Look:")]
    private float mouseX, mouseY;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float maxRotation;
    [SerializeField] private float minRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
