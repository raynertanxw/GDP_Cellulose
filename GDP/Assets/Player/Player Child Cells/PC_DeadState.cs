using UnityEngine;
using System.Collections;

public class PC_DeadState : IPCState
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
	public PC_DeadState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}
}
