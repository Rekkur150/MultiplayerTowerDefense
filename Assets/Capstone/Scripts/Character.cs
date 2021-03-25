using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Character : ServerObject
{
    [Header("Character Properties")]
    public float MaxHealth;

    [SyncVar]
    private float Health;

    [Tooltip("Used for aiming purposes")]
    public Transform CharacterCenter;
    [Tooltip("Used for accurate aiming")]
    protected Vector3 Velocity;

    [ServerCallback]
    protected void Start()
    {
        if (CharacterCenter == null)
            CharacterCenter = transform;

        Health = MaxHealth;
    }

    [Command(requiresAuthority = false)]
    public void SetHealth(float health)
    {
        Health = health;

        if (Health <= 0)
            Died();
    }

    public void Damage(float health)
    {
        SetHealth(Health - health);
    }

    [Command]
    public void SetVelocity(Vector3 velocity)
    {
        Velocity = velocity;
    }

    public Vector3 GetVelocity()
    {
        return Velocity;
    }

    protected virtual void Died() {}
}
