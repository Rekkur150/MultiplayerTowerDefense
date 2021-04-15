using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WaveManager : NetworkBehaviour
{

    public List<Wave> waves = new List<Wave>();

    public static WaveManager singleton;

    [Tooltip("Current Wave the players are on.")]
    private int currentWave = 0;

    [SyncVar]
    public bool isWaveActive = false;

    private List<NetworkConnectionToClient> ReadyNetworkIdentities = new List<NetworkConnectionToClient>();

    private List<GameObject> currentWaveEnemies = new List<GameObject>();
    private List<bool> SpawnPointsDoneSpawning = new List<bool>();

    private void Awake()
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

    [Command(requiresAuthority = false)]
    public void ToggleReadyPlayer(NetworkConnectionToClient conn = null)
    {
        if (conn == null)
            return;

        if (ReadyNetworkIdentities.Contains(conn))
            ReadyNetworkIdentities.Remove(conn);
        else
        {
            ReadyNetworkIdentities.Add(conn);
            CheckIfReady();
        }
    }

    [ServerCallback]
    public void CheckIfReady()
    {
        if (isWaveActive)
            return;

        if (ReadyNetworkIdentities.Count >= NetworkManagerTD.singleton.numPlayers)
        {
            Debug.Log("Starting wave");
            StartWave(currentWave);
        }
        else
        {
            Debug.Log(ReadyNetworkIdentities.Count + " ready out of " + NetworkManagerTD.singleton.numPlayers + " Players");
        }
    }

    [ServerCallback]
    private void StartWave(int waveIndex)
    {

        if (waveIndex >= waves.Count || waves[waveIndex] == null)
            return;

        isWaveActive = true;
        SpawnPointsDoneSpawning.Clear();

        for (int i = 0; i < waves[waveIndex].EnemySpawns.Count; i++)
        {
            StartCoroutine(StartSpawningFromEnemySpawn(waves[waveIndex].EnemySpawns[i], i));
            SpawnPointsDoneSpawning.Add(false);
        }
    }

    [ServerCallback]
    IEnumerator StartSpawningFromEnemySpawn(Wave.EnemySpawn enemySpawn, int index)
    {

        foreach (Wave.EnemySpawn.SpawningInformation spawnInfo in enemySpawn.spawning)
        {
            yield return new WaitForSeconds(spawnInfo.Wait);

            for (int i = 0; i < spawnInfo.NumberOfEnemies; i++)
            {
                GameObject newEnemy = Instantiate(spawnInfo.EnemyPrefab);
                newEnemy.transform.position = enemySpawn.spawnPoint.position;

                NetworkServer.Spawn(newEnemy);

                yield return new WaitForSeconds(spawnInfo.TimeBetweenEnemySpawn);
            }
        }

        SpawnPointsDoneSpawning[index] = true;
        isFinishedSpawning();
    }

    [ServerCallback]
    private void isFinishedSpawning()
    {
        foreach (bool done in SpawnPointsDoneSpawning)
        {
            if (done == false)
                return;
        }

        WaveFinished();
    }

    [ServerCallback]
    private void WaveFinished()
    {
        isWaveActive = false;
    }

    [ServerCallback]
    public void AddPlayer(NetworkConnection conn)
    {

    }

    [ServerCallback]
    public void RemovePlayer(NetworkConnection conn)
    {
        if (ReadyNetworkIdentities.Exists(item => item.connectionId == conn.connectionId))
        {
            ReadyNetworkIdentities.RemoveAll(item => item.connectionId == conn.connectionId);
        }
    }
   
}
