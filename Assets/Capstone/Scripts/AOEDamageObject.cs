using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(AreaFinder))]
public class AOEDamageObject : MonoBehaviour
{
    [Header("Damage Object")]
    [Tooltip("Can this object do damage to other objects")]
    public bool IsEnabled = true;
    [Tooltip("The tag it looks for on a collider's gameobject when determining to damage character")]
    public string TargetTag;

    [Header("In Damage")]
    [Tooltip("The amount of damage it does to other characters, when they stay in the collider")]
    public float InDamage = 0;
    [Tooltip("The amount of time between InDamage activation in seconds")]
    public float InRate = 0;
    [Tooltip("Boolean to keep tract of InRate")]
    private bool CanInDamage = true;
    private AreaFinder AreaFinder;

    [ServerCallback]
    void Start()
    {
        AreaFinder = GetComponent<AreaFinder>();
        AreaFinder.TargetTag = TargetTag;
    }

    [ServerCallback]
    void FixedUpdate()
    {
        if (IsEnabled && InDamage != 0 && CanInDamage)
        {
            List<Character> temp = AreaFinder.GetCharacters();

            foreach (Character character in temp)
            {
                character.Damage(InDamage);
            }

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
