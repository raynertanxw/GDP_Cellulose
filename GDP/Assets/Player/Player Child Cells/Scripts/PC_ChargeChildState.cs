using UnityEngine;
using System.Collections;

public class PC_ChargeChildState : IPCState
{
	public override void Enter()
	{
		
	}
	
	public override void Execute()
	{
		if (AreThereTargets() == true)
		{
			if (m_pcFSM.m_currentEnemyCellTarget == null)
			{
				// Switch targets.
				FindTarget();
			}
			else if (IsTargetAlive() == false)
			{
				// Switch targets.
				FindTarget();
			}
			else // Target is still alive.
			{

			}
		}
		else
		{
			// Transition to ChargeMainState
			m_pcFSM.ChangeState(PCState.ChargeMain);
		}
	}
	
	public override void Exit()
	{
		// Clean up
		m_pcFSM.m_currentEnemyCellTarget = null;
	}
	
	// Constructor.
	public PC_ChargeChildState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}
	
	
	
	
	
	
	private static float s_fDetectionRange = 1.0f;
	
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
		
	}

	private void FindTarget()
	{
		Collider2D enemyCell = Physics2D.OverlapCircle(m_pcFSM.transform.position, s_fDetectionRange, Constants.s_onlyEnemeyChildLayer);
		if (enemyCell != null)
		{
			// Assign the currentEnemyCellTarget in the FSM to the returned enemy cell.
			m_pcFSM.m_currentEnemyCellTarget = enemyCell.gameObject.GetComponent<EnemyChildFSM>();
		}
		else
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
	#endregion
}
