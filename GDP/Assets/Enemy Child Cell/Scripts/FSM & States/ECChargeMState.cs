using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeMState : IECState {

	//a float that determine the speed to charge towards a player child cell
	private float fChargeSpeed;
	
	//a gameobject instance that store the player main cell
	private GameObject m_PlayerMain;
	
	private List<Point> PathToTarget;
	private Point CurrentTargetPoint;
	private int CurrentTargetIndex;
	
	//Constructor for ECChargeMState
	public ECChargeMState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_PlayerMain = m_ecFSM.m_PMain;
		PathToTarget = new List<Point>();
		fChargeSpeed = 1f; //min 3f max 10f;
	}
	
	public override void Enter()
	{
		//set the velocity of the enemy child cell to be 0
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
		
		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_ecFSM.m_PMain.transform.position,false);
		PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.High);
		CurrentTargetIndex = 0;
		CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
	}
	
	public override void Execute()
	{
		//if the enemy child cell had not reach the player main cell, continue charge towards the player main cell
		if (!HasCellReachTargetPos(CurrentTargetPoint.Position))
		{
			ChargeTowards(CurrentTargetPoint);
		}
		else if(CurrentTargetIndex + 1 < PathToTarget.Count)
		{
			CurrentTargetIndex++;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
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
	private void ChargeTowards(Point _Point)
	{
		Vector2 targetPos = _Point.Position;
		Vector2 difference = new Vector2(targetPos.x - m_Child.transform.position.x, targetPos.y - m_Child.transform.position.y);
		Vector2 direction = difference.normalized;
		m_Child.GetComponent<Rigidbody2D>().velocity = direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,12f);
	}
	
	/*private void ChargeTowards(GameObject _PM)
	{
		Vector2 m_TargetPos = _PM.transform.position;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,12f);
	}*/
}

