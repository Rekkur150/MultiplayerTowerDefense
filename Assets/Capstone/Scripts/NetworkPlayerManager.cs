using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerManager : NetworkBehaviour
{
    public struct Player
    {
        public Player(int connectionID, string name, Character character)
        {
            this.connectionID = connectionID;
            this.name = name;
            this.character = character;
        }

        public int connectionID;
        public string name;
        public Character character;
    }


    [SyncVar]
    public List<Player> players = new List<Player>();

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    [ServerCallback]
    public void AddCharacter(int connectionID, string name, Character character)
    {
        players.Add(new Player(connectionID,name,character));
    }

    [ServerCallback]
    public void RemoveCharacter(int connectionID)
    {
        players.RemoveAll(item => item.connectionID == connectionID);
    }

    public void UpdateCharacter(int connectionID, Character character)
    {
        //players.Find(item => item.connectionID == connectionID).character = character;
    }
}
