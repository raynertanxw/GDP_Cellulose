using UnityEngine;
using System.Collections;

public class PC_IdleState : IPCState
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
	public PC_IdleState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}











	#region Helper functions
	private bool DetectEnemyInRange()
	{
		return false; // PLACEHOLDER
	}

	private void MoveAroundNode()
	{

	}
	#endregion
}
