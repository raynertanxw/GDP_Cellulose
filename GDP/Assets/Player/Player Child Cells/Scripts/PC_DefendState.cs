﻿using UnityEngine;
using System.Collections;

public class PC_DefendState : IPCState
{
	public override void Enter()
	{
		Debug.Log("Enter Defend State");
	}
	
	public override void Execute()
	{
		
	}
	
	public override void Exit()
	{
		
	}
	
	// Constructor.
	public PC_DefendState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}









	private static float s_fDetectionRange = 1.0f;

	#region Helper functions
	private bool isTargetAlive()
	{
		return false; // PLACEHOLDER
	}

	private void MoveTowardsTarget()
	{

	}

	private bool FindNewTarget()
	{
		return false; // PLACEHOLDER
	}
	#endregion
}
