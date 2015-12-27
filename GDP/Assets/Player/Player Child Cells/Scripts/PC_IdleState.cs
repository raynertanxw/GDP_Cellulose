using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PC_IdleState : IPCState
{
	private Vector3 m_nodeOrigin;
	private float m_fMaxDisplacement = 1f;
	private float m_fTargetReachRadius = 0.1f;
	private bool m_bReachedTarget = true;
	private Vector3 m_currentTarget;

	private Vector3 m_currentVelocity;
	private static float s_fPlayerChildIdleSpeed = 1f;
	private static float s_fDetectionRangeRadius = 0.6f;

	public override void Enter()
	{
		m_nodeOrigin = m_pcFSM.m_assignedNode.transform.position;
		m_currentVelocity = Vector3.zero;

        // Give a random velocity.
        m_pcFSM.rigidbody2D.velocity = Random.insideUnitCircle;
	}
	
	public override void Execute()
	{
		MoveAroundNode();
		if (DetectedEnemyInRange() == true)
		{
			if (m_pcFSM.m_bIsDefending == true)
			{
				// Switch to defending mode.
				m_pcFSM.ChangeState(PCState.Defend);
			}
			else
			{
				// Switch to avoid mode.
				m_pcFSM.ChangeState(PCState.Avoid);
			}
		}

		// Check for deferred state change.
		if (m_pcFSM.m_bHasAwaitingDeferredStateChange == true)
		{
			m_pcFSM.ExecuteDeferredStateChange();
		}
	}
	
	public override void Exit()
	{
		m_bReachedTarget = true;
	}

    public override void FixedExecute()
    {
        //// Get the cohesion, alignment, and separation components of the flocking.
        //Vector2 acceleration = Cohesion() * cohesionWeight;
        //acceleration += Alignment() * alignmentWeight;
        //acceleration += Separation() * separationWeight;
        //// Clamp the acceleration to a maximum value
        //acceleration = Vector2.ClampMagnitude(acceleration, maxAcceleration);

        //// Add the force to the rigidbody and face the direction of movement
        //rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);
        //FaceTowardsHeading();
    }

    // Draw the radius of the cohesion neighbourhood in green and the radius of the separation neightbouthood in red, in the scene view.
    #if UNITY_EDITOR
    public override void ExecuteOnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_pcFSM.transform.position, cohesionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_pcFSM.transform.position, separationRadius);
    }
    #endif

    // Constructor.
    public PC_IdleState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}











	#region Helper functions
	private bool DetectedEnemyInRange()
	{
		Collider2D enemyCell = Physics2D.OverlapCircle(m_pcFSM.transform.position, s_fDetectionRangeRadius, Constants.s_onlyEnemeyChildLayer);
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

	private void MoveAroundNode()
	{
		if (m_bReachedTarget == true)
		{
			SetNewWanderTarget();
		}
		else
		{
			// Calculate "force" vector.
			Vector3 direction = m_currentTarget - m_pcFSM.transform.position;
			m_currentVelocity += direction.normalized * s_fPlayerChildIdleSpeed;
			CapSpeed();
		}

		// Apply velocity vector.
		m_pcFSM.transform.position += m_currentVelocity * Time.deltaTime;

		// If reached target.
		if ((m_pcFSM.transform.position - m_currentTarget).sqrMagnitude < Mathf.Pow(m_fTargetReachRadius, 2))
		{
			m_bReachedTarget = true;
		}
	}

	private void SetNewWanderTarget()
	{
		m_currentTarget = (Vector3)(Random.insideUnitCircle * m_fMaxDisplacement) + m_nodeOrigin;
		m_bReachedTarget = false;
	}

	private void CapSpeed()
	{
		float sqrMag = m_currentVelocity.sqrMagnitude;
		if (sqrMag > Mathf.Pow(s_fPlayerChildIdleSpeed, 2))
		{
			float scalar = Mathf.Pow(s_fPlayerChildIdleSpeed, 2) / sqrMag;
			m_currentVelocity *= scalar;
		}
	}
    #endregion








    #region Flocking
    // Flocking related variables
    private static float s_fCohesionRadius = 1.0f;
    private static float s_fseparationRadius = 0.5f;
    private static float s_fMaxAcceleration = 10f;
    // Weights
    private static float s_fCohesionWeight = 30;
    private static float s_fAlignmentWeight = 1000;
    private static float s_fSeparationWeight = 5000;


    // Getters for the various values.
    public static float cohesionRadius { get { return s_fCohesionRadius; } }
    public static float separationRadius { get { return s_fseparationRadius; } }
    public static float maxAcceleration { get { return s_fMaxAcceleration; } }
    public static float cohesionWeight { get { return s_fCohesionWeight; } }
    public static float alignmentWeight { get { return s_fAlignmentWeight; } }
    public static float separationWeight { get { return s_fSeparationWeight; } }

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
                Debug.Log("Ignoring case boid against itself");
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
                Debug.Log("Ignoring case boid against itself");
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
                Debug.Log("Ignoring case boid against itself");
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
    #endregion
}
