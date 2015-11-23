using UnityEngine;
using System.Collections;

public class PC_ChargeChildState : IPCState
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
	public PC_ChargeChildState(PlayerChildFSM pcFSM)
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
