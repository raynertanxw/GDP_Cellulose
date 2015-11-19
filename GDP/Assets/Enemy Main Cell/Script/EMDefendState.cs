using UnityEngine;
using System.Collections;

public class EMDefendState : EnemyMainState
{
	public static EMDefendState instance;
	
	public EMDefendState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	// Singleton
	public static EMDefendState Instance()
	{
		if (instance == null)
			instance = new EMDefendState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}