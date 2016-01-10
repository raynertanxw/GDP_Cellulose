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
	
	//Constructor for ECChargeMState
	public ECChargeMState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_PlayerMain = m_ecFSM.m_PMain;
		PathToTarget = new List<Point>();
		fChargeSpeed = 1f; //min 3f max 10f;
		fMaxAcceleration = 10f;
	}
	
	public override void Enter()
	{
		//set the velocity of the enemy child cell to be 0
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
		
		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_ecFSM.m_PMain.transform.position,false);
		PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.High);
		CurrentTargetIndex = 0;
		CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		
		m_Child.GetComponent<Rigidbody2D>().drag = 2.6f;
	}
	
	public override void Execute()
	{

	}
	
	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;
		
		if (!HasCellReachTargetPos(CurrentTargetPoint.Position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,CurrentTargetPoint.Position,30f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 50f;
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
		m_Child.GetComponent<Rigidbody2D>().drag = 0f;
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
	
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();
		
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 1.25f);
		//Debug.Log("Neighbouring count: " + Neighbouring.Length);
		
		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.tag == Constants.s_strEnemyChildTag && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.ChargeMain)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}
		
		return Neighbours;
	}
}

