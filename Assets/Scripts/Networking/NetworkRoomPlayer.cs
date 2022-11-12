using Mirror;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class NetworkRoomPlayer : NetworkBehaviour
{
    public GameObject[] playerTypes; //Different player prefabs
    public GameObject currentPlayerPrefab; //Current player prefab attatched
    public string playerID;
    public string playerName;
    public bool isDead = false;

    public NetworkConnectionToClient conn;

    private void Start()
    {
        DontDestroyOnLoad(this);
        playerName = PlayerPrefs.GetString("PlayerName");
    }

    public void setConnection(NetworkConnectionToClient connection)
    {
        conn = connection;
    }

    public void RemoveAllChildren()
    {
        NetworkServer.ReplacePlayerForConnection(conn, this.gameObject); //Re-route current connection from player obejct to this holder

        NetworkServer.Destroy(currentPlayerPrefab);
    }

    public void AddLobbyCharacter()
    {
        currentPlayerPrefab = Instantiate(playerTypes[0], this.transform.position, this.transform.rotation);
        currentPlayerPrefab.transform.parent = gameObject.transform; //Spawn and child player gameobject
        //currentPlayerPrefab.GetComponent<InputHandlerAlive>().playerName.GetComponent<TextMeshProUGUI>().text = playerName;
        NetworkServer.ReplacePlayerForConnection(conn, currentPlayerPrefab); //Re-route current connection to lobby player gameobject
        currentPlayerPrefab.GetComponent<ChatBehaviourScript>().conn = conn;
    }

    public void AudioCommands(string eventName, string eventID)
    {
        EventSystem.Instance.Fire(eventName, eventID);
    }
}
