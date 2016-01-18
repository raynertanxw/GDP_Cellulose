using UnityEngine;
using System.Collections;

// PS_Logicaliser: The brain of the Player Squad Captain
public class PS_Logicaliser : MonoBehaviour
{
	// Editable Fields
	[Header("Conditions: Generic")]
	[Tooltip("The ratio to determine how aggressive or defensive the squad captain is")] [Range(0f, 1f)]
	[SerializeField] private float fAggressiveToDefensive = 0.5f;
	[Tooltip("The cooldown time between each thought process")]
	[SerializeField] private float fThinkCooldown = 0.5f;

	[Header("Conditions: Production")]
	[Tooltip("The minimum amount of child to consider producing")]
	[SerializeField] private int fMinimumChildProducing = 4;
	[Tooltip("The maximum amount of child to be at production")]
	[SerializeField] private int fMaximumChildProducing = 10;

	[Header("Conditions: Defensive")]
	[Tooltip("The maximum amount of aggressive cells at any one point")]
	[SerializeField] private int fMaximumChildDefence = 10;

	// Uneditable Fields
	private PlayerSquadFSM m_PlayerSquadFSM;    // m_SquadCaptain: The instance of the squad captain
	private float fCurrentThinkCooldown = 0f;   // fCurrentThinkCooldown: The current think cooldown of the brain - restricts the brain from thinking too much

	// Private Functions
	// Start(): Use this for initialisation
	void Start()
	{
		m_PlayerSquadFSM = PlayerSquadFSM.Instance;
	}

	// Public Functions
	// Think(): The think process of the Squad Captain. Call this method to return the desire state that the SquadCaptain should be in
	public void Think()
	{
		// if: Restricts the brain from think too much
		if (fCurrentThinkCooldown >= fThinkCooldown)
			fCurrentThinkCooldown = 0f;
		else
		{
			fCurrentThinkCooldown += Time.deltaTime;
			return;
		}

		// Production State Check
		// if: The number of alive squad child is less than fMininumChildCount <--------------------------------------------- DEPERATE TIMES
		if (PlayerSquadFSM.Instance.AliveChildCount() < fMinimumChildProducing)
		{
			SquadChildFSM.AdvanceSquadPercentage(SCState.Produce, 1f);
			m_PlayerSquadFSM.Advance(PSState.Produce);
		}
		// else if: There is more than enough child cells producing, moves to idle <----------------------------------------- RECOVERY
		else if (SquadChildFSM.StateCount(SCState.Produce) > fMaximumChildProducing)
		{
			SquadChildFSM.AdvanceSquadPercentage(SCState.Produce, SCState.Idle, 0.75f);
		}

		// if: There is child idling, assign them to defence <-------------------------------------------------------------- ASSIGN JOB
		if (SquadChildFSM.StateCount(SCState.Idle) > 100)
		{
			// Runs a randomiser to determine whether if the cell will be assign to aggesive or defensive
			// if: The cells will become aggressive
			float value = UnityEngine.Random.value;
			if (UnityEngine.Random.value > fAggressiveToDefensive)
			{
				SquadChildFSM.AdvanceSquadPercentage(SCState.Idle, SCState.Attack, 1f);
			}
			// else: The cells will become defensive
			else
			{
				// if: This will check if the current condition still have room to move squad childs to defence state,
				//     and is NOT the maximum number of child cells that can be in defence squad
				if (SquadChildFSM.StateCount(SCState.Defend) < fMaximumChildDefence)
				{
					SquadChildFSM.AdvanceSquadPercentage(SCState.Idle, SCState.Defend, 1f);
				}
			}
		}
	}
}

