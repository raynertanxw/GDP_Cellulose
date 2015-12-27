using UnityEngine;
using System.Collections;

public class PC_AvoidState : IPCState
{
	private Vector3 m_currentVelocity;
	private static float s_fPlayerChildAvoidSpeed = 5.0f;
	private static float s_fAvoidRange = 1.0f;

	public override void Enter()
	{

	}
	
	public override void Execute()
	{
		if (findClosestEnemy() == true)
		{
			MoveAwayFromEnemy();
		}
		else
		{
			// If nothing to avoid then change back to idle.
			m_pcFSM.ChangeState(PCState.Idle);
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
    public PC_AvoidState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}







	

	#region Helper functions
	private bool findClosestEnemy()
	{
		Collider2D[] enemyCells = Physics2D.OverlapCircleAll(m_pcFSM.transform.position, s_fAvoidRange, Constants.s_onlyEnemeyChildLayer);
		if (enemyCells.Length > 0)
		{
			// Assign the closest currentEnemyCellTarget in the FSM to the returned enemy cell.
			int nClosestEnemyCell = 0;
			float fClosestDistance = 1000.0f;
			for (int i = 0; i < enemyCells.Length; i++)
			{
				float fDistance = (enemyCells[i].transform.position - m_pcFSM.transform.position).sqrMagnitude;
				if (fDistance < fClosestDistance)
				{
					fClosestDistance = fDistance;
					nClosestEnemyCell = i;
				}
			}

			m_pcFSM.m_currentEnemyCellTarget = enemyCells[nClosestEnemyCell].gameObject.GetComponent<EnemyChildFSM>();
			return true;
		}
		else
		{
			return false;
		}
	}

	private void MoveAwayFromEnemy()
	{
		// Calculate "force" vector.
		Vector3 direction = m_pcFSM.m_currentEnemyCellTarget.transform.position - m_pcFSM.transform.position;
		m_currentVelocity += direction.normalized * -s_fPlayerChildAvoidSpeed;
		CapSpeed();
		
		// Apply velocity vector.
		m_pcFSM.transform.position += m_currentVelocity * Time.deltaTime;
	}

	private void CapSpeed()
	{
		float sqrMag = m_currentVelocity.sqrMagnitude;
		if (sqrMag > Mathf.Pow(s_fPlayerChildAvoidSpeed, 2))
		{
			float scalar = Mathf.Pow(s_fPlayerChildAvoidSpeed, 2) / sqrMag;
			m_currentVelocity *= scalar;
		}
	}
	#endregion
}
