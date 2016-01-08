using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
	private static float fMaxAcceleration;
	private static float fPreviousStatusTime;
	private bool bAllCellInMain;
	private static IdleStatus CurrentIdleState;
	
	private Vector2 SeperateDirection;

	private enum IdleStatus {Seperate, Cohesion};

	public ECIdleState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		fMaxAcceleration = 8f;
	
		if(CurrentIdleState == null)
		{
			CurrentIdleState = IdleStatus.Seperate;
			fPreviousStatusTime = Time.time;
		}
		else
		{
			m_ecFSM.StartChildCorountine(WaitForNextCohesion());
		}
	}

	public override void Enter()
	{
		SeperateDirection = DirectionDatabase.Instance.Extract();//new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f));
		
		//Debug.Log(m_Child.name + " direction: " + SeperateDirection);
	}
	
	public override void Execute()
	{  
		if(CurrentIdleState == IdleStatus.Cohesion && IsAllCellsWithinMain())
		{
			//SeperateDirection = new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f));
			
			m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			CurrentIdleState = IdleStatus.Seperate;
			fPreviousStatusTime = Time.time;
			bAllCellInMain = false;
		}
		else if(CurrentIdleState == IdleStatus.Cohesion && IsCurrentCellInMain())
		{
			m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			m_Child.transform.position = m_Main.transform.position;
		}
		else if(CurrentIdleState == IdleStatus.Seperate && HasReachSpreadLimit() && Time.time - fPreviousStatusTime > 1f || Time.time - fPreviousStatusTime > 1.25f)
		{
			CurrentIdleState = IdleStatus.Cohesion;
			fPreviousStatusTime = Time.time;
		} 
	}
	
	public override void FixedExecute()
	{  
		Vector2 Acceleration = Vector2.zero;
	
		if(CurrentIdleState == IdleStatus.Cohesion && !IsCurrentCellInMain())
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,6f);
		}
		else if(CurrentIdleState == IdleStatus.Seperate)
		{
			Acceleration += SeperateDirection.normalized * 0.75f;
			Acceleration += m_Main.GetComponent<Rigidbody2D>().velocity;
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours());
		}
		
		//Debug.Log(m_Child.name + ": " + Acceleration.magnitude);
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		if(Acceleration.magnitude >= 8f){Debug.Log("ping");}
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);
		m_ecFSM.RotateToHeading();
	}
	
	public override void Exit()
	{
		DirectionDatabase.Instance.Return(SeperateDirection);
	}
	
	private bool IsCurrentCellInMain()
	{
		if(Vector2.Distance(m_Child.transform.position, m_Main.transform.position) < m_Main.GetComponent<SpriteRenderer>().bounds.size.x/4)
		{
			return true;
		}
		return false;
	}
	
	private bool IsAllCellsWithinMain()
	{
		List<EnemyChildFSM> ECCells = m_ecFSM.m_EMain.GetComponent<EnemyMainFSM>().ECList;
		foreach(EnemyChildFSM Cell in ECCells)
		{
			if(Cell.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle && Vector2.Distance(Cell.transform.position, m_Main.transform.position) > m_Main.GetComponent<SpriteRenderer>().bounds.size.x/10)
			{
				return false;
			}
		}
		return true;
	}
	
	private bool HasReachSpreadLimit()
	{
		List<EnemyChildFSM> ChildList = m_ecFSM.m_EMain.GetComponent<EnemyMainFSM>().ECList;
		
		foreach(EnemyChildFSM Child in ChildList)
		{
			Collider2D[] HittedObjects = Physics2D.OverlapCircleAll(Child.transform.position,m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2,Constants.s_onlyEnemeyChildLayer);
			foreach(Collider2D HittedObject in HittedObjects)
			{
				if(HittedObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
				{
					return false;
				}
			}
		}
		
		return true;
	}
	
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();
		
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2);
		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.tag == Constants.s_strEnemyChildTag && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}
		
		return Neighbours;
	}
	
	private List<Vector2> TagNeighboursPosition()
	{
		List<Vector2> Neighbours = new List<Vector2>();
		Neighbours.Add(m_Main.transform.position);
		
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2);
		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.tag == Constants.s_strEnemyChildTag && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
			{
				Neighbours.Add(Neighbouring[i].gameObject.transform.position);
			}
		}
		
		return Neighbours;
	}
	
	private Vector2 GetRandomDirection()
	{
		return Random.insideUnitCircle;
	}
	
	private IEnumerator WaitForNextCohesion()
	{
		while(CurrentIdleState == IdleStatus.Seperate)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(1.25f);
	}
}