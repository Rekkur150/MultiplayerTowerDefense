using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tower))]
public class LineEffectTower : MonoBehaviour
{

    public GameObject LineEffectPrefab;

    [Tooltip("The location where the line effect will start")]
    public Transform TowerBeginningPoint;

    private List<LineController> LineEffects = new List<LineController>();
    private AreaFinder AreaFinder;

    private void Start()
    {

        TryGetComponent(out Tower tower);
        AreaFinder = tower.AreaFinder;

    }

    void FixedUpdate()
    {
        MirrorNumberOfLineEffectsWithAreaFinder();
        AttachLineEffectsToCharacters();
    }

    private void AttachLineEffectsToCharacters()
    {
        List<Character> characters = AreaFinder.GetCharacters();

        for (int i = 0; i < characters.Count; i++)
        {

            List<Transform> transforms = new List<Transform>();
            transforms.Add(TowerBeginningPoint);
            transforms.Add(characters[i].CharacterCenter);

            LineEffects[i].SetConnectedTransforms(transforms);
        }

    }

    private void MirrorNumberOfLineEffectsWithAreaFinder()
    {
        int CharacterCount = AreaFinder.GetCharacterCount();

        if (CharacterCount > LineEffects.Count)
        {
            int difference = CharacterCount - LineEffects.Count;
            for (int i = 0; i < difference; i++)
            {
                GameObject newLineEffect = Instantiate(LineEffectPrefab, transform);
                newLineEffect.transform.position = transform.position;

                LineController newLineController = newLineEffect.GetComponent<LineController>();
                LineEffects.Add(newLineController);
            }
        } else
        {
            int difference = LineEffects.Count - CharacterCount;
            for (int i = 0; i < difference; i++)
            {
                Destroy(LineEffects[i].gameObject);
                LineEffects.RemoveAt(i);
                i--;
                difference--;
            }
        }
    }
}
