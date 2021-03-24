using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerTD : NetworkManager
{
    public List<PlayerController> players { get; } = new List<PlayerController>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var player = conn.identity.GetComponent<PlayerController>();

        players.Add(player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        var player = conn.identity.GetComponent<PlayerController>();

        if (player.isReady)
        {
            player.isReady = false;
        }

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }
}
