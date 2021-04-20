using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class MapController : NetworkBehaviour
{

    static public MapController singleton;

    [SyncVar]
    public float BuildTimeMultiplier = 1f;
    [SyncVar]
    public float TowerRefundPercentage = 0.5f;

    public AudioSource gameOverMusic;

    [HideInInspector]
    public bool gameOver = false;
    [HideInInspector]
    public bool mapOver = false;

    void Start()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }

        if (gameOverMusic == null)
            Debug.LogError("No gameover music in the mapcontroller!", this);


        if (isServer)
        {
            WaveEnded(WaveManager.singleton);
            WaveManager.singleton.OnWaveEnded += new WaveManager.WaveManagerEventHandler(WaveEnded);
            WaveManager.singleton.OnWaveStarted += new WaveManager.WaveManagerEventHandler(WaveStarted);
        }
    }

    [ServerCallback]
    private void WaveEnded(WaveManager wave)
    {
        BuildTimeMultiplier = 0.1f;
    }

    [ServerCallback]
    private void WaveStarted(WaveManager wave)
    {
        BuildTimeMultiplier = 1f;
    }

    [ServerCallback]
    public void ServerGameOver()
    {
        if (gameOver == true)
            return;

        gameOver = true;

        ClientGameOver();
/*        StartCoroutine(ServerWaitTillReload());*/
    }

/*    [ServerCallback]
    private IEnumerator ServerWaitTillReload()
    {
        yield return new WaitForSeconds(10f);
        NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }*/

    [ClientRpc]
    public void ClientGameOver()
    {
        PlayerInformationPanel.singleton.UpdateText("The Crystal Was Destroyed! The game is over, host must restart the map");
        gameOverMusic.Play();
        //StartCoroutine(GameOverCoroutine());
    }

/*    private IEnumerator GameOverCoroutine()
    {
        for (int i = 10; i >= 0; i--)
        {
            PlayerInformationPanel.singleton.UpdateText("The Crystal Was Destroyed! The map will restart in " + i);
            yield return new WaitForSeconds(1f);
        }

        PlayerInformationPanel.singleton.UpdateText("");
    }*/

    [ServerCallback]
    public void ServerMapComplete()
    {

        if (mapOver == true)
            return;

        mapOver = true;

        ClientMapOver();
        StartCoroutine(ServerGotoNextMap());
    }

    [ServerCallback]
    private IEnumerator ServerGotoNextMap()
    {
        yield return new WaitForSeconds(10f);
        NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }

    [ClientRpc]
    public void ClientMapOver()
    {
        StartCoroutine(MapOverCoroutine());
    }

    private IEnumerator MapOverCoroutine()
    {
        for (int i = 10; i >= 0; i--)
        {
            PlayerInformationPanel.singleton.UpdateText("You beat the map! The next map will load in " + i);
            yield return new WaitForSeconds(1f);
        }

        PlayerInformationPanel.singleton.UpdateText("");
    }
}
