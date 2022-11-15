using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;



public class NetworkManagerLobby : NetworkManager
{
    //Game Manager
    [SerializeField] List<NetworkRoomPlayer> players;
    [SerializeField] GameObject[] spawnPoints = new GameObject[8];
    [SerializeField] GameObject countDownHolder;
    [SerializeField] List<GameObject> registeredObjects;
    [SerializeField] int lobbySize; //Number of players in lobby (used instead of connected players beacaus players can disconnect)
    [SerializeField] int roundNumber; //Round number
    [SerializeField] int finishedPlayers; //Players who have completed the current round
    [SerializeField] int Qualifiers; //Players who must qualify to move to the next round


    //Network Manager
    [SerializeField] private string menuScene = string.Empty;
    [SerializeField] private int readyPlayers;
    
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

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        StartCoroutine(delayCheck());
    }

    public override void OnClientDisconnect() //Remove cleint from playerList (prevent null references and ready-up problems)
    {
        base.OnClientDisconnect();
        StartCoroutine(delayCheck());
        OnClientConnected?.Invoke();
    }

    IEnumerator delayCheck()
    {
        yield return new WaitForSeconds(1);
        UpdatePlayerList();
    }

    public void CountDown(int remainingTime)
    {
        if(remainingTime < 6 && remainingTime > 0) { GameObject.FindGameObjectWithTag("CountDown").GetComponent<TextMeshProUGUI>().text = $"{remainingTime}"; }
        else if(remainingTime == 0) 
        { 
            GameObject.FindGameObjectWithTag("CountDown").GetComponent<TextMeshProUGUI>().text = "GO!"; 
        }
        if(remainingTime == -1) { Destroy(GameObject.FindGameObjectWithTag("CountDown")); return; }
        StartCoroutine(waitForSeconds(remainingTime));
    }

    IEnumerator waitForSeconds(int _countDown)
    {
        yield return new WaitForSeconds(1);
        CountDown(_countDown - 1);
    }

    public void UpdatePlayerList() //Update player list with current connected players
    {
        GameObject[] existingPlayers = GameObject.FindGameObjectsWithTag("PlayerHolder");
        players.Clear();
        foreach(GameObject player in existingPlayers)
        {
            if (player.GetComponent<NetworkRoomPlayer>().currentPlayerPrefab != null) //Add player to list if the player data has a current child
            {
                players.Add(player.GetComponent<NetworkRoomPlayer>()); //Destroy player data if they have no current child (player disconnected)
            }
            else
            {
                Destroy(player);
            }
        }
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
    public void PlayerReady()
    {
        readyPlayers++;
        if (readyPlayers == players.Count) //Check if all players are ready and if there are at least 4 players
        {
            lobbySize = players.Count;
            LoadLevel(); 
        }
    }

    public void PlayerUnReady() //Remove ready player
    {
        readyPlayers--;
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
            case 4:
                ServerChangeScene("Win_Screen");
                break;
        }
    }






    //Game manager-related code
    private void OnLevelWasLoaded(int level)
    {
        Initialize();
        GameObject countDown = Instantiate(countDownHolder, transform.position, transform.rotation);
        countDown.transform.parent = GameObject.FindObjectOfType<Canvas>().transform; //Child text to canvas.
        NetworkServer.Spawn(countDown);
        CountDown(10);
        finishedPlayers = 0;
        if(SceneManager.GetActiveScene().name == "Lobby")
            EventSystem.Instance.Fire("AUDIO", "lobby");
        else
            EventSystem.Instance.Fire("AUDIO", "level");
    }

    void Initialize() //Get spawnpoints, update round counter
    {
        if(lobbySize == 4) //Small lobby size
        {
            if (roundNumber == 1) { Qualifiers = 3; }
            else if (roundNumber == 2) { Qualifiers = 2; }
            else if (roundNumber == 3) { Qualifiers = 1; }
        }
        if(lobbySize > 4 && lobbySize < 7) //Medium lobby size (5 or 6)
        {
            if (roundNumber == 1) { Qualifiers = 4; }
            else if (roundNumber == 2) { Qualifiers = 2; }
            else if (roundNumber == 3) { Qualifiers = 1; }
        }
        if (lobbySize > 6) //Large lobby size (7 or 8)
        {
            if (roundNumber == 1) { Qualifiers = 5; }
            else if (roundNumber == 2) { Qualifiers = 3; }
            else if (roundNumber == 3) { Qualifiers = 1; }
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
                else if(SceneManager.GetActiveScene().name == "Win_Screen")
                {
                    newPlayer = Instantiate(player.playerTypes[4], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                    NetworkServer.Spawn(newPlayer);
                    player.currentPlayerPrefab = newPlayer;
                    NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);
                    return;
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
            //newPlayer.GetComponent<InputHandlerAlive>().playerName.text = roomPlayerPrefab.playerName;
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
            if(SceneManager.GetActiveScene().name == "Win_Screen")
            {
                GameObject winner = GameObject.FindGameObjectWithTag("Player");
                RoundEnd(winner);
            }
            else
            {
                RoundEnd(this.gameObject);
            }
        }
    }

    public void RoundEnd(GameObject protectedObject) //Normal round end logic
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
