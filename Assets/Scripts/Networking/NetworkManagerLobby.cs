using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    //Game Manager-related variables
    [SerializeField] int roundNumber; //For choosing the level
    [SerializeField] int numberOfQualifiers; //Number of people allowed to proceed
    [SerializeField] GameObject[] spawnPoints = new GameObject[8];
    [SerializeField] List<NetworkRoomPlayer> players;

    [SerializeField] string[] levelNames; //For random level pool


    //Network Manager-related variables
    [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;

    NetworkConnectionToClient lobbyConnection;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();


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

            //Give player ID
            roomPlayerInstance.playerID = $"{NetworkServer.connections.Count}";

            //Add player to game manager list
            AddPlayer(roomPlayerInstance);

            roomPlayerInstance.setConnection(conn);
            roomPlayerInstance.AddLobbyCharacter();

            if(lobbyConnection == null) { lobbyConnection = conn; }

        }
    }

    public void LoadLevel(string levelName)
    {
        foreach (KeyValuePair<int, NetworkConnectionToClient> player in NetworkServer.connections)
        {
            player.Value.identity.GetComponentInParent<NetworkRoomPlayer>().RemoveAllChildren();
        }

        ServerChangeScene(levelName);
    }

    private void OnLevelWasLoaded(int level)
    {
        InitializeRound();
    }

    //GameManager code
    public void AddPlayer(NetworkRoomPlayer player)
    {
        players.Add(player); //Add player to player list
    }

    public void ChooseRandomLevel()
    {

    }

    public void KillPlayer()
    {

    }

    public void InitializeRound()
    {
        roundNumber++; //Update round number

        GameObject[] newSpawns = GameObject.FindGameObjectsWithTag("SpawnP");
        int i = 0;
        foreach (GameObject spawn in newSpawns)
        {

            spawnPoints[i] = newSpawns[i];
            i++;
        }

        switch (roundNumber)
        {
            case 1:
                numberOfQualifiers = 6;
                break;

            case 2:
                numberOfQualifiers = 4;
                break;

            case 3:
                numberOfQualifiers = 1;
                break;

            default:
                break;
        }
        SpawnPlayers(); //Spawn players into scene
    }

    public void SpawnPlayers()
    {
        int i = 0;
        foreach (NetworkRoomPlayer player in players)
        {
            switch (player.isDead)
            {
                case false:
                    if (SceneManager.GetActiveScene().name == "Race_Obstacles") { SpawnTotWithTheJordans(spawnPoints[i].transform, player); }
                    else { SpawnTot(spawnPoints[i].transform, player); }
                    i++;
                    break;

                case true:
                    SpawnDeadTot(spawnPoints[i].transform, player);
                    i++;
                    break;
            }
        }
    }

    public void SpawnTot(Transform spawnPoint, NetworkRoomPlayer player)
    {
        player.currentPlayerPrefab = Instantiate(player.playerTypes[1], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(player.currentPlayerPrefab);
        player.currentPlayerPrefab.transform.parent = player.transform;

        NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);
        player.currentPlayerPrefab.GetComponent<InputHandlerAlive>().playerID = player.playerID;
    }

    public void SpawnTotWithTheJordans(Transform spawnPoint, NetworkRoomPlayer player)
    {
        player.currentPlayerPrefab = Instantiate(player.playerTypes[2], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(player.currentPlayerPrefab);
        player.currentPlayerPrefab.transform.parent = player.transform;

        NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);
        player.currentPlayerPrefab.GetComponent<InputHandlerAlive>().playerID = player.playerID;
    }

    public void SpawnDeadTot(Transform spawnPoint, NetworkRoomPlayer player)
    {
        player.currentPlayerPrefab = Instantiate(player.playerTypes[3], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(player.currentPlayerPrefab);
        player.currentPlayerPrefab.transform.parent = player.transform;

        NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);
        player.currentPlayerPrefab.GetComponent<InputHandlerAlive>().playerID = player.playerID;
    }
}
