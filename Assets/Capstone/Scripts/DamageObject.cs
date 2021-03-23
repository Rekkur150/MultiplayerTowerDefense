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

    [Header("In Damage")]
        [Tooltip("The amount of damage it does to other characters, when they stay in the collider")]
        public float InDamage = 0;
        [Tooltip("The amount of time between InDamage activation in seconds")]
        public float InRate = 0;
        [Tooltip("Boolean to keep tract of InRate")]
        private bool CanInDamage = true;

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (EnterDamage != 0 && other.tag == TargetTag && other.TryGetComponent(out Character character))
        {
            character.Damage(EnterDamage);
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)
    {
        if (ExitDamage != 0 && other.tag == TargetTag && other.TryGetComponent(out Character character))
        {
            character.Damage(ExitDamage);
        }
    }

    [ServerCallback]
    void OnTriggerStay(Collider other) //TODO Change this because there is a possible issue with this being only called on fixedupdate, so the rate may be too inconsistent, if is less than like 50ms, I would be fine with it
    {
        if (InDamage != 0 && CanInDamage && other.tag == TargetTag && other.TryGetComponent(out Character character))
        {
            character.Damage(InDamage);
            StartCoroutine("InDamageCooldown");
        }
    }

    private IEnumerator InDamageCooldown()
    {
        CanInDamage = false;
        yield return new WaitForSeconds(InRate);
        CanInDamage = true;
    }

}
