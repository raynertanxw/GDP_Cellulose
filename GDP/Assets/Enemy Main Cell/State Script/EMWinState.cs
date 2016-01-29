using UnityEngine;
using System.Collections;

public class EMWinState : IEMState
{
	private EMTransition transition;
	private EMController controller;
	private EMHelper helper;
	
	public EMWinState (EnemyMainFSM EMFSM)
	{
		m_EMFSM = EMFSM;
	}
	
	public override void Enter ()
	{
		Debug.Log ("Enter EMWinState");
		
		transition = m_EMFSM.emTransition;
		
		// Reset transition availability
		transition.CanTransit = false;
	}
	
	public override void Execute ()
	{

	}
	
	public override void Exit ()
	{
		
	}
}