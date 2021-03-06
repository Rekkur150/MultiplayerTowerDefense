using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class NetworkManagerTD : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        NetworkPlayerManager.singleton.AddPlayer(conn);
        WaveManager.singleton.AddPlayer(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkPlayerManager.singleton.RemovePlayer(conn);
        WaveManager.singleton.RemovePlayer(conn);

        NetworkPlayerManager.singleton.RemovePlayer(conn);

        base.OnServerDisconnect(conn);
    }
}
