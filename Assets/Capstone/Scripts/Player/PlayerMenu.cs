using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerInterface))]
public class PlayerMenu : MonoBehaviour
{
    public PlayerInterface playerInterface;
    public GameObject menu;

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
        menu.SetActive(true);
    }

    private void OnDisable()
    {
        menu.SetActive(false);
    }

    public void Resume()
    {
        playerInterface.SetState(PlayerInterface.State.Default);
    }

    public void Disconnect()
    {
        NetworkManager.singleton.StopClient();
    }


}
