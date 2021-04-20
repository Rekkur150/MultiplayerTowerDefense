using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ProjectileTower : Tower
{
    [Header("Projectile Tower")]
    public GameObject Projectile;
    public Transform ProjectileSpawnLocation;

    [Tooltip("The predicted location of the target")]
    [SyncVar]
    [HideInInspector]
    public Vector3 PredictedTargetLocation;
    [SyncVar]
    [HideInInspector]
    public Character Target;

    private float ProjectileSpeed;

    [ServerCallback]
    private new void Awake()
    {
        base.Awake();

        if (Projectile.TryGetComponent(out Projectile projectile))
            ProjectileSpeed = projectile.Speed;
        else ProjectileSpeed = Mathf.Infinity;
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (!IsTowerFunctional)
            return;

        FindNextTarget();

        if (Target != null)
            AttemptAttack();
    }

    [ServerCallback]
    private void AttemptAttack()
    {

        if (CanAttack)
        {
            CanAttack = false;
            LaunchAttack(Target);
            StartCoroutine("RateOfFireDelay");
        }
    }

    [ServerCallback]
    protected virtual void FindNextTarget()
    {
        Target = AreaFinder.GetClosestTargetInSight(transform.position);

        if (Target != null)
            PredictedTargetLocation = GetPredictedPosition(transform.position, ProjectileSpeed, Target);
    }

    private static Vector3 GetPredictedPosition(Vector3 towerPosition, float projectileSpeed, Character target)
    {
        float distance = Vector3.Distance(towerPosition, target.CharacterCenter.position);

        float time = distance / projectileSpeed;

        Vector3 expectedTravelLocation = target.GetVelocity() * time;

        Debug.DrawRay(towerPosition, target.CharacterCenter.position + expectedTravelLocation, Color.white);

        return target.CharacterCenter.position + expectedTravelLocation;
    }

    [ServerCallback]
    protected virtual void LaunchAttack(Character target)
    {

        GameObject newGameObject = Instantiate(Projectile);
        newGameObject.transform.position = ProjectileSpawnLocation.position;

        newGameObject.transform.LookAt(PredictedTargetLocation);

        NetworkServer.Spawn(newGameObject);
    }

}
