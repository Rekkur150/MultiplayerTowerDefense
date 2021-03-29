using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleColliderChecker : ColliderChecker
{

    public List<CapsuleCollider> CapsuleColliders = new List<CapsuleCollider>();

    public override bool Collide(LayerMask layerMask)
    {
        bool Collided = false;

        RemoveLayerFromObject();

        foreach (CapsuleCollider capCol in CapsuleColliders)
        {
            Debug.DrawLine(capCol.center + (transform.up * (capCol.height / 2)), capCol.center - (transform.up * (capCol.height / 2)), Color.red);

            if (Physics.CheckCapsule(capCol.center + (transform.up * ((capCol.height / 2) - capCol.radius)), capCol.center - (transform.up * ((capCol.height / 2) - capCol.radius)), capCol.radius, layerMask))
                Collided = true;
        }

        RestoreObjectLayer();

        return Collided;
    }
}
