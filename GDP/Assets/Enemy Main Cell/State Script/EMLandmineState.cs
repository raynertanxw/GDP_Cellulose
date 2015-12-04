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
		transition.CanTransit = true;
		// Pause the transition for 1 second
		m_EMFSM.StartPauseTransition (1f);
	}

	public override void Execute ()
	{
		if (transition.CanTransit)
			m_EMFSM.ChangeState (EMState.Production);
	}

	public override void Exit ()
	{
		Debug.Log ("Exit EMLandmineState");

		transition = m_EMFSM.emTransition;
		
		// Reset transition availability
		transition.CanTransit = true;
	}
}