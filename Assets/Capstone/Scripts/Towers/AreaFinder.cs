using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; //For NetworkBehaviour

public class AreaFinder : MonoBehaviour
{
    [Header("Area Finder")]

    [Tooltip("The tag of the collider to look for and add")]
    public string TargetTag = "Enemy";

    [Tooltip("The layers that the area finder cannot see through")]
    public LayerMask CantSeeMask;

    [Tooltip("The list of target characters in the collider")]
    private List<Character> TargetCharacters = new List<Character>();

    [HideInInspector]
    public Collider AreaFinderCollider;

    protected void Awake()
    {

        if (CantSeeMask.value == 0)
            CantSeeMask = LayerMask.GetMask("Default", "JumpDisabled");

        if (TryGetComponent(out Collider collider))
        {
            AreaFinderCollider = collider;
            collider.isTrigger = true;
        } else Debug.LogError("Collider missing!!!", this);

        gameObject.layer = LayerMask.NameToLayer("Utility");
    }

    protected void FixedUpdate()
    {
        TargetCharacters.RemoveAll(item => item == null);
        TargetCharacters.RemoveAll(item => item.gameObject.activeInHierarchy == false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        GameObject rootGameObject = GetRootCharacter(other);

        if (rootGameObject.tag == TargetTag && rootGameObject.TryGetComponent(out Character character))
        {
            if (TargetCharacters.Contains(character))
                return;

            TargetCharacters.Add(character);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        GameObject rootGameObject = GetRootCharacter(other);

        if (rootGameObject.tag == TargetTag && rootGameObject.TryGetComponent(out Character character))
        {
            TargetCharacters.Remove(character);
        }
    }

    public List<Character> GetCharacters()
    {
        return TargetCharacters;
    }
    public int GetCharacterCount()
    {
        return TargetCharacters.Count;
    }

    public bool HasTargets()
    {
        if (TargetCharacters.Count > 0)
            return true;

        return false;
    }

    public bool IsStillTarget(Character target)
    {
        if (TargetCharacters.Contains(target))
            return true;

        return false;
    }

    public Character GetClosestTarget(Vector3 position)
    {
        return GetClosestTargetHelper(position, false);
    }

    public Character GetClosestTargetInSight(Vector3 position)
    {
        return GetClosestTargetHelper(position, true);
    }

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
