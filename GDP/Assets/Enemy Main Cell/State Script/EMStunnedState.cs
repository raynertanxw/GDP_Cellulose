using UnityEngine;
using System.Collections;

public class EMStunnedState : IEMState
{
	public static EMStunnedState instance;
	
	public GameObject FSM;
	private EMController controller;
	
	void Awake ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
		controller = GetComponent<EMController> ();
	}

	// Singleton
	public static EMStunnedState Instance()
	{
		if (instance == null)
			instance = new EMStunnedState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		// If the enemy main cell is not stunned any more, transit to Maintain State
		if (!controller.Stunned) 
		{
			m_EMFSM.ChangeState (EMDefendState.Instance ());
		}
	}
}