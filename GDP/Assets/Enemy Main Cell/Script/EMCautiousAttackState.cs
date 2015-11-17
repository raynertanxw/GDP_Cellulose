using UnityEngine;
using System.Collections;

public class EMCautiousAttackState : EnemyMainState 
{
	public static EMCautiousAttackState instance;
	
	void Start()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
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