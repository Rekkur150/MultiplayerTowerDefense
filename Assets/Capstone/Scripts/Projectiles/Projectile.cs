using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : ServerObject
{

    [Header("Projectile")]

    [Tooltip("The maximum time a projectile can last in seconds")]
    static private float MaxiumDuration = 10;



    [ServerCallback]
    void Start()
    {
        StartCoroutine("DestroyCountDown");
    }
    
    void FixedUpdate()
    {
        Move();
    }


    private IEnumerator DestroyCountDown()
    {
        yield return new WaitForSeconds(MaxiumDuration);
        ServerDestory();
    }

    protected void Move() {}

}
