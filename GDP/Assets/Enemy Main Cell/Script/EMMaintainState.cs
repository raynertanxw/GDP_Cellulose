using UnityEngine;
using System.Collections;

public class EMMaintainState : IEMState
{
	public static EMMaintainState instance;

	public EMMaintainState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	// Singleton
	public static EMMaintainState Instance()
	{
		if (instance == null)
			instance = new EMMaintainState();
		
		return instance;
	}

	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}