using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
	//A static float to store the max magnitude that the acceleration can be acted on the enemy child cell
	private static float fMaxMagnitude;
	
	//A static float that store the time at which the previous idle status is changed
	private static float fPreviousStatusTime;
	
	//A float that store how far the enemy child cell need to be spreaded out
	private float fSpreadRange;
	
	//A float that dictate the strength of the idle directional vector and it is affected by the scale of the enemy main cell
	private float fIdleScale;
	
	//A static IdleStatus variable to store what the current idle status all the idle enemy child cell will have
	private static IdleStatus CurrentIdleState;
	
	//A vector2 that store the specific direction at which this enemy child will move during the seperate portion of idling
	private Vector2 SeperateDirection;
	
	private static int IdleCount;

	private SpriteRenderer EMSpriteRender;
	private Bounds EMBounds;
	private Transform ECTransform;
	private Transform EMTransform;
	private EnemyMainFSM EMFSM;
	private Collider2D[] Collisions;
	private Rigidbody2D ChildRB;
	private Rigidbody2D MainRB;

	//An enumeration for the type of idling the enemy child cell is having
	private enum IdleStatus {Seperate, Cohesion};

	//Constructor
	public ECIdleState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		
		fMaxMagnitude = 7.5f;
		
		fSpreadRange = m_Child.GetComponent<SpriteRenderer>().bounds.size.x/10;
		
		EMSpriteRender = m_Main.GetComponent<SpriteRenderer>();
		EMBounds = m_Main.GetComponent<SpriteRenderer>().bounds;
		ECTransform = m_Child.transform;
		EMTransform = m_Main.transform;
		EMFSM = m_Main.GetComponent<EnemyMainFSM>();
		
		ChildRB = m_Child.GetComponent<Rigidbody2D>();
		MainRB = m_Main.GetComponent<Rigidbody2D>();
		
		Collisions = new Collider2D[EMFSM.ECList.Count];
		
		m_Child.GetComponent<Rigidbody2D>().drag = 0f;
		IdleCount = 0;
	}

	public override void Enter()
	{
		//If there is no idle status specified, start the idle status being the seperate status and set the previous status time being the current time in-game
		if(CurrentIdleState == null)
		{
			CurrentIdleState = IdleStatus.Seperate;
			fPreviousStatusTime = Time.time;
		}
		
		//Obtain the specific seperate direction for the enemy child cell from the direction database
		m_ecFSM.bHitWall = false;
		SeperateDirection = DirectionDatabase.Instance.Extract();
		fIdleScale = m_Main.transform.localScale.x * 0.85f;
		IdleCount++;
		ECTracker.s_Instance.IdleCells.Add(m_ecFSM);
	}
	
	public override void Execute()
	{  
		/*If the current idle status is cohesioning and the child cell had enter the main cell, stop its movement and let it follow the enemy main cell. 
		Once all idling enemy child cell in the enemy main cell, reset their velocity, change the current idle status to seperate state and set the current time be the previous status time*/
		if(CurrentIdleState == IdleStatus.Cohesion)
		{
			if(HasChildEnterMain(m_Child))
			{
				ChildRB.velocity = MainRB.velocity;
				if(HasAllChildEnterMain())
				{
					ResetAllChildVelocity();
					ResetAllHitWall();
					CurrentIdleState = IdleStatus.Seperate;
					fPreviousStatusTime = Time.time;
				}
			}
		}
		//If the current idle status is seperating, all the child cells are being spread out fully and it has been 1.5s since the previous change of state or the time passed had been 1.75s, change the idle status to cohesion and record the time
		else if(CurrentIdleState == IdleStatus.Seperate && Time.time - fPreviousStatusTime > 1.5f && HasCellsSpreadOutMax() || CurrentIdleState == IdleStatus.Seperate && Time.time - fPreviousStatusTime > 1.75f)
		{
			ResetAllHitWall();
			CurrentIdleState = IdleStatus.Cohesion;
			fPreviousStatusTime = Time.time;
		}
	}
	
	public override void FixedExecute()
	{  
		Vector2 Acceleration = Vector2.zero;
		
		//If the current idle status is cohesioning and the enemy child cell had not enter the enemy main cell, continue seek towards the enemy main cell

		if(!m_ecFSM.bHitWall)
		{
			if(m_ecFSM.IsHittingSideWalls())
			{
				m_ecFSM.bHitWall = true;
				ChildRB.velocity = Vector2.zero;
			}
			else if(CurrentIdleState == IdleStatus.Cohesion && !HasChildEnterMain(m_Child))
			{
				Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,20f);
			}
			else if(CurrentIdleState == IdleStatus.Seperate && !m_ecFSM.IsHittingSideWalls())
			{
				Acceleration += SeperateDirection.normalized * fIdleScale;
				Acceleration += MainRB.velocity;
				Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours());
			}
		}
		else
		{
			if(!HasChildEnterMain(m_Child))
			{
				ChildRB.drag = 2.5f;
				Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,10f);
			}
			else
			{
				ChildRB.velocity = MainRB.velocity;
			}
		}

		/*if(m_ecFSM.IsHittingSideWalls() && m_ecFSM.bHitWall == false)
		{
			m_ecFSM.bHitWall = true;
			m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}
		else if(CurrentIdleState == IdleStatus.Cohesion && !HasChildEnterMain(m_Child) && m_ecFSM.bHitWall == false)
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,20f);
		}
		//Else if the current idle status is seperating, add the specific directional force to travel, the velocity of the main cell to follow the main cell and the seperation force to seperate itself from the nearby enemy child cell
		else if(CurrentIdleState == IdleStatus.Seperate && !m_ecFSM.IsHittingSideWalls() && m_ecFSM.bHitWall == false)
		{
			Acceleration += SeperateDirection.normalized * fIdleScale;
			Acceleration += m_Main.GetComponent<Rigidbody2D>().velocity;
			//Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours());
		}
		else if(m_ecFSM.bHitWall == true && !HasChildEnterMain(m_Child))
		{
			m_Child.GetComponent<Rigidbody2D>().drag = 2.5f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,10f);
		}
		else if(m_ecFSM.bHitWall == true && HasChildEnterMain(m_Child))
		{
			m_Child.GetComponent<Rigidbody2D>().velocity = m_Main.GetComponent<Rigidbody2D>().velocity;
		}*/

		//Clamp the acceleration of the enemy child cell to a specific maximum of magnitude and add that acceleration as a force on to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxMagnitude);
		ChildRB.AddForce(Acceleration);
		
		//Rotate the enemy child cell according to the specific direction of velocity it is enforced on
		m_ecFSM.RotateToHeading();
	}
	
	public override void Exit()
	{
		//Return that specific seperation direction back to the direction database so it can be used for another child cell
		DirectionDatabase.Instance.Return(SeperateDirection);
		IdleCount--;
		ECTracker.s_Instance.IdleCells.Remove(m_ecFSM);
	}
	
	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position,fSpreadRange,Constants.s_onlyEnemeyChildLayer);//Change this value to how spread out the spreading is
		
		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}
		
		return Neighbours;
	}
	
	//A function that return a boolean on whether the cells had spread out to its maximum potential
	private bool HasCellsSpreadOutMax()
	{
		List<EnemyChildFSM> ECList = ECTracker.s_Instance.IdleCells;
		Vector2 ReferencePos = Vector2.zero;
		
		for(int i = 0; i < ECList.Count; i++)
		{
			ReferencePos = ECList[i].transform.position;
			for(int t = 0; t < ECList.Count; t++)
			{
				if(i != t && Utility.Distance(ReferencePos,ECList[t].transform.position) <= fSpreadRange)
				{
					return false;
				}
			}
			
			/*Collisions = Physics2D.OverlapCircleAll(ECList[i].transform.position,fSpreadRange,Constants.s_onlyEnemeyChildLayer);
			for(int t = 0; t < Collisions.Length; t++)
			{
				if(Collisions[t].GetComponent<EnemyChildFSM>/().CurrentStateEnum == ECState.Idle)
				{
					return false;
				}
			}*/
		}
		
		return true;
	}
	
	//A function that return a boolean on whether that specific child cell had entered the enemy main cell
	private bool HasChildEnterMain(GameObject _Child)
	{
		return (Utility.Distance(_Child.transform.position,EMTransform.position) <= EMBounds.size.x/8f) ? true : false;
	}
	
	//A function that return a boolean on whether all enemy child cell had entered the enemy main cell
	private bool HasAllChildEnterMain()
	{
		List<EnemyChildFSM> ECList = EMFSM.ECList;
		for(int i = 0; i < ECList.Count; i++)
		{
			if(ECList[i].CurrentStateEnum == ECState.Idle && !HasChildEnterMain(ECList[i].gameObject))
			{
				return false;
			}
		}
		return true;
	}
	
	//A function that reset all enemy child cell velocity to the main cell velocity
	private void ResetAllChildVelocity()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		Vector2 MainVelo = m_Main.GetComponent<Rigidbody2D>().velocity;
		
		for(int i = 0; i < ECList.Count; i++)
		{
			if(ECList[i].CurrentStateEnum == ECState.Idle)
			{
				ECList[i].GetComponent<Rigidbody2D>().velocity = MainVelo;
			}
		}
	}

	private void ResetAllHitWall()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		for(int i = 0; i < ECList.Count; i++)
		{
			if(ECList[i].CurrentStateEnum == ECState.Idle)
			{
				ECList[i].bHitWall = false;
				ECList[i].GetComponent<Rigidbody2D>().drag = 0f;
			}
		}
	}
}

