using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
	private static float fMinMagnitude;
	private static float fMaxMagnitude;
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
		fMinMagnitude = 1.25f;
		fMaxMagnitude = 7f;
	}

	public override void Enter()
	{
		if(CurrentIdleState == null)
		{
			CurrentIdleState = IdleStatus.Seperate;
			fPreviousStatusTime = Time.time;
		}
				
		SeperateDirection = DirectionDatabase.Instance.Extract();
	}
	
	public override void Execute()
	{  
		if(CurrentIdleState == IdleStatus.Cohesion && IsAllCellsWithinMain())
		{
			m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			CurrentIdleState = IdleStatus.Seperate;
			fPreviousStatusTime = Time.time;
			bAllCellInMain = false;
		}
		else if(CurrentIdleState == IdleStatus.Cohesion && IsCurrentCellInMain())
		{
			m_Child.GetComponent<Rigidbody2D>().drag = 0f;
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
			//if(m_Child.name.Contains("22")){Debug.Log(m_Child.name + " seek");}
		}
		else if(CurrentIdleState == IdleStatus.Seperate)
		{
			Acceleration += SeperateDirection.normalized * 1.5f;
			Acceleration += m_Main.GetComponent<Rigidbody2D>().velocity;
			
			Vector2 SeperationVelo = SteeringBehavior.Seperation(m_Child,TagNeighbours());
			Vector2 SeperationNormal = SeperationVelo.normalized;
			float SeperationMagnitude =  SeperationVelo.magnitude;
			float MinimumMagnitude = 0.2f;
			Acceleration += Mathf.Clamp(SeperationMagnitude,MinimumMagnitude,SeperationMagnitude) * SeperationNormal;
			
			//Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours());
			//if(m_Child.name.Contains("22")){Debug.Log(m_Child.name + " Seperate");}
			//if(m_Child.name.Contains("22")){Debug.Log(m_Child.name + ": SD normal: " + SeperateDirection.normalized * 0.75f + " main velo: " + m_Main.GetComponent<Rigidbody2D>().velocity + "Seperation velo: " + SteeringBehavior.Seperation(m_Child,TagNeighbours()) + " Actual velo: " + Acceleration);}
		}

		Vector2 DirectionNormal = Acceleration.normalized;
		float AccelerationMagnitude = Acceleration.magnitude;
		if(CurrentIdleState == IdleStatus.Seperate)
		{
			Acceleration = Mathf.Clamp(AccelerationMagnitude,fMinMagnitude,4f) * DirectionNormal;
		}
		else if(CurrentIdleState == IdleStatus.Cohesion)
		{
			Acceleration = Mathf.Clamp(AccelerationMagnitude,fMinMagnitude,6f) * DirectionNormal;
		}
		//if(CurrentIdleState == IdleStatus.Seperate && Acceleration.magnitude > 3.5f){Debug.Log(Acceleration.magnitude);}
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
		//yield return new WaitForSeconds(2.5f);
	}
}
