using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class EnemyController : NetworkBehaviour
{
    private NavMeshAgent navAgent;
    private Transform goal;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            navAgent = GetComponent<NavMeshAgent>();
            FindGoal(goal);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FindGoal(Transform goal)
    {
        GameObject goalObject;

        goalObject = GameObject.FindGameObjectWithTag("Goal");

        if (goalObject != null)
        {
            goal = goalObject.transform;
            navAgent.destination = goal.position;
        }
        else
        {
            Debug.Log("No goal found");
        }
    }
}
