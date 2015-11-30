using UnityEngine;
using System.Collections;

// SC_ProduceState.cs: The produce state of the player squad's captain FSM
public class SC_ProduceState : ISCState 
{
    // Constructor
    public SC_ProduceState(PlayerSquadFSM m_PlayerSquadFSM)
    {
        m_scFSM = m_PlayerSquadFSM;
    }
}
