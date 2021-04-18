using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(SelectTowerController))]
[RequireComponent(typeof(PlayerInterface))]
public class PlayerTowerInteraction : NetworkBehaviour
{
    public enum State
    {
        Deleting,
        Repairing,
    }

    public Material DeleteMaterial;
    public Material RepairMaterial;

    private State currentState;
    public SelectTowerController selectTowerController;
    public PlayerInterface playerInterface;

    private IEnumerator Repairing;

    // Start is called before the first frame update
    void Awake()
    {
        if (TryGetComponent(out SelectTowerController selectTowerController))
            this.selectTowerController = selectTowerController;

        if (TryGetComponent(out PlayerInterface playerInterface))
            this.playerInterface = playerInterface;

        if (DeleteMaterial == null)
            Debug.LogError("Delete Material Missing!", this);

        if (RepairMaterial == null)
            Debug.LogError("Delete Repair Missing!", this);

    }

    public void SetState(State newState)
    {
        StateChanged();
        currentState = newState;

        switch (newState)
        {
            case State.Deleting:
                playerInterface.stateText.text = "Deleting";
                selectTowerController.ChangeHighlightMaterial(DeleteMaterial);
                break;
            case State.Repairing:
                playerInterface.stateText.text = "Repairing";
                selectTowerController.ChangeHighlightMaterial(RepairMaterial);
                break;
        }
    }

    private void OnDisable()
    {
        Debug.Log("On disabled");
        CancelRepairTower();
    }

    private void StateChanged()
    {
        switch (currentState)
        {
            case State.Repairing:
                CancelRepairTower();
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
            case State.Repairing:
                RepairTower();
                break;
        }
    }


    private void DeleteTower()
    {
        TowerInterface tower = selectTowerController.currentTower;

        if (tower == null)
            return;

        ServerDeleteTower(tower);
    }

    [Command]
    private void ServerDeleteTower(TowerInterface tower)
    {
        if (tower == null)
            return;

        tower.tower.SellTower(MapController.singleton.TowerRefundPercentage);
        tower.tower.ServerDestroy();

    }

    private void RepairTower()
    {
        TowerInterface tower = selectTowerController.currentTower;

        if (tower == null)
            return;

        playerInterface.SetState(PlayerInterface.State.Interacting);

        ServerRepairTower(tower);

    }

    [Command]
    private void ServerRepairTower(TowerInterface tower, NetworkConnectionToClient conn = null)
    {
        if (tower == null || tower.GetState() != TowerInterface.State.Default)
            ClientFinishedOrCanceledRepairingTower();

        Repairing = ServerRepairingTower(tower);
        StartCoroutine(Repairing);

    }

    [ServerCallback]
    private IEnumerator ServerRepairingTower(TowerInterface tower)
    {

        float ManaPerHealth = tower.tower.Cost / tower.tower.MaxHealth;

        while (tower.tower.GetHealth() < tower.tower.MaxHealth)
        {

            float Difference = tower.tower.MaxHealth - tower.tower.GetHealth();
            Difference = Mathf.Min(Difference, 0.5f);

            tower.tower.Damage(-Difference);
            ClientMoneyController.singleton.RemoveMoney(ManaPerHealth * Difference);

            yield return new WaitForSeconds(0.1f * MapController.singleton.BuildTimeMultiplier);

        }

        ClientFinishedOrCanceledRepairingTower();

    }

    [Command]
    private void CancelRepairTower(NetworkConnectionToClient conn = null)
    {

        if (conn == null)
            return;

        if (Repairing != null)
            StopCoroutine(Repairing);

        ClientFinishedOrCanceledRepairingTower();
    }

    [TargetRpc]
    private void ClientFinishedOrCanceledRepairingTower()
    {
        if (playerInterface.GetState() == PlayerInterface.State.Interacting)
            playerInterface.SetState(PlayerInterface.State.Selecting);
    }

    

}
