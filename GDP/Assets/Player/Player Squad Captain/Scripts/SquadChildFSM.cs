﻿using UnityEngine;
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

    // PlayerSquadFSM-Inherited variables
    private static Vector3 m_strafingVector;                    // m_strafingVector: The current direction of the strafing vector
    private static float fStrafingRadius;                       // fStrafingRadius: The maximum strafing radius of the child cells during production
    private static float fStrafingSpeed;                        // fStrafingSpeed: The maximum strafing speed of the child cells during production
    private static float fDefenceAngle;
    private static float fDefenceRadius;
    private static float fDefenceSpeed;
    private static float fDefenceRigidity;
    private static float fAttackSpeed;

    // Uneditable Fields
    private float fStrafingOffsetAngle = 0f;                    // fStrafingOffsetAngle: Stores the angular distances away from the main rotation vector
    private float fDefenceOffsetAngle = 0f;                     // fDefenceOffsetAngle: Stores the angular distances away from the leftmost angle 

    private Dictionary<SCState, ISCState> dict_States;          // dict_States: The dictionary to store all the states
    private SCState m_currentEnumState;                         // m_currentEnumState: The current enum state of the FSM
    private ISCState m_currentState;                            // m_currentState: the current state (as of type ISCState)
    private Vector2 mainDefenceVector;                          // mainDefenceVector: The main defence vector, this will be initilised at the start and will be use without change

    private Vector3 gizmosPosition;

    private static Vector3 parentPosition;                      // parentPosition: The position of the squad captain
    private static Vector3 playerPosition;                      // playerPosition: The position of the player main

    [HideInInspector] public bool bIsAlive = false;             // bIsAlive: Returns if the current child cell is alive
    [HideInInspector] public EnemyChildFSM attackTarget;        // attackTarget: The target to attack

    // GameObject/Component References
    public SpriteRenderer m_SpriteRenderer;                     // m_SpriteRenderer: It is public so that states can references it
    public Rigidbody2D m_RigidBody;                             // m_RigidBody: It is public so that states can references it
    public Collider2D m_Collider;                            // m_Collider: It is public so that states can references it

    // Private Functions
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawWireSphere(gizmosPosition, 1f);
    //}

    //public void gizmoo(Vector3 _position)
    //{
    //    gizmosPosition = _position;
    //}

    void OnCollisionEnter2D(Collision2D _collision)
    {
        // Hit Enemy Child
        if (_collision.gameObject.tag == Constants.s_strEnemyChildTag)
        {
            // Kill Enemy.
            _collision.gameObject.GetComponent<EnemyChildFSM>().KillChildCell();
            // Kill Self.
            KillSquadChild();
        }
        // Hit Enemy Main.
        else if (_collision.gameObject.tag == Constants.s_strEnemyTag)
        {
            // Damage Enemy.
            EMController.Instance().CauseDamageOne();

            KillSquadChild();
        }
    }

    // Awake(): is called at the start of the program
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

        // Initialisation
        playerPosition = PlayerMain.Instance.transform.position;
        m_strafingVector = Vector3.up;

        fStrafingRadius = PlayerSquadFSM.Instance.StrafingRadius;
        fStrafingSpeed = PlayerSquadFSM.Instance.StrafingSpeed;
        fDefenceAngle = PlayerSquadFSM.Instance.DefenceAngle;
        fDefenceRadius = PlayerSquadFSM.Instance.DefenceRadius;
        fDefenceSpeed = PlayerSquadFSM.Instance.DefenceSpeed;
        fDefenceRigidity = PlayerSquadFSM.Instance.DefenceRigidity;
        fAttackSpeed = PlayerSquadFSM.Instance.AttackSpeed;

        mainDefenceVector = Quaternion.Euler(0f, 0f, (fDefenceAngle / 2.0f)) * Vector2.up * fDefenceRadius;

        // Initialisation of first state
        m_currentEnumState = SCState.Dead;
        m_currentState = dict_States[m_currentEnumState];
        m_currentState.Enter();
    }
    
    // Private Functions
    void Update()
    {

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

        ExecuteMethod.OnceInUpdate("SquadChildFSM.StrafingVector", null, null);
        // targetPosition: The calculated target position - includes its angular offset from the main vector and the squad's captain position
        Vector3 targetPosition = Quaternion.Euler(0.0f, 0.0f, fStrafingOffsetAngle) * m_strafingVector + parentPosition;
        m_RigidBody.MovePosition((targetPosition - transform.position) * Time.deltaTime * 3.0f + transform.position);

        return true;
    }

    // DefenceSheild(): Handles the movement when the cells in defence state
    public bool DefenceSheild()
    {
        if (m_currentEnumState != SCState.Defend)
        {
            Debug.LogWarning(gameObject.name + ".SquadChildFSM.DefenceSheild(): Current state is not SCState.Defend! Ignore Defence!");
            return false;
        }

        Vector3 toTargetPosition = Quaternion.Euler(0f, 0f, -fDefenceOffsetAngle) * mainDefenceVector + playerPosition - transform.position;
        if (toTargetPosition.magnitude > fDefenceRigidity)
            m_RigidBody.AddForce(toTargetPosition * Time.deltaTime * fDefenceSpeed);
        m_RigidBody.velocity = Vector3.ClampMagnitude(m_RigidBody.velocity, Mathf.Max(fDefenceRigidity, toTargetPosition.magnitude));

        return true;
    }
    
    // AttackTarget(): Handles the movement when the cells in attack state
    public bool AttackTarget()
    {
        if (m_currentEnumState != SCState.Attack)
        {
            Debug.LogWarning(gameObject.name + ".SquadChildFSM.AttackTarget(): Current state is not SCState.Attack! Ignore Attack!");
            return false;
        }

        // if: The target is dead, somehow.
        if (attackTarget == null || attackTarget.CurrentStateEnum == ECState.Dead)
        {
            Advance(SCState.Idle);
            return false;
        }

        m_RigidBody.AddForce((attackTarget.transform.position - transform.position) * Time.deltaTime * fAttackSpeed, ForceMode2D.Force);
        return true;
    }

    // KillSquadChild(): Call this method to kill the current cell
    public bool KillSquadChild()
    {
        if (m_currentEnumState == SCState.Dead)
            return false;
        else
        {
            Advance(SCState.Dead);
            PlayerSquadFSM.Instance.CheckDieable();
            return true;
        }
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
        // Since there is at least one child now, sets the position for reference
        parentPosition = _position;

        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            if (s_array_SquadChildFSM[i].EnumState == SCState.Dead)
            {
                s_array_SquadChildFSM[i].transform.position = _position;
                s_array_SquadChildFSM[i].Advance(SCState.Produce);
                return s_array_SquadChildFSM[i];
            }
        }
        Debug.LogWarning("SquadChildFSM.Spawn(): Cannot spawn child. All child is alive.");
        return null;
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
                s_array_SquadChildFSM[i].KillSquadChild();
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
        if (_currentState == _nextState)
            return false;
        else if (SquadChildFSM.StateCount(_currentState) == 0)
            return false;

        // for: Checks through all the child in the array
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            // if: The current cell type is the trageted cell type
            if (s_array_SquadChildFSM[i].EnumState == _currentState)
            {
                // if: The it is within the chance range
                if (UnityEngine.Random.value <= _chance)
                {
                    s_array_SquadChildFSM[i].Advance(_nextState);
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Advance _chance amount of alive cells to _nextsState
    /// </summary>
    /// <returns><c>true</c> There was transition happening <c>false</c> There wasn't any transition happening </returns>
    /// <param name="_nextState"> The state in which the squad child advances to </param>
    /// <param name="_chance"> The chance in which the squad child cell will advance </param>
    public static bool AdvanceSquadPercentage(SCState _nextState, float _chance)
    {
        // if: All the alive child is the _nextstate state
        if (SquadChildFSM.AliveCount() == SquadChildFSM.StateCount(_nextState))
            return false;

        // for: Checksthrough all the child in the array
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            if (s_array_SquadChildFSM[i].EnumState != SCState.Dead)
            {
                // if: the current cell type is NOT the targeted cell type, as transition would be useless
                if (s_array_SquadChildFSM[i].EnumState != _nextState)
                {
                    // if: The it is within the chance range
                    if (UnityEngine.Random.value <= _chance)
                    {
                        s_array_SquadChildFSM[i].Advance(_nextState);
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Returns an list of alive squad children, in SquadChildFSM format
    /// </summary>
    /// <returns> The list of alive squad children </returns>
    public static List<SquadChildFSM> GetAliveChildList()
    {
        List<SquadChildFSM> list_AliveChild = new List<SquadChildFSM>();

        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            if (s_array_SquadChildFSM[i].EnumState != SCState.Dead)
                list_AliveChild.Add(s_array_SquadChildFSM[i]);
        }
        return list_AliveChild;
    }

    // CalculateStrafingOffset(): Calling this method will recalculate all strafing offsets for all production cells
    public static bool CalculateStrafingOffset()
    {
        // Resets all the strafing offset to 0
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
            s_array_SquadChildFSM[i].fStrafingOffsetAngle = 0f;

        // if: There is no squad child cells in produce state
        if (StateCount(SCState.Produce) == 0)
            return false;

        int produceCount = SquadChildFSM.StateCount(SCState.Produce);
        // for: Calculates strafing angle for squad child cells that are in production state
        // Calculation: Angles are split equally among each cells, which is also based on the number of production cells
        //              1 cell = 360 deg apart, 2 cells = 180 deg apart, 3 cells = 120 deg apart, 4 cells = 90 deg apart...
        int j = 0;
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            if (s_array_SquadChildFSM[i].EnumState == SCState.Produce)
            {
                s_array_SquadChildFSM[i].fStrafingOffsetAngle = 360f / produceCount * j;
                j++;
            }
            // if: The loop have checked through all production state cells, then it would break the loop
            if (j == produceCount)
                break;
        }

        return true;
    }

    // CalculateDefenceSheildOffset(): Calling this method will recalculates all defence offsets for all defence cells
    public static bool CalculateDefenceSheildOffset()
    {
        // Resets all the strafing offset to 0
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
            s_array_SquadChildFSM[i].fDefenceOffsetAngle = 0f;

        // if: There is no squad cells in defence state
        if (StateCount(SCState.Defend) == 0)
            return false;

        int defenceCount = SquadChildFSM.StateCount(SCState.Defend);
        // if: there is only 1 defence cell
        if (defenceCount == 1)
        {
            for (int i = 0; i < defenceCount; i++)
            {
                if (s_array_SquadChildFSM[i].EnumState == SCState.Defend)
                {
                    s_array_SquadChildFSM[i].fDefenceOffsetAngle = 0.5f * fDefenceAngle;
                    return true;
                }
            }
            return false;
        }
        else
        {
            // for: Calculates the distribution of the angle offset from the main vector
            int j = 0;
            for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
            {
                if (s_array_SquadChildFSM[i].EnumState == SCState.Defend)
                {
                    s_array_SquadChildFSM[i].fDefenceOffsetAngle = j / (defenceCount - 1f) * fDefenceAngle;
                    j++;
                }
                // if: The loop have checked through all defence state cells, then it would break the loop
                if (j == defenceCount)
                    break;
            }
            return true;
        }
    }

    // GetNearestTargetPosition(): Assign a target to aggressive squad child cells.
    public static bool GetNearestTargetPosition()
    {
        List<EnemyChildFSM> list_enemyChild = EnemyMainFSM.Instance().ECList;
        // if: There is no enemy
        if (list_enemyChild.Count == 0)
            return false;

        int attackCount = SquadChildFSM.StateCount(SCState.Attack);
        // if: There are no attacking squad children
        if (attackCount == 0)
            return false;

        // if: There are more aggressive squad child cells than enemies to attack
        if (attackCount > list_enemyChild.Count)
        {
            attackCount = list_enemyChild.Count;

            int toCheck = attackCount;
            // for: Send the extra amount of aggressive squad child cells back to idle (Since there are more squad child than enemy child)
            for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
            {
                if (s_array_SquadChildFSM[i].EnumState == SCState.Attack)
                {
                    if (toCheck >= attackCount)
                        s_array_SquadChildFSM[i].Advance(SCState.Idle);
                    else
                        toCheck++;
                }
            }
        }

        // array_nearestEnemyChild: An array to store the array of closest enemy child cells to Squad Captain
        EnemyChildFSM[] array_nearestEnemyChild = new EnemyChildFSM[attackCount];

        // for: Checks through all enemy child and creates a list of closest enemy child cells to Squad Captain
        for (int i = 0; i < list_enemyChild.Count; i++)
        {
            for (int j = 0; j < array_nearestEnemyChild.Length; j++)
            {
                if (array_nearestEnemyChild[j] == null)
                {
                    array_nearestEnemyChild[j] = list_enemyChild[i];
                    break;
                }
                // else if: The current enemy child is closer than the enemy child that are in the "TOP CLOSEST TO SQUAD CAPTAIN" list
                // it will add the current enemy child into the list
                else if (Vector3.Distance(parentPosition, list_enemyChild[i].transform.position) < Vector3.Distance(parentPosition, array_nearestEnemyChild[j].transform.position))
                {
                    array_nearestEnemyChild[j] = list_enemyChild[i];
                    break;
                }
            }
        }

        // for: Assigning enemy child as targets to aggressive squad child cells
        int k = 0;
        for (int i = 0; i < s_array_SquadChildFSM.Length; i++)
        {
            if (s_array_SquadChildFSM[i].EnumState == SCState.Attack)
            {
                s_array_SquadChildFSM[i].attackTarget = array_nearestEnemyChild[k];
                k++;
            }
        }

        return true;
    }

    // StrafingVector(): calculates and return the strafing vector
    //                   NOTE: This is handled in Update() because it should only be executed once every update (method may be called multiple times in the update)
    public static Vector3 StrafingVector()
    {
        float fCurrentRadius = Mathf.PingPong(Time.time, fStrafingRadius);
        if (fCurrentRadius < 0.7f)
            fCurrentRadius = 0.7f;
        // NOTE: Quaternions q * Vector v returns the v rotated in q direction, THOUGH REMEMBER TO NORMALIZED ELSE VECTOR WILL PISS OFF INTO SPACE
        m_strafingVector = (Quaternion.Euler(0, 0, fStrafingSpeed) * m_strafingVector).normalized * fCurrentRadius;
        return m_strafingVector;
    }

    public static SquadChildFSM[] SquadChildArray { get { return s_array_SquadChildFSM; } }

    // Getter-Setter Functions
    public SCState EnumState { get { return m_currentEnumState; } }
    public ISCState State { get { return m_currentState; } }
    public bool IsAlive { get { return bIsAlive; } }

}