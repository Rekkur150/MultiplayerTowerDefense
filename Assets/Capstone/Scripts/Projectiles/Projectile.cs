using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkTransform))]
public class Projectile : ServerObject
{

    [Header("Projectile")]
    public float Speed;

    [Tooltip("What the projectile can collide with and be destroyed by")]
    public LayerMask CollisionMask;

    [Header("Destroy On Hit")]
    [Tooltip("Destroy projectile if it hits a character")]
    public bool DestroyOnCharacterHit = true;

    [Tooltip("The tag this projectile will look when looking at hit characters in order to be destroyed")]
    public string TargetTag;

    [Tooltip("The maximum time a projectile can last in seconds")]
    static private float MaxiumDuration = 10;

    [ServerCallback]
    void Start()
    {
        StartCoroutine("DestroyCountDown");

        Rigidbody temp = GetComponent<Rigidbody>();
        temp.isKinematic = true;
        temp.useGravity = false;

        if (CollisionMask.value == 0)
                CollisionMask = LayerMask.GetMask("Default", "JumpDisabled");
    }

    [ServerCallback]
    void FixedUpdate()
    {
        Move();
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (DestroyOnCharacterHit && other.tag == TargetTag)
        {
            if (other.TryGetComponent(out Character character))
            {
                ServerDestroy();
            }
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)
    {
        if (CollisionMask == (CollisionMask | (1 << other.gameObject.layer)))
            ServerDestroy();
    }

    [ServerCallback]
    private IEnumerator DestroyCountDown()
    {
        yield return new WaitForSeconds(MaxiumDuration);
        ServerDestroy();
    }

    [ServerCallback]
    protected virtual void Move()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

}
