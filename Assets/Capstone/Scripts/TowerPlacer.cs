using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerInterface))]
public class TowerPlacer : NetworkBehaviour
{

    public PlayerInterface playerInterface;

    [Header("Player Related")]
    [Tooltip("The layer that the player can place towers on")]
    public LayerMask PlayerInteractMask;

    [Header("Tower Related")]
    [Tooltip("The layers the tower will check to see if there is any colliding colliders")]
    static public LayerMask TowerCollisionCheckMask;
    [Tooltip("The layers the we will check to see if a tower is on the ground")]
    static public LayerMask TowerFloorCheckMask;

    public List<GameObject> SpawnableTowers = new List<GameObject>();

    private List<TowerInterface> PlayerSpawnedTowers = new List<TowerInterface>();
    private List<TowerInterface> PlayerPreplacedTowers = new List<TowerInterface>();

    private IEnumerator TowerSpawnCoroutine;

    private void Awake()
    {
        if (TryGetComponent(out PlayerInterface playerInterface))
            this.playerInterface = playerInterface;

        if (TowerCollisionCheckMask == 0)
            TowerCollisionCheckMask = LayerMask.GetMask("Default", "JumpDisabled", "Tower", "Player");

        if (TowerFloorCheckMask == 0)
            TowerFloorCheckMask = LayerMask.GetMask("Default", "JumpDisabled");
    }

    private void Update()
    {   if (hasAuthority)
        {
            if (CanMoveTower())
                MovePrefabTower();
        }
    }

    public void ClientEnableDisabledTower()
    {
        if (PlayerPreplacedTowers.Count == 0 || PlayerPreplacedTowers[0] == null)
            return;

        if (OnStartBuildingATower != null)
            OnStartBuildingATower(PlayerPreplacedTowers[0]);

        playerInterface.SetState(PlayerInterface.State.Interacting);
        EnableDisabledTower(PlayerPreplacedTowers[0]);
    }

    public void ClientCancelSpawning()
    {

        if (PlayerPreplacedTowers.Count == 0)
            return;

        if (OnCancelingBuildingATower != null)
            OnCancelingBuildingATower(default);

        playerInterface.playerController.CanPlayerControlCharacter = true;
        playerInterface.playerController.CharacterAnimator.SetBool("Interacting", false);
        DestroyTower(PlayerPreplacedTowers[0], 0);
    }

    ///Spawning Disabled Tower
    public void ClientSpawnDisabledTower(int towerPrefabIndex)
    {
        if (PlayerPreplacedTowers.Count > 0)
            return;

        SpawnDisabledTower(towerPrefabIndex);

    }

    [Command]
    private void SpawnDisabledTower(int towerPrefabIndex, NetworkConnectionToClient conn = null)
    {
        if (towerPrefabIndex >= SpawnableTowers.Count)
            return;

        GameObject preplacedTower = Instantiate(SpawnableTowers[towerPrefabIndex]);
        if (!preplacedTower.TryGetComponent(out TowerInterface towerInterface))
        {
            Debug.LogError("Tower prab does not contain a tower Interface object", SpawnableTowers[towerPrefabIndex]);
            Destroy(preplacedTower);
            return;
        }

        towerInterface.SetState(TowerInterface.State.Placing);

        NetworkServer.Spawn(preplacedTower, conn);

        CollectPreplacedTower(towerInterface);

    }

    [TargetRpc]
    private void CollectPreplacedTower(TowerInterface towerInterface)
    {
       
        PlayerPreplacedTowers.Add(towerInterface);
    }

    /// Placing Tower
    [Command]
    private void EnableDisabledTower(TowerInterface towerInterface, NetworkConnectionToClient conn = null)
    {

        if (towerInterface == null)
            return;

        if (CheckTowerPlacement(towerInterface.gameObject) && ClientMoneyController.singleton.Money >= towerInterface.tower.Cost)
        {
            TowerSpawnCoroutine = TowerBuildWaitTime(towerInterface, conn);
            towerInterface.SetState(TowerInterface.State.Building);
            ChangeClientAuthority(towerInterface, false);
            StartCoroutine(TowerSpawnCoroutine);
        } else
        {
            PlacedTowerInvalid();
        }
            
    }

    [TargetRpc]
    private void PlacedTowerInvalid()
    {
        if (OnCancelingBuildingATower != null)
            OnCancelingBuildingATower(default);

        playerInterface.SetState(PlayerInterface.State.Default);
    }

    [ServerCallback]
    private IEnumerator TowerBuildWaitTime(TowerInterface towerInterface, NetworkConnectionToClient conn = null)
    {
        yield return new WaitForSeconds(towerInterface.tower.BuildTime);
        towerInterface.SetState(TowerInterface.State.Default);
        CollectPlacedTower(towerInterface);
        ClientMoneyController.singleton.RemoveMoney(towerInterface.tower.Cost);
    }

    [TargetRpc]
    private void CollectPlacedTower(TowerInterface towerInterface)
    {
        playerInterface.SetState(PlayerInterface.State.Default);

        if (OnFinishedBuildingATower != null)
            OnFinishedBuildingATower(default);

        if (towerInterface == null)
            return;

        for (int i = 0; i < PlayerPreplacedTowers.Count; i++)
        {
            if (PlayerPreplacedTowers[i] == towerInterface)
            {
                PlayerSpawnedTowers.Add(towerInterface);
                PlayerPreplacedTowers.RemoveAt(i);
                return;
            }
        }
    }


    ///Waiting to check for server confirmation that object is destroy
    [Command]
    private void DestroyTower(TowerInterface towerInterface, int index, NetworkConnectionToClient conn = null)
    {
        if (TowerSpawnCoroutine != null)
            StopCoroutine(TowerSpawnCoroutine);

        if (towerInterface == null)
            return;

        NetworkServer.Destroy(towerInterface.gameObject);
        DestroyedTower(index);
    }

    [TargetRpc]
    private void DestroyedTower(int index)
    {
        PlayerPreplacedTowers.RemoveAt(index);
    }

    /// <summary>
    /// Checks to see if the tower is colliding or is on the ground
    /// </summary>
    /// <param name="Tower"></param>
    /// <returns>true if it can be placed</returns>
    static public bool CheckTowerPlacement(GameObject Tower)
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

    public string ErrorPlacement()
    {
        if (PlayerPreplacedTowers.Count == 0 || PlayerPreplacedTowers[0] == null)
            return default;
        
        ColliderChecker TowerChecker = PlayerPreplacedTowers[0].GetComponentInChildren<ColliderChecker>();

        if (TowerChecker == null)
            return default;

        if (TowerChecker.Collide(TowerCollisionCheckMask))
            return "Not Enough Room!";

        if (!TowerChecker.OnGround(TowerFloorCheckMask))
            return "Not fully on the ground!";

        if (ClientMoneyController.singleton.Money < PlayerPreplacedTowers[0].tower.Cost)
            return "Not enough money";

        return default;
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

    private void MovePrefabTower()
    {
        if (PlayerPreplacedTowers.Count == 0 || PlayerPreplacedTowers[0] == null)
            return;

        PlayerPreplacedTowers[0].transform.position = GetMouseWorldPosition();
        PlayerPreplacedTowers[0].transform.rotation = transform.rotation;

    }

    [ClientRpc]
    private void ChangeClientAuthority(TowerInterface towerInterface, bool hasClientControl)
    {
        if (towerInterface == null)
            return;

        towerInterface.gameObject.GetComponent<NetworkTransform>().clientAuthority = hasClientControl;

    }

    private bool CanMoveTower()
    {
        if (PlayerPreplacedTowers.Count == 0 || PlayerPreplacedTowers[0] == null)
            return false;

        if (PlayerPreplacedTowers[0].GetState() == TowerInterface.State.Building)
            return false;

        return true;

    }

    public int GetPrefabTowerIndex()
    {
        if (PlayerPreplacedTowers.Count == 0)
            return -1;

        for (int i = 0; i < SpawnableTowers.Count; i++)
        {
            if (SpawnableTowers[i] == PlayerPreplacedTowers[0])
            {
                return i;
            }
        }

        return -1;
    }

    public bool HasPrefabTower()
    {
        if (PlayerPreplacedTowers.Count == 0)
            return false;

        return true;
    }

    public delegate void MyTowerEventHandler(TowerInterface tower);
    public event MyTowerEventHandler OnStartBuildingATower;
    public event MyTowerEventHandler OnCancelingBuildingATower;
    public event MyTowerEventHandler OnFinishedBuildingATower;

}
