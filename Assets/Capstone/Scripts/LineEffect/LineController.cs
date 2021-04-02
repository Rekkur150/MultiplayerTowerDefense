using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineController : MonoBehaviour
{
    public List<Transform> ConnectedTransform;

    private LineRenderer LineRenderer;

    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        Vector3[] positions = new Vector3[ConnectedTransform.Count];

        for (int i = 0; i < ConnectedTransform.Count; i++)
        {
            positions[i] = transform.InverseTransformPoint(ConnectedTransform[i].position);
        }

        LineRenderer.SetPositions(positions);
    }

    public void SetConnectedTransforms(List<Transform> newConnectedTransforms)
    {
        ConnectedTransform = newConnectedTransforms;
    }
}
