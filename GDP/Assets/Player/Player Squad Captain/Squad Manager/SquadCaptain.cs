using UnityEngine;
using System.Collections;

// SquadCaptain.cs: Function of script here.
public class SquadCaptain : MonoBehaviour 
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
    private static SquadCaptain s_m_Instance;   // m_Instance: Stores this instance in this variable, used for singleton purposes

	// Editables Fields
    [Header("Costs")]
    [Tooltip("The cost to initiate a squad")]
    [SerializeField] private int nCostPoints = 50;
    [Tooltip("The cost of one squad child")]
    [SerializeField] private int nChildCost = 10;

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
	
	// Uneditables Fields
    private int nNutrient = 0;                  // nNutrient: The number of nutrient the squad currently has
    private float fNextCooldown = 0.0f;         // fNextCooldown: Stores the time of the cooldown
    private bool bIsAlive = false;              // bIsAlive: Returns if the squad captain is alive
    private Vector3 m_strafingVector;           // m_strafingVector: The current direction of the strafing vector
    private bool isStrafeVectorUpdated = true;  // isStrafeVectorUpdated: Checks if the strafing vector is updated

    // Co-Routines
    IEnumerator SpawnRoutine()
    {
        bool isLoop = true;
        while (isLoop)
        {
            yield return new WaitForSeconds(fNextCooldown);

            if (!this.CalculateCooldown())
                isLoop = false;
            else
            {
                PlayerSquadFSM.Spawn(transform.position);
            }
        }
    }

	// Private Functions
    void OnEnable()
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
        m_strafingVector = Vector3.up;

        StartCoroutine(SpawnRoutine());
    }

    // Update(): is called once every frame
    void Update()
    {
        if (!isStrafeVectorUpdated)
        {
            float fCurrentRadius = Mathf.PingPong(Time.time * 0.5f, fStrafingRadius);
            if (fCurrentRadius < 0.4f)
                fCurrentRadius = 0.4f;
            // NOTE: Quaternions q * Vector v returns the v rotated in q direction, THOUGH REMEMBER TO NORMALIZED ELSE VECTOR WILL PISS OFF INTO SPACE
            m_strafingVector = (Quaternion.Euler(0, 0, fStrafingSpeed) * m_strafingVector).normalized * fCurrentRadius;
            isStrafeVectorUpdated = true;
        }
    }

    // GetProductionChildCount(): Returns the number of child cells in production state
    private int GetProductionChildCount()
    {
        return PlayerSquadFSM.StateCount(SCState.Produce);
    }
	
	// Public Functions
    // Initialise(): Make alive of Squad Captain <-------------------------------------------------------------------------------- EDIT
    public bool Initialise()
    {
        return false;
    }

    // CalculateCooldown(): Call this function to re-calculate the cooldown of production
    public bool CalculateCooldown()
    {
        // if: There is max number of production (all cells are alive)
        if (PlayerSquadFSM.AliveCount() == nMaximumChildCount)
        {
            fNextCooldown = fMinimumCooldown;
            return true;
        }

        fNextCooldown = (fMaximumCooldown - fMinimumCooldown) * ((float)(nMaximumChildCount - PlayerSquadFSM.StateCount(SCState.Idle)) / (float)nMaximumChildCount) + fMinimumCooldown;

        if (fNextCooldown <= 0f)
        {
            Debug.LogWarning(this.name + ".CalculateCooldown: fNextCooldown is less than or equal to 0! fNextCooldown = " + fNextCooldown);

            fNextCooldown = 0.0f;
            return false;
        }
        else
            return true;
    }

    // AliveChildCount(): Returns the number of squad child cells that is alive
    public int AliveChildCount()
    {
        return PlayerSquadFSM.AliveCount();
    }

    // StrafingVector(): calculates and return the strafing vector
    public Vector3 StrafingVector()
    {
        isStrafeVectorUpdated = false;
        return m_strafingVector;
    }

    // Public Static Functions
    public static SquadCaptain Instance { get { return s_m_Instance; } }

	// Getter-Setter Functions
    public int Nutrient { get { return nNutrient; } }
    public float Cooldown { get { return fNextCooldown; } }
    public int MaximumCount { get { return nMaximumChildCount; } }
    public bool IsAlive { get { return bIsAlive; } }
}
