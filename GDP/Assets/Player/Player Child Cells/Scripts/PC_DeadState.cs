using UnityEngine;
using System.Collections;

public class PC_DeadState : IPCState
{
	public override void Enter()
	{
		// Hide the object and reset all variables.
		m_pcFSM.rigidbody2D.isKinematic = true;
		m_pcFSM.collider2D.enabled = false;
		m_pcFSM.spriteRen.enabled = false;
	}

	public override void Execute()
	{

	}

	public override void Exit()
	{
		// Unhide the object, enabling the sprite and colliders, etc.
		m_pcFSM.rigidbody2D.isKinematic = false;
		m_pcFSM.collider2D.enabled = true;
		m_pcFSM.spriteRen.enabled = true;
	}

	// Constructor.
	public PC_DeadState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}
}
