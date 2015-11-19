using UnityEngine;
using System.Collections;

public class EMProductionState : IEMState
{
	public static EMProductionState instance;

	public GameObject FSM;
	
	void Awake ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
	}

	// Singleton
	public static EMProductionState Instance()
	{
		if (instance == null)
			instance = new EMProductionState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		StartCoroutine (m_EMFSM.ProduceChild ());
	}
}