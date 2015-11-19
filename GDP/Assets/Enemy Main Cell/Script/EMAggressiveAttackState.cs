using UnityEngine;
using System.Collections;

public class EMAggressiveAttackState : EnemyMainState
{
	public static EMAggressiveAttackState instance;
	
	public EMAggressiveAttackState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
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