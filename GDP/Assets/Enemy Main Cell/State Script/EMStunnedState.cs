using UnityEngine;
using System.Collections;

public class EMStunnedState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;

	public EMStunnedState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}

	public override void Enter ()
	{
		transition = m_EMFSM.emTransition;

		// Reset transition availability
		transition.bCanTransit = true;
	}

	public override void Execute ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		// If the enemy main cell is not stunned any more, transit to Maintain State
		if (!controller.Stunned) 
		{
			m_EMFSM.ChangeState (m_EMFSM.MaintainState);
		}
	}

	public override void Exit ()
	{
		transition = m_EMFSM.emTransition;

		// Reset transition availability
		transition.bCanTransit = true;
	}
}