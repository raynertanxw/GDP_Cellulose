using UnityEngine;
using System.Collections;

public class EMDefendState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMDefendState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{
		Debug.Log ("Enter EMDefendState");

		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		// Reset availability to command mini cell to Defend state
		if (!helper.CanAddDefend)
			helper.CanAddDefend = true;
		// Set status to defending
		controller.bIsDefend = true;

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

		#region Command child cells to transit to Defend state only when there are more player mini cells than enemy mini cells
		if (m_EMFSM.AvailableChildNum < PlayerChildFSM.GetActiveChildCount() && helper.CanAddDefend)
		{
			float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f + 1f;
			float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f + 1f;

			// If there are more than 10 and less than 25 available enemy mini cells
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25 && helper.CanAddDefend)
			{
				for (int nAmount = 0; nAmount < Random.Range (5, 11); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECDefendState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.EnemyMainObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Defend,0.0);
						helper.CanAddDefend = false;
					}
				}

				if (helper.CanAddDefend)
					m_EMFSM.StartPauseAddDefend (0.25f);
			}
			else if (m_EMFSM.AvailableChildNum > 25 && m_EMFSM.AvailableChildNum <= 50 && helper.CanAddDefend)
			{
				for (int nAmount = 0; nAmount < Random.Range (3, 9); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECDefendState(m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(m_EMFSM.EnemyMainObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Defend,0.0);
						helper.CanAddDefend = false;
					}
				}
			}

			// Pause commanding enemy mini cells to defend state
			m_EMFSM.StartPauseTransition (2f);
		}
		#endregion
		
		//Start checking transition only when transition is allowed
		if (transition.CanTransit) 
		{
			float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f;
			float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f;

			// Transit to other states only when there are more player mini cells than enemy mini cells
			if (nEnemyChildFactor >= nPlayerChildFactor && helper.CanAddDefend)
			{
				// Transit to Production only if has less than 25 mini cells and has nutrient
				if (m_EMFSM.AvailableChildNum < 25 && controller.NutrientNum > 0)
				{
					transition.Transition (10f - ((float)m_EMFSM.AvailableChildNum - 10f)/2f, EMState.Production);
				}

				// If not transit to Production state, then transit to Maintain state
				m_EMFSM.ChangeState (EMState.Maintain);
			}
		}

		// Check transition every 0.2 second to save computing power
		if (transition.CanTransit)
			m_EMFSM.StartPauseTransition (.2f);
	}

	public override void Exit ()
	{
		Debug.Log ("Exit EMDefendState");

		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		// Reset availability to command mini cell to Defend state
		if (!helper.CanAddDefend)
			helper.CanAddDefend = true;
		// Set status to not defending
		controller.bIsDefend = false;
		// Reset transition availability
		transition.CanTransit = true;
	}
}