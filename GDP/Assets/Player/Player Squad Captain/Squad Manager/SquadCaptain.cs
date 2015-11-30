using UnityEngine;
using System.Collections;

// SquadCaptain.cs: Function of script here.
public class SquadCaptain : MonoBehaviour 
{
    /* NOTES ON SQUAD CHILD SPAWNING ------------------------------------------------------------------------------------------------
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
    private static int s_nMaximumChildCount = 50;   // s_nMaximumChildCount: THe maximum number of child count in the squad
    private static bool s_isAlive = false;            // isAlive: Returns if the squad captain is alive

	// Editables Fields
    [Header("Costs")]
    [Tooltip("The cost to initiate a squad")]
    [SerializeField] private int nCostPoints = 50;
    [Tooltip("The cost of one squad child")]
    [SerializeField] private int nChildCost = 10;

    [Header("Child Spawn: Generic")]
    [Tooltip("The squad child transform")]
    [SerializeField] private Transform m_transformSquadChild;

    [Header("Child Spawn: Cooldown")]
    [Tooltip("The minimum amount of cooldown for each child spawn")]
    [SerializeField] private float fMinimumCooldown = 3f;
    [Tooltip("The maximum amount of cooldown for each child spawn")]
    [SerializeField] private float fMaximumCooldown = 10f;
	
	// Uneditables Fields
    private int nNutrient = 0;                  // nNutrient: The number of nutrient the squad currently has
    private float fNextCooldown = 0.0f;         // fNextCooldown: Stores the time of the cooldown

    private PlayerSquadFSM[] arraySquadChild;   // arraySquadChild: An array to store all the squad child

    float x = 0;

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
                x++;
            Debug.Log(fNextCooldown + " seconds to next, currently " + x + " cells");
        }
    }

	// Private Functions
    // GetProductionChildCount(): Returns the number of child cells in production state
    private int GetProductionChildCount()
    {
        return PlayerSquadFSM.StateCount(SCState.Produce);
    }

	// Start(): Use this for initialization
	void Start () 
	{
        arraySquadChild = new PlayerSquadFSM[s_nMaximumChildCount];
        // for: Initialises the squad child and insert them into an array
        //for (int i = 0; i < arraySquadChild.Length; i++)
        //{
        //    arraySquadChild[i] = ((GameObject)Instantiate(m_transformSquadChild, transform.position, Quaternion.identity)).GetComponent<PlayerSquadFSM>();
        //}

        StartCoroutine(SpawnRoutine());
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
        if (x == s_nMaximumChildCount)
        {
            fNextCooldown = fMinimumCooldown;
            return true;
        }

        fNextCooldown = (fMaximumCooldown - fMinimumCooldown) * ((s_nMaximumChildCount - x) / s_nMaximumChildCount) + fMinimumCooldown;

        if (fNextCooldown <= 0f)
        {
            Debug.LogWarning(this.name + ".CalculateCooldown: fNextCooldown is less than or equal to 0! fNextCooldown = " + fNextCooldown);

            fNextCooldown = 0.0f;
            return false;
        }
        else
            return true;
    }

    // GetAliveChildCount(): Returns the number of squad child cells that is alive <--------------------------------------------- EDIT
    public int GetAliveChildCount()
    {
        return PlayerSquadFSM.GetAliveCount();
    }

    // Public Static Functions
    public static int MaximumCount { get { return s_nMaximumChildCount; } }
    public static bool IsAlive { get { return s_isAlive; } }

	// Getter-Setter Functions
    public int Nutrient { get { return nNutrient; } }
    public float Cooldown { get { return fNextCooldown; } }
}
