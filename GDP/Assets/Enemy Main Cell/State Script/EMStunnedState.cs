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
		Debug.Log ("Enter EMStunnedState");

		transition = m_EMFSM.emTransition;

		// Reset transition availability
		transition.CanTransit = false;
		
		AudioManager.PlayEMSoundEffect(EnemyMainSFX.Stunned);
	}

	public override void Execute ()
	{
		transition = m_EMFSM.emTransition;
		controller = m_EMFSM.emController;
		helper = m_EMFSM.emHelper;

		// If the enemy main cell is not stunned any more, transit to Production State
		if (!controller.Stunned) 
		{
			m_EMFSM.ChangeState (EMState.Production);
		}
	}

	public override void Exit ()
	{
		Debug.Log ("Exit EMStunnedState");

		transition = m_EMFSM.emTransition;

		// Reset transition availability
		transition.CanTransit = true;
	}
}