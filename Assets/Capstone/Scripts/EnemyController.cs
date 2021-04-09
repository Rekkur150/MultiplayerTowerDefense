using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : Character
{
    private NavMeshAgent navAgent;
    private Vector3 target;
    private Vector3 goal;
    public AreaFinder playerFinder;
    public AreaFinder towerFinder;
    public Animator Animator;

    [ServerCallback]
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        if (Animator == null)
            Debug.LogWarning("No Animator on this object!", this);

        FindGoal();
    }

    // Update is called once per frame
    [ServerCallback]
    void FixedUpdate()
    {
        FindTarget();
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

    private void FindTarget()
    {
        Character player = playerFinder.GetClosestTarget(transform.position);
        Character tower = towerFinder.GetClosestTarget(transform.position);

        if (tower != null)
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

        Debug.Log(tower);

        navAgent.destination = target;
    }
}
