using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is responsible for killing the player if they touch lava.
/// </summary>
public class KillScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") //If the object is a player, play a sound and kill them
        {
            other.GetComponent<InputHandlerAlive>().owner.AudioCommands("AUDIO", "death");
            FindObjectOfType<NetworkManagerLobby>().PlayerDone(other.GetComponent<InputHandlerAlive>().owner);
        }
    }
}
