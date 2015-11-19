using UnityEngine;
using System.Collections;

public class EMStunnedState : EnemyMainState
{
	public static EMStunnedState instance;
	
	public EMStunnedState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	// Singleton
	public static EMStunnedState Instance()
	{
		if (instance == null)
			instance = new EMStunnedState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}