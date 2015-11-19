using UnityEngine;
using System.Collections;

public class EMDieState : EnemyMainState
{
	public static EMDieState instance;
	
	public EMDieState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	// Singleton
	public static EMDieState Instance()
	{
		if (instance == null)
			instance = new EMDieState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}