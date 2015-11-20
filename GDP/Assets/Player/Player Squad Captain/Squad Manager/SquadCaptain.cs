using UnityEngine;
using System.Collections;

// SquadCaptain.cs: Function of script here.
public class SquadCaptain : MonoBehaviour 
{
    /* NOTES ON SQUAD CHILD SPAWNING ------------------------------------------------------------------------------------------------
     * > The rate of squad child spawn is relative to the number of squad child in the production state (alive too)
     * > The cooldown can be calculated with the following formula:
     * 
     *      cooldownTime = (s_fMaxCooldown - s_fMinCooldown) * ((s_nMaxChildCount - "The number of cells in Production state") / s_nMaxChildCount) + s_fMinCooldown
     *
     * Refer to the variables below**
     * 
     * > TL;DR: The more the number of squad child in production, the lesser the cooldown, the faster the squad child are produced
     * ------------------------------------------------------------------------------------------------------------------------------
     */

	// Editables Fields
    [Header("Costs")]
    [Tooltip("The cost to initiate a squad")]
    [SerializeField] private static int s_nCostPoints = 50;
    [Tooltip("The cost of one squad child")]
    [SerializeField] private static int s_nChildCost = 10;

    [Header("Child Spawn: Generic")]
    [Tooltip("The maximum amount of child cells it can produce")]
    [SerializeField] private static int s_nMaximumChildCount = 50;
    [Tooltip("The squad child transform")]
    [SerializeField] private static Transform s_m_transformSquadChild;

    [Header("Child Spawn: Cooldown")]
    [Tooltip("The minimum amount of cooldown for each child spawn")]
    [SerializeField] private static float s_fMinimumCooldown = 3f;
    [Tooltip("The maximum amount of cooldown for each child spawn")]
    [SerializeField] private static float s_fMaximumCooldown = 10f;
	
	// Uneditables Fields
    private int nResource = 0;  // nResource: The number of resource the player squad has

    // arrayChildCount: 
    private PlayerSquadFSM[] arraySquadChild = new PlayerSquadFSM[s_nMaximumChildCount];
	
	// Component/GameObject Instances

	// Private Functions
	// Start(): Use this for initialization
	void Start () 
	{
        for (int i = 0; i < arraySquadChild.Length; i++)
        {
            arraySquadChild[i] = ((GameObject)Instantiate(s_m_transformSquadChild, transform.position, Quaternion.identity)).GetComponent<PlayerSquadFSM>();
        }
	}
	
	// Update(): is called once per frame
	void Update () 
	{
	
	}
	
	// Public Functions
    // Initialise(): Make alive of Squad Captain
    public bool Initialise()
    {
        return false;
    }
	
	// Getter-Setter Functions
    public int Resource { get { return nResource; } }
}
