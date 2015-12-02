using UnityEngine;
using System.Collections;

public class EMDieState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMDieState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{

	}
	
	public override void Execute ()
	{
		m_EMFSM.ChangeState (m_EMFSM.ProductionState);
	}
}