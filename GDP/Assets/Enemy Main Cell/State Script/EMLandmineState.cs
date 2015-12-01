using UnityEngine;
using System.Collections;

public class EMLandmineState : IEMState
{
	public static EMLandmineState instance;

	void Awake ()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
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