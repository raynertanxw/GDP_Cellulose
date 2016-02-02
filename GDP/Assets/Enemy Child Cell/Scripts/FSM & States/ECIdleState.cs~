using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
	private static float m_fMaxMagnitude;
	private static float m_fPreviousStatusTime;
	private static float m_fSpreadRange;
	private static float m_fIdleScale;
	private static float m_fTimerLimit;
	private static int m_nIdleCount;
	
	
	private static IdleStatus m_CurrentIdleState;
	private static Bounds m_EMBounds;
	private static Transform m_EMTransform;
	private static EnemyMainFSM m_EMFSM;
	private static Rigidbody2D m_MainRB;
	private static Vector3 m_ShrinkRate;
	
	private Vector2 m_SeperateDirection;
	private Rigidbody2D m_ChildRB;
	private EMController m_EMControl;
	
	private Collider2D[] m_Collisions;
	private List<GameObject> m_Neighbours;

	private enum IdleStatus {None, Seperate, Cohesion};

	//Constructor
	public ECIdleState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		m_EMTransform = m_Main.transform;
		
		m_fMaxMagnitude = 7.5f;
		m_fTimerLimit = 1.5f;
		
		m_fSpreadRange = m_Child.GetComponent<SpriteRenderer>().bounds.size.x/10;
		m_EMControl = m_Main.GetComponent<EMController>();
		m_EMBounds = m_Main.GetComponent<SpriteRenderer>().bounds;
		m_EMFSM = m_Main.GetComponent<EnemyMainFSM>();
		m_ChildRB = m_Child.GetComponent<Rigidbody2D>();
		m_MainRB = m_Main.GetComponent<Rigidbody2D>();
		
		m_Collisions = new Collider2D[m_EMFSM.ECList.Count];
		m_Neighbours = new List<GameObject>();
		
		m_ecFSM.rigidbody2D.drag = 0f;
		m_nIdleCount = 0;
		m_CurrentIdleState = IdleStatus.None;
		
		m_ShrinkRate = new Vector3(0.025f,-0.025f,0f);
	}

	public override void Enter()
	{
		m_Child.transform.localScale = Vector3.one;
	
		//If there is no idle status specified, start the idle status being the seperate status and set the previous status time being the current time in-game
		if(m_CurrentIdleState == IdleStatus.None)
		{
			m_CurrentIdleState = IdleStatus.Seperate;
			m_fPreviousStatusTime = Time.time;
		}
		
		//Obtain the specific seperate direction for the enemy child cell from the direction database
		m_ecFSM.m_bHitWall = false;
		m_SeperateDirection = DirectionDatabase.Instance.Extract();
		m_fIdleScale = m_Main.transform.localScale.x * 0.85f;
		m_ecFSM.rigidbody2D.drag = 0f;
		m_nIdleCount++;
		ECTracker.s_Instance.IdleCells.Add(m_ecFSM);
	}
	
	public override void Execute()
	{  
		/*If the current idle status is cohesioning and the child cell had enter the main cell, stop its movement and let it follow the enemy main cell. 
		Once all idling enemy child cell in the enemy main cell, reset their velocity, change the current idle status to seperate state and set the current time be the previous status time*/
		if(m_CurrentIdleState == IdleStatus.Cohesion)
		{
			if(HasChildEnterMain(m_Child))
			{
				m_ChildRB.velocity = m_MainRB.velocity;
				if(m_EMControl.bIsAllChildWithinMain)
				{
					AudioManager.PlayEMSoundEffect(EnemyMainSFX.IdleExpand);
					ResetAllChildVelocity();
					ResetAllHitWall();
					m_CurrentIdleState = IdleStatus.Seperate;
					m_fPreviousStatusTime = Time.time;
				}
			}
		}
		//If the current idle status is seperating, all the child cells are being spread out fully and it has been 1.5s since the previous change of state or the time passed had been 1.75s, change the idle status to cohesion and record the time
		else if(m_CurrentIdleState == IdleStatus.Seperate && Time.time - m_fPreviousStatusTime > m_fTimerLimit)
		{
			AudioManager.PlayEMSoundEffect(EnemyMainSFX.IdleContract);
			ResetAllHitWall();
			m_CurrentIdleState = IdleStatus.Cohesion;
			m_fPreviousStatusTime = Time.time;
		}
		
		if(m_CurrentIdleState == IdleStatus.Seperate && Time.time - m_fPreviousStatusTime > 0.75f * m_fTimerLimit && m_Child.transform.localScale.y > 0.5f)
		{
			m_Child.transform.localScale += m_ShrinkRate;
		}
		
		if(m_CurrentIdleState == IdleStatus.Cohesion && m_Child.transform.localScale != Vector3.one && m_Child.transform.localScale.y < 1f)
		{
			m_Child.transform.localScale -= m_ShrinkRate;
		}
	}
	
	public override void FixedExecute()
	{  
		Vector2 Acceleration = Vector2.zero;
		
		//If the current idle status is cohesioning and the enemy child cell had not enter the enemy main cell, continue seek towards the enemy main cell

		if(!m_ecFSM.m_bHitWall)
		{
			if(m_ecFSM.IsHittingSideWalls())
			{
				m_ecFSM.m_bHitWall = true;
				m_ChildRB.velocity = Vector2.zero;
			}
			else if(m_CurrentIdleState == IdleStatus.Cohesion && !HasChildEnterMain(m_Child))
			{
				Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,20f);
			}
			else if(m_CurrentIdleState == IdleStatus.Seperate && !m_ecFSM.IsHittingSideWalls())
			{
				Acceleration += m_SeperateDirection.normalized * m_fIdleScale * 1.2f;
				Acceleration += m_MainRB.velocity;
				Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours());
			}
		}
		else
		{
			if(!HasChildEnterMain(m_Child))
			{
				m_ChildRB.drag = 2.5f;
				Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,10f);
			}
			else
			{
				m_ChildRB.velocity = m_MainRB.velocity;
			}
		}

		//Clamp the acceleration of the enemy child cell to a specific maximum of magnitude and add that acceleration as a force on to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,m_fMaxMagnitude);
		m_ChildRB.AddForce(Acceleration);
		
		//Rotate the enemy child cell according to the specific direction of velocity it is enforced on
		if(!m_ecFSM.IsHittingSideWalls()){m_ecFSM.RotateToHeading();}
	}
	
	public override void Exit()
	{
		//Return that specific seperation direction back to the direction database so it can be used for another child cell
		m_Child.transform.localScale = Vector3.one;
		DirectionDatabase.Instance.Return(m_SeperateDirection);
		m_nIdleCount--;
		ECTracker.s_Instance.IdleCells.Remove(m_ecFSM);
	}
	
	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagNeighbours()
	{
		m_Neighbours.Clear();
		m_Collisions = Physics2D.OverlapCircleAll(m_Child.transform.position,m_fSpreadRange,Constants.s_onlyEnemeyChildLayer);//Change this value to how spread out the spreading is
			
		for(int i = 0; i < m_Collisions.Length; i++)
		{
			if(m_Collisions[i].gameObject != m_Child && m_Collisions[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
			{
				m_Neighbours.Add(m_Collisions[i].gameObject);
			}
		}
		
		return m_Neighbours;
	}
	
	//A function that return a boolean on whether that specific child cell had entered the enemy main cell
	public static bool HasChildEnterMain(GameObject _Child)
	{
		return (Utility.Distance(_Child.transform.position,m_EMTransform.position) <= m_EMBounds.size.x/7f) ? true : false;
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
				ECList[i].m_bHitWall = false;
				ECList[i].GetComponent<Rigidbody2D>().drag = 0f;
			}
		}
	}
	
	public static void ImmediateCohesion()
	{
		m_CurrentIdleState = IdleStatus.Cohesion;
		m_fPreviousStatusTime = Time.time;
	}
	
	public static void ResetStatics()
	{
		m_CurrentIdleState = IdleStatus.None;
	}
}

