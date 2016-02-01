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
	[SerializeField] private int nMinimumChildProducing = 4;
	[Tooltip("The maximum amount of child to be at production")]
	[SerializeField] private int nMaximumChildProducing = 10;
	[Tooltip("The amount of alive child to determine that its not important to produce anymore")]
	[SerializeField] private int nOptionalToProduceAt = 40;

	[Header("Conditions: Defensive")]
	[Tooltip("When there is less than this amount of defence child, the shield will fall apart")]
	[SerializeField] private int nMinimumChildDefence = 5;
	[Tooltip("When there is this amount of defence squad child, the squad captain will no longer assign more squad child to defence")]
	[SerializeField] private int nMaximumChildDefence = 15;

	[Header("Conditions: Find Resource")]
	[Tooltip("This amount of resources in player will determines that the player is low on resources")]
	[SerializeField] private int nNeedNutrients = 500;
	[Tooltip("The number of child must be idling before the squad captain thinks it is good to allocate some for finding resources")]
	[SerializeField] private int nAmountIdleBeforeConsider = 10;

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

		// Squad Child Cells Count Check
		// if: The number of alive squad child is less than fMininumChildCount <--------------------------------------------- DEPERATE TIMES
		if (PlayerSquadFSM.Instance.AliveChildCount() < nMinimumChildProducing)
		{
			SquadChildFSM.AdvanceSquadPercentage(SCState.Produce, 1f);
			m_PlayerSquadFSM.Advance(PSState.Produce);
		}
		// else if: There is more than enough child cells producing, moves to idle <----------------------------------------- RECOVERY
		else if (SquadChildFSM.StateCount(SCState.Produce) > nMaximumChildProducing)
		{
			SquadChildFSM.AdvanceSquadPercentage(SCState.Produce, SCState.Idle, 0.75f);
		}

		// if: There is child idling, assign them to defence <-------------------------------------------------------------- ASSIGN JOB
		if (SquadChildFSM.StateCount(SCState.Idle) > 0)
		{
			// if: Aggressive is triggered
			if (UnityEngine.Random.value > fAggressiveToDefensive)
				SquadChildFSM.AdvanceSquadPercentage(SCState.Idle, SCState.Attack, fAggressiveToDefensive);
			else if (SquadChildFSM.StateCount(SCState.Defend) < nMaximumChildDefence)
			{
				int nDefenceCapacity = nMaximumChildDefence - SquadChildFSM.StateCount(SCState.Defend);

				if (nDefenceCapacity > nMinimumChildDefence)
					SquadChildFSM.AdvanceSquadPercentage(SCState.Idle, SCState.Attack, nDefenceCapacity);
			}
		}

		// if: There is no child cells producing but it is still important to do it <-------------------------------------- ASSIGN JOB
		if (SquadChildFSM.StateCount(SCState.Produce) == 0 && SquadChildFSM.AliveCount() < nOptionalToProduceAt)
		{
			SquadChildFSM.AdvanceSquadPercentage(SCState.Produce, 0.2f);
		}

		// if: There is extra child in Idle and the player have relatively low amount of resource
		if (SquadChildFSM.StateCount(SCState.Idle) >= nAmountIdleBeforeConsider && player_control.Instance.s_nResources <= nNeedNutrients)
		{
			SquadChildFSM.AdvanceSquadPercentage(SCState.FindResource, 0.75f);
		}
	}
}