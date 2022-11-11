using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


// when you go back to the lobby:
//EventSystem.Instance.Fire("AUDIO", "lobby");

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
        EventSystem.Instance.Fire("AUDIO", "lobby");
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
        foreach (NetworkRoomPlayer player in players)
        {
            player.RemoveAllChildren();
        }

        roundNumber++;

        switch (roundNumber)
        {
            case 1:
                ServerChangeScene("Race_Obstacles");
                break;
            case 2:
                ServerChangeScene("Julian_test");
                break;
            case 3:
                ServerChangeScene("Tabea_test");
                break;
        }
    }






    //Game manager-related code

    private void OnLevelWasLoaded(int level)
    {
        Initialize();
        finishedPlayers = 0;
        EventSystem.Instance.Fire("AUDIO", "level");
    }

    void Initialize() //Get spawnpoints, update round counter
    {
        switch (roundNumber)
        {
            case 1:
                Qualifiers = 2;
                break;
            case 2:
                Qualifiers = 2;
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
        finishedPlayers++; //Qualification Logic
        if (finishedPlayers >= Qualifiers)
        {
            RoundEnd(player.currentPlayerPrefab);
        }
        else
        {
            NetworkServer.ReplacePlayerForConnection(player.conn, player.gameObject); //Give connection back to data object
            player.RemoveAllChildren(); //Destroy previous player object

            //Spawn spectator ghost
            GameObject spectator = Instantiate(player.playerTypes[3], spawnPoints[1].transform.position, spawnPoints[1].transform.rotation);
            NetworkServer.Spawn(spectator);
            player.currentPlayerPrefab = spectator;
            NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);
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
            RoundEnd(this.gameObject);
        }
    }

    public void RoundEnd(GameObject protectedObject)
    {
        //Get all alive players & kill them
        GameObject[] alivePlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in alivePlayers)
        {
            if (player != protectedObject)
            {
                NetworkRoomPlayer playerData = player.GetComponent<InputHandlerAlive>().owner;
                NetworkServer.ReplacePlayerForConnection(playerData.conn, playerData.gameObject); //Give connection back to player data
                playerData.RemoveAllChildren();
                playerData.isDead = true;
            }
        }

        LoadLevel(); //Load next level
    }
}
