using UnityEngine;
using System.Collections;

public class EMLandmineState : EnemyMainState
{
	public static EMLandmineState instance;
	
	public EMLandmineState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	// Singleton
	public static EMLandmineState Instance()
	{
		if (instance == null)
			instance = new EMLandmineState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}