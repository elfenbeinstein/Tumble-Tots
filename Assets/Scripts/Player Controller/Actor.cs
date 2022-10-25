using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public CharacterController cc;
    public GameObject body;
    public Vector3 projectileSpawn;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>();
    }
}
