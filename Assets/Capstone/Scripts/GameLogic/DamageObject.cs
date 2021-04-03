using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DamageObject : ServerObject
{
    [Header("Damage Object")]
        [Tooltip("Can this object do damage to other objects")]
        public bool IsEnabled = true;
        [Tooltip("The tag it looks for on a collider's gameobject when determining to damage character")]
        public string TargetTag;

    [Header("Enter Damage")]
        [Tooltip("The amount of damage it does to other characters, when a collider enters")]
        public float EnterDamage = 2;
    
    [Header("Exit Damage")]
        [Tooltip("The amount of damage it does to other characters, when they exit the collider")]
        public float ExitDamage = 0;

    [ServerCallback]
    void Start()
    {
        if (!TryGetComponent(out Collider collider))
            Debug.LogError("There is no collider attached to this damage object", this);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (IsEnabled && EnterDamage != 0 && other.tag == TargetTag && other.TryGetComponent(out Character character))
        {
            character.Damage(EnterDamage);
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)
    {
        if (IsEnabled && ExitDamage != 0 && other.tag == TargetTag && other.TryGetComponent(out Character character))
        {
            character.Damage(ExitDamage);
        }
    }
}
