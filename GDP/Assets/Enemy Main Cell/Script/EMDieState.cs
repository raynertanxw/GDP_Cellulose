using UnityEngine;
using System.Collections;

public class EMDieState : EnemyMainState
{
	public static EMDieState instance;
	
	void Start()
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
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}