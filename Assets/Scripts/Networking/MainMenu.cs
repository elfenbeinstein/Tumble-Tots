using UnityEngine;
/// <summary>
/// This script is responsible for the initial UI present in the lobby script
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null; //Reference to networkmanager

    [Header("UI")] //UI related variables
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby() //Host a game on the server if the host button was pressed
    {
        networkManager.StartServer();

        landingPagePanel.SetActive(false);
    }
}
