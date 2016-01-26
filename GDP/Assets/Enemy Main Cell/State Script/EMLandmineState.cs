using UnityEngine;
using System.Collections;

public class EMLandmineState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMLandmineState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{
		Debug.Log ("Enter EMLandmineState");

		transition = m_EMFSM.emTransition;
		helper = m_EMFSM.emHelper;

		// Turn blink animation on
		EMAnimation.Instance ().CanBlink = true;

		// Reset availability to command mini cell to Landmine state
		if (!helper.CanAddLandmine)
			helper.CanAddLandmine = true;

		// Enable expand animation
		if (!EMAnimation.Instance ().IsExpanding)
			EMAnimation.Instance ().IsExpanding = true;
		
		// Reset transition availability
		transition.CanTransit = true;
		// Pause the transition for randomized time
		float fPauseTime = Random.Range ((m_EMFSM.CurrentAggressiveness / 1.5f) / EMDifficulty.Instance().CurrentDiff, 
		                                 (m_EMFSM.CurrentAggressiveness) / EMDifficulty.Instance().CurrentDiff);
		helper.StartPauseTransition (fPauseTime);
	}

	public override void Execute ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		int nCommandNum = 0;
		
		#region Landmine Behaviour
		if (helper.CanAddLandmine) 
		{
			float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f + 1f;
			float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f + 1f;
			
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25)
			{
				nCommandNum = Random.Range (1, 3 + (int)nEnemyChildFactor);
				for (int nAmount = 0; nAmount < nCommandNum; nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Idle || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Defend || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Avoid)
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.EnemyMainObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Landmine,0.0);
						Debug.Log ("Lanmine Message Sent.");
						helper.CanAddLandmine = false;
					}
				}
			}
			else if (m_EMFSM.AvailableChildNum > 25 && m_EMFSM.AvailableChildNum <= 50)
			{
				nCommandNum = Random.Range (2, 4 + (int)nEnemyChildFactor);
				for (int nAmount = 0; nAmount < nCommandNum; nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Idle || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Defend || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Avoid)
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.EnemyMainObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Landmine,0.0);
						helper.CanAddLandmine = false;
					}
				}
			}
			else if (m_EMFSM.AvailableChildNum > 50)
			{
				nCommandNum = Random.Range (3, 5 + (int)nEnemyChildFactor);
				for (int nAmount = 0; nAmount < nCommandNum; nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Idle || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Defend || m_EMFSM.ECList[nIndex].CurrentStateEnum == ECState.Avoid)
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.EnemyMainObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Landmine,0.0);
						helper.CanAddLandmine = false;
					}
				}
			}
			
			// Pause commanding enemy mini cells to Attack state
			// Pause duration depends only on the number of enemy mini cell commanded
			//helper.StartPauseAddLandmine ((float)nCommandNum * 2f);
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
		Debug.Log ("Exit EMLandmineState");

		transition = m_EMFSM.emTransition;

		// Reset animation status
		if (EMAnimation.Instance ().CanBlink)
			EMAnimation.Instance ().CanBlink = false;
		// Reset availability to command mini cell to Landmine state
		if (!helper.CanAddLandmine)
			helper.CanAddLandmine = true;
		// Reset transition availability
		transition.CanTransit = true;
	}
}