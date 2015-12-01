using UnityEngine;
using System.Collections;

public class EMAggressiveAttackState : IEMState
{
	public EMAggressiveAttackState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}
	
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;
	
	void GetData ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;
		m_EMFSM = GetComponent<EnemyMainFSM> ();
	}

	public override void Enter ()
	{
		// Reset availability to command mini cell to Attack state
		if (!helper.CanAddAttack)
			helper.CanAddAttack = true;
		
		// Reset transition availability
		transition.bCanTransit = true;
		// Pause the transition for 1 second
		StartCoroutine (transition.TransitionAvailability (1f));
	}

	public override void Execute ()
	{
		#region Attack only when there are more enemy mini cells than player's 
		if (m_EMFSM.AvailableChildNum > PlayerChildFSM.GetActiveChildCount ()) 
		{
			float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f;
			float nPlayerChildFactor = (float)PlayerChildFSM.GetActiveChildCount () / 10f;
			
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25)
			{
				for (int nAmount = 0; nAmount < Random.Range (1, 3 + (int)nEnemyChildFactor); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECAttackState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(this.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Attack,0.0);
						helper.CanAddAttack = false;
					}
				}
			}
			else if (m_EMFSM.AvailableChildNum > 25 && m_EMFSM.AvailableChildNum <= 50)
			{
				for (int nAmount = 0; nAmount < Random.Range (2, 4 + (int)nEnemyChildFactor); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECAttackState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(this.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Attack,0.0);
						helper.CanAddAttack = false;
					}
				}
			}
			else if (m_EMFSM.AvailableChildNum > 50)
			{
				for (int nAmount = 0; nAmount < Random.Range (4, 6 + (int)nEnemyChildFactor); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECAttackState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(this.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Attack,0.0);
						helper.CanAddAttack = false;
					}
				}
			}
			
			// Pause commanding enemy mini cells to Attack state
			// Pause duration depends only on the number of enemy mini cells
			StartCoroutine (helper.PauseAddAttack (1.5f - nEnemyChildFactor/10f));
		}
		#endregion
		
		// Check transition every 0.1 second to save computing power
		if (transition.bCanTransit)
			StartCoroutine (transition.TransitionAvailability (.1f));
	}

	public override void Exit ()
	{
		// Reset availability to command mini cell to Attack state
		if (!helper.CanAddAttack)
			helper.CanAddAttack = true;
		// Reset transition availability
		transition.bCanTransit = true;
	}
}