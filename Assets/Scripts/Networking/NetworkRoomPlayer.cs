using Mirror;
using UnityEngine;
using System;

public class NetworkRoomPlayer : NetworkBehaviour
{
    [SerializeField] GameObject[] playerTypes; //Different player prefabs
    private GameObject currentPlayerPrefab; //Current player prefab attatched
    public string playerID;

    NetworkConnectionToClient conn;

    private void Start()
    {
        DontDestroyOnLoad(this);
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

        NetworkServer.ReplacePlayerForConnection(conn, currentPlayerPrefab); //Re-route current connection to lobby player gameobject
    }

    public void SpawnAliveTot()
    {
        currentPlayerPrefab = Instantiate(playerTypes[1], this.transform.position, this.transform.rotation);
        currentPlayerPrefab.transform.parent = gameObject.transform;

        NetworkServer.ReplacePlayerForConnection(conn, currentPlayerPrefab);
        //currentPlayerPrefab.GetComponent<InputHandlerAlive>().
    }

    public void SpawnTotWithTheJordans()
    {

    }

    public void SpawnDeadTot()
    {

    }
}
