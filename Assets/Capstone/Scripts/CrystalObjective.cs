using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CrystalObjective : Character
{

    private void Start()
    {
        base.Awake();
    }

    protected override void Died()
    {
        CallGameOver();
    }

    [Command(requiresAuthority = false)]
    private void CallGameOver()
    {
        MapController.singleton.ServerGameOver();
    }
}
