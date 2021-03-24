using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; //For NetworkBehaviour

public class AreaFinder : NetworkBehaviour
{
    [Header("Area Finder")]

    [Tooltip("The tag of the collider to look for and add")]
    public string TargetTag = "Enemy";

    [Tooltip("The layers that the area finder cannot see through")]
    public LayerMask CantSeeMask;

    [Tooltip("The list of target characters in the collider")]
    private List<Character> TargetCharacters = new List<Character>();

    [ServerCallback]
    void Start()
    {
        netIdentity.serverOnly = true;
        netIdentity.visible = Visibility.ForceHidden;

        if (CantSeeMask.value == 0)
            CantSeeMask = LayerMask.GetMask("Default", "JumpDisabled");

        if (TryGetComponent(out Collider collider))
            collider.isTrigger = true;
        else Debug.LogError("Collider missing!!!", this);

        gameObject.layer = LayerMask.NameToLayer("Utility");
    }

    [ServerCallback]
    void FixedUpdate()
    {
        TargetCharacters.RemoveAll(item => item == null);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == TargetTag && other.TryGetComponent(out Character character))
        {
            TargetCharacters.Add(character);
        }
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)
    {
        if (other.tag == TargetTag && other.TryGetComponent(out Character character))
        {
            TargetCharacters.Remove(character);
        }
    }

    [ServerCallback]
    public List<Character> GetCharacters()
    {
        return TargetCharacters;
    }

    [ServerCallback]
    public bool HasTargets()
    {
        if (TargetCharacters.Count > 0)
            return true;

        return false;
    }

    [ServerCallback]
    public bool IsStillTarget(Character target)
    {
        if (TargetCharacters.Contains(target))
            return true;

        return false;
    }

    [ServerCallback]
    public Character GetClosestTarget(Vector3 position)
    {
        return GetClosestTargetHelper(position, false);
    }

    [ServerCallback]
    public Character GetClosestTargetInSight(Vector3 position)
    {
        return GetClosestTargetHelper(position, true);
    }

    [ServerCallback]
    private Character GetClosestTargetHelper(Vector3 position, bool NeedLineOfSight)
    {
        float distance = Mathf.Infinity;
        Character closest = null;

        TargetCharacters.RemoveAll(item => item == null);

        foreach (Character character in TargetCharacters)
        {
            float characterDistance = Vector3.Distance(position, character.gameObject.transform.position);

            if (characterDistance < distance)
            {
                if (!NeedLineOfSight || !Physics.Linecast(position, character.gameObject.transform.position, CantSeeMask))
                {
                    closest = character;
                    distance = characterDistance;
                }
            }
        }

        return closest;
    }
}
