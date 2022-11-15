using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class DisplayPlayerTag : NetworkBehaviour
{
    [SerializeField] TextMeshPro playerNameText;
    Transform cameraToLookAt;
    [SyncVar(hook = "DisplayWinnerName")] public string playerName;

    private void Start()
    {
        if (!isServer)
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            CmdDisplayWinner(playerName);
        }
    }

    private void Update() //Rotate names to the current client's camera
    {
        GameObject objectToRotate = playerNameText.gameObject;
        Vector3 targetDirection = cameraToLookAt.position - objectToRotate.transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 360, 0.0f);
        objectToRotate.transform.rotation = Quaternion.LookRotation(-1 * newDirection);
    }

    private void FixedUpdate()
    {
        if(cameraToLookAt == null)
        {
            cameraToLookAt = GameObject.FindObjectOfType<Camera>().transform;
        }
    }


    [Command]
    public void CmdDisplayWinner(string name)
    {
        playerNameText.text = name;
    }

    public void DisplayWinnerName(string oldName, string newName)
    {
        playerNameText.text = newName;
    }
}
