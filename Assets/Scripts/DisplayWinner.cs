using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class DisplayWinner : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI winnerNameText;

    [SyncVar(hook = "DisplayWinnerName")] public string winnerName;

    private void Start()
    {
        if (!isServer)
        {
            CmdDisplayWinner(PlayerPrefs.GetString("PlayerName"));
        }
    }

    public void CallDisplayWinnerCommand()
    {
        CmdDisplayWinner(PlayerPrefs.GetString("PlayerName"));
    }

    [Command]
    public void CmdDisplayWinner(string playerName)
    {
        winnerName = playerName;
        winnerNameText.text = winnerName;
    }

    public void DisplayWinnerName(string oldName, string newName)
    {
        winnerNameText.text = newName;
    }
}
