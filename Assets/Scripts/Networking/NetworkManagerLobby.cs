using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
/// <summary>
/// This script is responsible for managing the network connections in the lobby, and also serves as a game manager
/// </summary>
public class NetworkManagerLobby : NetworkManager
{
    //Game Manager
    [SerializeField] List<NetworkRoomPlayer> players; //List of connected players
    [SerializeField] GameObject[] spawnPoints = new GameObject[8]; //Spawnpoints to spawn players (got later in script)
    [SerializeField] GameObject countDownHolder; //Holds the object that counts down the start of the round
    [SerializeField] List<GameObject> registeredObjects; //List of registered objects that can be spawned by the server
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

    private void Awake() //Start lobby music, create copy of registered objects
    {
        registeredObjects.Clear();
        foreach(GameObject rObject in spawnPrefabs)
        {
            registeredObjects.Add(rObject);
        }
        EventSystem.Instance.Fire("AUDIO", "lobby");
    }

    public override void OnStartClient() //Setup client networkManager's registered prefabs
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect() //Called when client connects
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) //Called when the server disconnects
    {
        base.OnServerDisconnect(conn);
        StartCoroutine(delayCheck());
    }

    public override void OnClientDisconnect() //Remove client from playerList (prevent null references and ready-up problems)
    {
        base.OnClientDisconnect();
        StartCoroutine(delayCheck());
        OnClientConnected?.Invoke();
    }

    IEnumerator delayCheck() //Delays server checks to account for lag
    {
        yield return new WaitForSeconds(1);
        UpdatePlayerList();
    }

    public void CountDown(int remainingTime) //Start countdown before round starts
    {
        if(remainingTime < 6 && remainingTime > 0) { GameObject.FindGameObjectWithTag("CountDown").GetComponent<TextMeshProUGUI>().text = $"{remainingTime}"; }
        else if(remainingTime == 0) 
        { 
            GameObject.FindGameObjectWithTag("CountDown").GetComponent<TextMeshProUGUI>().text = "GO!";

            // activate platforms so they start at the same time
            GameObject[] platforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
            foreach(GameObject platform in platforms)
            {
                platform.GetComponent<MovingPlatformT>().Go();
            }

            EventSystem.Instance.Fire("AUDIO", "start");
        }
        if(remainingTime == -1) { Destroy(GameObject.FindGameObjectWithTag("CountDown")); return; }
        StartCoroutine(waitForSeconds(remainingTime));
    }

    IEnumerator waitForSeconds(int _countDown) //Used for the countdown logic
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

    public override void OnServerConnect(NetworkConnectionToClient conn) //Ensure that the connecting client can join the lobby (not too full)
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

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) //Add a player object, add them to the player list
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
    public void PlayerReady() //Readys a player up, checks if conditions are met
    {
        readyPlayers++;
        if (readyPlayers == players.Count && players.Count > 3) //Check if all players are ready and if there are at least 4 players
        {
            lobbySize = players.Count;
            LoadLevel(); 
        }
    }

    public void PlayerUnReady() //Remove ready player
    {
        readyPlayers--;
    }

    public void LoadLevel() //Loads the specified level for the corresponding round number
    {
        foreach (NetworkRoomPlayer player in players) //Remove all child objects from playerdata holders
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
    private void OnLevelWasLoaded(int level) //Initialize the level when the game is loaded
    {
        Initialize();
        GameObject countDown = Instantiate(countDownHolder, transform.position, transform.rotation); //Create countdown object
        countDown.transform.parent = GameObject.FindObjectOfType<Canvas>().transform; //Child text to canvas.
        NetworkServer.Spawn(countDown); //Spawn the countdown object on all clients
        CountDown(10);
        finishedPlayers = 0;
        if(SceneManager.GetActiveScene().name == "Lobby")
            EventSystem.Instance.Fire("AUDIO", "lobby");
        else
            EventSystem.Instance.Fire("AUDIO", "level");
    }

    void Initialize() //Get spawnpoints, update round counter
    {
        if(lobbySize <= 4) //Small lobby size
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
        
        if (lobbySize == 2) { Qualifiers = 2; } //Used for debugging purposes (only occurs when force-starting a game on the server)

        spawnPrefabs.Clear();
        foreach(GameObject rObject in registeredObjects)
        {
            spawnPrefabs.Add(rObject);
        }

        //Get spawnpoints
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnP");

        SpawnPlayers();
    }

    void SpawnPlayers() //Spawns all connected players with their corresponding player prefab
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
                if(SceneManager.GetActiveScene().name == "Race_Obstacles") //Tot with the Jordans
                {
                    newPlayer = Instantiate(player.playerTypes[2], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                }
                else if(SceneManager.GetActiveScene().name == "Win_Screen") //Win-screen tot
                {
                    newPlayer = Instantiate(player.playerTypes[4], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                    NetworkServer.Spawn(newPlayer);
                    player.currentPlayerPrefab = newPlayer;
                    NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);
                    ShowCursors();
                    return;
                }
                else //Basic tot
                {
                    newPlayer = Instantiate(player.playerTypes[1], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                }
                //Spawn the created player on all clients
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

    [ClientRpc]
    void ShowCursors()
    {
        Cursor.visible = true;
    }
    public void PlayerDone(NetworkRoomPlayer player) //Ran when a player completes the round.
    {
        finishedPlayers++; //Qualification Logic
        if (finishedPlayers >= Qualifiers) //If enough players have qualified, end the round
        {
            RoundEnd(player.currentPlayerPrefab);
        }
        else //Else, turn the winner into a ghost spectator
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

    public void KillPlayer(NetworkRoomPlayer player) //Kill the player. Turn them into a ghost, set their status to dead
    {
        GameObject ghost = Instantiate(player.playerTypes[3], spawnPoints[1].transform.position, spawnPoints[1].transform.rotation);
        NetworkServer.Spawn(ghost);
        player.currentPlayerPrefab = ghost;
        player.isDead = true; //Set player data to dead
        NetworkServer.ReplacePlayerForConnection(player.conn, player.currentPlayerPrefab);

        finishedPlayers++; //Because there are less competitors, count it as a 'victory'
        if (finishedPlayers >= Qualifiers) //End the round differently, depending on the level
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

        foreach(GameObject player in alivePlayers) //Kill all remaining alive players 
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
