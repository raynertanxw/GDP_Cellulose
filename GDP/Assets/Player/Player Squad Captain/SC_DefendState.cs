using UnityEngine;
using System.Collections;

// SC_DefendState.cs: The defend state of the player squad's captain FSM
public class SC_DefendState : ISCState 
{
    // Constructor
    public SC_DefendState(PlayerSquadFSM m_PlayerSquadFSM)
    {
        m_scFSM = m_PlayerSquadFSM;
    }
}
