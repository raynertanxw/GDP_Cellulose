using UnityEngine;
using System.Collections;

public class PC_DefendState : IPCState
{
	private Vector3 m_currentVelocity;
	private static float s_fPlayerChildDefendSpeed = 5.0f;
	private static float s_fDetectionRange = 1.0f;

	public override void Enter()
	{

	}
	
	public override void Execute()
	{
		if (m_pcFSM.m_currentEnemyCellTarget == null)
		{
			// Find targets if any, otherwise switch back to idle.
			if (FindNewTarget() == false)
				m_pcFSM.ChangeState(PCState.Idle);
		}
		else if (IsTargetAlive() == false)
		{
			// Find targets if any, otherwise switch back to idle.
			if (FindNewTarget() == false)
				m_pcFSM.ChangeState(PCState.Idle);
		}
		else // Target is still alive.
		{
			MoveTowardsTarget();
		}



		// Check for deferred state change.
		if (m_pcFSM.m_bHasAwaitingDeferredStateChange == true)
		{
			m_pcFSM.ExecuteDeferredStateChange();
		}
	}
	
	public override void Exit()
	{

	}

    public override void FixedExecute()
    {
        
    }

    // Constructor.
    public PC_DefendState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}










	#region Helper functions
	private bool IsTargetAlive()
	{
		if (m_pcFSM.m_currentEnemyCellTarget.CurrentStateEnum == ECState.Dead)
			return false;
		else
			return true;
	}

	private void MoveTowardsTarget()
	{
		// Calculate "force" vector.
		Vector3 direction = m_pcFSM.m_currentEnemyCellTarget.transform.position - m_pcFSM.transform.position;
		m_currentVelocity += direction.normalized * s_fPlayerChildDefendSpeed;
		CapSpeed();
		
		// Apply velocity vector.
		m_pcFSM.transform.position += m_currentVelocity * Time.deltaTime;
	}

	private void CapSpeed()
	{
		float sqrMag = m_currentVelocity.sqrMagnitude;
		if (sqrMag > Mathf.Pow(s_fPlayerChildDefendSpeed, 2))
		{
			float scalar = Mathf.Pow(s_fPlayerChildDefendSpeed, 2) / sqrMag;
			m_currentVelocity *= scalar;
		}
	}

	private bool FindNewTarget()
	{
		Collider2D enemyCell = Physics2D.OverlapCircle(m_pcFSM.transform.position, s_fDetectionRange, Constants.s_onlyEnemeyChildLayer);
		if (enemyCell != null)
		{
			// Assign the currentEnemyCellTarget in the FSM to the returned enemy cell.
			m_pcFSM.m_currentEnemyCellTarget = enemyCell.gameObject.GetComponent<EnemyChildFSM>();
			return true;
		}
		else
		{
			return false;
		}
	}

	#endregion
}
