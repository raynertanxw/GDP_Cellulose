using UnityEngine;
using System.Collections;

public class EMAggressiveAttackState : IEMState
{	
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMAggressiveAttackState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;
	}

	public override void Enter ()
	{
		Debug.Log ("Enter EMAggressiveAttackState");

		transition = m_EMFSM.emTransition;
		helper = m_EMFSM.emHelper;
		
		// Reset availability to command mini cell to Attack state
		if (!helper.CanAddAttack)
			helper.CanAddAttack = true;
		
		// Reset transition availability
		transition.CanTransit = true;
		// Pause the transition for randomized time
		float fPauseTime = Random.Range (1.5f, 3f);
		m_EMFSM.StartPauseTransition (fPauseTime);
	}

	public override void Execute ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		#region Attack only when there are more enemy mini cells than player's 
		if (m_EMFSM.AvailableChildNum > PlayerChildFSM.GetActiveChildCount () && helper.CanAddAttack) 
		{
			float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f + 1f;
			float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f + 1f;
			
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25)
			{
				for (int nAmount = 0; nAmount < Random.Range (1, 3 + (int)nEnemyChildFactor); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Idle || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Defend || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Avoid)
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Attack,0.0);
					}
				}
			}
			else if (m_EMFSM.AvailableChildNum > 25 && m_EMFSM.AvailableChildNum <= 50)
			{
				for (int nAmount = 0; nAmount < Random.Range (2, 4 + (int)nEnemyChildFactor); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Idle || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Defend || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Avoid)
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Attack,0.0);
					}
				}
			}
			else if (m_EMFSM.AvailableChildNum > 50)
			{
				for (int nAmount = 0; nAmount < Random.Range (4, 6 + (int)nEnemyChildFactor); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Idle || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Defend || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Avoid)
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Attack,0.0);
					}
				}
			}
			
			// Pause commanding enemy mini cells to Attack state
			// Pause duration depends only on the number of enemy mini cells
			m_EMFSM.StartPauseAddAttack (1.5f - nEnemyChildFactor/10f);
		}
		#endregion
		#region Transition
		if (transition.CanTransit && controller.NutrientNum > 0)
			m_EMFSM.ChangeState (EMState.Production);
		else if (transition.CanTransit && controller.NutrientNum == 0)
			m_EMFSM.ChangeState (EMState.Maintain);
		#endregion
		
		// Check transition every 0.2 second to save computing power
		if (transition.CanTransit)
			m_EMFSM.StartPauseTransition (.2f);
	}

	public override void Exit ()
	{
		Debug.Log ("Exit EMAggressiveAttackState");

		transition = m_EMFSM.emTransition;
		helper = m_EMFSM.emHelper;

		// Reset availability to command mini cell to Attack state
		if (!helper.CanAddAttack)
			helper.CanAddAttack = true;
		// Reset transition availability
		transition.CanTransit = true;
	}
}