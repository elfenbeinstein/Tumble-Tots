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
            CmdDisplayWinner();
        }
    }

    public void CallDisplayWinnerCommand()
    {
        Debug.Log("Called");
        CmdDisplayWinner();
        Debug.Log("Called");
    }

    [Command]
    public void CmdDisplayWinner()
    {
        winnerName = PlayerPrefs.GetString("PlayerName");
        winnerNameText.text = winnerName;
    }

    public void DisplayWinnerName(string oldName, string newName)
    {
        winnerNameText.text = newName;
    }
}
