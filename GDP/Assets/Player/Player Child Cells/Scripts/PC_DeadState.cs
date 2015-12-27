using UnityEngine;
using System.Collections;

public class PC_DeadState : IPCState
{
	private bool m_bIsCalledFromPool = false;
	private Vector3 m_spawnPoint;

	public override void Enter()
	{
		// Hide the object and reset all variables.
		m_pcFSM.rigidbody2D.isKinematic = true;
		m_pcFSM.collider2D.enabled = false;
		m_pcFSM.spriteRen.enabled = false;

		// Teleport far away.
		m_pcFSM.transform.position = Constants.s_farfarAwayVector;

		// Update active Child Count.
		PlayerChildFSM.SetActiveChildCount(false);

		// Remove from NodeList if any assignedNode.
		if (m_pcFSM.m_assignedNode != null)
		{
			m_pcFSM.m_assignedNode.RemoveChildFromNodeList(m_pcFSM);
			m_pcFSM.m_assignedNode = null;
		}
	}

	public override void Execute()
	{
		if (m_bIsCalledFromPool == true)
		{
			m_pcFSM.transform.position = m_spawnPoint;
			m_pcFSM.ChangeState(PCState.Idle);
		}
	}

	public override void Exit()
	{
		// Unhide the object, enabling the sprite and colliders, etc.
		m_pcFSM.rigidbody2D.isKinematic = false;
		m_pcFSM.collider2D.enabled = true;
		m_pcFSM.spriteRen.enabled = true;

		// Reset State.
		m_bIsCalledFromPool = false;

		// Update active Child Count.
		PlayerChildFSM.SetActiveChildCount(true);
	}

    public override void FixedExecute()
    {

    }

    // Constructor.
    public PC_DeadState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}




	#region Helper functions
	public void CallFromPool(Vector3 spawnPoint)
	{
		m_spawnPoint = spawnPoint;
		m_bIsCalledFromPool = true;
	}
	#endregion
}
