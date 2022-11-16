using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used for command pattern --> which cc is handed over to move player
/// </summary>

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
