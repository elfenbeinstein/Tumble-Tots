using UnityEngine;
using System;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;

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

    public void ReadyUpdate(bool isReady)
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


    public override void OnStartAuthority()
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

    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    [Client]
    public void Send()
    {
        if(!Input.GetKeyDown(KeyCode.Return)) { return; }

        if (string.IsNullOrWhiteSpace(inputField.text)) { return; }

        CmdSendMessage(inputField.text);

        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        RpcHandleMessage($"[{PlayerPrefs.GetString(PlayerPrefsNameKey, "FawnAlloy")}]: {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }

}
