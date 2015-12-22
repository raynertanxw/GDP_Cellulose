using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// SquadChildFSM.cs: The finite state machine of the cells
public class SquadChildFSM : MonoBehaviour 
{
    /* SquadChildFSM.cs API - Everything you possibly need for your Player Squad Finite State Machine Needs!
     * ------------------------------------------------------------------------------------------------------------------------------
     * Static Functions:
     * - int StateCount(SCState _enumState): Returns the number of squad child cells that is in the state defined
     *                           _enumState: The state in which to check
     * - int AliveCount(): Returns the number of squad child cells that is alive
     * - bool KillThisChild(GameObject _GOchild): Identify the squad child cell and kills it (This method is best used with collider)
     *                                  _GOchild: The identity of the child
     * - int AdvanceSquadPercentage(SCState _currentState, SCState _nextState, float _chance): 
     *                              Advance _chance percentage of the squad child to _enumState
     *               _currentState: Checks for all child cells that is in the current state
     *                  _nextState: The state in which to advance towards
     *                     _chance: (from 0f to 100f) The chance of it advance to the next state
     * ------------------------------------------------------------------------------------------------------------------------------
    */

    // Static Fields
    private static SquadChildFSM[] s_array_SquadChildFSM;     // PlayerSquadFSM[]: Stores the array of all the PlayerSquadFSM (all the squad child cells)
    // s_dict_SingleTransition: Limits the execution of Advance to happen one per frame, eliminates the stacked transition that is called by multiple cell at once
    private static List<StatesAndPercentage> s_list_SingleAdvance = new List<StatesAndPercentage>();

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
    void OnCollisionEnter2D(Collision2D _collision)
    {
        if (_collision.contacts[0].collider.gameObject.layer == Constants.s_onlyEnemeyChildLayer.value)
        {
            Debug.Log(":D");
        }
    }

    void Awake()
    {
        // Initialisation of Array
        if (s_array_SquadChildFSM == null)
            s_array_SquadChildFSM = new SquadChildFSM[50];

        // Object Pooling: Putting all child into array (THIS SHOULD BE THE LAST ELEMENT IN THE void Start(), CAZ IT returns;)
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            // if: The current element of the array is empty
            if (s_array_SquadChildFSM[i] == null)
            {
                s_array_SquadChildFSM[i] = this;
                break;
            }
        }
    }

    // Start(): Use this for initialisation
    void Start()
    {

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

        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
            if (s_array_SquadChildFSM[i] == null)
                Debug.Log(i + ": D:");
    }

    // Private Functions
    void Update()
    {
        // Pre-Excution

        // Excution of the current state
        m_currentState.Execute();

        // Post-Excution

        // Advancement Transition Execution
        while (s_list_SingleAdvance.Count > 0)
        {
            // for: Checks through all the child in the array
            for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
            {
                // if: The current cell is the targeted cell that has a transition
                if (s_array_SquadChildFSM[i].EnumState == s_list_SingleAdvance[0].FromState)
                {
                    if (UnityEngine.Random.value * 100f <= s_list_SingleAdvance[0].Percentage)
                    {
                        Advance(s_list_SingleAdvance[0].ToState);
                    }
                }
            }
            s_list_SingleAdvance.RemoveAt(0);
        }
    }

    // Public Functions
    // Advance(): Advance to the next state, which is defined by _enumState
    public bool Advance(SCState _enumState) 
    {
        if (_enumState.Equals(m_currentEnumState))
        {
            Debug.LogWarning(this.name + ".SquadChildFSM.Advance(): Tried to advance to same state! m_currentEnumState = SC.State." + m_currentEnumState.ToString());
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
            Debug.LogWarning(gameObject.name + ".SquadChildFSM.Strafing(): Current state is not SCState.Produce! Ignore Strafing!");
            return false;
        }

        // targetPosition: The calculated target position - includes its angular offset from the main vector and the squad's captain position
        Vector3 targetPosition = Quaternion.Euler(Vector3.forward * fStrafingOffsetAngle) * PlayerSquadFSM.Instance.StrafingVector() + PlayerSquadFSM.Instance.transform.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, 3f * Time.deltaTime);

        return true;
    }

    // Public Static Functions
    /// <summary>
    /// Returns the number of squad child cells that is in the current state
    /// </summary>
    /// <param name="_enumState"> The state in which to check for </param>
    /// <returns></returns>
    public static int StateCount(SCState _enumState)
    {
        int nStateCount = 0;
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            if (s_array_SquadChildFSM[i].EnumState == _enumState)
                nStateCount++;
        }
        return nStateCount;
    }

    
    /// <summary>
    /// Returns the number of squad child that is alive
    /// </summary>
    /// <returns></returns>
    public static int AliveCount()
    {
        int nAliveCount = 0;
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            // if: The current element m_playerSquadFSM's state is not in SC_DeadState
            if (s_array_SquadChildFSM[i].EnumState != SCState.Dead)
                nAliveCount++;
        }
        return nAliveCount;
    }

    /// <summary>
    /// Make alive a squad child from the object pooling
    /// </summary>
    /// <param name="_position"> The position of spawn </param>
    /// <returns></returns>
    public static SquadChildFSM Spawn(Vector3 _position)
    {
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            if (s_array_SquadChildFSM[i].EnumState.Equals(SCState.Dead))
            {
                s_array_SquadChildFSM[i].Advance(SCState.Produce);
                s_array_SquadChildFSM[i].transform.position = _position;
                return s_array_SquadChildFSM[i];
            }
        }
        Debug.LogWarning("SquadChildFSM.Spawn(): Cannot spawn child. All child is alive.");
        return null;
    }

    /// <summary>
    /// Recalculates all the offset angle that is used in strafing. This is called in SC_ProduceState.cs, within Enter() and Exit() functions
    /// </summary>
    public static bool CalculateStrafingOffset()
    {
        // if: There is no squad child cells in produce state
        if (StateCount(SCState.Produce) == 0)
            return false;

        // Resets all the strafing offset to 0
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
            s_array_SquadChildFSM[i].fStrafingOffsetAngle = 0f;

        // for: Calculates strafing angle for squad child cells that are in production state
        // Calculation: Angles are split equally among each cells, which is also based on the number of production cells
        //              1 cell = 360 deg apart, 2 cells = 180 deg apart, 3 cells = 120 deg apart, 4 cells = 90 deg apart...
        for (int i = 0; i < SquadChildFSM.StateCount(SCState.Produce); i++)
            if (s_array_SquadChildFSM[i].EnumState.Equals(SCState.Produce))
                s_array_SquadChildFSM[i].fStrafingOffsetAngle = 360f / SquadChildFSM.StateCount(SCState.Produce) * i;

        return true;
    }


    /// <summary>
    /// Goes through the child array and kill the identified child
    /// </summary>
    /// <param name="m_GOchild"> The reference of the child to be identified </param>
    public static bool KillThisChild(GameObject m_GOchild)
    {
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            // if: The states matches
            if (s_array_SquadChildFSM[i] == m_GOchild.GetComponent<SquadChildFSM>())
            {
                s_array_SquadChildFSM[i].Advance(SCState.Dead);
                return true;
            }
        }
        Debug.LogWarning("SquadChildFSM.KillThisChild():" + m_GOchild + " does not match any child cell. Wrong Reference?");
        return false;
    }

    /// <summary>
    /// Advance all squad in the state _currentState to _nextState by a certain amount of _chance
    /// </summary>
    /// <param name="_currentState"> the state of squad child which would be advancing </param>
    /// <param name="_nextState"> The state that the squad child cell will advance towards </param>
    /// <param name="_chance"> The chance of which the squad child cell will advance </param>
    public static bool AdvanceSquadPercentage(SCState _currentState, SCState _nextState, float _chance)
    {
        for (int i = 0; i < s_list_SingleAdvance.Count; i++)
        {
            // if: Check if the transition is already in the list
            if (s_list_SingleAdvance[i].FromState == _currentState && s_list_SingleAdvance[i].ToState == _nextState)
            {
                return false;
            }
        }
        s_list_SingleAdvance.Add(new StatesAndPercentage(_currentState, _nextState, _chance));
        return true;
    }

    // Getter-Setter Functions
    public SCState EnumState { get { return m_currentEnumState; } }
    public ISCState State { get { return m_currentState; } }
    public bool IsAlive { get { return bIsAlive; } }
}

// StatesAndPercentage.cs: Stores a structure for advancing of cells purposes
public struct StatesAndPercentage
{
    // Constructor
    public StatesAndPercentage(SCState _fromState, SCState _toState, float _percentage)
    {
        FromState = _fromState;
        ToState = _toState;
        Percentage = _percentage;
    }

    // Editable Field
    public SCState FromState;
    public SCState ToState;
    public float Percentage;
}