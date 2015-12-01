using UnityEngine;
using System.Collections;

public class EMStunnedState : IEMState
{
	public EMStunnedState (EnemyMainFSM EMFSM)
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

	public override void Enter ()
	{
		// Reset transition availability
		transition.bCanTransit = true;
	}

	public override void Execute ()
	{
		// If the enemy main cell is not stunned any more, transit to Maintain State
		if (!controller.Stunned) 
		{
			m_EMFSM.ChangeState (m_EMFSM.MaintainState);
		}
	}

	public override void Exit ()
	{
		// Reset transition availability
		transition.bCanTransit = true;
	}
}