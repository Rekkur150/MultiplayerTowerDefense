using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CannonBall : Projectile
{
    [ServerCallback]
    protected new void Start()
    {
        base.Start();

        Rigidbody temp = GetComponent<Rigidbody>();
        temp.isKinematic = false;
        temp.useGravity = true;

        temp.AddForce(transform.forward * 5, ForceMode.VelocityChange);

    }

    [ServerCallback]
    protected override void Move() {}

}
