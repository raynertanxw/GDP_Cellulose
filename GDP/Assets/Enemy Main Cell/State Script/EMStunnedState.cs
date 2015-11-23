using UnityEngine;
using System.Collections;

public class EMStunnedState : IEMState
{
	public static EMStunnedState instance;
	
	public GameObject FSM;
	private EMController emController;
	
	void Awake ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
		emController = GetComponent<EMController> ();
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
		if (!emController.bStunned) 
		{
			m_EMFSM.ChangeState (EMMaintainState.Instance ());
		}
	}
}