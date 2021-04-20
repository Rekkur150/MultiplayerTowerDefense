using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleColliderChecker : ColliderChecker
{

    public List<CapsuleCollider> CapsuleColliders = new List<CapsuleCollider>();
    public List<BoxCollider> BoxColliders = new List<BoxCollider>();

    public override bool Collide(LayerMask layerMask)
    {
        bool Collided = false;

        RemoveLayerFromObject();

        foreach (CapsuleCollider capCol in CapsuleColliders)
        {

            if (Physics.CheckCapsule(capCol.transform.position + capCol.center + (transform.up * ((capCol.height / 2) - capCol.radius)), capCol.transform.position + capCol.center - (transform.up * ((capCol.height / 2) - capCol.radius)), capCol.radius, layerMask))
                Collided = true;
        }

        foreach (BoxCollider boxCol in BoxColliders)
        {

            Debug.DrawLine(boxCol.transform.position + boxCol.center, boxCol.transform.position + boxCol.size / 2f, Color.white);

            if (Physics.CheckBox(boxCol.transform.position + boxCol.center, (boxCol.size / 2f), boxCol.transform.rotation, layerMask))
                Collided = true;
        }

        RestoreObjectLayer();

        return Collided;
    }
}
