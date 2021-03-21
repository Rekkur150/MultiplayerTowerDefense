using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    /// <summary>
    /// Easy Reference to the character model position
    /// </summary>
    public enum CharacterModel
    {
        Head,
        Torso,
        Tail,
        Hand
    }

    public List<PrefabLinker> CharacterAdditonalModels = new List<PrefabLinker>(4);

    public void AddCharacterModel(CharacterModel charPart, GameObject newObject)
    {
        PrefabLinker prefabL = CharacterAdditonalModels[(int) charPart];

        

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
