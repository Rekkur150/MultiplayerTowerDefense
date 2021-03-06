using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientPlayerManager : NetworkBehaviour
{
    public static ClientPlayerManager singleton;

    public Transform SpectatorSpawnLocation; 

    [Tooltip("The local player's character")]
    private Character playerCharacter;
    public Character PlayerCharacter
    {
        get {
            return playerCharacter;
        }

        set {
            playerCharacter = value;
            ChangedPlayerCharacter(value);
        }
    }

    [Header("Player Death")]
    [Tooltip("The gameobject that will be given to the player on death, aka spectator controller or something")]
    public GameObject PlayerDeathPrefab;

    [SyncVar]
    [Tooltip("The time to respawn in seconds")]
    public float RespawnTime = 5;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        } else if (singleton != this)
        {
            Destroy(this);
        }
    }

    [ServerCallback]
    public void ClientCharacterDied(GameObject PlayerGameObject)
    {
        GameObject PlayerDeathObject = Instantiate(PlayerDeathPrefab);

        if (SpectatorSpawnLocation != null)
            PlayerDeathObject.transform.position = SpectatorSpawnLocation.position;

        NetworkServer.Spawn(PlayerDeathObject, PlayerGameObject.GetComponent<NetworkIdentity>().connectionToClient);

        GameObjectActive(PlayerGameObject, false);
        StartCoroutine(ClientRespawnRespite(PlayerGameObject, PlayerDeathObject));
    }



    [ServerCallback]
    private IEnumerator ClientRespawnRespite(GameObject PlayerGameObject, GameObject PlayerDeathObject)
    {
        yield return new WaitForSeconds(RespawnTime);
        ClientRespawn(PlayerGameObject, PlayerDeathObject);
    }

    [ServerCallback]
    private void ClientRespawn(GameObject PlayerGameObject, GameObject PlayerDeathObject)
    {
        if (PlayerGameObject.TryGetComponent(out Character character))
            character.SetHealth(character.MaxHealth);

        if (PlayerDeathObject.TryGetComponent(out ServerObject serverObject))
            serverObject.ServerDestroy();

        GameObjectActive(PlayerGameObject, true);
    }

    [ClientRpc]
    public void GameObjectActive(GameObject gameObject, bool state)
    {
        if (gameObject != null)
            gameObject.SetActive(state);
    }

    private void ChangedPlayerCharacter(Character player)
    {
        if (OnPlayerCharacterUpdate != null)
            OnPlayerCharacterUpdate(player);

        NetworkPlayerManager.singleton.PlayerCharacterUpdated(playerCharacter);
    }


    public delegate void PlayerCharacterChangedHandler(Character player);
    public event PlayerCharacterChangedHandler OnPlayerCharacterUpdate;

}
