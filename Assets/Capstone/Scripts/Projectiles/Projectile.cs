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

    [Tooltip("Destoryu the projectile when it hits terrain")]
    public bool DestoryWhenColliding = true;

    [Tooltip("The tag this projectile will look when looking at hit characters in order to be destroyed")]
    public List<string> TargetTags = new List<string>();

    [Tooltip("The maximum time a projectile can last in seconds")]
    static private float MaxiumDuration = 10;

    [ServerCallback]
    protected void Start()
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
    protected void OnTriggerEnter(Collider other)
    {

        GameObject parent = GetRootCharacter(other);

        if (DestroyOnCharacterHit && TargetTags.Contains(other.tag) && TargetTags.Contains(parent.tag))
        {
            if (parent.TryGetComponent(out Character character))
            {

                ServerDestroy();
            }
        }
    }

    [ServerCallback]
    protected void OnTriggerExit(Collider other)
    {
        if (DestoryWhenColliding && CollisionMask == (CollisionMask | (1 << other.gameObject.layer)))
        {
            ServerDestroy();
        }
            
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

    private GameObject GetRootCharacter(Collider other)
    {
        Transform current = other.transform;

        while (current.parent != null)
        {
            current = current.parent;
        }

        return current.gameObject;
    }

}
