using UnityEngine;
using System.Collections;

public class EMStunnedState : IEMState
{
	public static EMStunnedState instance;

	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;
	
	void Awake ()
	{
		transition = GetComponent<EMTransition> ();
		controller = GetComponent<EMController> ();
		helper = GetComponent<EMHelper> ();
		m_EMFSM = GetComponent<EnemyMainFSM> ();
	}

	// Singleton
	public static EMStunnedState Instance()
	{
		if (instance == null)
			instance = new EMStunnedState();
		
		return instance;
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
			m_EMFSM.ChangeState (EMDefendState.Instance ());
		}
	}

	public override void Exit ()
	{
		// Reset transition availability
		transition.bCanTransit = true;
	}
}