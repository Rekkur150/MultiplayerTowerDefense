using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(SelectTowerController))]
public class PlayerTowerInteraction : NetworkBehaviour
{
    public enum State
    {
        Deleting,
        Upgrading,
    }

    public Material DeleteMaterial;
    public Material UpgradeMaterial;

    private State currentState;
    public SelectTowerController selectTowerController;

    // Start is called before the first frame update
    void Awake()
    {
        if (TryGetComponent(out SelectTowerController selectTowerController))
            this.selectTowerController = selectTowerController;

        if (DeleteMaterial == null)
            Debug.LogError("Delete Material Missing!", this);

        if (UpgradeMaterial == null)
            Debug.LogError("Delete Upgrade Missing!", this);

    }

    public void SetState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Deleting:
                selectTowerController.ChangeHighlightMaterial(DeleteMaterial);
                break;
            case State.Upgrading:
                selectTowerController.ChangeHighlightMaterial(UpgradeMaterial);
                break;
        }
    }

    public State GetState()
    {
        return currentState;
    }

    public void Update()
    {
        if (!hasAuthority)
            return;

        if (Input.GetButtonDown("Fire1"))
            DoAction();
    }

    public void DoAction()
    {
        switch (currentState)
        {
            case State.Deleting:
                DeleteTower();
                break;
            case State.Upgrading:
                UpgradeTower();
                break;
        }
    }


    private void DeleteTower()
    {
        TowerInterface tower = selectTowerController.currentTower;

        if (tower == null)
            return;

        tower.tower.ServerDestroy();
    }

    private void UpgradeTower()
    {

    }
}
