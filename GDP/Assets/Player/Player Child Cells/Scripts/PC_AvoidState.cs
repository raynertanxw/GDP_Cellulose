using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PC_AvoidState : IPCState
{
	private Vector3 m_currentVelocity;
	private static float s_fPlayerChildAvoidSpeed = 5.0f;
	private static float s_fAvoidRange = 1.0f;
	private Vector2 m_nodeOrigin;

	public override void Enter()
	{
		m_nodeOrigin = m_pcFSM.m_assignedNode.transform.position;
	}
	
	public override void Execute()
	{
		if (findClosestEnemy() == false)
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

    #if UNITY_EDITOR
    public override void ExecuteOnDrawGizmos()
    {
        
    }
    #endif

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
	#endregion


	#region Flocking
	// Flocking related variables
	private static float s_fCohesionRadius = 2.0f;
	private static float s_fseparationRadius = 0.25f;
	private static float s_fMaxAcceleration = 10f;
	// Weights
	private static float s_fCohesionWeight = 300;
	private static float s_fAlignmentWeight = 100;
	private static float s_fSeparationWeight = 5000;
	private static float s_fOriginPullWeight = 500;
	
	
	// Getters for the various values.
	public static float cohesionRadius { get { return s_fCohesionRadius; } }
	public static float separationRadius { get { return s_fseparationRadius; } }
	public static float maxAcceleration { get { return s_fMaxAcceleration; } }
	public static float cohesionWeight { get { return s_fCohesionWeight; } }
	public static float alignmentWeight { get { return s_fAlignmentWeight; } }
	public static float separationWeight { get { return s_fSeparationWeight; } }
	public static float originPullWeight { get { return s_fOriginPullWeight; } }
	
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
	
	public Vector2 OriginPull()
	{
		Vector2 sumVector = m_nodeOrigin - m_pcFSM.rigidbody2D.position;
		
		return sumVector;
	}
	#endregion
}
