using TMPro;
using UnityEngine.UI;
using UnityEngine;
/// <summary>
/// This script is responsible for handling the player's name at the start of the game
/// </summary>
public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")] //UI related variables
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;

    public static string DisplayName { get; private set; } //Grab name from anywhere, can only set player name locally

    public const string PlayerPrefsNameKey = "PlayerName"; //Used as shorcut for writing code

    private void Start()
    {
        SetUpInputField();
    }

    private void SetUpInputField() //If the player had a name before, put that as the default.
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name) //Set the player name to what was in the input field
    {
        if(string.IsNullOrEmpty(name)) { name = nameInputField.text; }

        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName() //Save the player name locally using PlayerPrefs
    {
        DisplayName = nameInputField.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }
}
