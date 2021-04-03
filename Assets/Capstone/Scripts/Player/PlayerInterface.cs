using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(TowerPlacer))]
[RequireComponent(typeof(TowerPlacerController))]
[RequireComponent(typeof(SelectTowerController))]
[RequireComponent(typeof(PlayerTowerInteraction))]
public class PlayerInterface : NetworkBehaviour
{
    public enum State
    {
        Default,
        Disabled,
        Interacting,
        Selecting,
    }

    public PlayerController playerController;
    public TowerPlacer towerPlacer;
    public TowerPlacerController towerPlacerController;
    public SelectTowerController selectTowerController;
    public PlayerTowerInteraction playerTowerInteraction;
    private State currentState = State.Default;

    // Start is called before the first frame update
    void Awake()
    {
        if (TryGetComponent(out PlayerController playerController))
            this.playerController = playerController;

        if (TryGetComponent(out TowerPlacer towerPlacer))
            this.towerPlacer = towerPlacer;

        if (TryGetComponent(out TowerPlacerController towerPlacerController))
            this.towerPlacerController = towerPlacerController;

        if (TryGetComponent(out SelectTowerController selectTowerController))
            this.selectTowerController = selectTowerController;

        if (TryGetComponent(out PlayerTowerInteraction playerTowerInteraction))
            this.playerTowerInteraction = playerTowerInteraction;

        playerController.OnDeath += new PlayerController.MyEventHandler(OnDeath);

    }

    // Update is called once per frame
    public void SetState(State newState)
    {
        ResetState(currentState);
        currentState = newState;
        switch (newState)
        {
            case State.Default:
                StateChangeToDefault();
                break;
            case State.Disabled:
                StateChangeToDisable();
                break;
            case State.Interacting:
                StateChangeToInteracting();
                break;
            case State.Selecting:
                StateChangeToSelecting();
                break;
        }
    }

    public void ResetState(State oldState)
    {
        switch (oldState)
        {
            case State.Default:
                StateResetDefault();
                break;
            case State.Disabled:
                StateResetDisable();
                break;
            case State.Interacting:
                StateResetInteracting();
                break;
            case State.Selecting:
                StateResetSelecting();
                break;
        }
    }

    private void StateChangeToDefault()
    {
        playerController.CanPlayerControlCharacter = true;
        towerPlacerController.enabled = true;
    }
    private void StateResetDefault()
    {
        playerController.CanPlayerControlCharacter = false;
        
    }
    
    private void StateChangeToDisable()
    {
        towerPlacer.ClientCancelSpawning();
    }

    private void StateResetDisable() {}

    private void StateChangeToInteracting()
    {
        playerController.CanPlayerControlCharacter = false;
        playerController.CharacterAnimator.SetBool("Interacting", true);
    }

    private void StateResetInteracting()
    {
        playerController.CharacterAnimator.SetBool("Interacting", false);
    }

    private void StateChangeToSelecting()
    {
        towerPlacer.ClientCancelSpawning();
        playerController.CanPlayerControlCharacter = true;
        towerPlacerController.enabled = false;
        selectTowerController.enabled = true;
        playerController.enabled = true;

        playerController.CharacterAnimator.SetBool("Interacting", true);
    }

    private void StateResetSelecting()
    {
        towerPlacerController.enabled = true;
        selectTowerController.enabled = false;
        playerController.CanPlayerControlCharacter = true;
        playerController.CharacterAnimator.SetBool("Interacting", false);
    }

    public State GetState()
    {
        return currentState;
    }

    private void OnDeath(object source, PlayerController.MyEventArgs e)
    {
        towerPlacer.ClientCancelSpawning();
    }
}
