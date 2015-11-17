using UnityEngine;
using System.Collections;

public class EMMaintainState : EnemyMainState
{
	public static EMMaintainState instance;

	void Start()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
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