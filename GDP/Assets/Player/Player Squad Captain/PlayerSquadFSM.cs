using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// PlayerSquadFSM.cs: The finite state machine of the cells
public class PlayerSquadFSM : MonoBehaviour 
{
    // Static Fields
    private static List<PlayerSquadFSM> s_List_PlayerSquadFSM;   // PlayerSquadFSM[]: Stores the array of all the PlayerSquadFSM (all the squad child cells)

    // Uneditable Fields
    private Dictionary<SCState, ISCState> dict_States;          // dict_States: The dictionary to store all the states
    private SCState m_currentEnumState;                         // m_currentEnumState: The current enum state of the FSM
    private ISCState m_currentState;                            // m_currentState: the current state (as of type ISCState)

    // GameObject/Component References
    public SpriteRenderer m_SpriteRenderer;                    // m_SpriteRenderer: It is public so that states can reference to it

    // Private Functions
    // Start(): Use this for initialisation
    void Start()
    {
        // Initialisation of Array
        s_List_PlayerSquadFSM = new List<PlayerSquadFSM>();

        // Initialisation of Dictionary
        dict_States = new Dictionary<SCState, ISCState>();
        dict_States.Add(SCState.Dead, new SC_DeadState(this));
        dict_States.Add(SCState.Idle, new SC_IdleState(this));
        dict_States.Add(SCState.Attack, new SC_AttackState(this));
        dict_States.Add(SCState.Defend, new SC_DefendState(this));
        dict_States.Add(SCState.Produce, new SC_ProduceState(this));
        dict_States.Add(SCState.FindResource, new SC_FindResourceState(this));

        // Initialisation of first state
        m_currentEnumState = SCState.Dead;
        m_currentState = dict_States[m_currentEnumState];
        m_currentState.Enter();

        // Object Pooling: Putting all child into array
        s_List_PlayerSquadFSM.Add(this);

    }

    // Private Functions
    void Update()
    {
        m_currentState.Execute();
    }

    // Public Functions
    // Advance(): Advance to the next state, which is defined by _enumState
    public bool Advance(SCState _enumState) 
    {
        if (_enumState.Equals(m_currentEnumState))
        {
            Debug.LogWarning(this.name + ".PlayerSquadFSM.Advance(): Tried to advance to same state! m_currentEnumState = SC.State." + m_currentEnumState.ToString());
            return false;
        }

        m_currentState.Exit();

        m_currentEnumState = _enumState;
        m_currentState = dict_States[m_currentEnumState];
        m_currentState.Enter();
        return true;
    }

    // Public Static Functions
    // StateCount(): The number of cells that is in _state;
    public static int StateCount(SCState _enumState)
    {
        int nStateCount = 0;
        foreach (PlayerSquadFSM m_playerSquadFSM in s_List_PlayerSquadFSM)
        {
            if (m_playerSquadFSM.EnumState.Equals(_enumState))
                nStateCount++;
        }
        return nStateCount;
    }

    // GetAliveCount(): Returns the number of squad child that is alive
    public static int GetAliveCount()
    {
        int nAliveCount = 0;
        foreach (PlayerSquadFSM m_playerSquadFSM in s_List_PlayerSquadFSM)
        {
            // if: The current element m_playerSquadFSM's state is not in SC_DeadState
            if (!m_playerSquadFSM.EnumState.Equals(SCState.Dead))
                nAliveCount++;
        }
        return nAliveCount;
    }

    // Getter-Setter Functions
    public SCState EnumState { get { return m_currentEnumState; } }
    public ISCState State { get { return m_currentState; } }
}
