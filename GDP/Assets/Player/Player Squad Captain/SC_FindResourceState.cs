using UnityEngine;
using System.Collections;

// SC_FindResourceState.cs: The find resource state of the player squad's captain FSM
public class SC_FindResourceState : ISCState 
{
        // Constructor
    public SC_FindResourceState(PlayerSquadFSM m_PlayerSquadFSM)
    {
        m_scFSM = m_PlayerSquadFSM;
    }
}
