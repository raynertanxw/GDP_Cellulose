using UnityEngine;
using System.Collections;

public class EMMaintainState : IEMState
{
	public static EMMaintainState instance;

	public GameObject FSM;
	
	void Awake ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
	}

	// Singleton
	public static EMMaintainState Instance()
	{
		if (instance == null)
			instance = new EMMaintainState();
		
		return instance;
	}

	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}