using UnityEngine;
using System.Collections;

public class EMProductionState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMProductionState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{
		transition = m_EMFSM.emTransition;
		//Debug.Log ("Enter EMProductionState");
		// Reset transition availability
		transition.bCanTransit = true;
		// Pause the transition for 1 second
		m_EMFSM.StartPauseTransition (1f);
	}

	public override void Execute ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		// Produce enemy mini cell if has nutrient and can spawn
		if (controller.NutrientNum > 0 && m_EMFSM.CanSpawn)
			m_EMFSM.ProduceChild ();
		else if (controller.NutrientNum == 0 && m_EMFSM.CanSpawn)
			m_EMFSM.ChangeState (m_EMFSM.MaintainState);

		// Start checking transition only when there are more than 10 available enemy mini cells, transition is allowed and has nutrient
		if (m_EMFSM.AvailableChildNum > 10 && transition.bCanTransit && controller.NutrientNum > 0) 
		{
			// If there are more than 10  and less than 25 available enemy mini cells
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25) 
			{
				float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f;
				float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f;

				// Transition to Defend
				if (nEnemyChildFactor > nPlayerChildFactor && nPlayerChildFactor <= 5f && (nPlayerChildFactor - nEnemyChildFactor) > 1f) {
					transition.Transition (1000f / (helper.Pow (nPlayerChildFactor - nEnemyChildFactor, 2f) * 10f), m_EMFSM.DefendState);
				}

				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					transition.Transition (1000f / (helper.Pow (nEnemyChildFactor*1f / nPlayerChildFactor, 2f) * 3f + m_EMFSM.Aggressiveness * 5f), m_EMFSM.AggressiveAttackState);
				}

				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (1000f / (helper.Pow (nEnemyChildFactor*1.5f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.Aggressiveness * 3f), m_EMFSM.CautiousAttackState);
				}

				// Transition to Landmine
				if (nPlayerChildFactor > 5f) {
					transition.Transition (1000f / (helper.Pow (nPlayerChildFactor, 2f) * 2.5f), m_EMFSM.LandmineState);
				}

				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (1000f / helper.Pow (5f - Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f), m_EMFSM.MaintainState);
				}
			} 
			else if (m_EMFSM.AvailableChildNum > 25 && m_EMFSM.AvailableChildNum <= 50) 
			{
				float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f;
				float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f;

				// Transition to Defend
				if (nEnemyChildFactor < nPlayerChildFactor && nPlayerChildFactor <= 8f && (nPlayerChildFactor - nEnemyChildFactor) > 2f) {
					transition.Transition (1000f / (helper.Pow (nPlayerChildFactor - nEnemyChildFactor, 2f) * 5f), m_EMFSM.DefendState);
				}
				
				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					transition.Transition (1000f / (helper.Pow (nEnemyChildFactor*1.5f / nPlayerChildFactor, 2f) * 3f + m_EMFSM.Aggressiveness * 5f), m_EMFSM.AggressiveAttackState);
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (1000f / (helper.Pow (nEnemyChildFactor*2f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.Aggressiveness * 3f), m_EMFSM.CautiousAttackState);
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 5f) {
					transition.Transition (1000f / (helper.Pow (nPlayerChildFactor, 2f) * 1.5f), m_EMFSM.LandmineState);
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (1000f / helper.Pow (8f - Mathf.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f), m_EMFSM.MaintainState);
				}
			}
			else if (m_EMFSM.AvailableChildNum > 50)
			{
				float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f;
				float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f;
				
				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					transition.Transition (1000f / (helper.Pow (nEnemyChildFactor*1.75f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.Aggressiveness * 5f), m_EMFSM.AggressiveAttackState);
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (1000f / (helper.Pow (nEnemyChildFactor*2f / nPlayerChildFactor, 2f) * 7.5f + m_EMFSM.Aggressiveness * 3f), m_EMFSM.CautiousAttackState);
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 5f) {
					transition.Transition (1000f / (helper.Pow (nPlayerChildFactor, 2f) * 1.5f), m_EMFSM.LandmineState);
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (1000f / helper.Pow (5f - helper.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f), m_EMFSM.MaintainState);
				}
			}

			// Check transition every 0.1 second to save computing power
			if (transition.bCanTransit)
				m_EMFSM.StartPauseTransition (.1f);
		}
	}

	public override void Exit ()
	{
		transition = m_EMFSM.emTransition;

		//Debug.Log ("Exit EMProductionState");
		// Reset transition availability
		transition.bCanTransit = true;
	}
}