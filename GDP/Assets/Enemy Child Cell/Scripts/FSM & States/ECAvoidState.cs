using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECAvoidState : IECState {

	//A float to limit the amount of acceleration the child cell can take when avoiding
	private float fMaxAcceleration;
	
	//A float to dictate how far the enemy child cell can dected any player-related cells that are attacking
	private float fDetectRange;
	
	//Two floats to dictate how long the enemy child cell had waited to avoid any player cells and once over the limit, return the cell back to idle state
	private float fAvoidTimer;
	private float fTimerLimit;

	//A GameObject reference to the closest attacking player cell nearby
	private GameObject ClosestAttacker;
	
	//A GameObject List that store all the nearby attacking player cells to this enemy child cell
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
		//Check for any nearby player attacking cells and obtain the closest attacking cell, store it in the "ClosestAttacker" Gameobject
		UpdateAttackerList();
		ClosestAttacker = GetClosestAttacker();
		
		//If there is no attackers nearby, increase the avoid timer. Once it reach a limit, transition the child cell back to idle state
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

		//Add the velocity of the enemy main cell to let the child cell follow the main cell without falling behind
		Acceleration += m_Main.GetComponent<Rigidbody2D>().velocity ;

		//if there is any attacking player cell nearby and there is a closest attacker, add the velocity for the enemy child cell to evade that closest attacker
		if(AttackersNearby.Count > 0 && ClosestAttacker != null)
		{
			Acceleration += SteeringBehavior.Evade(m_Child,ClosestAttacker,24f);
		}

		//Clamp the velocity to a maximum value, so the speed will reach a constant value
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		
		//Add the calculate force to the enemy child cell to move it
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);
		
		//If the enemy child cell is avoiding, rotate the cell to whichever direction it travel. Else, just randomly rotate clockwise and anti-clockwise
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
		m_Child.GetComponent<Rigidbody2D>().drag = 0f;
    }

	private void UpdateAttackerList()
	{
		//Clear the attacker list and check the enemy child cell for any nearby attacking player cells based on the detect range provided
		AttackersNearby.Clear();

		Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(m_Child.transform.position,fDetectRange,Constants.s_onlyPlayerChildLayer);
		for(int i = 0; i < NearbyObjects.Length; i++)
		{
			if(NearbyObjects[i].GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeChild || NearbyObjects[i].GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain)
			{
				AttackersNearby.Add(NearbyObjects[i].gameObject);
			}
		}
	}
	
	private GameObject GetClosestAttacker()
	{
		//If there is no attacking child cell in the list, return null. Else, based on the distance from that attacking child cell to the enemy child cell, get the closest attacking player cell.
		if(AttackersNearby.Count <= 0)
		{
			return null;
		}

		float ClosestDistance = Mathf.Infinity;
		GameObject ClosestAttacker = AttackersNearby[0];
		foreach(GameObject Attacker in AttackersNearby)
		{
			float ChildtoAttacker = Utility.Distance(m_Child.transform.position, Attacker.transform.position);
			if(ChildtoAttacker < ClosestDistance)
			{
				ClosestAttacker = Attacker;
				ClosestDistance = ChildtoAttacker;
			}
		}
		return ClosestAttacker;
	}
}
