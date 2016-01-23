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
		Debug.Log ("Enter EMDieState");

		transition = m_EMFSM.emTransition;
		
		// Reset transition availability
		transition.CanTransit = false;
	}
	
	public override void Execute ()
	{
		//m_EMFSM.ChangeState (EMState.Production);
	}

	public override void Exit ()
	{

	}
}