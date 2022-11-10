using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<InputHandlerAlive>() != null)
        {
            EventSystem.Instance.Fire("AUDIO", "goal");
        }

        if (other.tag == "Player")
        {
            FindObjectOfType<NetworkManagerLobby>().PlayerDone(other.GetComponent<InputHandlerAlive>().owner);
        }
    }
}
