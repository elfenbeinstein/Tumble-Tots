using Mirror;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// This script acts as a data holder for each player
/// </summary>
public class NetworkRoomPlayer : NetworkBehaviour
{
    public GameObject[] playerTypes; //Different player prefabs
    public GameObject currentPlayerPrefab; //Current player prefab attatched
    public string playerID;
    public string playerName;
    public bool isDead = false;

    public NetworkConnectionToClient conn;

    private void Start() //Ensure this object can travel between scenes, set the palyer name to the one saved locally using PlayerPrefs
    {
        DontDestroyOnLoad(this);
        playerName = PlayerPrefs.GetString("PlayerName");
    }

    public void setConnection(NetworkConnectionToClient connection) //get a reference to the connection the object is connected too
    {
        conn = connection;
    }

    public void RemoveAllChildren() //Remove the current child of this object
    {
        NetworkServer.ReplacePlayerForConnection(conn, this.gameObject); //Re-route current connection from player obejct to this holder

        NetworkServer.Destroy(currentPlayerPrefab);
    }

    public void AddLobbyCharacter() //Add the lobby character (done here because these objects do not require in-world synchronization)
    {
        currentPlayerPrefab = Instantiate(playerTypes[0], this.transform.position, this.transform.rotation);
        currentPlayerPrefab.transform.parent = gameObject.transform; //Spawn and child player gameobject
        //currentPlayerPrefab.GetComponent<InputHandlerAlive>().playerName.GetComponent<TextMeshProUGUI>().text = playerName;
        NetworkServer.ReplacePlayerForConnection(conn, currentPlayerPrefab); //Re-route current connection to lobby player gameobject
        currentPlayerPrefab.GetComponent<ChatBehaviourScript>().conn = conn;
    }

    public void AudioCommands(string eventName, string eventID) //Audio command handler
    {
        EventSystem.Instance.Fire(eventName, eventID);
    }
}
