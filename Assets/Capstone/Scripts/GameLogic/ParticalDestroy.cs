using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ParticalDestroy : ServerObject
{
    public float Time = 10f;

    [ServerCallback]
    void Start()
    {
        StartCoroutine(TimedDestroy());
    }

    [ServerCallback]
    private IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(Time);
        ServerDestroy();
    }

    
}
