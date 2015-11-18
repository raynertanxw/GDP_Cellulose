using UnityEngine;
using System.Collections;

public class EMProductionState :IEMState
{
	public static EMProductionState instance;

	void Start()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
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
		m_EMFSM.ChangeState (EMMaintainState.Instance ());
	}
}