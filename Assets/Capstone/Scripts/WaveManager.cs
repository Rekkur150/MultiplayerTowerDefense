using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WaveManager : NetworkBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public EnemyController enemy;
    }

    public Wave[] waves;

    public static WaveManager singleton;

    [Tooltip("Array of all potential enemy spawn points on the map.")]
    public Transform[] enemySpawns;

    [SyncVar]
    [Tooltip("Current Wave the players are on.")]
    private int currentWave = 0;

    [Tooltip("Number of enemies left in the wave.")]
    private int enemiesAlive;

    [SyncVar]
    [System.NonSerialized]
    public int numOfPlayers;

    [SyncVar]
    [System.NonSerialized]
    public bool isWaveActive = false;

    [SyncVar]
    [Tooltip("The number of players connected")]
    public int numberOfReadyPlayers;

    public NetworkManagerTD networkManager;

    private void Start()
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

    private void Update()
    {
        
    }

    [Command]
    public void ReadyPlayer()
    {
        numberOfReadyPlayers++;
        CheckIfReady();
    }

    [Command(requiresAuthority = false)]
    public void UnreadyPlayer()
    {
        numberOfReadyPlayers--;
        CheckIfReady();
    }

    public void CheckIfReady()
    {
        if (numberOfReadyPlayers >= networkManager.players.Count)
        {
            Debug.Log("Starting wave");
        }
        else
        {
            Debug.Log(numberOfReadyPlayers + " ready out of " + networkManager.players.Count + " Players");
        }
    }
}
