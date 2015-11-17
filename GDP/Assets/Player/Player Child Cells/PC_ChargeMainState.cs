using UnityEngine;
using System.Collections;

public class PC_ChargeMainState : IPCState
{
	public override void Enter()
	{
		
	}
	
	public override void Execute()
	{
		
	}
	
	public override void Exit()
	{
		
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
