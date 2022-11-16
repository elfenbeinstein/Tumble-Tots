using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Script for every checkpoint that contains the according spawnpoint
    public GameObject spawnpoint;

    private void OnTriggerEnter(Collider other)
    {
        // Sets the current spawnpoint for the player to the spawnpoint of this checkpoint
        if (other.gameObject.GetComponent<InputHandlerAlive>() != null)
        {
            other.gameObject.GetComponent<InputHandlerAlive>().currentSpawnpoint = spawnpoint;
        }
    }
}
