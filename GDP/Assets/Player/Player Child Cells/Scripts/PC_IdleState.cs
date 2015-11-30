﻿using UnityEngine;
using System.Collections;

public class PC_IdleState : IPCState
{
	private Vector3 m_nodeOrigin;
	private float m_fMaxDisplacement = 0.6f;
	private float m_fTargetReachRadius = 0.1f;
	private bool m_bReachedTarget = true;
	private Vector3 m_currentTarget;

	private Vector3 m_currentVelocity;

	public override void Enter()
	{
		m_nodeOrigin = m_pcFSM.m_assignedNode.position;
		m_currentVelocity = Vector3.zero;
	}
	
	public override void Execute()
	{
		MoveAroundNode();
	}
	
	public override void Exit()
	{
		m_bReachedTarget = true;
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
		if (m_bReachedTarget == true)
		{
			SetNewWanderTarget();
		}
		else
		{
			// Calculate "force" vector.
			Vector3 direction = m_currentTarget - m_pcFSM.transform.position;
			float scalar = Mathf.Pow(m_pcFSM.GetSpeed(), 2) / direction.sqrMagnitude;
			m_currentVelocity += direction * scalar;
			CapSpeed();
		}

		// Apply velocity vector.
		m_pcFSM.transform.position += m_currentVelocity * Time.deltaTime;

		// If reached target.
		if ((m_pcFSM.transform.position - m_currentTarget).sqrMagnitude < Mathf.Pow(m_fTargetReachRadius, 2))
		{
			m_bReachedTarget = true;
		}
	}

	private void SetNewWanderTarget()
	{
		m_currentTarget = (Vector3)(Random.insideUnitCircle * m_fMaxDisplacement) + m_nodeOrigin;
		m_bReachedTarget = false;
	}

	private void CapSpeed()
	{
		float sqrMag = m_currentVelocity.sqrMagnitude;
		if (sqrMag > Mathf.Pow(m_pcFSM.GetSpeed(), 2))
		{
			float scalar = Mathf.Pow(m_pcFSM.GetSpeed(), 2) / sqrMag;
			m_currentVelocity *= scalar;
		}
	}
	#endregion
}
