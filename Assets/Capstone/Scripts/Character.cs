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

    [ServerCallback]
    protected void Start()
    {
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

    protected virtual void Died() {}
}
