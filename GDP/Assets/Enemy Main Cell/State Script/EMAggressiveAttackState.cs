using UnityEngine;
using System.Collections;

public class EMAggressiveAttackState : IEMState
{
	public static EMAggressiveAttackState instance;

	public GameObject FSM;

	void Awake ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
	}

	// Singleton
	public static EMAggressiveAttackState Instance()
	{
		if (instance == null)
			instance = new EMAggressiveAttackState();
		
		return instance;
	}

	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}