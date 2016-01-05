using UnityEngine;
using System.Collections;

public class EMProductionState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;
	private ECPoolManager ECPool;

	public EMProductionState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{
		Debug.Log ("Enter EMProductionState");
		
		// Reset transition availability
		EMTransition.Instance().CanTransit = true;
		// Reset spawn availability

		// Pause the transition for randomized time
		float fPauseTime = Random.Range (2f, 4f);
		m_EMFSM.StartPauseTransition (fPauseTime);
	}

	public override void Execute ()
	{
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;
		ECPool = GameObject.Find("Enemy Child Cell Pool").GetComponent<ECPoolManager>();
	
		// Produce enemy mini cell if has nutrient and can spawn
		if (controller.NutrientNum > 0 && m_EMFSM.CanSpawn)
			ECPool.SpawnFromPool (m_EMFSM.Position);
		else if (controller.NutrientNum == 0 && EMTransition.Instance().CanTransit)
			m_EMFSM.ChangeState (EMState.Maintain);

		// Start checking transition only when there are more than 10 available enemy mini cells, transition is allowed and has nutrient
		if (m_EMFSM.AvailableChildNum > 10 && EMTransition.Instance().CanTransit && controller.NutrientNum > 0) 
		{
			// If there are more than 10  and less than 25 available enemy mini cells
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25) 
			{
				float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f + 1f;
				float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f + 1f;
				
				// Transition to Defend
				if (nEnemyChildFactor < nPlayerChildFactor && nPlayerChildFactor <= 5f && (nPlayerChildFactor - nEnemyChildFactor) > 1f) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nPlayerChildFactor - nEnemyChildFactor, 2f) * 10f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Defend)),
					                       EMState.Defend);
				}

				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*1f / nPlayerChildFactor, 2f) * 3f) + m_EMFSM.CurrentAggressiveness * 3.5f + 
					 EMLeraningAgent.Instance().RealScore(EMState.AggressiveAttack)),
					                       EMState.AggressiveAttack);
				}

				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					EMTransition.Instance().Transition (1000f / 
					                       (helper.Pow (nEnemyChildFactor*1.5f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.CurrentAggressiveness * 2f + 
					 EMLeraningAgent.Instance().RealScore(EMState.CautiousAttack)),
					                       EMState.CautiousAttack);
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 0.5f) {
					EMTransition.Instance().Transition (1000f / 
					                       (helper.Pow (nPlayerChildFactor, 3f) * 2.5f + m_EMFSM.CurrentAggressiveness * 3f + 
					 EMLeraningAgent.Instance().RealScore(EMState.Landmine)), 
					                       EMState.Landmine);
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					EMTransition.Instance().Transition (1000f / 
					                       (helper.Pow (3f - helper.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Maintain)), 
					                       EMState.Maintain);
				}
			} 
			else if (m_EMFSM.AvailableChildNum > 25 && m_EMFSM.AvailableChildNum <= 50) 
			{
				float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f + 1f;
				float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f + 1f;
				
				// Transition to Defend
				if (nEnemyChildFactor < nPlayerChildFactor && nPlayerChildFactor <= 8f && (nPlayerChildFactor - nEnemyChildFactor) > 2f) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nPlayerChildFactor - nEnemyChildFactor, 2f) * 5f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Defend)),
					                       EMState.Defend);
				}
				
				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*1.5f / nPlayerChildFactor, 2f) * 3f + m_EMFSM.CurrentAggressiveness * 3.75f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.AggressiveAttack)),
					                       EMState.AggressiveAttack);
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*2f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.CurrentAggressiveness * 2.25f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.CautiousAttack)),
					                       EMState.CautiousAttack);
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 1f) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nPlayerChildFactor, 2f) / helper.Sqrt (nPlayerChildFactor) * 2f + m_EMFSM.CurrentAggressiveness * 1f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Landmine)), 
					                       EMState.Landmine);
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					EMTransition.Instance().Transition (1000f / 
					                       (helper.Pow (5f - helper.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Maintain)), 
					                       EMState.Maintain);
				}
			}
			else if (m_EMFSM.AvailableChildNum > 50)
			{
				float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f + 1f;
				float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f + 1f;
				
				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*1.75f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.CurrentAggressiveness * 4f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.AggressiveAttack)),
					                       EMState.AggressiveAttack);
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*2f / nPlayerChildFactor, 2f) * 7.5f + m_EMFSM.CurrentAggressiveness * 2.5f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.CautiousAttack)),
					                       EMState.CautiousAttack);
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 2f) {
					EMTransition.Instance().Transition (1000f / 
					                       ((helper.Pow (nPlayerChildFactor, 2f) / helper.Sqrt (nPlayerChildFactor) * 1.5f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Landmine)), 
					                       EMState.Landmine);
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					EMTransition.Instance().Transition (1000f / 
					                       (helper.Pow (3f - helper.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Maintain)), 
					                       EMState.Maintain);
				}
			}

			// Check transition every 0.2 second to save computing power
			if (EMTransition.Instance().CanTransit)
				m_EMFSM.StartPauseTransition (.2f);
		}
	}

	public override void Exit ()
	{
		Debug.Log ("Exit EMProductionState");
		
		// Reset transition availability
		EMTransition.Instance().CanTransit = true;
	}
}