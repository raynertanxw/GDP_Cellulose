using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
	private static float fMinMagnitude;
	private static float fMaxMagnitude;
	private static float fPreviousStatusTime;
	private float fSpreadRange;
	private static IdleStatus CurrentIdleState;
	
	private Vector2 SeperateDirection;

	private enum IdleStatus {Seperate, Cohesion};

	public ECIdleState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		fMinMagnitude = 1.25f;
		fMaxMagnitude = 7.5f;
		fSpreadRange = m_Child.GetComponent<SpriteRenderer>().bounds.size.x/10;
		m_Child.GetComponent<Rigidbody2D>().drag = 0f;
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
		if(CurrentIdleState == IdleStatus.Cohesion)
		{
			if(HasChildEnterMain(m_Child))
			{
				//if(m_Child.name.Contains("35")){Debug.Log(m_Child.name + " locked");}
				m_Child.GetComponent<Rigidbody2D>().velocity = m_Main.GetComponent<Rigidbody2D>().velocity;
			}
			
			if(HasAllChildEnterMain())
			{
				//Debug.Log(m_Child.name + " seperate");
				ResetAllChildVelocity();
				CurrentIdleState = IdleStatus.Seperate;
				fPreviousStatusTime = Time.time;
			}
		}
		else if(CurrentIdleState == IdleStatus.Seperate && HasCellsSpreadOutMax() && Time.time - fPreviousStatusTime > 1.5f || CurrentIdleState == IdleStatus.Seperate && Time.time - fPreviousStatusTime > 1.75f)
		{
			//Debug.Log(m_Child.name + " cohesion");
			CurrentIdleState = IdleStatus.Cohesion;
			fPreviousStatusTime = Time.time;
		}
	}
	
	public override void FixedExecute()
	{  
		Vector2 Acceleration = Vector2.zero;
		
		if(CurrentIdleState == IdleStatus.Cohesion && !HasChildEnterMain(m_Child))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,20f);
		}
		else if(CurrentIdleState == IdleStatus.Seperate)
		{
			Acceleration += SeperateDirection.normalized;
			Acceleration += m_Main.GetComponent<Rigidbody2D>().velocity;
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours());
		}
		
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxMagnitude);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration);
		m_ecFSM.RotateToHeading();
	}
	
	public override void Exit()
	{
		DirectionDatabase.Instance.Return(SeperateDirection);
	}
	
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position,fSpreadRange);//Change this value to how spread out the spreading is
		
		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.tag == Constants.s_strEnemyChildTag && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}
		
		return Neighbours;
	}
	
	private bool HasCellsSpreadOutMax()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		
		foreach(EnemyChildFSM Child in ECList)
		{
			Collider2D[] Collisions = Physics2D.OverlapCircleAll(Child.transform.position,fSpreadRange,Constants.s_onlyEnemeyChildLayer);
			foreach(Collider2D Hit in Collisions)
			{
				if(Hit.gameObject.tag == Constants.s_strEnemyChildTag && Hit.gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
				{
					return false;
				}
			}
		}
		
		return true;
	}
	
	private bool HasChildEnterMain(GameObject _Child)
	{
		if(Vector2.Distance(_Child.transform.position,m_Main.transform.position) <= m_Main.GetComponent<SpriteRenderer>().bounds.size.x/9.5f)
		{
			return true;
		}
		return false;
	}
	
	private bool HasAllChildEnterMain()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		foreach(EnemyChildFSM Child in ECList)
		{
			if(Child.CurrentStateEnum == ECState.Idle && !HasChildEnterMain(Child.gameObject))
			{
				return false;
			}
		}
		return true;
	}
	
	private void ResetAllChildVelocity()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		foreach(EnemyChildFSM Child in ECList)
		{
			if(Child.CurrentStateEnum == ECState.Idle)
			{
				Child.GetComponent<Rigidbody2D>().velocity = m_Main.GetComponent<Rigidbody2D>().velocity;
			}
		}
	}
}

