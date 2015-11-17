using UnityEngine;
using System.Collections;

public class EMStunnedState : EnemyMainState
{
	public static EMStunnedState instance;
	
	void Start()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
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