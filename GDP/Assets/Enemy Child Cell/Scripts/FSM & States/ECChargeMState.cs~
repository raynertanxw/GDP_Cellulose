using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeMState : IECState {

	//a float that determine the speed to charge towards a player child cell
	private float fChargeSpeed;
	private float fMaxAcceleration;
	
	//a gameobject instance that store the player main cell
	private GameObject m_PlayerMain;
	
	private List<Point> PathToTarget;
	private Point CurrentTargetPoint;
	private int CurrentTargetIndex;
	
	private Vector2 previous;
	
	//Constructor for ECChargeMState
	public ECChargeMState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_PlayerMain = m_ecFSM.m_PMain;
		PathToTarget = new List<Point>();
		fChargeSpeed = 1f; //min 3f max 10f;
		fMaxAcceleration = 30f;
	}
	
	public override void Enter()
	{
		//set the velocity of the enemy child cell to be 0
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
		
		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_ecFSM.m_PMain.transform.position,false);
		PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.High);
		CurrentTargetIndex = 0;
		CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		Utility.DrawPath(PathToTarget,Color.red,0.1f);
		
		m_Child.GetComponent<Rigidbody2D>().drag = 2.6f;
	}
	
	public override void Execute()
	{

	}
	
	public override void FixedExecute()
	{
		previous = m_Child.GetComponent<Rigidbody2D>().velocity;
	
		Vector2 Acceleration = Vector2.zero;
		
		if (!HasCellReachTargetPos(CurrentTargetPoint.Position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,CurrentTargetPoint.Position,24f);
			fChargeSpeed += 0.12f;
			fChargeSpeed = Mathf.Clamp(fChargeSpeed,12f,12f);
		}
		else if(CurrentTargetIndex + 1 < PathToTarget.Count)
		{
			CurrentTargetIndex++;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		}
	
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);
		m_ecFSM.RotateToHeading();
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
}

