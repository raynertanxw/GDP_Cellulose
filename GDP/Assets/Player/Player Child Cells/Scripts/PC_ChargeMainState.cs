using UnityEngine;
using System.Collections;

public class PC_ChargeMainState : IPCState
{
	public bool m_bIsLeader = false;
	public bool m_bIsLeaderAlive = false;
	public int m_nFormationPos = -1;

	public override void Enter()
	{
		if (m_pcFSM.m_formationCells[0] == m_pcFSM)
			m_bIsLeader = true;

		for (int i = 0; i < m_pcFSM.m_formationCells.Length; i++)
		{
			if (m_pcFSM.m_formationCells[i] == m_pcFSM)
			{
				m_nFormationPos = i;
				break;
			}
		}

		m_bIsLeaderAlive = true;
	}
	
	public override void Execute()
	{
		if (IsTargetAlive() == false)
			m_pcFSM.ChangeState(PCState.Idle);

		// Check for deferred state change.
		if (m_pcFSM.m_bHasAwaitingDeferredStateChange == true)
		{
			m_pcFSM.ExecuteDeferredStateChange();
		}
	}
	
	public override void Exit()
	{
		m_bIsLeader = false;
		m_bIsLeaderAlive = false;
		m_nFormationPos = -1;
		m_pcFSM.m_formationCells = null;
	}

    public override void FixedExecute()
    {
		if (m_bIsLeader == true)
		{
			// Get the cohesion, alignment, and separation components of the flocking.
			Vector2 acceleration = TargetPull() * targetPullWeight;
			acceleration += Avoidance() * avoidanceWeight;
			// Clamp the acceleration to a maximum value
			acceleration = Vector2.ClampMagnitude(acceleration, maxAcceleration);
			
			// Add the force to all the cell rigidbodies in formation.
			for (int i = 0; i < m_pcFSM.m_formationCells.Length; i++)
				m_pcFSM.m_formationCells[i].rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);
		}
		else if (m_bIsLeaderAlive == false)
		{
			// Get the cohesion, alignment, and separation components of the flocking.
			Vector2 acceleration = TargetPull() * targetPullWeight;
			acceleration += Avoidance() * avoidanceWeight;
			// Clamp the acceleration to a maximum value
			acceleration = Vector2.ClampMagnitude(acceleration, maxAcceleration);

			m_pcFSM.rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);
		}
		else if (m_pcFSM.m_formationCells[0].GetCurrentState() == PCState.Dead)
		{
			m_bIsLeaderAlive = false;
		}
		else
		{
			m_pcFSM.rigidbody2D.MovePosition(PosBehindLeader());
		}

		FaceTowardsHeading();
    }

    #if UNITY_EDITOR
    public override void ExecuteOnDrawGizmos()
    {
        
    }
    #endif

    // Constructor.
    public PC_ChargeMainState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}
	
	
	
	
	
	

	#region Movement and Physics
	private static float s_fAvoidRadius = 5.0f;
	private static float s_fSqrAvoidRadius = Mathf.Pow(s_fAvoidRadius, 2);
	private static float s_fMaxAcceleration = 1000f;
	private static float s_fFormationDist = 0.5f;
	// Weights
	private static float s_fTargetPullWeight = 5000;
	private static float s_fAvoidanceWeight = 10;

	// Getters for the various values.
	public static float sqrAvoidRadius { get { return s_fSqrAvoidRadius; } }
	public static float maxAcceleration { get { return s_fMaxAcceleration; } }
	public static float targetPullWeight { get { return s_fTargetPullWeight; } }
	public static float avoidanceWeight { get { return s_fAvoidanceWeight; } }

	// Flocking related Helper functions
	void FaceTowardsHeading()
	{
		Vector2 heading = m_pcFSM.rigidbody2D.velocity.normalized;
		float fRotation = -Mathf.Atan2(heading.x, heading.y) * Mathf.Rad2Deg;
		m_pcFSM.rigidbody2D.MoveRotation(fRotation);
	}

	// Calculate the sumVector for avoiding all the enemy child cells.
	private Vector2 Avoidance()
	{
		Vector2 sumVector = Vector2.zero;
		int count = 0;
		
		Collider2D[] enemyCells = Physics2D.OverlapCircleAll(m_pcFSM.transform.position, s_fAvoidRadius, Constants.s_onlyEnemeyChildLayer);
		
		Vector2 tmp_enemyPos = Vector2.zero;
		for (int i = 0; i < enemyCells.Length; i++)
		{
			tmp_enemyPos.x = enemyCells[i].transform.position.x;
			tmp_enemyPos.y = enemyCells[i].transform.position.y;
			
			float fDist = Vector2.Distance(m_pcFSM.rigidbody2D.position, tmp_enemyPos);
			sumVector += (m_pcFSM.rigidbody2D.position - tmp_enemyPos).normalized / fDist;
			
			count++;
		}
		
		// Average the sumVector and clamp magnitude
		if (count > 0)
		{
			sumVector /= count;
		}
		
		return sumVector;
	}
	
	private Vector2 TargetPull()
	{
		Vector2 sumVector = -m_pcFSM.rigidbody2D.position;
		sumVector.x += EnemyMainFSM.Instance().transform.position.x;
		sumVector.y += EnemyMainFSM.Instance().transform.position.y;
		
		return sumVector;
	}

	private Vector2 GetHeadingDirection()
	{
		Vector2 sumVector = Vector2.zero;
		sumVector.x = EnemyMainFSM.Instance().transform.position.x - m_pcFSM.m_formationCells[0].transform.position.x;
		sumVector.y = EnemyMainFSM.Instance().transform.position.y - m_pcFSM.m_formationCells[0].transform.position.y;
		return sumVector.normalized;
	}

	private Vector2 PosBehindLeader()
	{
		Vector2 posVector = -GetHeadingDirection() * s_fFormationDist * m_nFormationPos;
		posVector.x += m_pcFSM.m_formationCells[0].rigidbody2D.position.x;
		posVector.y += m_pcFSM.m_formationCells[0].rigidbody2D.position.y;

		return posVector;
	}
	#endregion

	
	#region Helper functions
	private bool IsTargetAlive()
	{
		if (EnemyMainFSM.Instance().Health > 0)
			return true;
		else
			return false;
	}
	#endregion
}
