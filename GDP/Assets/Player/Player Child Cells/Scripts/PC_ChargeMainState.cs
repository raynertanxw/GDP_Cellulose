using UnityEngine;
using System.Collections;

public class PC_ChargeMainState : IPCState
{
	public override void Enter()
	{
		Debug.Log("Entering ChargeMain State");
	}
	
	public override void Execute()
	{
		// Check for deferred state change.
		if (m_pcFSM.m_bHasAwaitingDeferredStateChange == true)
		{
			m_pcFSM.ExecuteDeferredStateChange();
		}
	}
	
	public override void Exit()
	{
		Debug.Log("Exiting ChargeMain State");
	}
	
	// Constructor.
	public PC_ChargeMainState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}
	
	
	
	
	
	
	
	
	#region Helper functions
	private bool IsTargetAlive()
	{
		return false; // PLACEHOLDER
	}

	private void MoveTowardsTarget()
	{

	}
	#endregion
}
