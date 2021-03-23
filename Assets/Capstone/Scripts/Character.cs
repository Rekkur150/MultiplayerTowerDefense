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

    protected void Start()
    {
        if (isServer)
            Health = MaxHealth;
    }

    [Command(requiresAuthority = false)]
    public void SetHealth(float health)
    {
        Health = health;

        if (Health <= 0)
            CharacterDied();
    }

    public void DamageCharacter(float health)
    {
        SetHealth(Health - health);
    }

    protected virtual void CharacterDied() {}
}
