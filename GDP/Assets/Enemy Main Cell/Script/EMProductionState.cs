using UnityEngine;
using System.Collections;

public class EMProductionState : IEMState
{
	public static EMProductionState instance;

	public GameObject FSM;

	private EMTransition transition;
	private EMController controller;
	
	void Awake ()
	{
		transition = gameObject.GetComponent<EMTransition> ();
		controller = gameObject.GetComponent<EMController> ();
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
	}

	// Singleton
	public static EMProductionState Instance()
	{
		if (instance == null)
			instance = new EMProductionState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		StartCoroutine (m_EMFSM.ProduceChild ());

		// Start checking transition only when there are more than 10 available enemy mini cells and transition is allowed
		if (m_EMFSM.nAvailableChildNum > 10 && transition.bCanTransit) 
		{
			// If there are more than 10  and less than 25 available enemy mini cells
			if (m_EMFSM.nAvailableChildNum > 10 && m_EMFSM.nAvailableChildNum <= 25) 
			{
				float nEnemyChildFactor = controller.nSize / 10;
				float nPlayerChildFactor;

				// Transition to Defend
				if (nPlayerChildFactor <= 5f && (nPlayerChildFactor - nEnemyChildFactor) > 1f) {
					transition.Transition (100f - Mathf.Pow (Mathf.Abs (nPlayerChildFactor - nEnemyChildFactor), 2f) * 5f, EMDefendState.Instance ());
				}

				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					transition.Transition (100f - (Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f) * 3f + m_EMFSM.nAggressiveness * 1.5f), EMAggressiveAttackState.Instance ());
				}

				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (100f - (Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f) * 3f + m_EMFSM.nAggressiveness), EMCautiousAttackState.Instance ());
				}

				// Transition to Landmine
				if (nPlayerChildFactor > 5f) {
					transition.Transition (100f - Mathf.Pow (nPlayerChildFactor, 2f) * 1f, EMLandmineState.Instance ());
				}

				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && Mathf.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (100f - Mathf.Pow (5f - Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f), EMMaintainState.Instance ());
				}
			} 
			else if (m_EMFSM.nAvailableChildNum > 25 && m_EMFSM.nAvailableChildNum <= 50) 
			{
				float nEnemyChildFactor = controller.nSize / 10;
				float nPlayerChildFactor;

				// Transition to Defend
				if (nPlayerChildFactor <= 8f && (nPlayerChildFactor - nEnemyChildFactor) > 2f) {
					transition.Transition (100f - Mathf.Pow (Mathf.Abs (nPlayerChildFactor - nEnemyChildFactor), 2f) * 3f, EMDefendState.Instance ());
				}
				
				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					transition.Transition (100f - (Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f) * 4f + m_EMFSM.nAggressiveness * 2f), EMAggressiveAttackState.Instance ());
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (100f - (Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f) * 4f + m_EMFSM.nAggressiveness * 1.5f), EMCautiousAttackState.Instance ());
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 5f) {
					transition.Transition (100f - Mathf.Pow (nPlayerChildFactor, 2f) * 1.5f, EMLandmineState.Instance ());
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && Mathf.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (100f - Mathf.Pow (8f - Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f), EMMaintainState.Instance ());
				}
			}
			else if (m_EMFSM.nAvailableChildNum > 50)
			{
				float nEnemyChildFactor = controller.nSize / 10;
				float nPlayerChildFactor;
				
				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					transition.Transition (100f - (Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f) * 4f + m_EMFSM.nAggressiveness * 2f), EMAggressiveAttackState.Instance ());
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (100f - (Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f) * 4f + m_EMFSM.nAggressiveness * 1.5f), EMCautiousAttackState.Instance ());
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 5f) {
					transition.Transition (100f - Mathf.Pow (nPlayerChildFactor, 2f) * 1.25f, EMLandmineState.Instance ());
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && Mathf.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (100f - Mathf.Pow (5f - Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f), EMMaintainState.Instance ());
				}
			}

			// Check transition every 0.25 second to save computing power
			StartCoroutine (transition.TransitionAvailability ());
		}
	}
}