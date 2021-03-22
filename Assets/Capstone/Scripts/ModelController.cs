using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; //For NetworkServer
using System; //For [Serializable]

public class ModelController : MonoBehaviour
{
    /// <summary>
    /// Struct to keep track of gameobject connections
    /// </summary>
    [Serializable]
    public struct PrefabLinker
    {
        public GameObject prefab;
        public Transform parent;
    }


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

    public List<PrefabLinker> CharacterAdditonalModels = new List<PrefabLinker>(CharacterModel.GetNames(typeof(CharacterModel)).Length);

    public void AddCharacterModel(CharacterModel characterPart, GameObject newObject)
    {
        PrefabLinker prefabLinker = CharacterAdditonalModels[(int)characterPart];

        RemoveCharacterModel(characterPart);

        GameObject instatiatedObject = Instantiate(newObject, prefabLinker.parent);
        NetworkServer.Spawn(instatiatedObject);

    }

    public void RemoveCharacterModel(CharacterModel charPart)
    {
        PrefabLinker prefabLinker = CharacterAdditonalModels[(int)charPart];
        NetworkServer.Destroy(prefabLinker.prefab);
        prefabLinker.prefab = null;
    }
}
