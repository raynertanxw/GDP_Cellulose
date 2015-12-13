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
		
		// Reset transition availability
		transition.CanTransit = true;
		// Pause the transition for 1 second
		m_EMFSM.StartPauseTransition (1f);
	}

	public override void Execute ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		int nCommandNum = 0;
		
		#region Landmine Behaviour
		if (m_EMFSM.AvailableChildNum > PlayerChildFSM.GetActiveChildCount () && helper.CanAddLandmine) 
		{
			float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f + 1f;
			float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f + 1f;
			
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25)
			{
				nCommandNum = Random.Range (1, 2 + (int)nEnemyChildFactor);
				for (int nAmount = 0; nAmount < nCommandNum; nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECMineState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.EnemyMainObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Landmine,0.0);
						helper.CanAddLandmine = false;
					}
				}
			}
			else if (m_EMFSM.AvailableChildNum > 25 && m_EMFSM.AvailableChildNum <= 50)
			{
				nCommandNum = Random.Range (2, 3 + (int)nEnemyChildFactor);
				for (int nAmount = 0; nAmount < nCommandNum; nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECMineState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.EnemyMainObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Landmine,0.0);
						helper.CanAddLandmine = false;
					}
				}
			}
			else if (m_EMFSM.AvailableChildNum > 50)
			{
				nCommandNum = Random.Range (3, 4 + (int)nEnemyChildFactor);
				for (int nAmount = 0; nAmount < nCommandNum; nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECMineState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.EnemyMainObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Landmine,0.0);
						helper.CanAddLandmine = false;
					}
				}
			}
			
			// Pause commanding enemy mini cells to Attack state
			// Pause duration depends only on the number of enemy mini cell commanded
			m_EMFSM.StartPauseAddLandmine ((float)nCommandNum);
		}
		#endregion
	}

	public override void Exit ()
	{
		Debug.Log ("Exit EMLandmineState");

		transition = m_EMFSM.emTransition;
		
		// Reset transition availability
		transition.CanTransit = true;
	}
}