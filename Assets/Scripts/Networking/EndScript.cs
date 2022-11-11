using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EndScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<InputHandlerAlive>() != null && GetComponent<NetworkIdentity>().serverOnly == false)
        {
            EventSystem.Instance.Fire("AUDIO", "goal");
            return;
        }

        if (other.tag == "Player")
        {
            FindObjectOfType<NetworkManagerLobby>().PlayerDone(other.GetComponent<InputHandlerAlive>().owner);
        }
    }
}
