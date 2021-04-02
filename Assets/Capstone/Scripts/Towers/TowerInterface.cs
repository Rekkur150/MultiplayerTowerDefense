using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(Tower))]
[RequireComponent(typeof(MaterialChanger))]
[RequireComponent(typeof(ColliderChecker))]
[RequireComponent(typeof(TowerBuildingController))]
[RequireComponent(typeof(TowerPlacingController))]
public class TowerInterface : NetworkBehaviour
{

    public Tower tower;
    public MaterialChanger materialChanger;
    public ColliderChecker colliderChecker;
    public TowerBuildingController towerBuildingController;
    public TowerPlacingController towerPlacingController;
    public enum State
    {
        Default,
        Disabled,
        Placing,
        Building,
        Upgrading
    }

    [SyncVar(hook = nameof(StateChanged))]
    private State currentState = State.Default;

    // Start is called before the first frame update
    void Awake()
    {

        if (TryGetComponent(out Tower tower))
            this.tower = tower;

        if (TryGetComponent(out MaterialChanger materialChanger))
            this.materialChanger = materialChanger;

        if (TryGetComponent(out ColliderChecker colliderChecker))
            this.colliderChecker = colliderChecker;

        if (TryGetComponent(out TowerBuildingController towerBuildingController))
            this.towerBuildingController = towerBuildingController;

        if (TryGetComponent(out TowerPlacingController towerPlacingController))
            this.towerPlacingController = towerPlacingController;

        materialChanger.IgnoredMeshes.Add(towerPlacingController.attackRangePreview.GetComponent<MeshRenderer>());

    }

    [ServerCallback]
    public void SetState(State newState)
    {
        currentState = newState;
    }

    public State GetState() 
    {
        return currentState;
    }

    private void StateChanged(State oldState, State newState)
    {
        ResetState(oldState);

        switch (newState)
        {
            case State.Default:
                StateChangeToDefault();
                break;
            case State.Disabled:
                StateChangeToDisable();
                break;
            case State.Placing:
                StateChangeToPlacing();
                break;
            case State.Building:
                StateChangeToBuilding();
                break;
            case State.Upgrading:
                StateChangeToUpgrading();
                break;
        }
    }

    private void ResetState(State oldState)
    {
        switch (oldState)
        {
            case State.Default:
                StateResetDefault();
                break;
            case State.Disabled:
                StateResetDisable();
                break;
            case State.Placing:
                StateResetPlacing();
                break;
            case State.Building:
                StateResetBuilding();
                break;
            case State.Upgrading:
                StateResetUpgrading();
                break;
        }
    }

    private void StateChangeToDefault()
    {
        tower.SetTowerEnabled(true);
        GetComponent<NetworkTransform>().syncInterval = 2f;
    }

    private void StateResetDefault()
    {
        tower.SetTowerEnabled(false);
        GetComponent<NetworkTransform>().syncInterval = 0.01f;
    }

    private void StateChangeToDisable() {}

    private void StateResetDisable() {}

    private void StateChangeToPlacing()
    {
        towerPlacingController.enabled = true;
    }

    private void StateResetPlacing()
    {
        towerPlacingController.enabled = false;
    }

    private void StateChangeToBuilding()
    {
        towerBuildingController.enabled = true;
    }

    private void StateResetBuilding()
    {
        towerBuildingController.enabled = false;
    }

    private void StateChangeToUpgrading() {}

    private void StateResetUpgrading() {}
}
