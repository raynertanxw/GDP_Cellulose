using UnityEngine;
using System.Collections;

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
		else if (IsTargetAlive() == false || IsTargetWithinDangerRange() == false)
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
		Vector2 acceleration = Separation() * separationWeight;
		acceleration += Target() * targetPullWeight;
		// Clamp the acceleration to a maximum value
		acceleration = Vector2.ClampMagnitude(acceleration, maxAcceleration);
		
		// Add the force to the rigidbody and face the direction of movement
		m_pcFSM.rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);
		m_pcFSM.rigidbody2D.velocity = Vector2.ClampMagnitude(m_pcFSM.rigidbody2D.velocity, maxVelocity);
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

		if (Vector2.Distance(PlayerMain.Instance.transform.position, m_pcFSM.m_currentEnemyCellTarget.transform.position) > PlayerMain.Instance.m_fDetectionRadius)
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
			if (PlayerMain.Instance.hasSurroundingEnemyCells == true)
			{
				m_pcFSM.m_currentEnemyCellTarget = PlayerMain.Instance.surroundingEnemyCells[0].gameObject.GetComponent<EnemyChildFSM>();
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
	private static float s_fSqrCohesionRadius = Mathf.Pow(s_fCohesionRadius, 2);
	private static float s_fSeparationRadius = 0.5f;
	private static float s_fSqrSeperationRadius = Mathf.Pow(s_fSeparationRadius, 2);
	private static float s_fMaxAcceleration = 1000f;
	private static float s_fMacVelocity = 2.5f;
	// Weights
	private static float s_fCohesionWeight = 30;
	private static float s_fAlignmentWeight = 10;
	private static float s_fSeparationWeight = 1500;
	private static float s_fTargetPullWeight = 5000;
	
	
	// Getters for the various values.
	public static float sqrCohesionRadius { get { return s_fSqrCohesionRadius; } }
	public static float sqrSeparationRadius { get { return s_fSqrSeperationRadius; } }
	public static float maxAcceleration { get { return s_fMaxAcceleration; } }
	public static float maxVelocity { get { return s_fMacVelocity; } }
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
	
	public Vector2 Alignment()
	{
		Vector2 sumVector = Vector2.zero;
		int count = 0;

		Collider2D[] nearbyChildren = Physics2D.OverlapCircleAll(m_pcFSM.transform.position, s_fCohesionRadius, Constants.s_onlyPlayerChildLayer);

		for (int i = 0; i < nearbyChildren.Length; i++)
		{
			// Skip itself
			if (nearbyChildren[i].gameObject == m_pcFSM.gameObject)
				continue;

			sumVector += nearbyChildren[i].attachedRigidbody.velocity;
			count++;
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
	
	public Vector2 Target()
	{
		Vector2 sumVector = -m_pcFSM.rigidbody2D.position;
		float fSqrDist = (sumVector + m_pcFSM.m_currentEnemyCellTarget.rigidbody2D.position).sqrMagnitude;
		if (fSqrDist < 1.0f)
			sumVector += PredictPos(0f, m_pcFSM.m_currentEnemyCellTarget.rigidbody2D.position, m_pcFSM.m_currentEnemyCellTarget.rigidbody2D.velocity);
		else
			sumVector += PredictPos(0.35f, m_pcFSM.m_currentEnemyCellTarget.rigidbody2D.position, m_pcFSM.m_currentEnemyCellTarget.rigidbody2D.velocity);

		return sumVector;
	}

	private Vector2 PredictPos(float _T, Vector2 _posVec, Vector2 _velVec)
	{
		return _posVec + _velVec * _T;
	}
	#endregion
}
