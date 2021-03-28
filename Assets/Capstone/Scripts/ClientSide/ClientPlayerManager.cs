using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientPlayerManager : NetworkBehaviour
{
    public static ClientPlayerManager singleton;

    [Tooltip("The local player's character")]
    public Character PlayerCharacter;

    [Header("Player Death")]
    [Tooltip("The gameobject that will be given to the player on death, aka spectator controller or something")]
    public GameObject PlayerDeathPrefab;

    [SyncVar]
    [Tooltip("The time to respawn in seconds")]
    public float RespawnTime = 5;

    private void Start()
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

}
