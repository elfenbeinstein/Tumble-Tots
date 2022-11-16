using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
/// <summary>
/// This script displays the winner at the end of the game. It is extremely simillar to the DisplayPlayerTag script
/// </summary>
public class DisplayWinner : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI winnerNameText; //Text that will show the winner's name

    [SyncVar(hook = "DisplayWinnerName")] public string winnerName; //Sync variable to display the winner's name on all other clients

    private void FixedUpdate()
    {
        if (isServer)
        {
            CmdDisplayWinner(PlayerPrefs.GetString("PlayerName"));
        }
    }

    [Command] //Set the winner name on the server and sync to other clients
    public void CmdDisplayWinner(string playerName)
    {
        winnerName = playerName;
        winnerNameText.text = winnerName;
    }
    //Sync variable hook for winnerName
    public void DisplayWinnerName(string oldName, string newName)
    {
        winnerNameText.text = newName;
    }
}
