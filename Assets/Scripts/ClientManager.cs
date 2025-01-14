using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ClientManager : NetworkManager
{
    public Transform[] playerSpawn;
    private int index = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Instantiate the player object
        GameObject newPlayer = Instantiate(playerPrefab, playerSpawn[index].position, playerSpawn[index].rotation);
        NetworkServer.AddPlayerForConnection(conn, newPlayer);

        index++;
        print(index);
        if (index >= playerSpawn.Length)
        {
            index = 0;
        }

        foreach (PlayerHandler player in FindObjectsOfType<PlayerHandler>())
        {
            if (NetworkServer.active)
            {
                player.RpcUpdateCameraTargets();
            }
            else
            {
                Debug.LogError("Tried to call ClientRpc while server was not active.");
            }
        }
    }
}
