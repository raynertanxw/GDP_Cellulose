using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECMineState : IECState {
	
	private bool bReachTarget;
	private float bSeperateRate;
	private float bGatherRate;
	private float fMaxAcceleration;
	
	private PositionType CurrentPositionType;
	private GameObject Target;
	private Vector2 TargetLandminePos;
	private List<Point> PathToTarget;
	
	private bool GatherTogether;
	private static Point SpreadPoint;
	private bool ReachSpreadPoint;
	private static Point GeneralTargetPoint;
	private static int GeneralTargetIndex;
	
	private float fSpeed;
	private float fSeperateInterval;
	private int ECMineNearby;
	private Spread CurrentSpreadness;
	
	private GameObject m_LeftWall;
	private GameObject m_RightWall;
	
	private bool bExploding;
	private bool bExpanding;
	private Vector2 ExpansionLimit;
	private Vector2 ShrinkLimit;
	private float fExpansionSpeed;
	
	private enum Spread{Empty,Tight, Wide};
	
	//Constructor
	public ECMineState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		m_LeftWall = GameObject.Find("Left Wall");
		m_RightWall = GameObject.Find("Right Wall");
		fSpeed = 1.5f;
		fMaxAcceleration = 30f;
		ExpansionLimit = new Vector2(0.6f,0.6f);
		ShrinkLimit = new Vector2(0.3f,0.3f);
		fExpansionSpeed = 0.025f;
		bExpanding = true;
	}
	
	public override void Enter()
	{
		GatherTogether = false;
		bExploding = false;
		bReachTarget = false;
		bSeperateRate = 0f;
		bGatherRate = 1f;
		ECMineNearby = 0;
		CurrentSpreadness = Spread.Empty;
		
		m_Main.GetComponent<Rigidbody2D>().drag = 2.6f;
	}
	
	public override void Execute()
	{
		if(!GatherTogether)
		{
			ECMineNearby = GetNearbyECMineAmount();
			if(ECMineNearby == GetLandmines().Count)
			{
				CurrentPositionType = DeterminePositionType();
				
				Target = PositionQuery.Instance.GetLandmineTarget(CurrentPositionType,m_Child);
				TargetLandminePos = PositionQuery.Instance.GetLandminePos(DetermineRangeValue(),CurrentPositionType,m_Child);
				Utility.DrawCross(TargetLandminePos,Color.red,0.5f);
				PathQuery.Instance.AStarSearch(m_Child.transform.position,TargetLandminePos,false);
				PathToTarget = PathQuery.Instance.GetPathToTarget(DetermineDirectness(Target));
				GeneralTargetIndex = 0;
				GeneralTargetPoint = PathToTarget[GeneralTargetIndex];
				
				ReachSpreadPoint = false;
				SpreadPoint = PathQuery.Instance.ReturnVertSequenceStartPoint(PathToTarget);
				
				fSeperateInterval = CalculateSpreadRate(GetCenterOfMines(GetLandmines()),Target);
				GatherTogether = true;
			}
		}
		else if(GatherTogether && HasCenterReachTarget(GetCenterOfMines(GetLandmines()),SpreadPoint.Position))
		{
			ReachSpreadPoint = true;
		}
		else if(!bExploding && (HasReachedExplosiveRange() || IsMineReachingPlayer()))
		{
			bExploding = true;
			m_Child.GetComponent<Rigidbody2D>().drag = 0f;
			m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(m_Child.GetComponent<Rigidbody2D>().velocity.x * 0.75f,m_Child.GetComponent<Rigidbody2D>().velocity.y);
			m_ecFSM.StartChildCorountine(ExplodeCorountine());
		}
		
		if(IsCellReachingWall())
		{
			m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,m_Child.GetComponent<Rigidbody2D>().velocity.y);;
		}
		
		if(bExploding)
		{
			ExplodingGrowShrink();
		}
	}
	
	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;
	
		if(!GatherTogether)
		{
			//Acceleration += SteeringBehavior.GatherAllECSameState(m_Child,ECState.Landmine, 24f);
			m_Child.GetComponent<Rigidbody2D>().drag = 3f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,5f);
		}
		
		
		else if(GatherTogether && !bExploding && (CurrentPositionType == PositionType.Aggressive || CurrentPositionType == PositionType.Defensive))
		{
			Vector2 CrowdCenter = GetCenterOfMines(GetLandmines());
			
			if(!HasCenterReachTarget(CrowdCenter,GeneralTargetPoint.Position) && bReachTarget == false)
			{
				m_Child.GetComponent<Rigidbody2D>().drag = 5f;
				Acceleration += SteeringBehavior.CrowdAlignment(CrowdCenter,GeneralTargetPoint,12f);
				Acceleration += SteeringBehavior.Seperation(m_Child,TagLandmines(Spread.Tight)) * 5.5f;
			}
			else if(HasCenterReachTarget(CrowdCenter,GeneralTargetPoint.Position) && bReachTarget == false && GeneralTargetIndex + 1 < PathToTarget.Count)
			{
				GeneralTargetIndex++;
				GeneralTargetPoint = PathToTarget[GeneralTargetIndex];
			}
		}
		else if(GatherTogether && !bExploding && CurrentPositionType == PositionType.Neutral)
		{
			Vector2 CrowdCenter = GetCenterOfMines(GetLandmines());
			
			if(CurrentSpreadness == Spread.Empty)
			{
				CurrentSpreadness = DetermineSpreadness();
			}
			if(ReachSpreadPoint == true)
			{
				m_ecFSM.RandomRotation(0.75f);
			}
			
			if(!HasCenterReachTarget(CrowdCenter,GeneralTargetPoint.Position) && bReachTarget == false)
			{
				if(CurrentSpreadness == Spread.Tight)
				{
					m_Child.GetComponent<Rigidbody2D>().drag = 5f;
					Acceleration += SteeringBehavior.CrowdAlignment(CrowdCenter,GeneralTargetPoint,12f);
					Acceleration += SteeringBehavior.Seperation(m_Child,TagLandmines(Spread.Tight)) * 7.5f;
				}
				else if(CurrentSpreadness == Spread.Wide)
				{
					m_Child.GetComponent<Rigidbody2D>().drag = 5f;
					Acceleration += SteeringBehavior.CrowdAlignment(CrowdCenter,GeneralTargetPoint,12f);
					Acceleration += SteeringBehavior.Seperation(m_Child,TagLandmines(Spread.Wide)) * 9f;//TagLandmines(Spread.Wide));
				}
			}
			else if(HasCenterReachTarget(CrowdCenter,GeneralTargetPoint.Position) && bReachTarget == false && GeneralTargetIndex + 1 < PathToTarget.Count)
			{
				GeneralTargetIndex++;
				GeneralTargetPoint = PathToTarget[GeneralTargetIndex];
			}
		}
		
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);
		if(!bExploding)
		{
			m_ecFSM.RotateToHeading();
		}
		else if(bExploding)
		{
			m_ecFSM.RandomRotation(0.75f);
		}
	}
	
	public override void Exit()
	{
		m_Child.GetComponent<Rigidbody2D>().drag = 0f;
		m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}
	
	private List<GameObject> GetLandmines()
	{
		GameObject[] EnemyChild = GameObject.FindGameObjectsWithTag(Constants.s_strEnemyChildTag);
		List<GameObject> Landmines = new List<GameObject>();
		foreach(GameObject Child in EnemyChild)
		{
			if(Child.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
			{
				Landmines.Add(Child);
			}
		}
		return Landmines;
	}
	
	private Spread DetermineSpreadness()
	{
		List<GameObject> NodeList = new List<GameObject>();
		NodeList.Add(GameObject.Find("Node_Left"));
		NodeList.Add(GameObject.Find("Node_Right"));
		
		for(int i = 0; i < NodeList.Count; i++)
		{
			if(NodeList[i].GetComponent<Node_Manager>().GetNodeChildList().Count > 3)
			{
				return Spread.Wide;
			}
		}
		
		return Spread.Tight;
	}
	
	private Directness DetermineDirectness(GameObject _Target)
	{
		if(Mathf.Abs(m_Child.transform.position.x - _Target.transform.position.x) > 3.5f)
		{
			return Directness.Low;
		}
		
		if(Mathf.Abs(m_Child.transform.position.x - _Target.transform.position.x) > 1.9f)
		{
			return Directness.Low;
		}
		
		return Directness.High;
	}
	
	private float CalculateSpreadRate(Vector2 _Center, GameObject _Target)
	{
		//From screen center to player main: SpreadRate = 0.1f
		float ScreenCenterToTarget = Vector2.Distance(new Vector2(0f,0f), _Target.transform.position);
		float CenterOfMassToTarget = Vector2.Distance(_Center,_Target.transform.position);
		return (CenterOfMassToTarget/ScreenCenterToTarget) * 0.20f;
	}
	
	private RangeValue DetermineRangeValue()
	{
		float fEMainAggressiveness = m_Main.GetComponent<EnemyMainFSM>().CurrentAggressiveness;
		
		if(fEMainAggressiveness >= 12)
		{
			return RangeValue.Max;
		}
		else if(fEMainAggressiveness <= 6)
		{
			return RangeValue.Min;
		}
		return RangeValue.None;
	}
	
	//a function that return the type of position the landmine it should take based on the aggressiveness of 
	//the enemy main cell
	private PositionType DeterminePositionType()
	{
		float fEMainAggressiveness = m_Main.GetComponent<EnemyMainFSM>().CurrentAggressiveness;
		
		if(fEMainAggressiveness >= 12)
		{
			return PositionType.Aggressive;
		}
		else if(fEMainAggressiveness <= 6)
		{
			return PositionType.Defensive;
		}
		return PositionType.Neutral;
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTarget (Vector2 _TargetPos)
	{
		if (Vector2.Distance(m_Child.transform.position, _TargetPos) <= 0.25f)
		{
			return true;
		}
		return false;
	}
	
	private bool HasCenterReachTarget(Vector2 _Center, Vector2 _TargetPos)
	{
		if (Vector2.Distance(_Center, _TargetPos) <= 0.4f)
		{
			return true;
		}
		return false;
	}
	
	private bool HasAllCellsReachTarget (Vector2 _TargetPos)
	{
		List<GameObject> mines = GetLandmines();
		foreach(GameObject mine in mines)
		{
			if(!HasCellReachTarget(_TargetPos))
			{
				return false;
			}
		}
		return true;
	}
	
	//a function that return a boolean on whether the enemy child cell is collding with any player cell
	private bool IsCollidingWithPlayerCell()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, 1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		
		if(m_SurroundingObjects.Length <= 0)
		{
			return false;
		}
		
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			if(m_SurroundingObjects[i] != null && (m_SurroundingObjects[i].tag == Constants.s_strPlayerChildTag || m_SurroundingObjects[i].tag == Constants.s_strPlayerTag))
			{
				return true;
			}
		}
		
		return false;
	}
	
	//a function that return a boolean on whether the enemy child cell had collided with any player cell
	private bool HasCollidedWithPlayerCells()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, 0.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		if(m_SurroundingObjects.Length <= 0)
		{
			return false;
		}
		
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			if(m_SurroundingObjects[i] != null && (m_SurroundingObjects[i].tag == Constants.s_strPlayerChildTag || m_SurroundingObjects[i].tag == Constants.s_strPlayerTag))
			{
				return true;
			}
		}
		
		return false;
	}
	
	private Vector2 GetCenterOfMines(List<GameObject> _Mines)
	{
		Vector2 Center = new Vector2(0f,0f);
		foreach(GameObject mine in _Mines)
		{
			Center.x += mine.transform.position.x;
			Center.y += mine.transform.position.y;
		}
		Center /= _Mines.Count;
		return Center;
	}
	
	private bool IsTimetoSpread()
	{
		float EMtoPM = Vector2.Distance(m_Main.transform.position, m_ecFSM.m_PMain.transform.position);
		float TargetDistance = 0.95f * EMtoPM;
		Vector2 CenterOfMass = GetCenterOfMines(GetLandmines());
		if(Vector2.Distance(CenterOfMass,m_ecFSM.m_PMain.transform.position) < TargetDistance)
		{
			return true;
		}
		return false;
	}
	
	private List<GameObject> TagLandmines(Spread _Spreadness)
	{
		List<GameObject> NeighbouringLandmine = new List<GameObject>();
		
		if(_Spreadness == Spread.Tight)
		{
			Collider2D[] m_NeighbourChilds = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x);//Physics2D.OverlapAreaAll(m_SpreadTopLeft,m_SpreadBotRight,LayerMask.NameToLayer ("EnemyChild"));
			
			for(int i = 0; i < m_NeighbourChilds.Length; i++)
			{
				if(m_NeighbourChilds[i] != null && m_NeighbourChilds[i] != m_Child.GetComponent<BoxCollider2D>() && m_NeighbourChilds[i].tag == Constants.s_strEnemyChildTag && m_NeighbourChilds[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
				{
					NeighbouringLandmine.Add(m_NeighbourChilds[i].gameObject);
				}
			}
		}
		else if(_Spreadness == Spread.Wide)
		{
			Collider2D[] m_NeighbourChilds = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 2.75f);//Physics2D.OverlapAreaAll(m_SpreadTopLeft,m_SpreadBotRight,LayerMask.NameToLayer ("EnemyChild"));
			
			for(int i = 0; i < m_NeighbourChilds.Length; i++)
			{
				if(m_NeighbourChilds[i] != null && m_NeighbourChilds[i] != m_Child.GetComponent<BoxCollider2D>() && m_NeighbourChilds[i].tag == Constants.s_strEnemyChildTag && m_NeighbourChilds[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
				{
					NeighbouringLandmine.Add(m_NeighbourChilds[i].gameObject);
					/*if(m_Child.name.Contains("22"))
					{
						Debug.Log(m_NeighbourChilds[i].gameObject);
					}*/
				}
			}
		}
		
		return NeighbouringLandmine;
	}
	
	private bool IsSpreadNearCompletion(GameObject[] Landmines)
	{
		for(int a = 0; a < Landmines.Length; a++)
		{
			Collider2D[] m_NeighbourChilds = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 2.75f);//Physics2D.OverlapAreaAll(m_SpreadTopLeft,m_SpreadBotRight,LayerMask.NameToLayer ("EnemyChild"));
			int NeighbourCount = 0;
			
			for(int b = 0; b < m_NeighbourChilds.Length; b++)
			{
				if(m_NeighbourChilds[b] != null && m_NeighbourChilds[b] != m_Child.GetComponent<BoxCollider2D>() && m_NeighbourChilds[b].tag == Constants.s_strEnemyChildTag && m_NeighbourChilds[b].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
				{
					NeighbourCount++;
				}
			}
			
			if(NeighbourCount >= 2)
			{
				return false;
			}
		}
		return true;
	}
	
	private void EnforceNonPenetrationConstraint(GameObject _Agent, GameObject[] Landmines)
	{
		//Debug.Log("Enforce");
		for(int i = 0; i < Landmines.Length; i++)
		{
			if(Landmines[i] == _Agent)
			{
				continue;
			}
			
			//distance
			Vector2 difference = new Vector2(_Agent.transform.position.x - Landmines[i].transform.position.x,_Agent.transform.position.y - Landmines[i].transform.position.y);
			float distance = difference.magnitude;
			
			float OverlapDist = (_Agent.GetComponent<SpriteRenderer>().bounds.size.x/2 + Landmines[i].GetComponent<SpriteRenderer>().bounds.size.x/2) - distance;
			
			if(OverlapDist >= 0f)
			{
				Vector2 targetPos = new Vector2(_Agent.transform.position.x + difference.x/distance * OverlapDist, _Agent.transform.position.y + difference.y/distance * OverlapDist);
				m_Child.transform.position = targetPos;
			}
		}
	}
	
	private int GetNearbyECMineAmount()
	{
		Collider2D[] Collisions = Physics2D.OverlapCircleAll(m_Child.transform.position,m_Child.GetComponent<SpriteRenderer>().bounds.size.x/8);
		int ECMineCount = 0;
		
		for(int i = 0; i < Collisions.Length; i++)
		{
			if(Collisions[i].tag == Constants.s_strEnemyChildTag && Collisions[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
			{
				ECMineCount++;
			}
		}
		
		return ECMineCount;
	}
	
	private bool HasReachedExplosiveRange()
	{
		if(m_Child.transform.position.y < -2f)
		{
			return true;
		}
		return false;
	}
	
	private bool IsMineReachingPlayer()
	{
		Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 1.5f);
		foreach(Collider2D NearbyObject in NearbyObjects)
		{
			if(NearbyObject.tag == Constants.s_strPlayerChildTag && NearbyObject.GetComponent<PlayerChildFSM>().GetCurrentState() != PCState.Dead || NearbyObject.tag == Constants.s_strPlayerTag)
			{
				return true;
			}
		}
		return false;
	}
	
	private void CallAllMinePassThroughDeath()
	{
		List<GameObject> Landmines = GetLandmines();
		foreach(GameObject Landmine in Landmines)
		{
			Landmine.GetComponent<EnemyChildFSM>().StartChildCorountine(Landmine.GetComponent<EnemyChildFSM>().PassThroughDeath());
		}
	}
	
	private bool IsCellReachingWall()
	{
		float CellRadius = m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2;
		float WallWidth = m_LeftWall.GetComponent<SpriteRenderer>().bounds.size.x/2;
		
		if(m_Child.transform.position.x < m_LeftWall.transform.position.x + CellRadius + WallWidth || m_Child.transform.position.x > m_RightWall.transform.position.x - CellRadius - WallWidth)
		{
			return true;
		}
		
		return false;
	}
	
	private Vector2 GetAwayFromWall()
	{
		float DistToLeftWall = Vector2.Distance(m_Child.transform.position,m_LeftWall.transform.position);
		float DistToRightWall = Vector2.Distance(m_Child.transform.position,m_RightWall.transform.position);
		
		GameObject ClosestWall = DistToLeftWall <= DistToRightWall ? m_LeftWall : m_RightWall;
		
		if(ClosestWall.transform.position.x > 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x > 0)
		{
			return new Vector2(0f, m_Child.GetComponent<Rigidbody2D>().velocity.y);
		}
		else if(ClosestWall.transform.position.x > 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x < 0)
		{
			return m_Main.GetComponent<Rigidbody2D>().velocity;
		}
		else if(ClosestWall.transform.position.x < 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x < 0)
		{
			return new Vector2(0f, m_Child.GetComponent<Rigidbody2D>().velocity.y);
		}
		else if(ClosestWall.transform.position.x < 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x > 0)
		{
			return m_Main.GetComponent<Rigidbody2D>().velocity;
		}
		
		return Vector2.zero;
	}
	
	private void ExplodingGrowShrink()
	{
		Vector2 CurrentScale = m_Child.transform.localScale;
		
		if(m_Child.transform.localScale.x >= ExpansionLimit.x && m_Child.transform.localScale.y >= ExpansionLimit.y)
		{
			bExpanding = false;
		}
		else if(m_Child.transform.localScale.x <= ShrinkLimit.x && m_Child.transform.localScale.y <= ShrinkLimit.y)
		{
			bExpanding = true;
		}
		
		if(bExpanding)
		{
			CurrentScale += new Vector2(fExpansionSpeed,fExpansionSpeed);
			m_Child.transform.localScale = CurrentScale;
		}
		else if(!bExpanding)
		{
			CurrentScale -= new Vector2(fExpansionSpeed,fExpansionSpeed);
			m_Child.transform.localScale = CurrentScale;
		}
	}
	
	//a function for any potential sound effects or visual effects to be played 
	//during explosions
	private void ExplodeSetup()
	{

	}
	
	//Go through all the surround cells, destroy any player child cells and damaing the player main cell if in range
	private void ExplodeDestroy()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position,1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			//if the player child cell is within the exploding range, kill the player child
			if(m_SurroundingObjects[i] != null && m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerChildTag)
			{
				m_SurroundingObjects[i].GetComponent<PlayerChildFSM>().DeferredChangeState(PCState.Dead);
			}
			//if the player main cell is within the exploding range, damage the player main
			else if(m_SurroundingObjects[i] != null && m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerTag)
			{
				m_SurroundingObjects[i].GetComponent<PlayerMain>().HurtPlayerMain();
			}
		}
		
		//After all the damaging and killing is done, transition the enemy child to a dead state
		MessageDispatcher.Instance.DispatchMessage(m_Child, m_Child,MessageType.Dead,0);
	}
	
	//a corountine for the cell explosion
	IEnumerator ExplodeCorountine()
	{
		//play explode sound/animation whatever
		ExplodeSetup();
		
		yield return new WaitForSeconds(1.5f);
		
		ExplodeDestroy();
	}
}
