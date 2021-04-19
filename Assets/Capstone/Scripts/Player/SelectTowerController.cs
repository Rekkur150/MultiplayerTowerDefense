using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerInterface))]
public class SelectTowerController : NetworkBehaviour
{

    [Tooltip("The layers we want to look for towers")]
    public LayerMask TowerSelectionMask;

    public TowerInterface currentTower;
    public Material HighlightMaterial;

    private PlayerInterface playerInterface;

    private void Awake()
    {

        if (TryGetComponent(out PlayerInterface playerInterface))
            this.playerInterface = playerInterface;

        if (HighlightMaterial == null)
            Debug.LogError("NO highlight material in this object!", this);

        if (TowerSelectionMask == 0)
            TowerSelectionMask = LayerMask.GetMask("Tower");

        enabled = false;

    }

    private void FixedUpdate()
    {
        if (hasAuthority)
        {
            SelectTower();

            if (Input.GetButtonDown("Cancel"))
                playerInterface.SetState(PlayerInterface.State.Default);

        }
    }

    private void OnDisable()
    {
        playerInterface.stateText.text = "";
        ResetTowerSelect();
    }

    private void SelectTower()
    {
        TowerInterface tower = GetTowerInterface();

        if (currentTower == tower)
            return;

        if (tower != null && tower.GetState() == TowerInterface.State.Building)
            return;

        if (currentTower != null)
            currentTower.materialChanger.RevertMaterial();

        currentTower = tower;

        if (currentTower != null)
            currentTower.materialChanger.ChangeMaterial(HighlightMaterial);

    }

    private TowerInterface GetTowerInterface()
    {
        Camera cam = ClientPlayerManager.singleton.PlayerCharacter.gameObject.GetComponentInChildren<Camera>();

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, TowerSelectionMask))
        {
            Transform currentGameObject = hit.collider.gameObject.transform;

            while (currentGameObject.parent != null)
            {
                currentGameObject = currentGameObject.parent;
            }

            if (currentGameObject.TryGetComponent(out TowerInterface towerInterface))
                return towerInterface;
        }

        return default;
    }
    private void ResetTowerSelect()
    {
        if (currentTower == null)
            return;

        currentTower.materialChanger.RevertMaterial();

        currentTower = null;
    }

    public void ChangeHighlightMaterial(Material newMaterial)
    {
        if (currentTower != null)
            currentTower.materialChanger.ChangeMaterial(newMaterial);

        HighlightMaterial = newMaterial;
    }


}
