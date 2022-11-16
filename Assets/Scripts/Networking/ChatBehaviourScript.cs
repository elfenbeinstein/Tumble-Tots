using UnityEngine;
using System;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;
/// <summary>
/// This script handles the pre-game lobby chat. The 3 serialized fields are references to text object which display the chat and input field.
/// This script also handles if the client is currently ready to start. The function resides here, but this ready data is stored on the server networkmanager
/// </summary>
public class ChatBehaviourScript : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;

    public NetworkConnectionToClient conn;

    private static event Action<string> OnMessage;
    public const string PlayerPrefsNameKey = "PlayerName"; //Used as shorcut for writing code

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void ReadyUpdate(bool isReady) //Updates the status of this player to if they are ready/not ready to start playing
    {
        if (isReady) { Ready(); }
        else { UnReady(); }
    }

    [Command] //Tell server that player is ready, call start game check
    void Ready()
    {
        NetworkManagerLobby man = FindObjectOfType<NetworkManagerLobby>();
        man.PlayerReady();
    }

    [Command] //Tell server player is not ready
    void UnReady()
    {
        NetworkManagerLobby man = FindObjectOfType<NetworkManagerLobby>();
        man.PlayerUnReady();
    }


    public override void OnStartAuthority() //Enable the chat UI when this object gains the authority to use it
    {
        chatUI.SetActive(true);

        OnMessage += HandleNewMessage;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!hasAuthority) { return; }

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message) //Set the chat text to include the player's message
    {
        chatText.text += message;
    }

    [Client]
    public void Send() //Send and package the written message for the server to process (client-side)
    {
        if(!Input.GetKeyDown(KeyCode.Return)) { return; }

        if (string.IsNullOrWhiteSpace(inputField.text)) { return; }

        CmdSendMessage(PlayerPrefs.GetString(PlayerPrefsNameKey), inputField.text);

        inputField.text = string.Empty;
    }

    [Command] //Send written message (server-side)
    private void CmdSendMessage(string name, string message)
    {
        RpcHandleMessage($"[{name}]: {message}");
    }

    [ClientRpc] //Rpc check
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }

}
