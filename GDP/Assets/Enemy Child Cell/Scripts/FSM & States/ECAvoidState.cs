using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECAvoidState : IECState {

	private static float m_fMaxAcceleration;
	private static float m_fDetectRange;
	
	private static bool m_bReturnToMain;
	
	private float m_fAvoidTimer;
	private float m_fTimerLimit;

	private PlayerChildFSM m_ClosestAttacker;
	private List<GameObject> m_AttackersNearby;

    //Constructor
    public ECAvoidState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		
		m_fMaxAcceleration = 4f;
		m_fTimerLimit = 1.5f;
		
		m_fDetectRange = m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 10;
		m_AttackersNearby = new List<GameObject>();
	}
    
	public override void Enter()
	{
		m_fAvoidTimer = 0f;
		m_fMaxAcceleration = 4f;
		m_ecFSM.rigidbody2D.drag = 2.6f;
		
		m_bReturnToMain = false;
		ECTracker.s_Instance.AvoidCells.Add(m_ecFSM);
	}
	
    public override void Execute()
    {
		//Check for any nearby player attacking cells and obtain the closest attacking cell, store it in the "ClosestAttacker" Gameobject
		UpdateAttackerList();
		m_ClosestAttacker = GetClosestAttacker();
		if(m_ClosestAttacker.attackMode == PlayerAttackMode.SwarmTarget){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0.0f);}
		
		//If there is no attackers nearby, increase the avoid timer. Once it reach a limit, transition the child cell back to idle state
		if(m_AttackersNearby.Count <= 0)
		{
			m_fAvoidTimer += Time.deltaTime;
		}
		
		if(m_fAvoidTimer >= m_fTimerLimit)
		{
			m_bReturnToMain = true;
		}
    }

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		//Add the velocity of the enemy main cell to let the child cell follow the main cell without falling behind
		Acceleration += m_Main.GetComponent<Rigidbody2D>().velocity ;

		//if there is any attacking player cell nearby and there is a closest attacker, add the velocity for the enemy child cell to evade that closest attacker
		if(!m_bReturnToMain && m_AttackersNearby.Count > 0 && m_ClosestAttacker != null && m_ClosestAttacker.attackMode != PlayerAttackMode.SwarmTarget)
		{
			Acceleration += LimitMaxAccelBasedOnHeight(SteeringBehavior.Evade(m_Child,m_ClosestAttacker.gameObject,24f));
		}
		else if(m_bReturnToMain && !ECIdleState.HasChildEnterMain(m_Child))
		{
			m_ecFSM.rigidbody2D.drag = 1.4f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,10f);
		}
		else if(m_bReturnToMain && ECIdleState.HasChildEnterMain(m_Child))
		{
			m_ecFSM.rigidbody2D.velocity = Vector2.zero;
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
		else 
		{
			Acceleration += SteeringBehavior.ShakeOnSpot(m_Child,1f,8f);
		}

		//Clamp the velocity to a maximum value, so the speed will reach a constant value
		Acceleration = Vector2.ClampMagnitude(Acceleration,m_fMaxAcceleration);
		
		//Add the calculate force to the enemy child cell to move it
		m_ecFSM.rigidbody2D.AddForce(Acceleration,ForceMode2D.Force);
		
		//If the enemy child cell is avoiding, rotate the cell to whichever direction it travel. Else, just randomly rotate clockwise and anti-clockwise
		if(Acceleration.magnitude > m_Main.GetComponent<Rigidbody2D>().velocity.magnitude)
		{
			m_ecFSM.RotateToHeading();
		}
		else
		{
			m_ecFSM.RandomRotation();
		}
	}

    public override void Exit()
    {
		ECTracker.s_Instance.AvoidCells.Remove(m_ecFSM);
    }

	private void UpdateAttackerList()
	{
		//Clear the attacker list and check the enemy child cell for any nearby attacking player cells based on the detect range provided
		m_AttackersNearby.Clear();

		Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(m_Child.transform.position,m_fDetectRange,Constants.s_onlyPlayerChildLayer);
		for(int i = 0; i < NearbyObjects.Length; i++)
		{
			if(NearbyObjects[i].GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeChild || NearbyObjects[i].GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain)
			{
				m_AttackersNearby.Add(NearbyObjects[i].gameObject);
			}
		}
	}
	
	private PlayerChildFSM GetClosestAttacker()
	{
		//If there is no attacking child cell in the list, return null. Else, based on the distance from that attacking child cell to the enemy child cell, get the closest attacking player cell.
		if(m_AttackersNearby.Count <= 0)
		{
			return null;
		}

		Vector2 ChildPos = m_Child.transform.position;
		float ClosestDistance = Mathf.Infinity;
		float ChildtoAttacker = 0f;
		GameObject ClosestAttacker = m_AttackersNearby[0];
		
		for(int i = 0; i < m_AttackersNearby.Count; i++)
		{
			ChildtoAttacker = Utility.Distance(ChildPos, m_AttackersNearby[i].transform.position);
			if(ChildtoAttacker < ClosestDistance)
			{
				ClosestAttacker = m_AttackersNearby[i];
				ClosestDistance = ChildtoAttacker;
			}
		}

		return ClosestAttacker.GetComponent<PlayerChildFSM>();
	}
	
	private Vector2 LimitMaxAccelBasedOnHeight(Vector2 _Accel)
	{
		float MaxEvadeRange = 4.0f;
		float DifferenceYFromChildToMain = Mathf.Abs(m_Child.transform.position.y - m_Main.transform.position.y);
		
		if(DifferenceYFromChildToMain > MaxEvadeRange)
		{
			return new Vector2(_Accel.x, 0.5f * _Accel.y);
		}
		
		float Ratio = 1f - (DifferenceYFromChildToMain/MaxEvadeRange);
		return new Vector2(_Accel.x, Ratio * _Accel.y);
	}
}
