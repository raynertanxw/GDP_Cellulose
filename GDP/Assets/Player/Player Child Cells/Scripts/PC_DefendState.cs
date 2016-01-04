using UnityEngine;
using System.Collections;

public class PC_DefendState : IPCState
{
	private Vector3 m_currentVelocity;
	private static float s_fPlayerChildDefendSpeed = 4.0f;
	private static float s_fNearDetectionRange = 1.0f;

	public override void Enter()
	{
		FindNewTarget();
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

    #if UNITY_EDITOR
    public override void ExecuteOnDrawGizmos()
    {
        
    }
    #endif

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
		if (IsTargetWithinDangerRange() == false)
		{
			m_pcFSM.m_currentEnemyCellTarget = null;
			return;
		}

		// Calculate "force" vector.
		Vector3 direction = m_pcFSM.m_currentEnemyCellTarget.transform.position - m_pcFSM.transform.position;
		m_currentVelocity += direction.normalized * s_fPlayerChildDefendSpeed;
		CapSpeed();
		
		// Apply velocity vector.
		m_pcFSM.transform.position += m_currentVelocity * Time.deltaTime;
	}

	private bool IsTargetWithinDangerRange()
	{
		if (m_pcFSM.m_currentEnemyCellTarget == null)
			return false;

		if (Vector2.Distance(PlayerMain.s_Instance.transform.position, m_pcFSM.m_currentEnemyCellTarget.transform.position) > PlayerMain.s_Instance.m_fDetectionRadius)
		{
			if (Vector2.Distance(m_pcFSM.transform.position, m_pcFSM.m_currentEnemyCellTarget.transform.position) > s_fNearDetectionRange)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		else
		{
			return true;
		}
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

	private bool FindNewLocalTarget()
	{
		Collider2D enemyChild = Physics2D.OverlapCircle(m_pcFSM.transform.position, s_fNearDetectionRange, Constants.s_onlyEnemeyChildLayer);
		if (enemyChild != null)
		{
			// Assign the currentEnemyCellTarget in the FSM to the returned enemy cell.
			m_pcFSM.m_currentEnemyCellTarget = enemyChild.gameObject.GetComponent<EnemyChildFSM>();
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool FindNewTarget()
	{
		if (FindNewLocalTarget() == false)
		{
			Collider2D enemyChild = Physics2D.OverlapCircle(PlayerMain.s_Instance.transform.position, PlayerMain.s_Instance.m_fDetectionRadius, Constants.s_onlyEnemeyChildLayer);
			if (enemyChild != null)
			{
				m_pcFSM.m_currentEnemyCellTarget = enemyChild.gameObject.GetComponent<EnemyChildFSM>();
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return true;
		}
	}

	#endregion
}
