using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeMState : IECState {

	//A float to dictate the maximum acceleration that the Enemy Child can undertake
	private static int m_nCurrentTargetIndex;
	private static bool m_bReachPosition;
	private static float m_fMaxAcceleration;
	private static float m_fSpreadRange;
	
	//A GameObject reference that store the player main cell
	private static Point m_CurrentTargetPoint;
	private static GameObject m_PlayerMain;
	private static List<Point> m_PathToTarget;
	
	private static Vector2 m_EndPos;
	private static Vector3 m_ShrinkRate;
	
	private bool m_bSqueezeToggle;
	private bool m_bSqueezeDone;

	public ECChargeMState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		m_PlayerMain = m_ecFSM.m_PMain;
		
		m_PathToTarget = null;
		m_CurrentTargetPoint = null;
		m_nCurrentTargetIndex = 0;
		
		m_fSpreadRange = m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 1.75f;
		m_ShrinkRate = new Vector3(-0.4f, 0.4f, 0.0f);
		
		m_fMaxAcceleration = 25f;
		m_EndPos = Vector2.zero;
	}
	
	public override void Enter()
	{
		//Reset the velocity of enemy child cell
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;
		m_ecFSM.rigidbody2D.drag = 2.6f;
		m_bReachPosition = false;
		m_bSqueezeDone = false;
		m_bSqueezeToggle = false;
		ECTracker.s_Instance.ChargeMainCells.Add(m_ecFSM);
	}
	
	public override void Execute()
	{
		if(!m_bSqueezeDone)
		{
			//use a boolean to track the squeeze corountine had been activated
			// activated the corountine then toggle the boolean, toggle the boolean again in the boolean once squeeze done
			//if the squeeze corountine is done, toggle squeeze done
			if(!m_bSqueezeToggle)
			{
				m_ecFSM.StartChildCorountine(SqueezeBeforeCharge(m_Main.transform.position));
				m_bSqueezeToggle = true;
			}
		}
		else if(m_bSqueezeDone && m_ecFSM.HitBottomOfScreen())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0.0f);
		}
		
		if(m_CurrentTargetPoint != null && m_Child.transform.position.y < m_CurrentTargetPoint.Position.y && m_nCurrentTargetIndex + 1 < m_PathToTarget.Count)
		{
			m_nCurrentTargetIndex++;
			m_CurrentTargetPoint = m_PathToTarget[m_nCurrentTargetIndex];
		}
	}
	
	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		//remove the gather movement, once squeeze done, perform the a star search

		//once squeeze done and a star search done, start move along the a star path (no more center based movement) and extend the Y scale of the child while moving
		if(!m_bReachPosition && m_bSqueezeDone && m_PathToTarget != null && HasCellReachTargetPos(m_Child.transform.position,m_PathToTarget[m_PathToTarget.Count - 1].Position))
		{
			m_bReachPosition = true;
			m_EndPos = new Vector2(m_PlayerMain.transform.position.x, -10f);
		}
		else if(!m_bReachPosition && m_bSqueezeDone && m_PathToTarget != null && !HasCellReachTargetPos(m_Child.transform.position,m_CurrentTargetPoint.Position))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_CurrentTargetPoint.Position,25f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 20f;
			if(m_Child.transform.localScale.y < 1f && m_Child.transform.localScale.x > 0.5f){m_Child.transform.localScale += m_ShrinkRate;}
		}
		else if(!m_bReachPosition && m_bSqueezeDone && m_nCurrentTargetIndex + 1 < m_PathToTarget.Count)
		{
			m_nCurrentTargetIndex++;
			m_CurrentTargetPoint = m_PathToTarget[m_nCurrentTargetIndex];
		}
		else if(m_bReachPosition && m_bSqueezeDone)
		{
			Acceleration += SteeringBehavior.Seek(m_Child,new Vector2(m_Child.transform.position.x,m_EndPos.y),10f);
		}
	
		//Clamp the acceleration of the enemy child cell and add that acceleration to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,m_fMaxAcceleration);
		m_ecFSM.rigidbody2D.AddForce(Acceleration,ForceMode2D.Force);
		
		//Rotate the enemy child to the direction of the velocity
		m_ecFSM.RotateToHeading();
	}
	
	public override void Exit()
	{
		//Reset the velocity and the force acting on the enemy child cell
		m_ecFSM.rigidbody2D.drag = 0f;
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;
		m_Child.transform.localScale = Vector3.one;

		if(ECTracker.s_Instance.ChargeMainCells.Count <= 1)
		{
			m_bSqueezeDone = false;
			m_bSqueezeToggle = false;
			m_bReachPosition = false;
			m_PathToTarget = null;
		}
		
		ECTracker.s_Instance.ChargeMainCells.Remove(m_ecFSM);
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _CellPos,Vector2 _TargetPos)
	{
		return (Utility.Distance(_CellPos, _TargetPos) <= 0.4f) ? true : false;
	}
	
	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();
		
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_fSpreadRange,Constants.s_onlyEnemeyChildLayer);
		
		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.ChargeMain)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}
		
		return Neighbours;
	}
	
	private EnemyChildFSM GetClosestChargerToPMain()
	{
		List<EnemyChildFSM> Chargers = ECTracker.s_Instance.ChargeMainCells;
		int ChargeIndex = 0;
		float ClosestDist = Mathf.Infinity;
		
		for(int i = 0; i < Chargers.Count; i++)
		{
			if(Utility.Distance(Chargers[i].transform.position,m_PlayerMain.transform.position) < ClosestDist)
			{
				ChargeIndex = i;
				ClosestDist = Utility.Distance(Chargers[i].transform.position,m_PlayerMain.transform.position);
			}
		}
		
		return Chargers[ChargeIndex];
	}
	
	private IEnumerator SqueezeBeforeCharge(Vector2 _TargetPos)
	{
		//The child cell will retreat slightly back before charging 
		m_ecFSM.rigidbody2D.velocity = new Vector2(Random.Range(-0.05f,0.05f),2.5f);
		
		Vector3 ShrinkScale = new Vector3(0f,-0.1f,0f);
		Vector3 ExpandScale = new Vector3(0.1f,0f,0f);
		
		while(m_Child.transform.localScale.y > 0.5f)
		{
			Vector2 Heading = new Vector2(m_Child.transform.position.x - _TargetPos.x,m_Child.transform.position.y - _TargetPos.y).normalized;
			float Rotation = -Mathf.Atan2(Heading.x, Heading.y) * Mathf.Rad2Deg;
			m_ecFSM.rigidbody2D.MoveRotation(Rotation);
		
			m_Child.transform.localScale += ShrinkScale;
			m_Child.transform.localScale += ExpandScale;
			yield return new WaitForSeconds(0.25f);//0.0005
		}
		
		if(m_PathToTarget == null)
		{
			PathQuery.Instance.AStarSearch(GetClosestChargerToPMain().transform.position,m_ecFSM.m_PMain.transform.position,false);
			m_PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.High);
			m_nCurrentTargetIndex = 0;
			m_CurrentTargetPoint = m_PathToTarget[m_nCurrentTargetIndex];
		}

		m_bSqueezeDone = true;

		AudioManager.PlayECSoundEffect(EnemyChildSFX.CellChargeTowards, m_ecFSM.Audio);
	}
}

