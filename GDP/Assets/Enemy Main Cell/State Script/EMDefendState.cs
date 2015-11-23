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
		float nEnemyChildFactor = controller.nSize / 10;
		float nPlayerChildFactor = 1f;


		if (nEnemyChildFactor < nPlayerChildFactor)
		{
			// If there are more than 10  and less than 25 available enemy mini cells
			if (m_EMFSM.nAvailableChildNum > 10 && m_EMFSM.nAvailableChildNum <= 25 && helper.bCanAddDefend)
			{
				/*
				for (int nAmount = 0; nAmount < Random.Range (3, 6); nAmount++)
				{
					do {
							int nIndex = Random.Range (0, m_EMFSM.ecList.Count);
							//if (child cell not defending)
							{
								// make it defend
								EMHelper.bCanAddDefend = false;
							}
						} while (EMHelper.bCanAddDefend)
				}
				*/
				StartCoroutine (helper.PauseAddDefend (0.5f));
			}
			else if (m_EMFSM.nAvailableChildNum > 25 && m_EMFSM.nAvailableChildNum <= 50 && helper.bCanAddDefend)
			{
				/*
				for (int nAmount = 0; nAmount < Random.Range (1, 4); nAmount++)
				{
					do {
							int nIndex = Random.Range (0, m_EMFSM.ecList.Count);
							//if (child cell not defending)
							{
								// make it defend
								EMHelper.bCanAddDefend = false;
							}
						} while (EMHelper.bCanAddDefend)
				}
				*/					
				StartCoroutine (helper.PauseAddDefend (1f));
			}
		}
		
		//Start checking transition only when transition is allowed
		if (transition.bCanTransit) 
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