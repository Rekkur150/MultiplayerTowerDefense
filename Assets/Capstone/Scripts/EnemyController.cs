using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : Character
{
    [Header("Enemy Controller")]
    public List<GameObject> SpawnOnDestroy = new List<GameObject>();
    public AreaFinder playerFinder;
    public AreaFinder towerFinder;
    public Animator Animator;
    public DamageObject damageObject;

    public float ManaDroppedOnDeath = 0;

    private NavMeshAgent navAgent;
    private Vector3 target;
    private Vector3 goal;

    [ServerCallback]
    // Start is called before the first frame update
    void Start()
    {
        base.Awake();
        navAgent = GetComponent<NavMeshAgent>();

        if (Animator == null)
            Debug.LogWarning("No Animator on this object!", this);

        if (damageObject == null)
            Debug.LogWarning("No damage Object on this object!", this);

        FindGoal();

        navAgent.stoppingDistance = .76f;
    }

    // Update is called once per frame
    [ServerCallback]
    void FixedUpdate()
    {
        FindTarget();
        CheckAttackRange();
        UpdateAnimation();
    }

    void FindGoal()
    {
        GameObject goalObject;

        goalObject = GameObject.FindGameObjectWithTag("Goal");

        if (goalObject != null)
        {
            goal = goalObject.transform.position;
            navAgent.destination = goal;
        }
        else
        {
            Debug.Log("No goal found");
        }
    }

    private void UpdateAnimation()
    {
        Vector3 Velocity = transform.InverseTransformDirection(navAgent.velocity);
        Vector2 xyVelocity = new Vector2(Velocity.x, Velocity.z);
        xyVelocity = xyVelocity.normalized;
        Animator.SetFloat("Side", xyVelocity.x);
        Animator.SetFloat("Forward", xyVelocity.y);
    }

    [ServerCallback]
    private void FindTarget()
    {
        Character player = playerFinder.GetClosestTarget(transform.position);
        Character tower = towerFinder.GetClosestTarget(transform.position);

        if (tower != null && tower.GetComponent<TowerInterface>().GetState() == TowerInterface.State.Default) 
        {
            target = tower.transform.position;
        }
        else if (player != null)
        {
            target = player.transform.position;
        }
        else
        {
            target = goal;
        }

/*        Debug.Log(tower);*/

        navAgent.destination = target;
    }

    [ServerCallback]
    protected override void OnObjectDestroy()
    {
        SpawnOnDestruction();
    }

    [ServerCallback]
    private void SpawnOnDestruction()
    {
        foreach (GameObject obj in SpawnOnDestroy)
        {
            GameObject newObject = Instantiate(obj);
            newObject.transform.position = transform.position;

            NetworkServer.Spawn(newObject);
        }
    }

    [ServerCallback]
    protected override void Died()
    {
        WaveManager.singleton.currentWaveEnemies.Remove(gameObject);
        WaveManager.singleton.isFinishedSpawning();


        if (ManaDroppedOnDeath > 0)
            ManaDropper.singleton.SpawnMana(ManaDroppedOnDeath, transform);

        ServerDestroy();
    }

    [ServerCallback]
    private void CheckAttackRange()
    {
        if (Vector3.Distance(this.transform.position, target) <= 3)
        {
            AnimatorSetBool("Attacking", true);
            damageObject.IsEnabled = true;
            FaceTarget(target);
        }
        else
        {
            AnimatorSetBool("Attacking", false);
            damageObject.IsEnabled = false;
        }
    }

    [ClientRpc]
    private void AnimatorSetBool(string name, bool value)
    {
        Animator.SetBool(name, value);
    }

    [ServerCallback]
    private void FaceTarget(Vector3 target)
    {
        Vector3 lookPos = target - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, .1f);
    }
}
