using UnityEngine;
using System.Collections;

public class PC_ChargeMainState : IPCState
{
	private Vector3 m_currentVelocity;
	private static float s_fPlayerChildChargeSpeed = 5.0f;
	private static float s_fDetectionRange = 2.0f;

	public override void Enter()
	{

	}
	
	public override void Execute()
	{
		if (IsTargetAlive() == true)
		{
			MoveTowardsTarget();
		}
		else
		{
			// Move back to Idle, game should be over.
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
    public PC_ChargeMainState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}
	
	
	
	
	
	
	
	
	#region Helper functions
	private bool IsTargetAlive()
	{
		if (EnemyMainFSM.Instance().Health > 0)
			return true;
		else
			return false;
	}

	private void MoveTowardsTarget()
	{
		// Calculate "force" vector.
		Vector3 direction = EnemyMainFSM.Instance().transform.position - m_pcFSM.transform.position;
		m_currentVelocity = direction.normalized * s_fPlayerChildChargeSpeed;
		CapSpeed();
		
		// Apply velocity vector.
		m_pcFSM.transform.position += m_currentVelocity * Time.deltaTime;
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
