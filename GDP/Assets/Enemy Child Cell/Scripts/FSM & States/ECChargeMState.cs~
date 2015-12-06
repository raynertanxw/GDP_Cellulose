using UnityEngine;
using System.Collections;

public class ECChargeMState : IECState {

	//a float that determine the speed to charge towards a player child cell
	private float fChargeSpeed;
	
	//a gameobject instance that store the player main cell
	private GameObject m_PlayerMain;
	
	//Constructor for ECChargeMState
	public ECChargeMState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_PlayerMain = m_ecFSM.m_PMain;
		fChargeSpeed = 1f; //min 3f max 10f;
	}
	
	public override void Enter()
	{
		//set the velocity of the enemy child cell to be 0
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
	}
	
	public override void Execute()
	{
		//if the enemy child cell had not reach the player main cell, continue charge towards the player main cell
		if (!HasCellReachTargetPos(m_PlayerMain.transform.position))
		{
			ChargeTowards(m_PlayerMain);
		}
	}
	
	public override void Exit()
	{
		//set the velocity of the enemy child cell to be 0
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 + m_PlayerMain.GetComponent<SpriteRenderer>().bounds.size.x/2)
		{
			return true;
		}
		return false;
	}
	
	//a function that direct the enemy child cell towards a gameObject by changing its velocity through calculation
	private void ChargeTowards(GameObject _PM)
	{
		Vector2 m_TargetPos = _PM.transform.position;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,12f);
	}
}

