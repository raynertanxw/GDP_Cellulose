using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// SquadCaptain.cs: Function of script here.
public class PlayerSquadFSM : MonoBehaviour
{
    /* SquadCaptain.cs API - Everything you need to know about SquadCaptain
     * ------------------------------------------------------------------------------------------------------------------------------
     * Static Functions:
     * - SquadCaptain Instance: Return the main instance of the squad captain
     * ------------------------------------------------------------------------------------------------------------------------------
     * Public Functions:
     * - void Initialise(): Initialise the Squad Captain; Make the squad captain alive
     * - int AliveChildCount(): Get the amount of alive squad child cells
     * ------------------------------------------------------------------------------------------------------------------------------
     * NOTES ON SQUAD CHILD SPAWNING ------------------------------------------------------------------------------------------------
     * > The rate of squad child spawn is relative to the number of squad child in the production state (alive too)
     * > The cooldown can be calculated with the following formula:
     * 
     *      cooldownTime = (fMaxCooldown - fMinCooldown) * ((nMaxChildCount - "The number of cells in Production state") / nMaxChildCount) + fMinCooldown
     *
     * Refer to the variables below**
     * 
     * > TL;DR: The more the number of squad child in production, the lesser the cooldown, the faster the squad child are produced
     * ------------------------------------------------------------------------------------------------------------------------------
     */

    // Static Fields
    private static PlayerSquadFSM s_m_Instance;   // m_Instance: Stores this instance in this variable, used for singleton purposes

    // Editables Fields
    [Header("Child Spawn: Generic")]
    [Tooltip("The maximum number of squad child cell")]
    [SerializeField] private int nMaximumChildCount = 50;

    [Header("Child Spawn: Cooldown")]
    [Tooltip("The minimum amount of cooldown for each child spawn")]
    [SerializeField] private float fMinimumCooldown = 3f;
    [Tooltip("The maximum amount of cooldown for each child spawn")]
    [SerializeField] private float fMaximumCooldown = 10f;

    [Header("Child State: Production")]
    // Strafing is used to rotate around the squad captain, used in production state
    [Tooltip("The radius of the circle from the center in which the cells are travelling")]
    [SerializeField] private float fStrafingRadius = 1.0f;
    [Tooltip("The speed of the rotation for strafing")]
    [SerializeField] private float fStrafingSpeed = 0.5f;

    [Header("Child State: Attack")]
    [Tooltip("The speed of the attack")]
    [SerializeField] private float fAttackSpeed = 3f;

    [Header("Child State: Defence")]
    [Tooltip("The angle of formation of the defence mechanism")]
    [SerializeField] private float fDefenceAngle = 30f;
    [Tooltip("The distance of the shield from the player main")]
    [SerializeField] private float fDefenceRadius = 3f;
    [Tooltip("The speed of cells when in defence mode")]
    [SerializeField] private float fDefenceSpeed = 3f;

    // Uneditables Fields
    [HideInInspector] public bool bIsAlive = false;     // bIsAlive: Returns if the squad captain is alive

    private PS_Logicaliser m_Brain;                     // brain: Access the brain of the Player Squad Captain
    private Dictionary<PSState, IPSState> dict_States;  // dict_States: The dictionary to store the states
    private PSState m_CurrentEnumState;                 // m_CurrentEnumState: The current enum state of the squad captain
    private IPSState m_CurrentState;                    // m_CurrentState: The current state of the squad captain
    private bool bIsProduce = false;                    // bIsProduce: Returns (or define) if the spawn routine is activated

    public SpriteRenderer m_SpriteRenderer;             // m_SpriteRenderer: It is public so that states can references it
    public Collider2D m_Collider;                       // m_Collider: It is public so that states can references it

    private float fNextCooldown = 0.0f;                 // fNextCooldown: Stores the time of the cooldown

    // Co-Routines
    IEnumerator SpawnRoutine()
    {
        while (bIsProduce)
        {
            yield return new WaitForSeconds(fNextCooldown);

            if (!this.CalculateCooldown() || SquadChildFSM.StateCount(SCState.Produce) == 0)
            {
                bIsProduce = false;
            }
            else
            {
                SquadChildFSM.Spawn(transform.position);
            }
        }
    }

    // Private Functions
    // Awake(): Called when the object is instantiated
    void Awake()
    {
        // Singleton Implementation
        if (s_m_Instance == null)
            s_m_Instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start(): Use this for initialization
    void Start()
    {
        // Variable Initialisation
        m_Brain = this.GetComponent<PS_Logicaliser>();

        dict_States = new Dictionary<PSState, IPSState>();
        dict_States.Add(PSState.Idle, new PS_IdleState(this, m_Brain));
        dict_States.Add(PSState.Attack, new PS_AttackState(this));
        dict_States.Add(PSState.Defend, new PS_DefendState(this));
        dict_States.Add(PSState.Produce, new PS_ProduceState(this));
        dict_States.Add(PSState.FindResource, new PS_FindResourceState(this));
        dict_States.Add(PSState.Dead, new PS_DeadState(this));
        m_CurrentEnumState = PSState.Dead;
        m_CurrentState = dict_States[m_CurrentEnumState];
        m_CurrentState.Enter();
    }

    // Update(): is called once every frame
    void Update()
    {
        // Pre-Execution

        // Execution
        m_CurrentState.Execute();

        // Post-Execution
    }

    // Public Functions
    /// <summary>
    /// Spawns the squad captain
    /// </summary>
    /// <param name="_position"> The position in which the squad captain to spawn in </param>
    public bool Initialise(Vector3 _position)
    {
        transform.position = _position;
        this.Advance(PSState.Idle);
        SquadChildFSM.Spawn(_position);
        return false;
    }

    /// <summary>
    /// Advance the squad captain to the next state
    /// </summary>
    /// <param name="_enumState"></param>
    public void Advance(PSState _enumState)
    {
        m_CurrentState.Exit();
        m_CurrentEnumState = _enumState;
        m_CurrentState = dict_States[m_CurrentEnumState];
        m_CurrentState.Enter();
    }

    /// <summary>
    /// Returns the number of squad child cells that is alive
    /// </summary>
    /// <returns></returns>
    public int AliveChildCount()
    {
        return SquadChildFSM.AliveCount();
    }

    // CalculateCooldown(): Call this function to re-calculate the cooldown of production
    public bool CalculateCooldown()
    {
        fNextCooldown = (fMaximumCooldown - fMinimumCooldown) * ((float)(nMaximumChildCount - SquadChildFSM.StateCount(SCState.Idle)) / (float)nMaximumChildCount) + fMinimumCooldown;

        if (fNextCooldown <= 0f)
        {
            Debug.LogWarning(this.name + ".CalculateCooldown: fNextCooldown is less than or equal to 0! fNextCooldown = " + fNextCooldown);

            fNextCooldown = 0.0f;
            return false;
        }
        else
            return true;
    }

    // EnableSpawnRoutine(): Enables the spawn routine
    public void EnableSpawnRoutine()
    {
        if (!bIsProduce)
        {
            bIsProduce = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    // CheckDieable(): Since the health of the squad captain is determine by the number of squad child, it checks if the health reaches 0
    public bool CheckDieable()
    {
        if (SquadChildFSM.AliveCount() == 0)
        {
            Advance(PSState.Dead);
            return true;
        }
        else
            return false;
    }

    // Public Static Functions
    public static PlayerSquadFSM Instance { get { return s_m_Instance; } }

    // Getter-Setter Functions
    public float Cooldown { get { return fNextCooldown; } }
    public int MaximumCount { get { return nMaximumChildCount; } }

    public float StrafingRadius { get { return fStrafingRadius; } }
    public float StrafingSpeed { get { return fStrafingSpeed; } }
    public float DefenceAngle { get { return fDefenceAngle; } }
    public float DefenceRadius { get { return fDefenceRadius; } }
    public float DefenceSpeed { get { return fDefenceSpeed; } }
    public float AttackSpeed { get { return fAttackSpeed; } }

    public bool IsAlive { get { return bIsAlive; } }
}
