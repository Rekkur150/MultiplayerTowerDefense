using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AOETower : Tower
{
    [ServerCallback]
    protected void FixedUpdate()
    {
        if (!IsTowerFunctional)
            return;

        AttemptAttack();
    }

    [ServerCallback]
    private void AttemptAttack()
    {
        if (CanAttack)
        {
            CanAttack = false;
            LaunchAttack();
            StartCoroutine("RateOfFireDelay");
        }
    }

    [ServerCallback]
    protected virtual void LaunchAttack() { }

}
