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

	// Editables Fields
    [Header("Costs")]
    [Tooltip("The cost to initiate a squad")]
    [SerializeField] private int nCostPoints = 50;
    [Tooltip("The cost of one squad child")]
    [SerializeField] private int nChildCost = 10;

    [Header("Child Spawn: Generic")]
    [Tooltip("The maximum amount of child cells it can produce")]
    [SerializeField] private int nMaximumChildCount = 50;
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
        return 0;
    }

	// Start(): Use this for initialization
	void Start () 
	{
        arraySquadChild = new PlayerSquadFSM[nMaximumChildCount];
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
        if (x == nMaximumChildCount)
        {
            fNextCooldown = fMinimumCooldown;
            return true;
        }

        fNextCooldown = (fMaximumCooldown - fMinimumCooldown) * ((nMaximumChildCount - x) / nMaximumChildCount) + fMinimumCooldown;

        if (fNextCooldown <= 0f)
        {
            Debug.LogWarning(this.name + ".CalculateCooldown: fNextCooldown is less than or equal to 0! fNextCooldown = " + fNextCooldown);

            fNextCooldown = 0.0f;
            return false;
        }
        else
            return true;
    }

    // GetActiveChildCount(): Returns the number of squad child cells that is alive <--------------------------------------------- EDIT
    public int GetActiveChildCount()
    {
        return 0;
    }
	
	// Getter-Setter Functions
    public int Nutrient { get { return nNutrient; } }
    public float Cooldown { get { return fNextCooldown; } }
}
