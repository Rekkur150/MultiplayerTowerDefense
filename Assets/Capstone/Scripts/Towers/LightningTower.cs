using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LightningTower : AOETower
{
    [Header("Lightning Tower")]

    public float DamagePerAttack = 0.1f;

    [ServerCallback]
    protected override void LaunchAttack()
    {
        List <Character> targetCharacters= AreaFinder.GetCharacters();

        foreach (Character character in targetCharacters) {
            character.Damage(DamagePerAttack);
        }
    }

}
