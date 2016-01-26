using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECMineState : IECState {

	//Two booleans that state whether the landmines had reach the last point on the calculated path and whether it reach the point at which the landmines should spread out
	private static bool bReachTarget;
	private bool ReachSpreadPoint;

	//Three booleans that state the different state of the landmine (Is exploding, Had exploded, Is expanding its size)
	private bool bExploding;
	private bool bExploded;
	private bool bExpanding;
	private bool bExplodeCorountineStart;

	//A float for the maximum amount of acceleration that an EC can take in a landmine states
	private static float fMaxAcceleration;

	//A float that dictate the speed at which the enemy child cell expand and shrink as it reaches the target
	private static float fExpansionSpeed;

	//A float that dicate how spread out the landmines will be from each other
	private float fSeperateInterval;

	//An integer thatdictate how many landmines are nearby
	private int ECMineNearby;

	private static float fExplosiveRange;
	private static float fKillRange;

	private static bool GatherTogether;
	private static int GeneralTargetIndex;
	private static Point SpreadPoint;
	private static Point GeneralTargetPoint;

	private Vector2 TargetLandminePos;
	private static Vector2 ExpansionLimit;
	private static Vector2 ShrinkLimit;
	private Spread CurrentSpreadness;
	private static GameObject Target;
	private static PositionType CurrentPositionType;
	private static List<Point> PathToTarget;

	private bool ExpandContractStart;
	private Animate Animator;

	private Point CurrentTargetPoint;
	private int CurrentTargetIndex;

	private enum Spread{Empty,Tight, Wide};

	//Constructor
	public ECMineState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;

		fMaxAcceleration = 40f;
		ExpansionLimit = new Vector2(1.7f,1.7f);
		ShrinkLimit = new Vector2(0.8f,0.8f);
		fExpansionSpeed = 0.1f;
		bExpanding = true;

		fExplosiveRange = 4f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x;
		fKillRange = 0.75f * fExplosiveRange;
		GeneralTargetIndex = 0;
		SpreadPoint = null;
		GeneralTargetPoint = null;
		Target = null;
		CurrentPositionType = PositionType.Empty;
		PathToTarget = new List<Point>();

		CurrentTargetPoint = null;
		

		ExpandContractStart = false;
		Animator = new Animate(m_Child.transform);
	}

	public override void Enter()
	{
		if(m_ecFSM.m_AttackTarget != null){CheckIfEnoughCells();};
	
		GatherTogether = false;
		bExploding = false;
		bReachTarget = false;
		bExplodeCorountineStart = false;
		ECMineNearby = 0;
		CurrentSpreadness = Spread.Empty;

		m_Main.GetComponent<Rigidbody2D>().drag = 2.4f;
		ECTracker.Instance.LandmineCells.Add(m_ecFSM);
	}

	public override void Execute()
	{
		if(!GatherTogether)
		{
			ECMineNearby = GetNearbyECMineAmount();

			if(ECMineNearby == GetLandmines().Count)
			{
				CurrentPositionType = DeterminePositionType();

				Target = m_ecFSM.m_AttackTarget;
				if(Target == null)
				{
					Target = PositionQuery.Instance.GetLandmineTarget(CurrentPositionType,m_Child);
				}
				TargetLandminePos = PositionQuery.Instance.GetLandminePos(DetermineRangeValue(),CurrentPositionType,m_Child);
				PathQuery.Instance.AStarSearch(m_Child.transform.position,TargetLandminePos,false);
				PathToTarget = PathQuery.Instance.GetPathToTarget(DetermineDirectness(Target));
				CurrentTargetIndex = 0;
				CurrentTargetPoint = PathToTarget[CurrentTargetIndex];

				ReachSpreadPoint = false;
				SpreadPoint = PathQuery.Instance.ReturnVertSequenceStartPoint(PathToTarget);

				fSeperateInterval = CalculateSpreadRate(GetCenterOfMines(GetLandmines()),Target);
				GatherTogether = true;
				//Utility.DrawPath(PathToTarget,Color.red,0.1f);
			}
		}

		if(!bExploding && !bExplodeCorountineStart && IsMineReachingPlayer(Target))
		{
			bExploding = true;
			bExplodeCorountineStart = true;
			m_ecFSM.rigidbody2D.drag = 0f;
			m_ecFSM.rigidbody2D.velocity = new Vector2(m_ecFSM.rigidbody2D.velocity.x * 0.75f,m_ecFSM.rigidbody2D.velocity.y);
			m_ecFSM.StartChildCorountine(ExplodeCorountine());
		}

		if(IsCellReachingWall())
		{
			m_ecFSM.rigidbody2D.velocity = new Vector2(0f,m_ecFSM.rigidbody2D.velocity.y);
		}

		if(GatherTogether && HasCellReachTarget(PathToTarget[PathToTarget.Count - 1].Position))
		{
			bReachTarget = true;
			m_ecFSM.StopChildCorountine(ExplodeCorountine());
			m_ecFSM.StartChildCorountine(m_ecFSM.PassThroughDeath(1f));
		}

		if(GatherTogether && m_ecFSM.HitBottomOfScreen())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0.0f);
		}

		if(GatherTogether && !ExpandContractStart)
		{
			Animator.ExpandContract(15f,60,1.9f);//ExplodingGrowShrink();
			ExpandContractStart = true;
		}
	}

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		if(!GatherTogether)
		{
			m_ecFSM.rigidbody2D.drag = 3f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,7.5f);
		}
		else if(GatherTogether && PathToTarget == null)
		{
			Target = PositionQuery.Instance.GetLandmineTarget(CurrentPositionType,m_Child);
			TargetLandminePos = PositionQuery.Instance.GetLandminePos(DetermineRangeValue(),CurrentPositionType,m_Child);
			PathQuery.Instance.AStarSearch(m_Child.transform.position,TargetLandminePos,false);
			PathToTarget = PathQuery.Instance.GetPathToTarget(DetermineDirectness(Target));
			//Utility.DrawPath(PathToTarget,Color.red,0.1f);
		}


		if(GatherTogether && !bExploding && (CurrentPositionType == PositionType.Aggressive || CurrentPositionType == PositionType.Defensive) && bReachTarget == false)
		{
			Vector2 CrowdCenter = GetCenterOfMines(GetLandmines());

			if(CurrentTargetPoint == null)
			{
				CurrentTargetIndex = 0;
				CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
			}
			
			if(!HasCellReachTarget(CurrentTargetPoint.Position))
			{
				m_ecFSM.rigidbody2D.drag = 3f;
				Acceleration += SteeringBehavior.Seek(m_Child,CurrentTargetPoint.Position,9f);
				Acceleration += SteeringBehavior.Seperation(m_Child,TagLandmines(Spread.Wide)) * 9f;
			}
			else if((HasCellReachTarget(CurrentTargetPoint.Position)|| m_Child.transform.position.y < CurrentTargetPoint.Position.y) && CurrentTargetIndex + 1 < PathToTarget.Count)
			{
				CurrentTargetIndex++;
				CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
			}
		}
		else if(GatherTogether && !bExploding && CurrentPositionType == PositionType.Neutral && bReachTarget == false)
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
			
			if(CurrentTargetPoint == null)
			{
				CurrentTargetIndex = 0;
				CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
			}

			if(!HasCellReachTarget(CurrentTargetPoint.Position))
			{
				m_ecFSM.rigidbody2D.drag = 5f;
				Acceleration += SteeringBehavior.Seek(m_Child,CurrentTargetPoint.Position,12f);
				Acceleration += SteeringBehavior.Seperation(m_Child,TagLandmines(Spread.Wide)) * 9f;//TagLandmines(Spread.Wide));
			}
			else if((HasCellReachTarget(CurrentTargetPoint.Position) || (m_Child.transform.position.y < CurrentTargetPoint.Position.y)) && CurrentTargetIndex + 1 < PathToTarget.Count)
			{
				CurrentTargetIndex++;
				CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
			}
		}

		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.rigidbody2D.AddForce(Acceleration,ForceMode2D.Force);

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
		bExplodeCorountineStart = false;
		m_ecFSM.rigidbody2D.drag = 0f;
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;

		//if the landmine has not exploded and its going to die, it self-destruct instantly
		if(!bExploded)
		{
			MainCamera.CameraShake();
			//Utility.DrawCircleCross(m_Child.transform.position,fExplosiveRange,Color.green);
			//Utility.DrawCircleCross(m_Child.transform.position,fKillRange,Color.red);
			m_ecFSM.StartChildCorountine(ExplodeCorountine());//ExplodeDestroy();
		}

		m_Child.transform.localScale = Vector3.one;
		ECTracker.Instance.LandmineCells.Remove(m_ecFSM);
	}

	private List<GameObject> GetLandmines()
	{
		List<EnemyChildFSM> ECList = m_Main.GetComponent<EnemyMainFSM>().ECList;
		List<GameObject> Landmines = new List<GameObject>();
		for(int i = 0; i < ECList.Count; i++)
		{
			if(ECList[i].CurrentStateEnum == ECState.Landmine)
			{
				Landmines.Add(ECList[i].gameObject);
			}
		}
		return Landmines;
	}

	private Spread DetermineSpreadness()
	{
		if(Target.name.Contains("Captain"))
		{
			return Spread.Tight;
		}

		if(Target.GetComponent<Node_Manager>() != null && Target.GetComponent<Node_Manager>().activeChildCount > 6)
		{
			return Spread.Wide;
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
		float ScreenCenterToTarget = Utility.Distance(Vector2.zero, _Target.transform.position);
		float CenterOfMassToTarget = Utility.Distance(_Center,_Target.transform.position);
		return (CenterOfMassToTarget/ScreenCenterToTarget) * 0.2f;
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
		return Utility.Distance(m_Child.transform.position,_TargetPos) <= 0.4f ? true : false;
	}

	private bool HasCenterReachTarget(Vector2 _Center, Vector2 _TargetPos)
	{
		return (_Center.y < _TargetPos.y) ? true : false;
		//return Utility.Distance(_Center,_TargetPos) <= 0.4f ? true : false;
	}

	private bool HasAllCellsReachTarget (Vector2 _TargetPos)
	{
		List<GameObject> mines = GetLandmines();
		for(int i = 0; i < mines.Count; i++)
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
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, 1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x,Constants.s_onlyPlayerChildLayer);

		if(m_SurroundingObjects.Length <= 0)
		{
			return false;
		}

		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			if(m_SurroundingObjects[i] != null)
			{
				return true;
			}
		}

		return false;
	}

	//a function that return a boolean on whether the enemy child cell had collided with any player cell
	private bool HasCollidedWithPlayerCells()
	{
		//Check a ciruclar area around the enemy child cell for any player cells, if there is none, return false. If there is, check whether is it a player child cell.
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

	//A function that calculate the center position of all the landmines
	private Vector2 GetCenterOfMines(List<GameObject> _Mines)
	{
		//Loop through all the landmines, adding all of their position and dividing them over the amount of landmine
		Vector2 Center = Vector2.zero;
		for(int i = 0; i < _Mines.Count; i++)
		{
			Center.x += _Mines[i].transform.position.x;
			Center.y += _Mines[i].transform.position.y;
		}
		return Center/_Mines.Count;
	}

	//A function that return a boolean on whether is it time to spread based on the distance from the center mass of the landmine and the player main's position
	private bool IsTimetoSpread()
	{
		float EMtoPM = Utility.Distance(m_Main.transform.position, m_ecFSM.m_PMain.transform.position);
		float TargetDistance = 0.95f * EMtoPM;
		Vector2 CenterOfMass = GetCenterOfMines(GetLandmines());
		
		return Utility.Distance(CenterOfMass,m_ecFSM.m_PMain.transform.position) < TargetDistance ? true : false;
	}

	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagLandmines(Spread _Spreadness)
	{
		List<GameObject> NeighbouringLandmine = new List<GameObject>();

		if(_Spreadness == Spread.Tight)
		{
			Collider2D[] m_NeighbourChilds = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 20f,Constants.s_onlyEnemeyChildLayer);//Physics2D.OverlapAreaAll(m_SpreadTopLeft,m_SpreadBotRight,LayerMask.NameToLayer ("EnemyChild"));

			for(int i = 0; i < m_NeighbourChilds.Length; i++)
			{
				if(m_NeighbourChilds[i] != null && m_NeighbourChilds[i].gameObject != m_Child && m_NeighbourChilds[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
				{
					NeighbouringLandmine.Add(m_NeighbourChilds[i].gameObject);
				}
			}
		}
		else if(_Spreadness == Spread.Wide)
		{
			Collider2D[] m_NeighbourChilds = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 40f,Constants.s_onlyEnemeyChildLayer);//Physics2D.OverlapAreaAll(m_SpreadTopLeft,m_SpreadBotRight,LayerMask.NameToLayer ("EnemyChild"));

			for(int i = 0; i < m_NeighbourChilds.Length; i++)
			{
				if(m_NeighbourChilds[i] != null && m_NeighbourChilds[i].gameObject != m_Child && m_NeighbourChilds[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
				{
					NeighbouringLandmine.Add(m_NeighbourChilds[i].gameObject);
				}
			}
		}

		return NeighbouringLandmine;
	}

	//A function that return the amount of landmine near the current enemy child cell
	private int GetNearbyECMineAmount()
	{
		Collider2D[] Collisions = Physics2D.OverlapCircleAll(m_Child.transform.position,m_Child.GetComponent<SpriteRenderer>().bounds.size.x/4,Constants.s_onlyEnemeyChildLayer);
		int ECMineCount = 0;

		for(int i = 0; i < Collisions.Length; i++)
		{
			if(Collisions[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
			{
				ECMineCount++;
			}
		}

		return ECMineCount;
	}

	//A function that return a boolean as to whether the landmine is reaching any player-related cells
	private bool IsMineReachingPlayer(GameObject _PlayerCell)
	{
		if(_PlayerCell == null)
		{
			return false;
		}

		Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 1);

		for(int i = 0; i < NearbyObjects.Length; i++)
		{
			if(NearbyObjects[i].tag == Constants.s_strPlayerChildTag || NearbyObjects[i].tag == Constants.s_strPlayerTag || NearbyObjects[i].name.Contains("Squad"))
			{
				return true;
			}
		}

		return false;
	}

	//A function that activate the "PassThroughDeath" corountine on all landmines whereby they will continue to travel in their velocity for a short period of time before exploding and die
	private void CallAllMinePassThroughDeath()
	{
		List<GameObject> Landmines = GetLandmines();
		for(int i = 0; i < Landmines.Count; i++)
		{
			EnemyChildFSM LandmineFSM = Landmines[i].GetComponent<EnemyChildFSM>();
			LandmineFSM.StartChildCorountine(LandmineFSM.PassThroughDeath(1f));
		}
	}

	//A function that return a boolean as to whether the landmine is reaching a wall
	private bool IsCellReachingWall()
	{
		float CellRadius = m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2;
		float WallX = 4.5f;

		return (m_Child.transform.position.x + CellRadius < -WallX || m_Child.transform.position.x - CellRadius > WallX) ? true : false;
	}

	//A function that return a velocity vector for the landmine to get away from the nearest wall
	private Vector2 GetAwayFromWall()
	{
		float WallX = 4.5f;
		float DistToLeftWall = m_Child.transform.position.x - (-WallX);
		float DistToRightWall = m_Child.transform.position.x - WallX;
		float ClosestWallX = DistToLeftWall <= DistToRightWall ? -WallX : WallX;
		Vector2 MainVelo = m_Main.GetComponent<Rigidbody2D>().velocity;

		if(ClosestWallX > 0 && MainVelo.x > 0)
		{
			return new Vector2(0f, MainVelo.y);
		}
		else if(ClosestWallX > 0 && MainVelo.x < 0)
		{
			return MainVelo;
		}
		else if(ClosestWallX < 0 && MainVelo.x < 0)
		{
			return new Vector2(0f, MainVelo.y);
		}
		else if(ClosestWallX < 0 && MainVelo.x > 0)
		{
			return MainVelo;
		}

		return Vector2.zero;
	}

	private void CheckIfEnoughCells()
	{
		if(m_ecFSM.m_AttackTarget.name.Contains("LeftNode") && ECTracker.s_Instance.LandmineCells.Count >= GameObject.Find("UI_Player_LeftNode").GetComponent<Node_Manager>().activeChildCount)
		{
			m_Child.transform.localScale = Vector3.one;
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
		else if(m_ecFSM.m_AttackTarget.name.Contains("RightNode") && ECTracker.s_Instance.LandmineCells.Count >= GameObject.Find("UI_Player_RightNode").GetComponent<Node_Manager>().activeChildCount)
		{
			m_Child.transform.localScale = Vector3.one;
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
		else if(m_ecFSM.m_AttackTarget.name.Contains("Squad") && ECTracker.Instance.LandmineCells.Count >= GameObject.Find("Squad_Captain_Cell").GetComponent<PlayerSquadFSM>().AliveChildCount())
		{
			m_Child.transform.localScale = Vector3.one;
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
	}

	private Vector2 GetBlastAwayForce(float _Distance)
	{
		Vector2 Direction = Random.insideUnitCircle.normalized;
		float Force = _Distance /(fExplosiveRange - fKillRange) * 80f;
		return Direction * Force;
	}

	//A function that drive the landmine to grow and shrink when its going through the exploding process
	private void ExplodingGrowShrink()
	{
		Vector2 CurrentScale = m_Child.transform.localScale;

		if(CurrentScale.x >= ExpansionLimit.x && CurrentScale.y >= ExpansionLimit.y)
		{
			bExpanding = false;
		}
		else if(CurrentScale.x <= ShrinkLimit.x && CurrentScale.y <= ShrinkLimit.y)
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
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;
		bExploding = false;
		m_Child.transform.localScale = Vector3.one;
		Animator.ExpandContract(0.0f,0,0.0f,true,0.0f);
	}

	//Go through all the surround cells, destroy any player child cells and damaing the player main cell if in range
	private void ExplodeDestroy()
	{
		MainCamera.CameraShake();

		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position,fExplosiveRange);
		float DistanceFromCenterOfBlast = 0f;

		Utility.DrawCircleCross(m_Child.transform.position,fExplosiveRange,Color.green);
		Utility.DrawCircleCross(m_Child.transform.position,fKillRange,Color.red);
			
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			//if the player child cell is within the exploding range, kill the player child
			if(m_SurroundingObjects[i] != null && m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerChildTag)
			{
				DistanceFromCenterOfBlast = Utility.Distance(m_Child.transform.position,m_SurroundingObjects[i].transform.position);
				
				if(DistanceFromCenterOfBlast > fKillRange)
				{
					m_SurroundingObjects[i].GetComponent<Rigidbody2D>().AddForce(GetBlastAwayForce(DistanceFromCenterOfBlast - fKillRange));
				}
				else
				{
					m_SurroundingObjects[i].GetComponent<PlayerChildFSM>().DeferredChangeState(PCState.Dead);
				}
			}
			//if the player main cell is within the exploding range, damage the player main
			else if(m_SurroundingObjects[i] != null && m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerTag)
			{
				m_SurroundingObjects[i].GetComponent<PlayerMain>().HurtPlayerMain();
			}
			else if(m_SurroundingObjects[i] != null && m_SurroundingObjects[i].name.Contains("Squad_Child"))
			{
				DistanceFromCenterOfBlast = Utility.Distance(m_Child.transform.position,m_SurroundingObjects[i].transform.position);
				if(DistanceFromCenterOfBlast > fKillRange)
				{
					m_SurroundingObjects[i].GetComponent<Rigidbody2D>().AddForce(GetBlastAwayForce(DistanceFromCenterOfBlast - fKillRange));
				}
				else
				{
					m_SurroundingObjects[i].GetComponent<SquadChildFSM>().KillSquadChild();
				}
			}
			m_ecFSM.StopChildCorountine(ExplodeCorountine());
		}

		//After all the damaging and killing is done, transition the enemy child to a dead state
		bExploded = true;
		MessageDispatcher.Instance.DispatchMessage(m_Child, m_Child,MessageType.Dead,0);
	}

	//a corountine for the cell explosion
	IEnumerator ExplodeCorountine()
	{
		//play explode sound/animation whatever
		ExplodeSetup();

		Vector3 ExpandScale = new Vector3(0.75f,0.75f,0.75f);

		while(m_Child.transform.localScale.x < 7f)
		{
			m_Child.transform.localScale += ExpandScale;
			yield return new WaitForSeconds(0.0005f);//0.0005
		}

		if(m_ecFSM.CurrentStateEnum == ECState.Landmine)
		{
			ExplodeDestroy();
		}
	}
}
