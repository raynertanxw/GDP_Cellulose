using UnityEngine;
using System.Collections;

// SC_IdleState.cs: The idle state of the player squad's captain FSM
public class SC_IdleState : ISCState 
{
        // Constructor
    public SC_IdleState(PlayerSquadFSM m_PlayerSquadFSM)
    {
        m_scFSM = m_PlayerSquadFSM;
    }
}
