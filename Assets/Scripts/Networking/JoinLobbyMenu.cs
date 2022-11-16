using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script is responsible for handling the lobby UI when joining a game
/// </summary>
public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null; //Reference to network manager

    [Header("UI")] //References to required UI
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAdressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby() //Join the network at the IP address put into the input field
    {
        string ipAdress = ipAdressInputField.text;

        networkManager.networkAddress = ipAdress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected() //Ensure the client is connected, disable UI once they do
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected() //If the client could not connect, allow them to press the join button again
    {
        joinButton.interactable = true;
    }
}
