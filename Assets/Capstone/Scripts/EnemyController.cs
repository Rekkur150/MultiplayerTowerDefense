using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class EnemyController : Character
{
    private NavMeshAgent navAgent;
    private Vector3 target;
    private Vector3 goal;
    public AreaFinder playerFinder;
    public AreaFinder towerFinder;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            navAgent = GetComponent<NavMeshAgent>();
            FindGoal();
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
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

    private void FindTarget()
    {
        Character player = playerFinder.GetClosestTarget(transform.position);
        Character tower = towerFinder.GetClosestTarget(transform.position);

        if (player != null)
        {
            target = player.transform.position;
        }
        else if (tower != null)
        {
            target = tower.transform.position;
        }
        else
        {
            target = goal;
        }

        Debug.Log(tower);

        navAgent.destination = target;
    }
}
