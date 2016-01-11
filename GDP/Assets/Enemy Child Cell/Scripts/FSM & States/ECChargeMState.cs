using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeMState : IECState {

	//A float to dictate the maximum acceleration that the Enemy Child can undertake
	private float fMaxAcceleration;
	
	//A GameObject reference that store the player main cell
	private GameObject m_PlayerMain;
	
	//A list of Points that is used to store a sequence of points which are then used as a path for the enemy child cell to travel to the player main cell
	private List<Point> PathToTarget;
	
	//A Point variable that store the current Point that the enemy child cell is traveling towards in the path
	private Point CurrentTargetPoint;
	
	//An integer variable that store the current Point index that the enemy child cell is traveling towards
	private int CurrentTargetIndex;
	
	//Constructor
	public ECChargeMState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_PlayerMain = m_ecFSM.m_PMain;
		
		PathToTarget = new List<Point>();
		fMaxAcceleration = 10f;
	}
	
	public override void Enter()
	{
		//Reset the velocity of enemy child cell
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
		
		//Calculate a path for the enemy child cell to travel to the enemy main cell
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
		Vector2 Acceleration = Vector2.zero;
		
		//If the enemy child cell had not reached the next target point in the path, calculate seek towards that point while remaining seperated from other nearby enemy child cells
		if (!HasCellReachTargetPos(CurrentTargetPoint.Position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,CurrentTargetPoint.Position,30f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 50f;
		}
		//Else if current target index is still not the last index in the path, set the target point for the enemy child cell to be the next point in the path searched
		else if(CurrentTargetIndex + 1 < PathToTarget.Count)
		{
			CurrentTargetIndex++;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		}
	
		//Clamp the acceleration of the enemy child cell and add that acceleration to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);
		
		//Rotate the enemy child to the direction of the velocity
		m_ecFSM.RotateToHeading();
	}
	
	public override void Exit()
	{
		//Reset the velocity and the force acting on the enemy child cell
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
	
	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
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

