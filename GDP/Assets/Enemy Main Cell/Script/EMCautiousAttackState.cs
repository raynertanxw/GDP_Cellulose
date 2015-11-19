using UnityEngine;
using System.Collections;

public class EMCautiousAttackState : IEMState
{
	public static EMCautiousAttackState instance;
	
	public GameObject FSM;
	
	void Awake ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
	}
	
	// Singleton
	public static EMCautiousAttackState Instance()
	{
		if (instance == null)
			instance = new EMCautiousAttackState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}