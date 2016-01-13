using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeMState : IECState {

	//A float to dictate the maximum acceleration that the Enemy Child can undertake
	private float fMaxAcceleration;
	
	//A GameObject reference that store the player main cell
	private GameObject m_PlayerMain;
	
	//A list of Points that is used to store a sequence of points which are then used as a path for the enemy child cell to travel to the player main cell
	private static List<Point> PathToTarget;
	
	//A Point variable that store the current Point that the enemy child cell is traveling towards in the path
	private static Point CurrentTargetPoint;
	
	//An integer variable that store the current Point index that the enemy child cell is traveling towards
	private static int CurrentTargetIndex;

	private static Vector2 CellsCenter;

	private static bool bGatherTogether;

	private int ChargeNearby;

	private static bool bReachPosition;

	private static Vector2 EndPos;

	//Constructor
	public ECChargeMState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		m_PlayerMain = m_ecFSM.m_PMain;
		
		PathToTarget = null;
		fMaxAcceleration = 10f;
	}
	
	public override void Enter()
	{
		//Reset the velocity of enemy child cell
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
		m_Child.GetComponent<Rigidbody2D>().drag = 2.6f;
		bReachPosition = false;
	}
	
	public override void Execute()
	{
		CellsCenter = GetCenterOfAttackers();

		if(!bGatherTogether)
		{
			ChargeNearby = GetNearbyECChargeAmount();

			if(PathToTarget == null && ChargeNearby == GetEnemyChargingCells().Count)
			{
				//Calculate a path for the enemy child cell to travel to the enemy main cell
				PathQuery.Instance.AStarSearch(m_Child.transform.position,m_ecFSM.m_PMain.transform.position,false);
				PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.High);
				CurrentTargetIndex = 0;
				CurrentTargetPoint = PathToTarget[CurrentTargetIndex];

				bGatherTogether = true;
			}
		}
		else if(bGatherTogether && m_ecFSM.HitBottomOfScreen())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0.0f);
		}
	}
	
	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		if(!bGatherTogether && PathToTarget == null)
		{
			m_Child.GetComponent<Rigidbody2D>().drag = 3f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,7.5f);
		}
		else if(bGatherTogether && PathToTarget == null)
		{
			PathQuery.Instance.AStarSearch(m_Child.transform.position,m_Main.transform.position,false);
			PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.High);
			CurrentTargetIndex = 0;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		}

		if(!bReachPosition && bGatherTogether && PathToTarget != null && HasCenterReachTarget(CellsCenter,PathToTarget[PathToTarget.Count - 1].Position))
		{
			bReachPosition = true;
			EndPos = new Vector2(m_PlayerMain.transform.position.x, -10f);
		}
		else if(!bReachPosition && bGatherTogether && PathToTarget != null && !HasCenterReachTarget(CellsCenter,CurrentTargetPoint.Position))
		{
			Acceleration += SteeringBehavior.CrowdAlignment(CellsCenter,CurrentTargetPoint,25f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 20f;
		}
		else if(!bReachPosition && bGatherTogether && CurrentTargetIndex + 1 < PathToTarget.Count)
		{
			CurrentTargetIndex++;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		}
		else if(bReachPosition && bGatherTogether)
		{
			Acceleration += SteeringBehavior.Seek(m_Child,new Vector2(m_Child.transform.position.x,EndPos.y),10f);
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

		if(GetEnemyChargingCells().Count <= 1)
		{
			bGatherTogether = false;
			bReachPosition = false;
			PathToTarget = null;
		}
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= 0.4f)
		{
			return true;
		}
		return false;
	}
	
	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();
		
		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2);
		
		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.tag == Constants.s_strEnemyChildTag && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.ChargeMain)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}
		
		return Neighbours;
	}

	private Vector2 GetCenterOfAttackers()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		Vector2 Center = new Vector2(0f,0f);
		int AttackerCount = 0;

		foreach(EnemyChildFSM EC in ECList)
		{
			if(EC.CurrentStateEnum == ECState.ChargeMain)
			{
				Center.x += EC.transform.position.x;
				Center.y += EC.transform.position.y;
				AttackerCount++;
			}
		}
		Center /= AttackerCount;
		return Center;
	}

	private bool HasCenterReachTarget(Vector2 _Center, Vector2 _TargetPos)
	{
		if (Vector2.Distance(_Center, _TargetPos) <= 0.4f)
		{
			return true;
		}
		return false;
	}

	private int GetNearbyECChargeAmount()
	{
		Collider2D[] Collisions = Physics2D.OverlapCircleAll(m_Child.transform.position,m_Child.GetComponent<SpriteRenderer>().bounds.size.x/4);
		int ECChargeCount = 0;
		
		for(int i = 0; i < Collisions.Length; i++)
		{
			if(Collisions[i].tag == Constants.s_strEnemyChildTag && Collisions[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.ChargeMain)
			{
				ECChargeCount++;
			}
		}
		
		return ECChargeCount;
	}

	private List<GameObject> GetEnemyChargingCells()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		List<GameObject> ECChargers = new List<GameObject>();
		foreach(EnemyChildFSM EC in ECList)
		{
			if(EC.CurrentStateEnum == ECState.ChargeMain)
			{
				ECChargers.Add(EC.gameObject);
			}
		}
		return ECChargers;
	}
}

