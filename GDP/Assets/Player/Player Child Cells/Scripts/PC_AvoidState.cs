using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PC_AvoidState : IPCState
{
	private Vector2 m_nodeOrigin;

	public override void Enter()
	{
		m_nodeOrigin = m_pcFSM.m_assignedNode.transform.position;
	}
	
	public override void Execute()
	{
		if (DetectedEnemyInRange() == false)
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
        Debug.Log("Exited Avoid State.");
    }

    public override void FixedExecute()
    {
        // Get the cohesion, alignment, and separation components of the flocking.
        Vector2 acceleration = Cohesion() * cohesionWeight;
        acceleration += Alignment() * alignmentWeight;
        acceleration += Separation() * separationWeight;
        acceleration += OriginPull() * originPullWeight;
        acceleration += Avoidance() * avoidanceWeight;
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
    public PC_AvoidState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}









    #region Helper functions
    private bool DetectedEnemyInRange()
    {
        Collider2D enemyChild = Physics2D.OverlapCircle(PlayerMain.s_Instance.transform.position, PlayerMain.s_Instance.m_fDetectionRadius, Constants.s_onlyEnemeyChildLayer);
        if (enemyChild != null)
        {
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
    private static float s_fAvoidRadius = 5.0f;
    private static float s_fMaxAcceleration = 500f;
	// Weights
	private static float s_fCohesionWeight = 0;
	private static float s_fAlignmentWeight = 0;
	private static float s_fSeparationWeight = 100;
	private static float s_fOriginPullWeight = 5;
    private static float s_fAvoidanceWeight = 1000;
	
	
	// Getters for the various values.
	public static float cohesionRadius { get { return s_fCohesionRadius; } }
	public static float separationRadius { get { return s_fseparationRadius; } }
    public static float avoidRadius { get { return s_fAvoidRadius; } }
	public static float maxAcceleration { get { return s_fMaxAcceleration; } }
	public static float cohesionWeight { get { return s_fCohesionWeight; } }
	public static float alignmentWeight { get { return s_fAlignmentWeight; } }
	public static float separationWeight { get { return s_fSeparationWeight; } }
	public static float originPullWeight { get { return s_fOriginPullWeight; } }
    public static float avoidanceWeight { get { return s_fAvoidanceWeight; } }
	
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

    // Calculate the sumVector for avoiding all the enemy child cells.
    public Vector2 Avoidance()
    {
        Vector2 sumVector = Vector2.zero;
        int count = 0;

        Collider2D[] enemyCells = Physics2D.OverlapCircleAll(m_pcFSM.transform.position, avoidRadius, Constants.s_onlyEnemeyChildLayer);

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
	
	public Vector2 OriginPull()
	{
		Vector2 sumVector = m_nodeOrigin - m_pcFSM.rigidbody2D.position;
		
		return sumVector;
	}
	#endregion
}
