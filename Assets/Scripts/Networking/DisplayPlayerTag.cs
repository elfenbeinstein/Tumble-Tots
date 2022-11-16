using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
/// <summary>
/// This script is responsible for displaying a player's tag above their head using-
/// sync Variables and sending a player's name to the server to update other clients
/// </summary>
public class DisplayPlayerTag : NetworkBehaviour
{
    [SerializeField] TextMeshPro playerNameText; //Name component
    GameObject[] objectToRotate;
    [SerializeField] Transform cameraToLookAt; //Camera to rotate text towards
    [SyncVar(hook = "DisplayWinnerName")] public string playerName; //Sync variable of the player's name
    public string pID;

    private void Start()
    {
        objectToRotate = GameObject.FindGameObjectsWithTag("PlayerTag");
    }

    private void Update() //Rotate names to the current client's camera
    {
        if (isClient) //If is client, update the server with this player's name (stored locally on machine with PlayerPrefs)
        {
            CmdDisplayWinner(PlayerPrefs.GetString("PlayerName"));
        }
        //Lookat the active camera
        //GameObject objectToRotate[] = playerNameText.gameObject;
        if (cameraToLookAt == null) return;
        foreach (GameObject tag in objectToRotate)
        {
            Vector3 targetDirection = cameraToLookAt.position - tag.transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 360, 0.0f);
            tag.transform.rotation = Quaternion.LookRotation(-1 * newDirection);
        }
    }

    private void FixedUpdate() //If there is no camera to look at, find and set the variable. Called in fixed update to account for delayed connections to game
    {
        if(cameraToLookAt == null || !cameraToLookAt.gameObject.activeInHierarchy)
        {
            cameraToLookAt = GameObject.FindObjectOfType<Camera>().transform;
            /*
            if (gameObject.GetComponent<InputHandlerAlive>() != null) cameraToLookAt = gameObject.GetComponent<InputHandlerAlive>().GetCamera().transform;
            else if (gameObject.GetComponent<InputHandlerDead>() != null) cameraToLookAt = gameObject.GetComponent<InputHandlerDead>().GetCamera().transform;
            else cameraToLookAt = GameObject.FindObjectOfType<Camera>().transform;
            */

            /*
            Camera[] cams = GameObject.FindObjectsOfType<Camera>();
            if (cams.Length != 0)
            {
                for (int i = 0; i < cams.Length; i++)
                {
                    string idInCam = "";
                    if (cams[i].GetComponentInParent<InputHandlerAlive>() != null) idInCam = cams[i].GetComponentInParent<InputHandlerAlive>().playerID;
                    else if (cams[i].GetComponentInParent<InputHandlerDead>() != null) idInCam = cams[i].GetComponentInParent<InputHandlerDead>().playerID;
                    if (idInCam != "")
                    {
                        if (idInCam == pID) cameraToLookAt = cams[i].transform;
                    }
                }
            }
            */
        }
    }


    [Command] //Updates the other clients with the given name
    public void CmdDisplayWinner(string name)
    {
        playerName = name;
    }
    //Hook for playerName sync variable
    public void DisplayWinnerName(string oldName, string newName)
    {
        playerNameText.text = newName;
    }
}
