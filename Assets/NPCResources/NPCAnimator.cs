using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimator : NetworkBehaviour
{
    private NPCState _npcState;

    public override void FixedUpdateNetwork()
    {
        if (IsProxy == true) return;

        if (_npcState != null)
        {
            _npcState.Activate(0.15f);
        }
    }

    protected void Awake()
    {
        _npcState = GetComponentInChildren<NPCState>();
    }
}
