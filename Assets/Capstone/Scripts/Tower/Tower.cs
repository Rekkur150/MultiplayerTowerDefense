using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Tower : Character
{
    [Header("Tower")]
    [Tooltip("Needed in order for the tower to find prey")]
    public AreaFinder AreaFinder;

    [Tooltip("Disables the tower, if need be")]
    public bool IsTowerFunctional = true;

    [Tooltip("The delay between actions")]
    public float RateOfFire = Mathf.Infinity;

    [SyncVar]
    [HideInInspector]
    public Character nextTarget;
    private bool CanAttack = true;

    [ServerCallback]
    protected new void Start()
    {
        base.Start();

        if (AreaFinder == null)
            Debug.LogError("There is no area finder in this tower!", this);

    }

    [ServerCallback]
    protected void FixedUpdate()
    {

        if (!IsTowerFunctional)
            return;

        FindNextTarget();

        if (nextTarget != null && CanAttack)
            AttemptAttack();
    }

    protected void FindNextTarget()
    {
        nextTarget = AreaFinder.GetClosestTargetInSight(transform.position);
    }

    [ServerCallback]
    private void AttemptAttack()
    {
        CanAttack = false;
        LaunchAttack(nextTarget);
        StartCoroutine("RateOfFireDelay");
    }

    [ServerCallback]
    private IEnumerator RateOfFireDelay()
    {
        yield return new WaitForSeconds(RateOfFire);
        CanAttack = true;
    }

    [ServerCallback]
    protected virtual void LaunchAttack(Character target) {}






}
