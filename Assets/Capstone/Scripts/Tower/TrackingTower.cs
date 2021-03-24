using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tower))]
public class TrackingTower : MonoBehaviour
{
    [Header("Tracking Tower")]

    [Tooltip("This is the horizontally rotated object")]
    public Transform Turret;

    [Tooltip("This is the vertically rotated object")]
    public Transform Barrel;

    private Tower Tower;

    void Start()
    {
        Tower = GetComponent<Tower>();

        if (Barrel.parent != Turret)
            Barrel.parent = Turret;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Tower.nextTarget == null)
            return;

        Vector3 tempPosition = Tower.nextTarget.gameObject.transform.position;
        tempPosition.y = Turret.position.y;
        Turret.LookAt(tempPosition);

        Barrel.LookAt(Tower.nextTarget.gameObject.transform.position + new Vector3(0,1,0));
    }
}
