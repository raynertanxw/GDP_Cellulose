using UnityEngine;
using System.Collections;

public class EMCautiousAttackState : EnemyMainState 
{
	public static EMCautiousAttackState instance;
	
	public EMCautiousAttackState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}
	
	// Singleton
	public static EMCautiousAttackState Instance()
	{
		if (instance == null)
			instance = new EMCautiousAttackState();
		
		return instance;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMProductionState.Instance ());
	}
}