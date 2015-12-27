using UnityEngine;
using System.Collections;

public class PC_ChargeChildState : IPCState
{
	private Vector3 m_currentVelocity;
	private static float s_fPlayerChildChargeSpeed = 5.0f;
	private static float s_fDetectionRange = 2.0f;

	public override void Enter()
	{


		m_currentVelocity = Vector3.zero;
	}
	
	public override void Execute()
	{
		if (AreThereTargets() == true)
		{
			if (m_pcFSM.m_currentEnemyCellTarget == null)
			{
				// Switch targets.
				if (FindNearTarget() == false)
					FindFarTarget();
			}
			else if (IsTargetAlive() == false)
			{
				// Switch targets.
				if (FindNearTarget() == false)
					FindFarTarget();
			}
			else // Target is still alive.
			{
				MoveTowardsTarget();

				// Check for nearer target.
				FindNearTarget();
			}
		}
		else
		{
			// Transition to ChargeMainState
			m_pcFSM.ChangeState(PCState.ChargeMain);
		}



		// Check for deferred state change.
		if (m_pcFSM.m_bHasAwaitingDeferredStateChange == true)
		{
			m_pcFSM.ExecuteDeferredStateChange();
		}
	}
	
	public override void Exit()
	{


		// Clean up
		m_pcFSM.m_currentEnemyCellTarget = null;
	}

    public override void FixedExecute()
    {

    }

    // Constructor.
    public PC_ChargeChildState(PlayerChildFSM pcFSM)
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
		m_currentVelocity += direction.normalized * s_fPlayerChildChargeSpeed;
		CapSpeed();
	
		// Apply velocity vector.
		m_pcFSM.transform.position += m_currentVelocity * Time.deltaTime;
	}

	private bool FindNearTarget()
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

	private void FindFarTarget()
	{
		int nClosestEnemyCell = 0;
		float fSqrDistance = 10000f; // Arbitrarily high number.
		// Get Random Target from List.
		for (int i = 0; i < EnemyMainFSM.Instance().ECList.Count; i++)
		{
			if ((m_pcFSM.transform.position - EnemyMainFSM.Instance().ECList[i].transform.position).sqrMagnitude < fSqrDistance)
			{
				nClosestEnemyCell = i;
			}
		}
		
		m_pcFSM.m_currentEnemyCellTarget = EnemyMainFSM.Instance().ECList[nClosestEnemyCell];
	}

	private bool AreThereTargets()
	{
		if (EnemyMainFSM.Instance().ECList.Count > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private void CapSpeed()
	{
		float sqrMag = m_currentVelocity.sqrMagnitude;
		if (sqrMag > Mathf.Pow(s_fPlayerChildChargeSpeed, 2))
		{
			float scalar = Mathf.Pow(s_fPlayerChildChargeSpeed, 2) / sqrMag;
			m_currentVelocity *= scalar;
		}
	}
	#endregion
}
