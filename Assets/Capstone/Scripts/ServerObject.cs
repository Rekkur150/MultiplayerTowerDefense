using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerObject : NetworkBehaviour
{
    public string UniqueName { get; }

    public void ServerDestory()
    {
        OnObjectDestory();
        NetworkServer.Destroy(gameObject);
    }

    protected virtual void OnObjectDestory() {}
}
