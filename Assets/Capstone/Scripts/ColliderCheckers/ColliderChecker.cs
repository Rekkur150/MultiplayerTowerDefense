using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ColliderChecker : MonoBehaviour
{
    /// <summary>
    /// To keep tract of what layers each object was.
    /// </summary>
    public struct LayerLinker
    {
        public GameObject gameObject;
        public LayerMask layer;
    }

    [Header("ColliderChecker")]
    [Tooltip("The points that will be check to see if there is ground around")]
    public List<Transform> Groundpoints = new List<Transform>();
    [Tooltip("Will assume it is the transform with no parent if null")]
    public Transform TopParent;

    private List<LayerLinker> LayerLinkers = new List<LayerLinker>();

    private void Start()
    {
        if (TopParent == null)
            TopParent = GetTopParent();
    }

    public bool OnGround(LayerMask layerMask)
    {
        foreach (Transform trans in Groundpoints)
        {
            if (!Physics.CheckSphere(trans.position, 0.05f, layerMask))
                return false;
        }

        return true;
    }

    public bool TouchingGround(LayerMask layerMask)
    {
        foreach (Transform trans in Groundpoints)
        {
            if (Physics.CheckSphere(trans.position, 0.05f, layerMask))
                return true;
        }

        return false;
    }
    public abstract bool Collide(LayerMask layerMask);

    protected void RemoveLayerFromObject()
    {
        LayerLinkers.Clear();
        HelperRemoveLayerFromObject(TopParent);

    }
    protected void HelperRemoveLayerFromObject(Transform newTransform)
    {
        foreach (Transform trans in newTransform)
        {
            if (trans == null)
                continue;

            LayerLinker temp = new LayerLinker();
            temp.gameObject = trans.gameObject;
            temp.layer = temp.gameObject.layer;

            LayerLinkers.Add(temp);

            trans.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            HelperRemoveLayerFromObject(trans);

        }
    }

    protected void RestoreObjectLayer()
    {
        foreach (LayerLinker layerLink in LayerLinkers)
        {
            layerLink.gameObject.layer = layerLink.layer;
        }
    }

    protected Transform GetTopParent()
    {
        Transform currentTransform = transform;

        while (currentTransform.parent != null)
        {
            currentTransform = currentTransform.parent;
        }

        return currentTransform;
    }
}
