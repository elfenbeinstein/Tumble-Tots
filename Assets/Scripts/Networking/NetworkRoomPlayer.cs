using Mirror;
using UnityEngine;
using System;

public class NetworkRoomPlayer : NetworkBehaviour
{
    [SerializeField] GameObject[] playerTypes; //Different player prefabs
    private GameObject currentPlayerPrefab; //Current player prefab attatched

    public void AddLobbyCharacter()
    {
        currentPlayerPrefab = Instantiate(playerTypes[0], this.transform.position, this.transform.rotation);
        currentPlayerPrefab.transform.parent = gameObject.transform;
    }
}
