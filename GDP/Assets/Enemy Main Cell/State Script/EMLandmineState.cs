using UnityEngine;
using System.Collections;

public class EMLandmineState : IEMState
{
	public EMLandmineState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}
	
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;
	
	void GetData ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;
	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (m_EMFSM.ProductionState);
	}
}