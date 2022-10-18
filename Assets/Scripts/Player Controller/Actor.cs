using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public CharacterController cc;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>();
    }
}
