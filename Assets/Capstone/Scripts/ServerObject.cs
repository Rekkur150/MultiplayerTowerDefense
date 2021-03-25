using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerObject : NetworkBehaviour
{
    public string UniqueName { get; }

    public void ServerDestroy()
    {
        OnObjectDestroy();
        NetworkServer.Destroy(gameObject);
    }

    protected virtual void OnObjectDestroy() {}
}
