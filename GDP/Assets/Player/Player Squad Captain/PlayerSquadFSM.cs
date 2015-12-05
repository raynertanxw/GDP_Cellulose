using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// PlayerSquadFSM.cs: The finite state machine of the cells
public class PlayerSquadFSM : MonoBehaviour 
{
    // Static Fields
    private static PlayerSquadFSM[] s_array_PlayerSquadFSM;     // PlayerSquadFSM[]: Stores the array of all the PlayerSquadFSM (all the squad child cells)

    // Uneditable Fields
    [HideInInspector] public float fStrafingOffsetAngle = 0f;   // fStrafingOffsetAngle: Stores the angular distances away from the main rotation vector

    private Dictionary<SCState, ISCState> dict_States;          // dict_States: The dictionary to store all the states
    private SCState m_currentEnumState;                         // m_currentEnumState: The current enum state of the FSM
    private ISCState m_currentState;                            // m_currentState: the current state (as of type ISCState)
    [HideInInspector] public bool bIsAlive = false;             // bIsAlive: Returns if the current child cell is alive

    // GameObject/Component References
    public SpriteRenderer m_SpriteRenderer;                     // m_SpriteRenderer: It is public so that states can references it
    public Rigidbody2D m_RigidBody;                             // m_RigidBody: It is public so that states can references it
    public BoxCollider2D m_Collider;                            // m_Collider: It is public so that states can references it

    // Private Functions
    // Start(): Use this for initialisation
    void Start()
    {
        // Initialisation of Array
        if (s_array_PlayerSquadFSM == null)
            s_array_PlayerSquadFSM = new PlayerSquadFSM[SquadCaptain.Instance.MaximumCount];

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

        // Object Pooling: Putting all child into array (THIS SHOULD BE THE LAST ELEMENT IN THE void Start(), CAZ IT returns;)
        for (int i = 0; i < s_array_PlayerSquadFSM.Length; i++)
        {
            // if: The current element of the array is empty
            if (s_array_PlayerSquadFSM[i] == null)
            {
                s_array_PlayerSquadFSM[i] = this;
                return;
            }
        }
    }

    // Private Functions
    void Update()
    {
        // Pre-Excution

        // Excution of the current state
        m_currentState.Execute();

        // Post-Excution
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

        // State Changing
        m_currentState.Exit();

        m_currentEnumState = _enumState;
        m_currentState = dict_States[m_currentEnumState];

        m_currentState.Enter();
        return true;
    }

    // Strafing(): Handles the movement when the cells are in production state
    public bool Strafing()
    {
        if (m_currentEnumState != SCState.Produce)
        {
            Debug.LogWarning(gameObject.name + ".PlayerSquadFSM.Strafing(): Current state is not SCState.Produce! Ignore Strafing!");
            return false;
        }

        // targetPosition: The calculated target position - includes its angular offset from the main vector and the squad's captain position
        Vector3 targetPosition = Quaternion.Euler(Vector3.forward * fStrafingOffsetAngle) * SquadCaptain.Instance.StrafingVector() + SquadCaptain.Instance.transform.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, 3f * Time.deltaTime);

        return true;
    }

    // Kill(): Kill the current cell
    public void Kill()
    {
        Advance(SCState.Dead);
    }

    // Public Static Functions
    // StateCount(): The number of cells that is in _state;
    public static int StateCount(SCState _enumState)
    {
        int nStateCount = 0;
        for (int i = 0; i < s_array_PlayerSquadFSM.Length; i++)
        {
            if (s_array_PlayerSquadFSM[i].EnumState.Equals(_enumState))
                nStateCount++;
        }
        return nStateCount;
    }

    // GetAliveCount(): Returns the number of squad child that is alive
    public static int AliveCount()
    {
        int nAliveCount = 0;
        for (int i = 0; i < s_array_PlayerSquadFSM.Length; i++)
        {
            // if: The current element m_playerSquadFSM's state is not in SC_DeadState
            if (s_array_PlayerSquadFSM[i].EnumState != SCState.Dead)
                nAliveCount++;
        }
        return nAliveCount;
    }

    // Public Static Functions
    // Spawn(): Make alive a squad child from the object pooling
    public static PlayerSquadFSM Spawn(Vector3 _position)
    {
        for (int i = 0; i < s_array_PlayerSquadFSM.Length; i++)
        {
            if (s_array_PlayerSquadFSM[i].EnumState.Equals(SCState.Dead))
            {
                s_array_PlayerSquadFSM[i].Advance(SCState.Produce);
                s_array_PlayerSquadFSM[i].transform.position = _position;
                return s_array_PlayerSquadFSM[i];
            }
        }
        Debug.LogWarning("PlayerSquadFSM.Spawn(): Cannot spawn child. All child is alive.");
        return null;
    }

    // CalculateStrafingOffset(): Recalculates all the offset angle that is used in strafing. This is called in SC_ProduceState.cs, within Enter() and Exit() functions
    public static bool CalculateStrafingOffset()
    {
        // if: There is no squad child cells in produce state
        if (StateCount(SCState.Produce) == 0)
            return false;

        // Resets all the strafing offset to 0
        for (int i = 0; i < s_array_PlayerSquadFSM.Length; i++)
            s_array_PlayerSquadFSM[i].fStrafingOffsetAngle = 0f;

        // for: Calculates strafing angle for squad child cells that are in production state
        // Calculation: Angles are split equally among each cells, which is also based on the number of production cells
        //              1 cell = 360 deg apart, 2 cells = 180 deg apart, 3 cells = 120 deg apart, 4 cells = 90 deg apart...
        for (int i = 0; i < PlayerSquadFSM.StateCount(SCState.Produce); i++)
            if (s_array_PlayerSquadFSM[i].EnumState.Equals(SCState.Produce))
                s_array_PlayerSquadFSM[i].fStrafingOffsetAngle = 360f / PlayerSquadFSM.StateCount(SCState.Produce) * i;

        return true;
    }

    // KillThisChild(): Goes through the child array and kill the identified child
    public static bool KillThisChild(GameObject m_GOchild)
    {
        for (int i = 0; i < s_array_PlayerSquadFSM.Length; i++)
        {
            // if: The states matches
            if (s_array_PlayerSquadFSM[i] == m_GOchild.GetComponent<PlayerSquadFSM>())
            {
                s_array_PlayerSquadFSM[i].Advance(SCState.Dead);
                return true;
            }
        }
        Debug.LogWarning("PlayerSquadFSM.KillThisChild():" + m_GOchild + " does not match any child cell. Wrong Reference?");
        return false;
    }


    //public static PlayerSquadFSM[] ChildArray { get { return s_array_PlayerSquadFSM; } }

    // Getter-Setter Functions
    public SCState EnumState { get { return m_currentEnumState; } }
    public ISCState State { get { return m_currentState; } }
    public bool IsAlive { get { return bIsAlive; } }
}
