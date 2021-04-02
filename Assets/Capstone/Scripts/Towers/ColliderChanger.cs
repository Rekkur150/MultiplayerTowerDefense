using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChanger : MonoBehaviour
{

    public struct ColliderLinker
    {
        public ColliderLinker(Collider collider, bool enabled)
        {
            this.collider = collider;
            this.enabled = enabled;
        }

        public Collider collider;
        public bool enabled;
    }

    public List<ColliderLinker> ColliderLinkers;

    public List<Collider> IgnoredColliders = new List<Collider>();

    // Start is called before the first frame update
    void Awake()
    {
        GetDefaultCollider();
    }

    private void GetDefaultCollider()
    {
        List<Collider> Colliders = new List<Collider>();

        MeshCollider[] MeshColliders = gameObject.GetComponentsInChildren<MeshCollider>();

        foreach (MeshCollider mc in MeshColliders)
        {
            Colliders.Add(mc);
        }


        List<ColliderLinker> colliderLinkers = new List<ColliderLinker>();

        foreach (Collider collider in Colliders)
        {
            colliderLinkers.Add(new ColliderLinker(collider, collider.enabled));
        }

        ColliderLinkers = colliderLinkers;


    }

    public void ChangeCollider(bool Enabled)
    {
        if (ColliderLinkers == null)
            GetDefaultCollider();

        foreach (ColliderLinker colliderLink in ColliderLinkers)
        {
            Debug.Log(colliderLink);
            colliderLink.collider.enabled = Enabled;
        }
    }

    public void RevertCollider()
    {
        if (ColliderLinkers == null)
            return;


        foreach (ColliderLinker colliderLink in ColliderLinkers)
        {
            colliderLink.collider.enabled = colliderLink.enabled;
        }
    }
}
