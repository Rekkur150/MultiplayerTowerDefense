using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TowerPlacer : NetworkBehaviour
{
    public bool CanPlaceTower = true;
    public bool IsPlacingTower = false;

    public GameObject temp;

    [Header("Player Related")]
    [Tooltip("The layer that the player can place towers on")]
    public LayerMask PlayerInteractMask;

    [Header("Tower Related")]
    [Tooltip("The layers the tower will check to see if there is any colliding colliders")]
    public LayerMask TowerCollisionCheckMask;
    [Tooltip("The layers the we will check to see if a tower is on the ground")]
    public LayerMask TowerFloorCheckMask;

    public List<GameObject> SpawnableTowers = new List<GameObject>();

    private List<Tower> PlayerSpawnedTowers = new List<Tower>();
    private List<Tower> PlayerPreplacedTowers = new List<Tower>();

    private void Update()
    {   if (hasAuthority)
        {
            if (Input.GetKeyDown("g"))
                SpawnDisabledTower(0);

            if (Input.GetKeyDown("x"))
                EnableDisabledTower(PlayerPreplacedTowers[0]);

            if (PlayerPreplacedTowers.Count > 0)
            {
                PlayerPreplacedTowers[0].transform.position = GetMouseWorldPosition();
            }
        }
    }

    private bool CheckTowerPlacement(GameObject Tower) 
    {
        ColliderChecker TowerChecker = Tower.GetComponentInChildren<ColliderChecker>();

        if (TowerChecker == null)
        {
            Debug.LogWarning("Does not contain a ColliderChecker", Tower);
            return false;
        }

        if (TowerChecker.Collide(TowerCollisionCheckMask))
            return false;

        if (!TowerChecker.OnGround(TowerFloorCheckMask))
            return false;

        return true;
    }

    [Command]
    private void SpawnDisabledTower(int towerPrefabIndex, NetworkConnectionToClient conn = null)
    {
        GameObject preplacedTower = Instantiate(SpawnableTowers[towerPrefabIndex]);
        if (!preplacedTower.TryGetComponent(out Tower tower))
        {
            Debug.LogError("Tower prab does not contain a tower object", SpawnableTowers[towerPrefabIndex]);
            Destroy(preplacedTower);
            return;
        }

        tower.SetTowerEnabled(false);
        //Todo change material

        
        NetworkServer.Spawn(preplacedTower, conn);

        CollectPreplacedTower(tower);

    }

    [TargetRpc]
    private void CollectPreplacedTower(Tower tower)
    {
       
        PlayerPreplacedTowers.Add(tower);
    }

    [Command]
    private void EnableDisabledTower(Tower tower, NetworkConnectionToClient conn = null)
    {

        
        if (tower == null)
            return;

        Debug.Log(CheckTowerPlacement(tower.gameObject));

        //Todo reset material
        if (CheckTowerPlacement(tower.gameObject))
        {
            tower.SetTowerEnabled(true);
            CollectPlacedTower(tower);
            ChangeClientAuthority(tower, false);
        }
            
    }

    [TargetRpc]
    private void CollectPlacedTower(Tower tower)
    {
        if (tower == null)
            return;

        for (int i = 0; i < PlayerPreplacedTowers.Count; i++)
        {
            if (PlayerPreplacedTowers[i] == tower)
            {
                PlayerSpawnedTowers.Add(tower);
                PlayerPreplacedTowers.RemoveAt(i);
                return;
            }
        }
    }

    [ClientRpc]
    private void ChangeClientAuthority(Tower tower, bool hasClientControl)
    {
        if (tower == null)
            return;

        tower.gameObject.GetComponent<NetworkTransform>().clientAuthority = hasClientControl;

    }

    private Vector3 GetMouseWorldPosition()
    {
        Camera cam = ClientPlayerManager.singleton.PlayerCharacter.gameObject.GetComponentInChildren<Camera>();


        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, PlayerInteractMask))
        {
            return hit.point;
        }

        return new Vector3();
    }

}
