using UnityEngine;
using System.Collections;

public class EMDefendState : IEMState
{
	public static EMDefendState instance;
	
	public GameObject FSM;
	
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;
	
	void Awake ()
	{
		transition = GetComponent<EMTransition> ();
		controller = GetComponent<EMController> ();
		helper = GetComponent<EMHelper> ();
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
	}

	// Singleton
	public static EMDefendState Instance()
	{
		if (instance == null)
			instance = new EMDefendState();
		
		return instance;
	}

	public override void Enter ()
	{
		// Reset availability to command mini cell to Defend state
		if (!helper.bCanAddDefend)
			helper.bCanAddDefend = true;
		// Set status to defending
		controller.bIsDefend = true;

		// Pause the 
		StartCoroutine (transition.TransitionAvailability (1f));
	}
	
	public override void Execute ()
	{
		float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f;
		float nPlayerChildFactor = (float)(PlayerChildFSM.GetActiveChildCount ()) / 10f;

		// Command child cells to transit to Defend state only when there are more player mini cells than enemy mini cells
		if (nEnemyChildFactor < nPlayerChildFactor && helper.bCanAddDefend)
		{
			// If there are more than 10 and less than 25 available enemy mini cells
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25 && helper.bCanAddDefend)
			{
				for (int nAmount = 0; nAmount < Random.Range (3, 6); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECDefendState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(this.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Defend,0.0);
						helper.bCanAddDefend = false;
					}
				}

				StartCoroutine (helper.PauseAddDefend (0.25f));
			}
			else if (m_EMFSM.AvailableChildNum > 25 && m_EMFSM.AvailableChildNum <= 50 && helper.bCanAddDefend)
			{
				for (int nAmount = 0; nAmount < Random.Range (1, 4); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState != new ECDefendState(m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
					{
						MessageDispatcher.Instance.DispatchMessage(this.gameObject,m_EMFSM.ECList[nIndex].gameObject,MessageType.Defend,0.0);
						helper.bCanAddDefend = false;
					}
				}
			}

			StartCoroutine (helper.PauseAddDefend (0.5f));
		}
		
		//Start checking transition only when transition is allowed
		if (transition.bCanTransit) 
		{
			// Transit to other states only when there are more player mini cells than enemy mini cells
			if (nEnemyChildFactor >= nPlayerChildFactor && helper.bCanAddDefend)
			{
				// Transit to Production only if has less than 25 mini cells and has nutrient
				if (m_EMFSM.AvailableChildNum < 25 && controller.NutrientNum > 0)
				{
					transition.Transition (10f - ((float)m_EMFSM.AvailableChildNum - 10f)/2f, EMDefendState.Instance ());
				}

				// If not transit to Production state, then transit to Maintain state
				m_EMFSM.ChangeState (EMMaintainState.Instance ());
			}
		}

		// Check transition every 0.1 second to save computing power
		if (transition.bCanTransit)
			StartCoroutine (transition.TransitionAvailability (.1f));
	}

	public override void Exit ()
	{
		// Reset availability to command mini cell to Defend state
		if (!helper.bCanAddDefend)
			helper.bCanAddDefend = true;
		// Set status to not defending
		controller.bIsDefend = false;
		// Reset transition availability
		transition.bCanTransit = true;
	}
}