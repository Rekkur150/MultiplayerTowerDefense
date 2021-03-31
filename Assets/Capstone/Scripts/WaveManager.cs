using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WaveManager : NetworkBehaviour
{
    [System.Serializable]
    public struct Wave
    {
        public int enemyCount;
    }

    public Wave[] waves;

    public static WaveManager singleton;

    [Tooltip("Enemy prefab for default enemy.")]
    public GameObject enemy;

    [Tooltip("Array of all potential enemy spawn points on the map.")]
    public GameObject[] enemySpawns;

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

    [Tooltip("The enemies for the current wave will be stored here for when we need to spawn them")]
    private List<GameObject> currentWaveEnemies = new List<GameObject>();

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

    [Command(requiresAuthority = false)]
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
        if (numberOfReadyPlayers >= NetworkManagerTD.singleton.numPlayers)
        {
            Debug.Log("Starting wave");
            StartWave(currentWave);
        }
        else
        {
            Debug.Log(numberOfReadyPlayers + " ready out of " + NetworkManagerTD.singleton.numPlayers + " Players");
        }
    }

    [ServerCallback]
    private void StartWave(int waveNumber)
    {
        for (int i = 0; i < waves[waveNumber].enemyCount; i++)
        {
            currentWaveEnemies.Add(enemy);
        }

        isWaveActive = true;

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (currentWaveEnemies.Count > 0)
        {
            // Index for random element in currentWaveEnemies list.
            int index = Random.Range(0, currentWaveEnemies.Count);

            GameObject spawn = enemySpawns[Random.Range(0, enemySpawns.Length)];
            GameObject childObject = Instantiate(currentWaveEnemies[index]);

            // Spawn the enemy at one of the available spawn points.
            NetworkServer.Spawn(childObject);
            Debug.Log(enemySpawns[Random.Range(0, enemySpawns.Length)].transform);

            ChangeObjectParent(childObject, spawn);

            // Remove enemy from list.
            currentWaveEnemies.RemoveAt(index);

            yield return new WaitForSeconds(2f);
        }
    }
    
    [ClientRpc]
    private void ChangeObjectParent(GameObject childObject, GameObject parent)
    {
        childObject.transform.parent = parent.transform;
    }
}
