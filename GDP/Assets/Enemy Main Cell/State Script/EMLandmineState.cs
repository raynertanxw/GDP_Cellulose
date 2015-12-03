using UnityEngine;
using System.Collections;

public class EMLandmineState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMLandmineState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{
		Debug.Log ("Enter EMLandmineState");

		transition = m_EMFSM.emTransition;
		
		// Reset transition availability
		transition.bCanTransit = true;
	}

	public override void Execute ()
	{
		m_EMFSM.ChangeState (EMState.Production);
	}

	public override void Exit ()
	{
		Debug.Log ("Exit EMLandmineState");

		transition = m_EMFSM.emTransition;
		
		// Reset transition availability
		transition.bCanTransit = true;
	}
}