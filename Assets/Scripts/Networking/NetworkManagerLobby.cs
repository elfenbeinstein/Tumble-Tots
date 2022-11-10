using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    //Game Manager
    [SerializeField] List<NetworkRoomPlayer> players;
    [SerializeField] GameObject[] spawnPoints = new GameObject[8];
    [SerializeField] List<GameObject> registeredObjects;
    [SerializeField] int roundNumber;
    [SerializeField] int finishedPlayers;
    [SerializeField] int Qualifiers;


    //Network Manager
    [SerializeField] private string menuScene = string.Empty;
    
    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;
    NetworkConnectionToClient lobbyConnection;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    private void Start()
    {
        registeredObjects.Clear();
        foreach(GameObject rObject in spawnPrefabs)
        {
            registeredObjects.Add(rObject);
        }
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnClientConnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if(SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().path == menuScene)
        {
            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);

            //Give player ID & add to list
            roomPlayerInstance.playerID = $"{NetworkServer.connections.Count - 1}";
            players.Add(roomPlayerInstance);

            roomPlayerInstance.setConnection(conn);
            roomPlayerInstance.AddLobbyCharacter();

            if(lobbyConnection == null) { lobbyConnection = conn; }

        }
    }

    public void LoadLevel()
    {
        switch (roundNumber)
        {
            case 0:
                ServerChangeScene("Race_Obstacles");
                break;
            case 1:
                ServerChangeScene("Julian_test");
                break;
            case 2:
                ServerChangeScene("Tabea_test");
                break;
        }
        foreach (KeyValuePair<int, NetworkConnectionToClient> player in NetworkServer.connections)
        {
            player.Value.identity.GetComponentInParent<NetworkRoomPlayer>().RemoveAllChildren();
        }
    }




    //Game manager-related code

    private void OnLevelWasLoaded(int level)
    {
        Initialize();
    }

    void Initialize() //Get spawnpoints, update round counter
    {
        roundNumber++;

        switch (roundNumber)
        {
            case 1:
                Qualifiers = 2;
                break;
            case 2:
                Qualifiers = 1;
                break;
            case 3:
                Qualifiers = 1;
                break;

        }

        spawnPrefabs.Clear();
        foreach(GameObject rObject in registeredObjects)
        {
            spawnPrefabs.Add(rObject);
        }

        //Get spawnpoints
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnP");

        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        int i = 0;

        foreach(NetworkRoomPlayer player in players)
        {
            GameObject newPlayer = null;
            if (player.isDead) //Dead players
            {
                newPlayer = Instantiate(player.playerTypes[3], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                NetworkServer.Spawn(newPlayer);
                player.currentPlayerPrefab = newPlayer;
                NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);
            }
            else //Type of tot
            {
                if(SceneManager.GetActiveScene().name == "Race_Obstacles")
                {
                    newPlayer = Instantiate(player.playerTypes[2], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                }
                else
                {
                    newPlayer = Instantiate(player.playerTypes[1], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                }
                NetworkServer.Spawn(newPlayer);
                player.currentPlayerPrefab = newPlayer;
                newPlayer.GetComponent<InputHandlerAlive>().playerID = player.playerID;
                newPlayer.GetComponent<InputHandlerAlive>().owner = player;
                NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);
            }
            i++;
        }
    }

    public void PlayerDone(NetworkRoomPlayer player)
    {
        player.RemoveAllChildren(); //Destroy object

        //Spawn spectator ghost
        GameObject spectator = Instantiate(player.playerTypes[3], spawnPoints[1].transform.position, spawnPoints[1].transform.rotation);
        NetworkServer.Spawn(spectator);
        player.currentPlayerPrefab = spectator;
        NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);

        finishedPlayers++; //Qualification Logic
        if(finishedPlayers >= Qualifiers)
        {
            GameObject[] badPlayers = GameObject.FindGameObjectsWithTag("Player");

            foreach(GameObject badPlayer in badPlayers)
            {
                badPlayer.GetComponent<InputHandlerAlive>().owner.isDead = true;
                badPlayer.GetComponent<InputHandlerAlive>().owner.RemoveAllChildren();
            }
            LoadLevel();
        }
    }

    public void KillPlayer(NetworkRoomPlayer player)
    {
        GameObject ghost = Instantiate(player.playerTypes[3], spawnPoints[1].transform.position, spawnPoints[1].transform.rotation);
        NetworkServer.Spawn(ghost);
        player.currentPlayerPrefab = ghost;
        player.isDead = true; //Set player data to dead
        NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);

        finishedPlayers++;
        if (finishedPlayers >= Qualifiers)
        {
            GameObject[] badPlayers = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject badPlayer in badPlayers)
            {
                badPlayer.GetComponent<InputHandlerAlive>().owner.isDead = true;
                badPlayer.GetComponent<InputHandlerAlive>().owner.RemoveAllChildren();
            }
            LoadLevel();
        }
    }


}
