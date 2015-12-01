using UnityEngine;
using System.Collections;

public class PC_IdleState : IPCState
{
	private Vector3 m_nodeOrigin;
	private float m_fMaxDisplacement = 0.6f;
	private float m_fTargetReachRadius = 0.1f;
	private bool m_bReachedTarget = true;
	private Vector3 m_currentTarget;

	private Vector3 m_currentVelocity;
	private static float s_fPlayerChildIdleSpeed = 0.1f;
	private static float s_fDetectionRangeRadius = 0.6f;

	public override void Enter()
	{
		m_nodeOrigin = m_pcFSM.m_assignedNode.position;
		m_currentVelocity = Vector3.zero;

		m_pcFSM.m_assignedSquad.AddChildToSquadList(m_pcFSM);
	}
	
	public override void Execute()
	{
		MoveAroundNode();
		if (DetectedEnemyInRange() == true)
		{
			if (m_pcFSM.m_bIsDefending == true)
			{
				// Switch to defending mode.
				m_pcFSM.ChangeState(PCState.Defend);
			}
			else
			{
				// Switch to avoid mode.
				m_pcFSM.ChangeState(PCState.Avoid);
			}
		}
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
	private bool DetectedEnemyInRange()
	{
		Collider2D enemyCell = Physics2D.OverlapCircle(m_pcFSM.transform.position, s_fDetectionRangeRadius, Constants.s_onlyEnemeyChildLayer);
		if (enemyCell != null)
		{
			// Assign the currentEnemyCellTarget in the FSM to the returned enemy cell.
			m_pcFSM.m_currentEnemyCellTarget = enemyCell.transform;

			return true;
		}
		else
		{
			return false;
		}
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
			float scalar = Mathf.Pow(s_fPlayerChildIdleSpeed, 2) / direction.sqrMagnitude;
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
		if (sqrMag > Mathf.Pow(s_fPlayerChildIdleSpeed, 2))
		{
			float scalar = Mathf.Pow(s_fPlayerChildIdleSpeed, 2) / sqrMag;
			m_currentVelocity *= scalar;
		}
	}
	#endregion
}
