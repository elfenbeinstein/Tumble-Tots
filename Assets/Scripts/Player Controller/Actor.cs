using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public CharacterController cc;
    public GameObject body;
    public Transform shootingPoint;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>();
    }
}
