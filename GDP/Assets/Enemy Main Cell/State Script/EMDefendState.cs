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
		if (!helper.bCanAddDefend)
			helper.bCanAddDefend = true;


	}
	
	public override void Execute ()
	{
		float nEnemyChildFactor = (float)m_EMFSM.AvailableChildNum / 10f;
		float nPlayerChildFactor = (float)(PlayerChildFSM.GetActiveChildCount ()) / 10f;

		// Command child cells to transit to Defend state only when 
		if (nEnemyChildFactor < nPlayerChildFactor && helper.bCanAddDefend)
		{
			// If there are more than 10 and less than 25 available enemy mini cells
			if (m_EMFSM.AvailableChildNum > 10 && m_EMFSM.AvailableChildNum <= 25 && helper.bCanAddDefend)
			{
				for (int nAmount = 0; nAmount < Random.Range (3, 6); nAmount++)
				{
					int nIndex = Random.Range (0, m_EMFSM.ECList.Count);
					if (m_EMFSM.ECList[nIndex].CurrentState == new ECDefendState (m_EMFSM.ECList[nIndex].gameObject, m_EMFSM.ECList[nIndex]))
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

				StartCoroutine (helper.PauseAddDefend (0.5f));
			}
		}
		
		//Start checking transition only when transition is allowed
		if (transition.CanTransit) 
		{
			
		}

		// Check transition every 0.1 second to save computing power
		StartCoroutine (transition.TransitionAvailability (.1f));
	}

	public override void Exit ()
	{
		if (!helper.bCanAddDefend)
			helper.bCanAddDefend = true;
	}
}