using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerManager : NetworkBehaviour
{
    public static NetworkPlayerManager singleton;

    public struct Player
    {
        public Player(NetworkConnection conn)
        {
            this.networkConnection = conn;
            this.name = null;
            this.character = null;
            this.towers = new List<TowerInterface>();
        }

        public NetworkConnection networkConnection;
        public string name;
        public Character character;
        public List<TowerInterface> towers;
    }

    public List<Player> players = new List<Player>();

    // Start is called before the first frame update
    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }

    [ServerCallback]
    public void AddPlayer(NetworkConnection conn)
    {
        Player player = new Player(conn);
        players.Add(player);
    }

    [ServerCallback]
    public void RemovePlayer(NetworkConnection conn)
    {
        if (!players.Exists(item => item.networkConnection.connectionId == conn.connectionId))
            return;

        Player player = players.Find(item => item.networkConnection.connectionId == conn.connectionId);

        SellTowers(player);

        players.RemoveAll(item => item.networkConnection.connectionId == conn.connectionId);
    }

    [Command(requiresAuthority = false)]
    public void PlayerCharacterUpdated(Character newCharacter, NetworkConnectionToClient conn = null)
    {
        if (newCharacter == null || conn == null)
            return;

        if (!players.Exists(item => item.networkConnection.connectionId == conn.connectionId))
            return;

        Player player = players.Find(item => item.networkConnection.connectionId == conn.connectionId);
        player.character = newCharacter;
    }

    [ServerCallback]
    public void AddTowerToPlayer(TowerInterface tower, NetworkConnectionToClient conn)
    {
        if (tower == null || conn == null)
            return;

        if (!players.Exists(item => item.networkConnection.connectionId == conn.connectionId))
            return;

        Player player = players.Find(item => item.networkConnection.connectionId == conn.connectionId);
        player.towers.Add(tower);
    }

    [Command(requiresAuthority = false)]
    public void RemoveTowerFromPlayer(TowerInterface tower)
    {
        if (tower == null)
            return;

        if (!players.Exists(item => item.towers.Contains(tower)))
            return;

        Player player = players.Find(item => item.towers.Contains(tower));
        player.towers.Remove(tower);
    }

    [ServerCallback]
    private void SellTowers(Player player)
    {
        foreach (TowerInterface tower in player.towers)
        {
            tower.tower.SellTower(1);
        }
    }
}
