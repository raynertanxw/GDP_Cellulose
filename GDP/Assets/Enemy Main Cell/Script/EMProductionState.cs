using UnityEngine;
using System.Collections;

public class EMProductionState :IEMState
{
	public static EMProductionState instance;

	public EMProductionState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
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