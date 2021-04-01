using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    void FixedUpdate()
    {
        if (ClientPlayerManager.singleton.PlayerCharacter != null)
        {
            transform.LookAt(ClientPlayerManager.singleton.PlayerCharacter.CharacterCenter);
        }
    }
}
