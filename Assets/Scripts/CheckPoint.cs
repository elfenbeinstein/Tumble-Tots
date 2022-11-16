using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject spawnpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<InputHandlerAlive>() != null)
        {
            other.gameObject.GetComponent<InputHandlerAlive>().currentSpawnpoint = spawnpoint;
        }
    }
}
