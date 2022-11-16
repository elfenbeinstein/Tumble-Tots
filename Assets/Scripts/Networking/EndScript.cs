using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
/// <summary>
/// This script is responsible for qualifying the player when they reach the goal, and turning them into a ghost
/// </summary>
public class EndScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //If the object is a server-only object, play the qualify sound
        if (other.gameObject.GetComponent<InputHandlerAlive>() != null && GetComponent<NetworkIdentity>().serverOnly == false)
        {
            EventSystem.Instance.Fire("AUDIO", "goal");
            return;
        }
        //Destroy the current player object and turn them into a ghost spectator
        if (other.tag == "Player")
        {
            FindObjectOfType<NetworkManagerLobby>().PlayerDone(other.GetComponent<InputHandlerAlive>().owner);
        }
    }
}
