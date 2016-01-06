using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECAvoidState : IECState {

	private float fMaxAcceleration;
	private float fDetectRange;

	private GameObject ClosestAttacker;
	private List<GameObject> AttackersNearby;

    //Constructor
    public ECAvoidState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		fMaxAcceleration = 30f;
		fDetectRange = m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 10;
		AttackersNearby = new List<GameObject>();
	}
    
	public override void Enter()
	{
		m_Child.GetComponent<Rigidbody2D>().drag = 2.6f;
	}
	
    public override void Execute()
    {
		UpdateAttackerList();
		ClosestAttacker = GetClosestAttacker();
    }

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		if(AttackersNearby.Count > 0 && ClosestAttacker != null)
		{
			Acceleration += SteeringBehavior.Evade(m_Child,ClosestAttacker,24f);
		}

		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);
		m_ecFSM.RotateToHeading();
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
