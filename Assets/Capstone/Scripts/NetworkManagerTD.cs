using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerTD : NetworkManager
{
    //public List<PlayerController> Players { get; } = new List<PlayerController>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        //var player = conn.identity.GetComponent<PlayerController>();

        //Players.Add(player);

        NetworkPlayerManager.singleton.AddPlayer(conn);
        WaveManager.singleton.AddPlayer(conn);

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        /*        var player = conn.identity.GetComponent<PlayerController>();

                if (player.isReady)
                {
                    player.isReady = false;
                }
        */
        //Players.Remove(player);

        NetworkPlayerManager.singleton.RemovePlayer(conn);
        WaveManager.singleton.RemovePlayer(conn);

        base.OnServerDisconnect(conn);
    }
}
