using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInterface))]
public class PlayerMenu : NetworkBehaviour
{
    public PlayerInterface playerInterface;
    public GameObject menu;
    public GameObject hostmenu;

    private bool MenuOpen = false;

    private void Awake()
    {

        if (menu == null)
            Debug.LogError("No Menu in thie playermenu", this);

        if (TryGetComponent(out PlayerInterface playerInterface))
            this.playerInterface = playerInterface;

        enabled = false;
    }

    private void OnEnable()
    {
        if (isServer)
            hostmenu.SetActive(true);
        else
            menu.SetActive(true);
    }

    private void OnDisable()
    {
        if (isServer)
            hostmenu.SetActive(false);
        else
            menu.SetActive(false);
    }

    public void Resume()
    {
        playerInterface.SetState(PlayerInterface.State.Default);
    }

    public void SwitchLevel(int scene)
    {
        NetworkManager.singleton.ServerChangeScene(SceneManager.GetSceneByBuildIndex(scene).name);
    }

    public void Disconnect()
    {
        if (isServer)
            NetworkManager.singleton.StopHost();
        else
            NetworkManager.singleton.StopClient();
    }


}
