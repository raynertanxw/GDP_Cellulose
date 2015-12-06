using UnityEngine;
using System.Collections;

// SC_IdleState.cs: The idle state of the player squad's captain FSM
public class SC_IdleState : ISCState 
{
    // Uneditable Fields

    // Constructor
    public SC_IdleState(PlayerSquadFSM m_PlayerSquadFSM)
    {
        m_scFSM = m_PlayerSquadFSM;
    }

    // State Execution:
    public override void Execute()
    {
        // if: The number of squad child cells is less than 30 <- Production Check
        if (PlayerSquadFSM.AliveCount() < 4f)
        {
            m_scFSM.Advance(SCState.Produce);
            return;
        }
    }
}
