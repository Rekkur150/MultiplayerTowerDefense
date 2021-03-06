using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerInterface))]
[RequireComponent(typeof(PlayerTowerInteraction))]
public class PlayerControls : NetworkBehaviour
{

    public PlayerInterface playerInterface;
    public PlayerTowerInteraction playerTowerInteraction;

    // Start is called before the first frame update
    void Awake()
    {
        if (TryGetComponent(out PlayerInterface playerInterface))
            this.playerInterface = playerInterface;

        if (TryGetComponent(out PlayerTowerInteraction playerTowerInteraction))
            this.playerTowerInteraction = playerTowerInteraction;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        if (Input.GetButtonDown("TowerDelete"))
        {
            PlayerInterfaceState(PlayerTowerInteraction.State.Deleting);
        }

        if (Input.GetButtonDown("TowerUpgrade"))
        {
            PlayerInterfaceState(PlayerTowerInteraction.State.Repairing);
        }

        if (Input.GetButtonDown("ReadyUp"))
        {
                WaveManager.singleton.ToggleReadyPlayer();
        }

        if (Input.GetButtonDown("Menu"))
        {
            playerInterface.SetState(PlayerInterface.State.Menu);
        }

    }

    private void PlayerInterfaceState(PlayerTowerInteraction.State newState)
    {

        if (playerInterface.GetState() != PlayerInterface.State.Selecting)
        {
            playerInterface.SetState(PlayerInterface.State.Selecting);
            playerTowerInteraction.SetState(newState);
            return;
        }

        if (playerTowerInteraction.GetState() != newState)
            playerTowerInteraction.SetState(newState);
        else playerInterface.SetState(PlayerInterface.State.Default);
    }
}
