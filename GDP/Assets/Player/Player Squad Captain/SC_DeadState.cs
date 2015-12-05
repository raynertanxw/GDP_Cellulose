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
        m_scFSM.m_Collider.enabled = false;
        m_scFSM.bIsAlive = false;

        m_scFSM.transform.position = Constants.s_farfarAwayVector;
    }

    public override void Exit()
    {
        m_scFSM.m_SpriteRenderer.enabled = true;
        m_scFSM.m_Collider.enabled = true;
        m_scFSM.bIsAlive = true;
    }
}
