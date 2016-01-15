using UnityEngine;
using System.Collections;

public class PC_IdleState : IPCState
{
	private Vector2 m_nodeOrigin;
	private static float s_fDetectionRangeRadius = 1.0f;

	public override void Enter()
	{
		m_nodeOrigin = m_pcFSM.m_assignedNode.transform.position;

        // Give a random velocity.
        m_pcFSM.rigidbody2D.velocity = (Random.insideUnitCircle * 0.25f);
	}
	
	public override void Execute()
	{
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
		
	}

    public override void FixedExecute()
    {
        // Get the cohesion, alignment, and separation components of the flocking.
        Vector2 acceleration = Separation() * separationWeight;
        acceleration += OriginPull() * originPullWeight;
        // Clamp the acceleration to a maximum value
        acceleration = Vector2.ClampMagnitude(acceleration, maxAcceleration);

        // Add the force to the rigidbody and face the direction of movement
        m_pcFSM.rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);
        FaceTowardsHeading();
    }

    // Draw the radius of the cohesion neighbourhood in green and the radius of the separation neightbouthood in red, in the scene view.
    #if UNITY_EDITOR    
    public override void ExecuteOnDrawGizmos()
    {
        m_pcFSM.fGizmoCohesionRadius = s_fCohesionRadius;
        m_pcFSM.fGizmoSeparationRadius = s_fSeparationRadius;
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
		Collider2D enemyChild = Physics2D.OverlapCircle(PlayerMain.s_Instance.transform.position, PlayerMain.s_Instance.m_fDetectionRadius, Constants.s_onlyEnemeyChildLayer);
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
    #endregion








    #region Flocking
    // Flocking related variables
    private static float s_fCohesionRadius = 0.75f;
	private static float s_fSqrCohesionRadius = Mathf.Pow(s_fCohesionRadius, 2);
    private static float s_fSeparationRadius = 0.25f;
	private static float s_fSqrSeperationRadius = Mathf.Pow(s_fSeparationRadius, 2);
    private static float s_fMaxAcceleration = 10f;
    // Weights
    private static float s_fCohesionWeight = 300;
    private static float s_fSeparationWeight = 1000;
    private static float s_fOriginPullWeight = 1500;


    // Getters for the various values.
	public static float sqrCohesionRadius { get { return s_fSqrCohesionRadius; } }
	public static float sqrSeparationRadius { get { return s_fSqrSeperationRadius; } }
    public static float maxAcceleration { get { return s_fMaxAcceleration; } }
    public static float cohesionWeight { get { return s_fCohesionWeight; } }
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

		Collider2D[] nearbyChildren = Physics2D.OverlapCircleAll(m_pcFSM.transform.position, s_fCohesionRadius, Constants.s_onlyPlayerChildLayer);

		for (int i = 0; i < nearbyChildren.Length; i++)
		{
			// Skip itself.
			if (nearbyChildren[i].gameObject == m_pcFSM.gameObject)
				continue;

			sumVector.x += nearbyChildren[i].transform.position.x;
			sumVector.y += nearbyChildren[i].transform.position.y;
			count++;
		}

        // Average the sumVector
        if (count > 0)
        {
            sumVector /= count;
            return sumVector - m_pcFSM.rigidbody2D.position;
        }

        return sumVector;
    }

    public Vector2 Separation()
    {
        Vector2 sumVector = Vector2.zero;
        int count = 0;

		Collider2D[] nearbyChildren = Physics2D.OverlapCircleAll(m_pcFSM.transform.position, s_fSeparationRadius, Constants.s_onlyPlayerChildLayer);

		for (int i = 0; i < nearbyChildren.Length; i++)
		{
			// Skip itself
			if (nearbyChildren[i].gameObject == m_pcFSM.gameObject)
				continue;

			float fSqrDist = 	Mathf.Pow(m_pcFSM.rigidbody2D.position.x - nearbyChildren[i].transform.position.x, 2) +
								Mathf.Pow(m_pcFSM.rigidbody2D.position.y - nearbyChildren[i].transform.position.y, 2);

			Vector2 tmpDistVec = m_pcFSM.rigidbody2D.position;
			tmpDistVec.x -= nearbyChildren[i].transform.position.x;
			tmpDistVec.y -= nearbyChildren[i].transform.position.y;
			sumVector += tmpDistVec.normalized / fSqrDist;
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
