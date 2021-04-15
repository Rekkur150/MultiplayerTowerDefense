using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ManaDropper : NetworkBehaviour
{

    public GameObject manaPrefab;

    static public ManaDropper singleton;

    private List<GameObject> manas = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }

    [ServerCallback]
    public void SpawnMana(float value, Transform trans)
    {
        GameObject manaGameObject = Instantiate(manaPrefab);
        ManaPickup manaPickup = manaGameObject.GetComponent<ManaPickup>();
        manaPickup.SetValue(value);
        manaGameObject.transform.position = trans.position;

        manas.Add(manaGameObject);

        NetworkServer.Spawn(manaGameObject);
    }
    
    [ServerCallback]
    public void DestroyMana(GameObject manaObject)
    {
        manas.Remove(manaObject);
        NetworkServer.Destroy(manaObject);
    }
}
