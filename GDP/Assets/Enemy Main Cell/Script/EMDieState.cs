using UnityEngine;
using System.Collections;

public class EMDieState : IEMState
{
	public static EMDieState instance;
	
	public GameObject FSM;
	
	void Awake ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		m_PCFSM = FSM.GetComponent<PlayerChildFSM> ();
	}

	// Singleton
	public static EMDieState Instance()
	{
		if (instance == null)
			instance = new EMDieState();
		
		return instance;
	}

	public override void Enter ()
	{
		Destroy (this.gameObject);
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}