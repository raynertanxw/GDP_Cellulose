﻿using UnityEngine;
using System.Collections;

public class EMMaintainState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMMaintainState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{
		transition = m_EMFSM.emTransition;
		helper = m_EMFSM.emHelper;
		
		transition = m_EMFSM.emTransition;
		// Reset transition availability
		transition.CanTransit = true;
		// Pause the transition for randomized time based on num of available child cells
		float fPauseTime = Random.Range (Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().AvailableChildNum) * 10f) / EMDifficulty.Instance().CurrentDiff, 
		                                 Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().AvailableChildNum) * 50f) / EMDifficulty.Instance().CurrentDiff);
		helper.StartPauseTransition (fPauseTime);
	}

	public override void Execute ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		// Start checking transition only when there are more than 10 available enemy mini cells and transition is allowed 
		if (m_EMFSM.AvailableChildNum > 10 && transition.CanTransit) 
		{
			// If there are more than 10  and less than 25 available enemy mini cells
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25) 
			{
				float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f + 1f;
				float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f + 1f;
				
				// Transition to Defend
				if (nEnemyChildFactor < nPlayerChildFactor && nPlayerChildFactor <= 5f && (nPlayerChildFactor - nEnemyChildFactor) > 1f) {
					transition.Transition (1000f / 
					                       ((helper.Pow (nPlayerChildFactor - nEnemyChildFactor, 2f) * 10f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Defend)),
					                       EMState.Defend);
				}
				
				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					transition.Transition (1000f / 
					                       (helper.Pow (nEnemyChildFactor*1f / nPlayerChildFactor, 2f) * 3f + m_EMFSM.CurrentAggressiveness * 3.5f + 
					 EMLeraningAgent.Instance().RealScore(EMState.AggressiveAttack)),
					                       EMState.AggressiveAttack);
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (1000f / 
					                       (helper.Pow (nEnemyChildFactor*1.5f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.CurrentAggressiveness * 2f + 
					 EMLeraningAgent.Instance().RealScore(EMState.CautiousAttack)),
					                       EMState.CautiousAttack);
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 1.5f) {
					transition.Transition (1000f / 
					                       (helper.Pow (nPlayerChildFactor, 3f) * 2f + m_EMFSM.CurrentAggressiveness * 3f + 
					 EMLeraningAgent.Instance().RealScore(EMState.Landmine)), 
					                       EMState.Landmine);
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (1000f / 
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
					transition.Transition (1000f / 
					                       (helper.Pow (nPlayerChildFactor - nEnemyChildFactor, 2f) * 5f + 
					 EMLeraningAgent.Instance().RealScore(EMState.Defend)),
					                       EMState.Defend);
				}
				
				// Transition to EMAggressiveAttack
				if (nEnemyChildFactor > nPlayerChildFactor * 1.5f) {
					transition.Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*1.5f / nPlayerChildFactor, 2f) * 3f + m_EMFSM.CurrentAggressiveness * 3.75f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.AggressiveAttack)),
					                       EMState.AggressiveAttack);
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*2f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.CurrentAggressiveness * 2.25f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.CautiousAttack)),
					                       EMState.CautiousAttack);
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 1f) {
					transition.Transition (1000f / 
					                       ((helper.Pow (nPlayerChildFactor, 2f) / helper.Sqrt (nPlayerChildFactor) * 1.5f + m_EMFSM.CurrentAggressiveness * 1f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Landmine)), 
					                       EMState.Landmine);
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (1000f / 
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
					transition.Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*1.75f / nPlayerChildFactor, 2f) * 5f + m_EMFSM.CurrentAggressiveness * 4f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.AggressiveAttack)),
					                       EMState.AggressiveAttack);
				}
				
				// Transition to EMCautiousAttack
				if (nEnemyChildFactor > nPlayerChildFactor) {
					transition.Transition (1000f / 
					                       ((helper.Pow (nEnemyChildFactor*2f / nPlayerChildFactor, 2f) * 7.5f + m_EMFSM.CurrentAggressiveness * 2.5f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.CautiousAttack)),
					                       EMState.CautiousAttack);
				}
				
				// Transition to Landmine
				if (nPlayerChildFactor > 1f) {
					transition.Transition (1000f / 
					                       ((helper.Pow (nPlayerChildFactor, 2f) / helper.Sqrt (nPlayerChildFactor) * 1f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Landmine)), 
					                       EMState.Landmine);
				}
				
				// Transition to Maintain
				if (nPlayerChildFactor <= 5f && helper.Abs ((nEnemyChildFactor - nPlayerChildFactor)) <= 1f) {
					transition.Transition (1000f / 
					                       (helper.Pow (3f - helper.Pow (nEnemyChildFactor - nPlayerChildFactor, 2f), 2f) + 
					 EMLeraningAgent.Instance().RealScore(EMState.Maintain)), 
					                       EMState.Maintain);
				}
			}
			
			// Check transition every 0.1 second to save computing power
			if (transition.CanTransit)
				helper.StartPauseTransition (.1f);
		}
	}

	public override void Exit ()
	{
		transition = m_EMFSM.emTransition;
		
		// Reset transition availability
		transition.CanTransit = true;
	}
}