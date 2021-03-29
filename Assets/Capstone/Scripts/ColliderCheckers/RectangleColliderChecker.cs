using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleColliderChecker : ColliderChecker
{
    [Header("Rectangle Collider Checker")]
    public List<BoxCollider> BoxColliders = new List<BoxCollider>();

    public override bool Collide(LayerMask layerMask)
    {
        bool Collided = false;

        RemoveLayerFromObject();

        foreach (BoxCollider boxCol in BoxColliders)
        {
            if (Physics.CheckBox(boxCol.center, boxCol.bounds.extents, transform.rotation, layerMask))
                Collided = true;
        }

        RestoreObjectLayer();

        return Collided;
    }
}
