using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MapController : NetworkBehaviour
{

    static public MapController singleton;

    [SyncVar]
    public float BuildTimeMultiplier = 1f;
    [SyncVar]
    public float TowerRefundPercentage = 0.5f;

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
}
