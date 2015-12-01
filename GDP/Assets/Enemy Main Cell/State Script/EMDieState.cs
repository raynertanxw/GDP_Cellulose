using UnityEngine;
using System.Collections;

public class EMDieState : IEMState
{
	public static EMDieState instance;

	void Awake ()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
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