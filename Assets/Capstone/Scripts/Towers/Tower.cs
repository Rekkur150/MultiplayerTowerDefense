using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Tower : Character
{
    [Header("Tower")]
    [Tooltip("Needed in order for the tower to find prey")]
    public AreaFinder AreaFinder;

    [SyncVar (hook = nameof(TowerEnabledChanged))]
    [Tooltip("Disables the tower, if need be")]
    protected bool IsTowerFunctional = true;

    public List<GameObject> GameObjectDisableOnTowerNonFunctional = new List<GameObject>();
    public List<Behaviour> BehavioursDisableOnTowerNonFunctional = new List<Behaviour>();

    [Tooltip("The delay between actions")]
    public float RateOfFire = Mathf.Infinity;

    protected bool CanAttack = true;

    protected new void Start()
    {
        base.Start();

        if (isServer)
        {
            if (AreaFinder == null)
                Debug.LogError("There is no area finder in this tower!", this);
        }

        if (!IsTowerFunctional)
            SetDisabledObjectActive(false);

    }

    [ServerCallback]
    protected IEnumerator RateOfFireDelay()
    {
        yield return new WaitForSeconds(RateOfFire);
        CanAttack = true;
    }

    public void SetTowerEnabled(bool isEnabled)
    {
        if (isEnabled)
        {
            IsTowerFunctional = true;
            SetDisabledObjectActive(true);
        } else
        {
            IsTowerFunctional = false;
            SetDisabledObjectActive(false);
        }
    }

    protected void TowerEnabledChanged(bool oldFunction, bool newFunction)
    {
        SetTowerEnabled(newFunction);
    }


    private void SetDisabledObjectActive(bool isEnabled)
    {
        foreach (Behaviour behaviour in BehavioursDisableOnTowerNonFunctional)
        {
            behaviour.enabled = isEnabled;
        }

        foreach (GameObject gameObj in GameObjectDisableOnTowerNonFunctional)
        {
            gameObj.SetActive(isEnabled);
        }
    }




}
