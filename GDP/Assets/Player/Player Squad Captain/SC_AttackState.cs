using UnityEngine;
using System.Collections;

// SC_AttackState.cs: The attack state of the player squad's captain FSM
public class SC_AttackState : ISCState 
{
    // Constructor
    public SC_AttackState(PlayerSquadFSM m_PlayerSquadFSM)
    {
        m_scFSM = m_PlayerSquadFSM;
    }
}
