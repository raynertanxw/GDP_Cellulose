using UnityEngine;
using System.Collections;

public class PC_ChargeChildState : IPCState
{
    public bool m_bIsLeader = false;
    public bool m_bIsLeaderAlive = false;
    public int m_nLeaderIndex = -1;

    private Transform m_targetPos;

    public override void Enter()
	{
        if (m_pcFSM.m_formationCells[0] == m_pcFSM)
            m_bIsLeader = true;

        m_bIsLeaderAlive = true;
        m_nLeaderIndex = 0;
        m_targetPos = m_pcFSM.transform; // In case leader doesn't process first.

		if (m_pcFSM.attackMode == PlayerAttackMode.ScatterShot)
			m_targetPos = EnemyMainFSM.Instance().transform;
    }
	
	public override void Execute()
	{
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
        m_targetPos = null;
        m_pcFSM.m_currentEnemyCellTarget = null;
	}

    public override void FixedExecute()
    {
        switch (m_pcFSM.attackMode)
        {
            case PlayerAttackMode.SwarmTarget:
                if (m_bIsLeader)
                {
                    if (AreThereTargets() == true)
                    {
                        if (m_pcFSM.m_currentEnemyCellTarget == null)
                        {
                            // Switch targets.
                            if (FindNearTarget() == false)
                                FindFarTarget();
                        }
                        else if (IsTargetAlive() == false)
                        {
                            // Switch targets.
                            if (FindNearTarget() == false)
                                FindFarTarget();
                        }

                        m_targetPos = m_pcFSM.m_currentEnemyCellTarget.transform;
                    }
                    else
                    {
                        // Set the target to the player main.
                        m_pcFSM.m_currentEnemyCellTarget = null;
                        m_targetPos = EnemyMainFSM.Instance().transform;
                    }
                }
                else
                {
                    if (m_bIsLeaderAlive == false)
                    {
                        // Assign new leader
                        for (int i = 0; i < m_pcFSM.m_formationCells.Length; i++)
                        {
                            // The first alive cell in formation.
                            if (m_pcFSM.m_formationCells[i].GetCurrentState() != PCState.Dead)
                            {
                                if (m_pcFSM.m_formationCells[i].gameObject == m_pcFSM.gameObject)
                                    m_bIsLeader = true;

                                m_bIsLeaderAlive = true;
                                m_nLeaderIndex = i;
                            }
                        }
                    }
                    else
                    {
                        if (m_pcFSM.m_formationCells[m_nLeaderIndex].GetCurrentState() == PCState.Dead)
                            m_bIsLeaderAlive = false;
                    }

                    if (AreThereTargets() == true)
                    {
                        if (m_pcFSM.m_formationCells[m_nLeaderIndex].m_currentEnemyCellTarget != null)
                            m_targetPos = m_pcFSM.m_formationCells[m_nLeaderIndex].m_currentEnemyCellTarget.transform;
                    }
                    else
                    {
                        // Set the target to the player main.
                        m_pcFSM.m_currentEnemyCellTarget = null;
                        m_targetPos = EnemyMainFSM.Instance().transform;
                    }
                }

                SwarmTargetFixedExecute();
                break;
            case PlayerAttackMode.ScatterShot:
                ScatterShotFixedExecute();
                break;
        }
    }

    #if UNITY_EDITOR
    public override void ExecuteOnDrawGizmos()
    {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(m_pcFSM.transform.position, s_fSeparationRadius);
    }
    #endif

    // Constructor.
    public PC_ChargeChildState(PlayerChildFSM pcFSM)
	{
		m_pcFSM = pcFSM;
	}









    #region Movement and Physics
    private static float s_fAvoidRadius = 2.0f;
    private static float s_fSqrAvoidRadius = Mathf.Pow(s_fAvoidRadius, 2);
	private static float s_fSeparationRadius = 0.5f;
	private static float s_fSqrSeparationRadius = Mathf.Pow(s_fSeparationRadius, 2);
    private static float s_fMaxAcceleration = 2000f;
    // Weights
    private static float s_fTargetPullWeight = 50;
    private static float s_fAvoidanceWeight = 2000;
	private static float s_fSeparationWeight = 2000;

    // Getters for the various values.
    public static float sqrAvoidRadius { get { return s_fSqrAvoidRadius; } }
	public static float sqrSeparationRadius { get { return s_fSqrSeparationRadius; } }
    public static float maxAcceleration { get { return s_fMaxAcceleration; } }
    public static float targetPullWeight { get { return s_fTargetPullWeight; } }
    public static float avoidanceWeight { get { return s_fAvoidanceWeight; } }
	public static float separationWeight { get { return s_fSeparationWeight; } }

    private void SwarmTargetFixedExecute()
    {
        // Get the components of the flocking.
        Vector2 acceleration = TargetPull() * targetPullWeight;
        acceleration += Avoidance() * avoidanceWeight;
        // Clamp the acceleration to a maximum value
        acceleration = Vector2.ClampMagnitude(acceleration, maxAcceleration);

        m_pcFSM.rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);

        FaceTowardsHeading();
    }

    private void ScatterShotFixedExecute()
    {
		// Get the componenets for the flocking.
		Vector2 acceleration = TargetPull() * targetPullWeight;
		acceleration += Avoidance() * avoidanceWeight;
		acceleration += Separation() * separationWeight;
		// Clamp the acceleration to a maximum value
		acceleration = Vector2.ClampMagnitude(acceleration, maxAcceleration);
		m_pcFSM.rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);

        FaceTowardsHeading();
    }

    // Flocking related Helper functions
    private void FaceTowardsHeading()
    {
        Vector2 heading = m_pcFSM.rigidbody2D.velocity.normalized;
        float fRotation = -Mathf.Atan2(heading.x, heading.y) * Mathf.Rad2Deg;
        m_pcFSM.rigidbody2D.MoveRotation(fRotation);
    }

    // Calculate the sumVector for avoiding all the enemy child cells.
    private Vector2 Avoidance()
    {
        Vector2 sumVector = Vector2.zero;

        Collider2D enemyMain = Physics2D.OverlapCircle(m_pcFSM.transform.position, s_fAvoidRadius, Constants.s_onlyEnemyMainLayer);

        if (enemyMain != null)
        {
            sumVector = m_pcFSM.rigidbody2D.position;
            sumVector -= enemyMain.attachedRigidbody.position;

            float fDist = Vector2.Distance(Vector2.zero, sumVector);
            sumVector = sumVector.normalized / fDist;

            return sumVector;
        }
        else
        {
            return sumVector;
        }
    }

    private Vector2 TargetPull()
    {
        Vector2 sumVector = -m_pcFSM.rigidbody2D.position;
        sumVector.x += m_targetPos.position.x;
        sumVector.y += m_targetPos.position.y;

        return sumVector;
    }

	private Vector2 Separation()
	{
		Vector2 sumVector = Vector2.zero;
		int count = 0;
		
		for (int i = 0; i < m_pcFSM.m_formationCells.Length; i++)
		{
			// Skip itself
			if (m_pcFSM.m_formationCells[i].gameObject == m_pcFSM.gameObject)
				continue;
			
			float fSqrDist = 	Mathf.Pow(m_pcFSM.rigidbody2D.position.x - m_pcFSM.m_formationCells[i].rigidbody2D.position.x, 2) +
								Mathf.Pow(m_pcFSM.rigidbody2D.position.y - m_pcFSM.m_formationCells[i].rigidbody2D.position.y, 2);
			if (fSqrDist < sqrSeparationRadius)
			{
				Vector2 tmpDistVec = m_pcFSM.rigidbody2D.position - m_pcFSM.m_formationCells[i].rigidbody2D.position;
				sumVector += tmpDistVec.normalized / fSqrDist;
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







    #region Helper functions
    private static float s_fDetectionRange = 2.0f;

    private bool IsTargetAlive()
	{
		if (m_pcFSM.m_currentEnemyCellTarget.CurrentStateEnum == ECState.Dead)
			return false;
		else
			return true;
	}

	private bool FindNearTarget()
	{
		Collider2D enemyCell = Physics2D.OverlapCircle(m_pcFSM.transform.position, s_fDetectionRange, Constants.s_onlyEnemeyChildLayer);
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

	private void FindFarTarget()
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

	private bool AreThereTargets()
	{
		if (EnemyMainFSM.Instance().ECList.Count > 0)
			return true;
		else
			return false;
	}
	#endregion
}
