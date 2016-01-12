using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PC_DefendState : IPCState
{
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
		// Get the cohesion, alignment, and separation components of the flocking.
		Vector2 acceleration = Cohesion() * cohesionWeight;
		acceleration += Alignment() * alignmentWeight;
		acceleration += Separation() * separationWeight;
		acceleration += Target() * targetPullWeight;
		// Clamp the acceleration to a maximum value
		acceleration = Vector2.ClampMagnitude(acceleration, maxAcceleration);
		
		// Add the force to the rigidbody and face the direction of movement
		m_pcFSM.rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);
		FaceTowardsHeading();
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
		if (FindNewLocalTarget() == false) // If can't find local target, find in danger zone.
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















	#region Flocking
	// Flocking related variables
	private static float s_fCohesionRadius = 2.0f;
	private static float s_fseparationRadius = 0.5f;
	private static float s_fMaxAcceleration = 10f;
	// Weights
	private static float s_fCohesionWeight = 30;
	private static float s_fAlignmentWeight = 10;
	private static float s_fSeparationWeight = 50;
	private static float s_fTargetPullWeight = 500;
	
	
	// Getters for the various values.
	public static float cohesionRadius { get { return s_fCohesionRadius; } }
	public static float separationRadius { get { return s_fseparationRadius; } }
	public static float maxAcceleration { get { return s_fMaxAcceleration; } }
	public static float cohesionWeight { get { return s_fCohesionWeight; } }
	public static float alignmentWeight { get { return s_fAlignmentWeight; } }
	public static float separationWeight { get { return s_fSeparationWeight; } }
	public static float targetPullWeight { get { return s_fTargetPullWeight; } }
	
	// Flocking related Helper functions
	void FaceTowardsHeading()
	{
		Vector2 heading = m_pcFSM.rigidbody2D.velocity.normalized;
		float fRotation = -Mathf.Atan2(heading.x, heading.y) * Mathf.Rad2Deg;
		m_pcFSM.rigidbody2D.MoveRotation(fRotation);
	}
	
	public Vector2 Cohesion()
	{
		Vector2 sumVector = Vector2.zero;
		int count = 0;
		
		List<PlayerChildFSM> nodeChildren = m_pcFSM.m_assignedNode.GetNodeChildList();
		
		// For each boid, check the distance from this boid, and if within a neighbourhood, add to the sumVector
		for (int i = 0; i < nodeChildren.Count; i++)
		{
			// If it is itself skip itself.
			if (nodeChildren[i] == m_pcFSM)
			{
				continue;
			}
			
			float fDist = Vector2.Distance(m_pcFSM.rigidbody2D.position, nodeChildren[i].rigidbody2D.position);
			
			if (fDist < cohesionRadius)
			{
				sumVector += nodeChildren[i].rigidbody2D.position;
				count++;
			}
		}
		
		
		// Average the sumVector
		if (count > 0)
		{
			sumVector /= count;
			return sumVector - m_pcFSM.rigidbody2D.position;
		}
		
		return sumVector;
	}
	
	public Vector2 Alignment()
	{
		Vector2 sumVector = Vector2.zero;
		int count = 0;
		
		List<PlayerChildFSM> nodeChildren = m_pcFSM.m_assignedNode.GetNodeChildList();
		
		// For each boid, check the distance from this boid, and if within a neighbourhood, add to the sum_vector.
		for (int i = 0; i < nodeChildren.Count; i++)
		{
			// If it is itself skip itself.
			if (nodeChildren[i] == m_pcFSM)
			{
				continue;
			}
			
			float fDist = Vector2.Distance(m_pcFSM.rigidbody2D.position, nodeChildren[i].rigidbody2D.position);
			
			if (fDist < cohesionRadius)
			{
				sumVector += nodeChildren[i].rigidbody2D.velocity;
				count++;
			}
		}
		
		// Average the sumVector and clamp magnitude
		if (count > 0)
		{
			sumVector /= count;
			sumVector = Vector2.ClampMagnitude(sumVector, 1);
		}
		
		return sumVector;
	}
	
	public Vector2 Separation()
	{
		Vector2 sumVector = Vector2.zero;
		int count = 0;
		
		List<PlayerChildFSM> nodeChildren = m_pcFSM.m_assignedNode.GetNodeChildList();
		
		// For each boid, check the distance from this boid, and if within a neighbourhood, add to the sum_vector.
		for (int i = 0; i < nodeChildren.Count; i++)
		{
			// If it is itself skip itself.
			if (nodeChildren[i] == m_pcFSM)
			{
				continue;
			}
			
			float fDist = Vector2.Distance(m_pcFSM.rigidbody2D.position, nodeChildren[i].rigidbody2D.position);
			
			if (fDist < separationRadius)
			{
				sumVector += (m_pcFSM.rigidbody2D.position - nodeChildren[i].rigidbody2D.position).normalized / fDist;
				count++;
			}
		}
		
		// Average the sumVector and clamp magnitude
		if (count > 0)
		{
			sumVector /= count;
		}
		
		return sumVector;
	}
	
	public Vector2 Target()
	{
		Vector2 sumVector = -m_pcFSM.rigidbody2D.position;
		sumVector.x += m_pcFSM.m_currentEnemyCellTarget.transform.position.x;
		sumVector.y += m_pcFSM.m_currentEnemyCellTarget.transform.position.y;
		
		return sumVector;
	}
	#endregion
}
