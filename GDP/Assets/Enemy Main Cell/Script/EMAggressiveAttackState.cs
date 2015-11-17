using UnityEngine;
using System.Collections;

public class EMAggressiveAttackState : EnemyMainState
{
	public static EMAggressiveAttackState instance;
	
	void Start()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
	}
	
	// Singleton
	public static EMAggressiveAttackState Instance()
	{
		if (instance == null)
			instance = new EMAggressiveAttackState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}