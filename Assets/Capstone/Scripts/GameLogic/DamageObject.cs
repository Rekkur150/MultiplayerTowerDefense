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
    public List<string> TargetTags = new List<string>();

    [Header("Enter Damage")]
        [Tooltip("The amount of damage it does to other characters, when a collider enters")]
        public float EnterDamage = 2;
    
    [Header("Exit Damage")]
        [Tooltip("The amount of damage it does to other characters, when they exit the collider")]
        public float ExitDamage = 0;

    private List<Character> Characters = new List<Character>();

    [ServerCallback]
    void Start()
    {
        if (!TryGetComponent(out Collider collider))
            Debug.LogError("There is no collider attached to this damage object", this);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        GameObject root = GetRootCharacter(other);

        if (IsEnabled && EnterDamage != 0 && TargetTags.Contains(root.tag) && root.TryGetComponent(out Character character))
        {
            if (!Characters.Contains(character))
            {
                Characters.Add(character);
                character.Damage(EnterDamage);
            }
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)
    {
        GameObject root = GetRootCharacter(other);

        if (TargetTags.Contains(root.tag) && root.TryGetComponent(out Character character))
        {
            if (Characters.Contains(character))
                Characters.Remove(character);

            if (IsEnabled && ExitDamage != 0)
                character.Damage(ExitDamage);

        }
    }

    [ServerCallback]
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
