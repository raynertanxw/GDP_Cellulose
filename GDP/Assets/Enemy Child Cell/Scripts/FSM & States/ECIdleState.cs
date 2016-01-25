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
	private static float fSpreadRange;
	
	//A float that dictate the strength of the idle directional vector and it is affected by the scale of the enemy main cell
	private static float fIdleScale;
	
	//A static IdleStatus variable to store what the current idle status all the idle enemy child cell will have
	private static IdleStatus CurrentIdleState;
	
	//A vector2 that store the specific direction at which this enemy child will move during the seperate portion of idling
	private Vector2 SeperateDirection;
	
	private static int IdleCount;

	private static SpriteRenderer EMSpriteRender;
	private static Bounds EMBounds;
	private Transform ECTransform;
	private static Transform EMTransform;
	private static EnemyMainFSM EMFSM;
	private Collider2D[] Collisions;
	private Rigidbody2D ChildRB;
	private static Rigidbody2D MainRB;
	private EMController EMControl;
	private List<GameObject> Neighbours;

	private float fDirectionalWeight;
	private float fSeperationalWeight;
	private static float fTimerLimit;

	//An enumeration for the type of idling the enemy child cell is having
	private enum IdleStatus {None, Seperate, Cohesion};

	//Constructor
	public ECIdleState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		
		fMaxMagnitude = 7.5f;
		fDirectionalWeight = 1.0f;
		fTimerLimit = 1.5f;
		
		fSpreadRange = m_Child.GetComponent<SpriteRenderer>().bounds.size.x/10;
		
		EMSpriteRender = m_Main.GetComponent<SpriteRenderer>();
		EMControl = m_Main.GetComponent<EMController>();
		EMBounds = m_Main.GetComponent<SpriteRenderer>().bounds;
		ECTransform = m_Child.transform;
		EMTransform = m_Main.transform;
		EMFSM = m_Main.GetComponent<EnemyMainFSM>();
		
		ChildRB = m_Child.GetComponent<Rigidbody2D>();
		MainRB = m_Main.GetComponent<Rigidbody2D>();
		
		Collisions = new Collider2D[EMFSM.ECList.Count];
		Neighbours = new List<GameObject>();
		
		m_ecFSM.rigidbody2D.drag = 0f;
		IdleCount = 0;
		CurrentIdleState = IdleStatus.None;
	}

	public override void Enter()
	{
		m_Child.transform.localScale = Vector3.one;
	
		//If there is no idle status specified, start the idle status being the seperate status and set the previous status time being the current time in-game
		if(CurrentIdleState == IdleStatus.None)
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
				if(EMControl.bIsAllChildWithinMain)
				{
					ResetAllChildVelocity();
					ResetAllHitWall();
					CurrentIdleState = IdleStatus.Seperate;
					fPreviousStatusTime = Time.time;
				}
			}
		}
		//If the current idle status is seperating, all the child cells are being spread out fully and it has been 1.5s since the previous change of state or the time passed had been 1.75s, change the idle status to cohesion and record the time
		else if(CurrentIdleState == IdleStatus.Seperate && Time.time - fPreviousStatusTime > fTimerLimit)
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
				Acceleration += SeperateDirection.normalized * fIdleScale * 1.2f;
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
		Neighbours.Clear();
		Collisions = Physics2D.OverlapCircleAll(m_Child.transform.position,fSpreadRange,Constants.s_onlyEnemeyChildLayer);//Change this value to how spread out the spreading is
			
		for(int i = 0; i < Collisions.Length; i++)
		{
			if(Collisions[i].gameObject != m_Child && Collisions[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
			{
				Neighbours.Add(Collisions[i].gameObject);
			}
		}
		
		return Neighbours;
	}
	
	//A function that return a boolean on whether that specific child cell had entered the enemy main cell
	public static bool HasChildEnterMain(GameObject _Child)
	{
		return (Utility.Distance(_Child.transform.position,EMTransform.position) <= EMBounds.size.x/8f) ? true : false;
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

	private float GetTimeLimit()
	{
		return 1.5f + 0.015f * (ECTracker.s_Instance.IdleCells.Count);
	}
	
	public static void ImmediateCohesion()
	{
		CurrentIdleState = IdleStatus.Cohesion;
		fPreviousStatusTime = Time.time;
	}
	
	public static void ResetStatics()
	{
		CurrentIdleState = IdleStatus.None;
	}
}

