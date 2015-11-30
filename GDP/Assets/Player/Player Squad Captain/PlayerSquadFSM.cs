using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// PlayerSquadFSM.cs: The finite state machine of the cells
public class PlayerSquadFSM : MonoBehaviour 
{
    // Static Fields
    private static PlayerSquadFSM[] array_PlayerSquadFSM;   // PlayerSquadFSM[]: Stores the array of all the PlayerSquadFSM (all the squad child cells)

    // Editable Fields

    // Uneditable Fields
    private Dictionary<SCState, ISCState> dict_States;
    private ISCState currentState;                          // currentState: The current state of the FSM

    // Private Functions
    // Start(): Use this for initialisation
    void Start()
    {
        array_PlayerSquadFSM = new PlayerSquadFSM[SquadCaptain.MaximumCount];

        // Initialisation of Dictionary
        dict_States = new Dictionary<SCState, ISCState>();
        dict_States.Add(SCState.Dead, new SC_DeadState(this));
        dict_States.Add(SCState.Idle, new SC_IdleState(this));
        dict_States.Add(SCState.Attack, new SC_AttackState(this));
        dict_States.Add(SCState.Defend, new SC_DefendState(this));
        dict_States.Add(SCState.Produce, new SC_ProduceState(this));
        dict_States.Add(SCState.FindResource, new SC_FindResourceState(this));
    }

    // Public Functions

    // Public Static Functions
    // StateCount(): The number of cells that is in _state;
    public static int StateCount(Type m_StateType)
    {
        int nStateCount = 0;
        foreach (PlayerSquadFSM m_playerSquadFSM in array_PlayerSquadFSM)
        {
            if (m_playerSquadFSM.State.GetType().Equals(m_StateType))
                nStateCount++;
        }
        return nStateCount;
    }

    // GetAliveCount(): Returns the number of squad child that is alive
    public static int GetAliveCount()
    {
        int nAliveCount = 0;
        foreach (PlayerSquadFSM m_playerSquadFSM in array_PlayerSquadFSM)
        {
            // if: The current element m_playerSquadFSM's state is not in SC_DeadState
            if (!m_playerSquadFSM.State.GetType().Equals(typeof(SC_DeadState)))
                nAliveCount++;
        }
        return nAliveCount;
    }

    // Getter-Setter Functions
    public ISCState State { get { return currentState; } }
}
