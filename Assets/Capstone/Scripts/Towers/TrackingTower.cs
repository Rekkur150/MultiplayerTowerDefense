using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileTower))]
public class TrackingTower : MonoBehaviour
{
    [Header("Tracking Tower")]

    [Tooltip("This is the horizontally rotated object")]
    public Transform Turret;

    [Tooltip("This is the vertically rotated object")]
    public Transform Barrel;

    private ProjectileTower ProjectileTower;

    void Start()
    {
        ProjectileTower = GetComponent<ProjectileTower>();

        if (Barrel == null || Turret == null)
            return;

        if (Barrel.parent != Turret)
            Barrel.parent = Turret;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ProjectileTower.Target == null)
            return;

        Vector3 tempPosition = ProjectileTower.PredictedTargetLocation;
        tempPosition.y = Turret.position.y;
        
        if (Turret != null)
            Turret.LookAt(tempPosition);

        if (Barrel != null)
            Barrel.LookAt(ProjectileTower.PredictedTargetLocation);
    }
}
