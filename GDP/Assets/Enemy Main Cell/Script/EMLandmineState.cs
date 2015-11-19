using UnityEngine;
using System.Collections;

public class EMLandmineState : IEMState
{
	public static EMLandmineState instance;
	
	public GameObject FSM;
	
	void Awake ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
	}

	// Singleton
	public static EMLandmineState Instance()
	{
		if (instance == null)
			instance = new EMLandmineState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}