using UnityEngine;
using System.Collections;

// SC_DeadState.cs: The death state of the player squad's captain FSM
public class SC_DeadState : ISCState 
{
    // Constructor
    public SC_DeadState(PlayerSquadFSM m_PlayerSquadFSM)
    {
        m_scFSM = m_PlayerSquadFSM;
    }

    public override void Enter()
    {
        m_scFSM.m_SpriteRenderer.enabled = false;
    }

    public override void Exit()
    {
        m_scFSM.m_SpriteRenderer.enabled = true;
    }
}
