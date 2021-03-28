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

    protected bool CanAttack = true;

    [ServerCallback]
    protected new void Start()
    {
        base.Start();

        if (AreaFinder == null)
            Debug.LogError("There is no area finder in this tower!", this);

    }

    [ServerCallback]
    protected IEnumerator RateOfFireDelay()
    {
        yield return new WaitForSeconds(RateOfFire);
        CanAttack = true;
    }







}
