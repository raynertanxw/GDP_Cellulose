using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECAvoidState : IECState {

	private float fMaxAcceleration;
	private float fDetectRange;
	private float fAvoidTimer;
	private float fTimerLimit;

	private GameObject ClosestAttacker;
	private List<GameObject> AttackersNearby;

    //Constructor
    public ECAvoidState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		fMaxAcceleration = 4f;
		fTimerLimit = 1.5f;
		fDetectRange = m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 10;
		AttackersNearby = new List<GameObject>();
	}
    
	public override void Enter()
	{
		fAvoidTimer = 0f;
		m_Child.GetComponent<Rigidbody2D>().drag = 2.6f;
	}
	
    public override void Execute()
    {
		UpdateAttackerList();
		ClosestAttacker = GetClosestAttacker();
		
		if(AttackersNearby.Count <= 0)
		{
			fAvoidTimer += Time.deltaTime;
		}
		
		if(fAvoidTimer >= fTimerLimit)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
    }

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		Acceleration += m_Main.GetComponent<Rigidbody2D>().velocity ;

		if(AttackersNearby.Count > 0 && ClosestAttacker != null)
		{
			Acceleration += SteeringBehavior.Evade(m_Child,ClosestAttacker,24f);
		}

		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);
		
		if(Acceleration.magnitude > m_Main.GetComponent<Rigidbody2D>().velocity.magnitude)
		{
			m_ecFSM.RotateToHeading();
		}
		else
		{
			m_ecFSM.RandomRotation(0.75f);
		}
	}

    public override void Exit()
    {
		m_Child.GetComponent<Rigidbody2D>().drag = 0.0f;
    }

	private void UpdateAttackerList()
	{
		AttackersNearby.Clear();

		Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(m_Child.transform.position,fDetectRange);
		for(int i = 0; i < NearbyObjects.Length; i++)
		{
			if(NearbyObjects[i].tag == Constants.s_strPlayerChildTag && (NearbyObjects[i].GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeChild || NearbyObjects[i].GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain))
			{
				AttackersNearby.Add(NearbyObjects[i].gameObject);
			}
		}
	}
	
	private GameObject GetClosestAttacker()
	{
		if(AttackersNearby.Count <= 0)
		{
			return null;
		}

		float ClosestDistance = Mathf.Infinity;
		GameObject ClosestAttacker = AttackersNearby[0];
		foreach(GameObject Attacker in AttackersNearby)
		{
			if(Vector2.Distance(m_Child.transform.position,Attacker.transform.position) < ClosestDistance)
			{
				ClosestAttacker = Attacker;
				ClosestDistance = Vector2.Distance(m_Child.transform.position,Attacker.transform.position);
			}
		}
		return ClosestAttacker;
	}
}
