using UnityEngine;
using System.Collections;

public class PC_AvoidState : IPCState
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
	public PC_AvoidState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}









	private float m_fAvoidRange;

	#region Helper functions
	private bool findClosestEnemy()
	{
		return false; // PLACEHOLDER
	}

	private void MoveAwayFromEnemy()
	{

	}
	#endregion
}
