using UnityEngine;
using System.Collections;

public class EMCautiousAttackState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMCautiousAttackState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{
		Debug.Log ("Enter EMCautiousAttackState");

		transition = m_EMFSM.emTransition;
		helper = m_EMFSM.emHelper;

		// Turn blink animation on
		EMAnimation.Instance ().CanBlink = true;

		// Reset availability to command mini cell to Attack state
		if (!helper.CanAddAttack)
			helper.CanAddAttack = true;

		// Reset transition availability
		transition.CanTransit = true;
		// Pause the transition for randomized time
		float fPauseTime = Random.Range (Mathf.Sqrt (m_EMFSM.CurrentAggressiveness) * 1.5f / EMDifficulty.Instance().CurrentDiff, 
		                                 Mathf.Sqrt (m_EMFSM.CurrentAggressiveness) * 2f / EMDifficulty.Instance().CurrentDiff);
		helper.StartPauseTransition (fPauseTime);
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
				for (int nAmount = 0; nAmount < Random.Range (1, 3 + (int)(nEnemyChildFactor - nPlayerChildFactor)); nAmount++)
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
				for (int nAmount = 0; nAmount < Random.Range (2, 4 + (int)(nEnemyChildFactor - nPlayerChildFactor)); nAmount++)
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
				for (int nAmount = 0; nAmount < Random.Range (4, 6 + (int)(nEnemyChildFactor - nPlayerChildFactor)); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Idle || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Defend || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Avoid)
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Attack,0.0);
					}
				}
			}

			// Pause commanding enemy mini cells to Attack state
			// Pause duration depends on the difference in cell numbers
			float fPauseTime = 1.5f - (nEnemyChildFactor - nPlayerChildFactor) / 10f / EMDifficulty.Instance().CurrentDiff;
			if (fPauseTime > 0f)
				helper.StartPauseAddAttack (fPauseTime);
		}
		#endregion

		#region Transition
		if (transition.CanTransit && controller.NutrientNum > 0)
			m_EMFSM.ChangeState (EMState.Production);
		else if (transition.CanTransit && controller.NutrientNum == 0)
			m_EMFSM.ChangeState (EMState.Maintain);
		#endregion
	}

	public override void Exit ()
	{
		Debug.Log ("Exit EMCautiousAttackState");

		transition = m_EMFSM.emTransition;
		helper = m_EMFSM.emHelper;

		// Reset animation status
		if (EMAnimation.Instance ().CanBlink)
			EMAnimation.Instance ().CanBlink = false;
		// Reset availability to command mini cell to Attack state
		if (!helper.CanAddAttack)
			helper.CanAddAttack = true;
		// Reset transition availability
		transition.CanTransit = true;
	}
}