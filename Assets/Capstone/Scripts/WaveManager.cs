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

    public bool Enabled = true;

    private bool preparingWave = false;

    private List<NetworkConnectionToClient> ReadyNetworkIdentities = new List<NetworkConnectionToClient>();

    public List<GameObject> currentWaveEnemies = new List<GameObject>();
    private float NumberOfEnemiesSpawning = 0;
    private float NumberOfEnemiesThatDied = 0;

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
        if (conn == null || preparingWave || isWaveActive || !Enabled)
            return;

        if (ReadyNetworkIdentities.Contains(conn))
            ReadyNetworkIdentities.Remove(conn);
        else
        {
            ReadyNetworkIdentities.Add(conn);
            CheckIfReady();
        }

        if (currentWave < waves.Count && !isWaveActive)
            UpdatePlayerInformationPanel("The wave is complete press G to ready up! " + ReadyNetworkIdentities.Count + "/" + NetworkManagerTD.singleton.numPlayers);
    }


    [ClientRpc]
    private void UpdatePlayerInformationPanel(string text)
    {
        PlayerInformationPanel.singleton.UpdateText(text);
    }

    [ServerCallback]
    public void CheckIfReady()
    {
        if (isWaveActive || preparingWave)
            return;

        if (ReadyNetworkIdentities.Count >= NetworkManagerTD.singleton.numPlayers)
        {
            StartClientCountDown();
            StartCoroutine(ServerStartingWave());
        }
    }

    [ClientRpc]
    private void StartClientCountDown()
    {
        StartCoroutine(ClientStartingWave());
    }

    [ClientCallback]
    private IEnumerator ClientStartingWave()
    {
        for (int i = 10; i >= 0; i--)
        {
            PlayerInformationPanel.singleton.UpdateText("Starting wave in " + i);
            yield return new WaitForSeconds(1f);
       }

        PlayerInformationPanel.singleton.UpdateText("");
    }
    
    [ServerCallback]
    private IEnumerator ServerStartingWave()
    {
        preparingWave = true;
        yield return new WaitForSeconds(10f);
        StartWave(currentWave);
        preparingWave = false;
    }

    [ServerCallback]
    private void StartWave(int waveIndex)
    {

        if (waveIndex >= waves.Count || waves[waveIndex] == null)
            return;

        if (OnWaveStarted != null)
            OnWaveStarted(this);


        isWaveActive = true;

        NumberOfEnemiesSpawning = 0;
        NumberOfEnemiesThatDied = 0;


        foreach (Wave wave in waves)
        {

            for (int i = 0; i < wave.EnemySpawns.Count; i++)
            {
                for (int x = 0; x < wave.EnemySpawns[i].spawning.Count; x++)
                {
                    NumberOfEnemiesSpawning += wave.EnemySpawns[i].spawning[x].NumberOfEnemies;
                }
            }
        }

        for (int i = 0; i < waves[waveIndex].EnemySpawns.Count; i++)
        {
            StartCoroutine(StartSpawningFromEnemySpawn(waves[waveIndex].EnemySpawns[i], i));
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
                currentWaveEnemies.Add(newEnemy);

                yield return new WaitForSeconds(spawnInfo.TimeBetweenEnemySpawn);
            }
        }
    }

    [ServerCallback]
    public void isFinishedSpawning()
    {
        NumberOfEnemiesThatDied++;

        if (NumberOfEnemiesThatDied >= NumberOfEnemiesSpawning)
        {
            WaveFinished();
        }
    }

    [ServerCallback]
    private void WaveFinished()
    {

        if (!Enabled)
            return;

        isWaveActive = false;
        currentWave++;

        if (OnWaveEnded != null)
            OnWaveEnded(this);

        if (currentWave >= waves.Count)
            MapController.singleton.ServerMapComplete();

        else
            UpdatePlayerInformationPanel("The wave is complete press G to ready up! " + ReadyNetworkIdentities.Count + "/" + NetworkManagerTD.singleton.numPlayers);

    }




    [ServerCallback]
    public void AddPlayer(NetworkConnection conn)
    {
        if (!Enabled)
            return;

        if (currentWave < waves.Count && !isWaveActive)
            UpdatePlayerInformationPanel("The wave is complete press G to ready up! " + ReadyNetworkIdentities.Count + "/" + NetworkManagerTD.singleton.numPlayers);
    }

    [ServerCallback]
    public void RemovePlayer(NetworkConnection conn)
    {
        if (!Enabled)
            return;

        if (ReadyNetworkIdentities.Exists(item => item.connectionId == conn.connectionId))
        {
            ReadyNetworkIdentities.RemoveAll(item => item.connectionId == conn.connectionId);
        }

        UpdatePlayerInformationPanel("The wave is complete press G to ready up! " + ReadyNetworkIdentities.Count + "/" + NetworkManagerTD.singleton.numPlayers);
    }


    public delegate void WaveManagerEventHandler(WaveManager manager);
    public event WaveManagerEventHandler OnWaveStarted;
    public event WaveManagerEventHandler OnWaveEnded;
   
}
