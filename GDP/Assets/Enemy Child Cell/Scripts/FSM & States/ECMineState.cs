using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECMineState : IECState {

	//Two booleans that state whether the landmines had reach the last point on the calculated path and whether it reach the point at which the landmines should spread out
	private static bool m_bReachTarget;
	private static bool m_bGatherTogether;
	
	private static float m_fMaxAcceleration;
	private static float m_fExpansionSpeed;
	private static float m_fExplosiveRange;
	private static float m_fKillRange;
	
	private static Vector2 m_ExpansionLimit;
	private static Vector2 m_ShrinkLimit;
	private static Vector2 m_EndPosition;
	private static GameObject m_Target;
	private static PositionType m_CurrentPositionType;
	private static List<Point> m_PathToTarget;
	
	private Vector2 m_TargetLandminePos;
	private Spread m_CurrentSpreadness;
	private Animate m_Animator;
	private static Point m_CurrentTargetPoint;
	
	private bool m_bExploding;
	private bool m_bExploded;
	private bool m_bExpanding;
	private bool m_bExplodeCorountineStart;
	private bool m_bExpandContractStart;
	
	private int m_nECMineNearby;
	private static int m_nCurrentTargetIndex;
	
	private static GameObject m_LeaderMine;

	private enum Spread{Empty,Tight, Wide};

	//Constructor
	public ECMineState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		
		m_ExpansionLimit = new Vector2(1.7f,1.7f);
		m_ShrinkLimit = new Vector2(0.8f,0.8f);
		m_PathToTarget = new List<Point>();
		m_Animator = new Animate(m_Child.transform);
		
		m_bExpanding = true;
		m_Target = null;
		m_CurrentTargetPoint = null;
		m_bExpandContractStart = false;
		
		m_fExpansionSpeed = 0.1f;
		m_fMaxAcceleration = 40f;
		m_fExplosiveRange = 4f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x;
		m_fKillRange = 0.75f * m_fExplosiveRange;
		
		m_CurrentPositionType = PositionType.Empty;
	}

	public override void Enter()
	{
		if(m_ecFSM.m_AttackTarget != null){CheckIfEnoughCells();};
	
		m_bGatherTogether = false;
		m_bExploding = false;
		m_bReachTarget = false;
		m_bExplodeCorountineStart = false;
		m_nECMineNearby = 0;
		m_CurrentSpreadness = Spread.Empty;

		m_Main.GetComponent<Rigidbody2D>().drag = 2.4f;
		ECTracker.Instance.LandmineCells.Add(m_ecFSM);
		
		if(m_LeaderMine == null){m_LeaderMine = ObtainLeaderFrmMines();}
	}

	public override void Execute()
	{
		m_LeaderMine = ObtainLeaderFrmMines(); 	
		//if(m_LeaderMine == null || m_LeaderMine.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Dead){m_LeaderMine = ObtainLeaderFrmMines(); Debug.Log("new leader mine assigned");}
		
		if(!m_bGatherTogether)
		{
			m_nECMineNearby = GetNearbyECMineAmount();

			if(m_nECMineNearby == ECTracker.Instance.LandmineCells.Count)
			{
				if(m_LeaderMine != null)
				{
					m_CurrentPositionType = DeterminePositionType();
					m_Target = m_ecFSM.m_AttackTarget;
					
					if(m_Target == null)
					{
						m_Target = PositionQuery.Instance.GetLandmineTarget(m_CurrentPositionType,m_Child);
					}
					
					m_TargetLandminePos = PositionQuery.Instance.GetLandminePos(DetermineRangeValue(),m_CurrentPositionType,m_LeaderMine);
					PathQuery.Instance.AStarSearch(m_LeaderMine.transform.position,m_TargetLandminePos,false);
					m_PathToTarget = PathQuery.Instance.GetPathToTarget(DetermineDirectness(m_Target));
					m_nCurrentTargetIndex = 0;
					m_CurrentTargetPoint = m_PathToTarget[m_nCurrentTargetIndex];
					
					//Debug.Log("A* path searched");
				}

				//Debug.Log("Gather done");
				m_bGatherTogether = true;
				AudioManager.PlayECSoundEffect(EnemyChildSFX.DeployLandmine,m_ecFSM.Audio);
			}
		}

		if(!m_bExploding && !m_bExplodeCorountineStart && IsMineReachingPlayer(m_Target))
		{
			m_bExploding = true;
			m_bExplodeCorountineStart = true;
			m_ecFSM.rigidbody2D.drag = 0f;
			m_ecFSM.rigidbody2D.velocity = new Vector2(m_ecFSM.rigidbody2D.velocity.x * 0.75f,m_ecFSM.rigidbody2D.velocity.y);
			m_ecFSM.StartChildCorountine(ExplodeCorountine());
		}

		if(IsCellReachingWall())
		{
			m_ecFSM.rigidbody2D.velocity = new Vector2(0f,m_ecFSM.rigidbody2D.velocity.y);
		}

		if (m_bGatherTogether && (HasCellReachTarget(m_PathToTarget[m_PathToTarget.Count - 1].Position) || ECTracker.Instance.LandmineCells.Count <= 1 || m_Child.transform.position.y < m_PathToTarget[m_PathToTarget.Count - 1].Position.y))
		{
			//Debug.Log("float towards bot of screen");
			m_bReachTarget = true;
			m_EndPosition = new Vector2(m_Child.transform.position.x,-99f);
			m_ecFSM.StopChildCorountine(ExplodeCorountine());
			//m_ecFSM.StartChildCorountine(m_ecFSM.PassThroughDeath(1f));
		}

		if(m_bGatherTogether && m_ecFSM.HitBottomOfScreen())
		{
			//Debug.Log("dead");
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0.0f);
		}

		if(m_bGatherTogether && !m_bExpandContractStart)
		{
			m_Animator.ExpandContract(15f,60,1.9f);
			m_bExpandContractStart = true;
		}
	}

	public override void FixedExecute()
	{
		Vector2 Acceleration = Vector2.zero;

		if(!m_bGatherTogether)
		{
			m_ecFSM.rigidbody2D.drag = 3f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,7.5f);
		}
		else if(m_bGatherTogether && m_PathToTarget == null && m_Child == m_LeaderMine)
		{
			m_Target = PositionQuery.Instance.GetLandmineTarget(m_CurrentPositionType,m_Child);
			m_TargetLandminePos = PositionQuery.Instance.GetLandminePos(DetermineRangeValue(),m_CurrentPositionType,m_Child);
			PathQuery.Instance.AStarSearch(m_Child.transform.position,m_TargetLandminePos,false);
			m_PathToTarget = PathQuery.Instance.GetPathToTarget(DetermineDirectness(m_Target));
		}

		if(!m_bReachTarget && m_Child == m_LeaderMine && m_bGatherTogether)
		{
			//Debug.Log("leader movement: " + m_LeaderMine.name);
		
			if(!m_bReachTarget && m_bGatherTogether && !m_bExploding )
			{
				//Debug.Log("following path");
				if(m_CurrentTargetPoint == null)
				{
					m_nCurrentTargetIndex = 0;
					m_CurrentTargetPoint = m_PathToTarget[m_nCurrentTargetIndex];
				}
				
				if(!HasCellReachTarget(m_CurrentTargetPoint.Position))
				{
					//Debug.Log("leader seek: " + SteeringBehavior.Seek(m_Child,m_CurrentTargetPoint.Position,12f));
					//m_ecFSM.rigidbody2D.drag = 3f;
					Acceleration += SteeringBehavior.Seek(m_Child,m_CurrentTargetPoint.Position,12f);
					AudioManager.PlayEMSoundEffectNoOverlap(EnemyMainSFX.LandmineBeeping);
				}
				else if((HasCellReachTarget(m_CurrentTargetPoint.Position)|| m_Child.transform.position.y < m_CurrentTargetPoint.Position.y) && m_nCurrentTargetIndex + 1 < m_PathToTarget.Count)
				{
					m_nCurrentTargetIndex++;
					m_CurrentTargetPoint = m_PathToTarget[m_nCurrentTargetIndex];
				}
				
				if(m_Child.transform.position.y < m_CurrentTargetPoint.Position.y && m_nCurrentTargetIndex + 1 < m_PathToTarget.Count)
				{
					m_nCurrentTargetIndex++;
					m_CurrentTargetPoint = m_PathToTarget[m_nCurrentTargetIndex];
				}
				//Utility.DrawCross(m_Child.transform.position,Color.green,0.1f);
			}
		}
		else if(!m_bReachTarget && m_Child != m_LeaderMine && m_bGatherTogether)
		{
			//Debug.Log("non-leader movement: " + m_Child.name);
			Acceleration += SteeringBehavior.Seek(m_Child,m_LeaderMine.transform.position,12f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagLandmines(Spread.Wide)) * 12f;
			AudioManager.PlayEMSoundEffectNoOverlap(EnemyMainSFX.LandmineBeeping);
		}
		else if(m_bReachTarget)
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_EndPosition,12f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagLandmines(Spread.Wide)) * 12f;
			AudioManager.PlayEMSoundEffectNoOverlap(EnemyMainSFX.LandmineBeeping);
		}
		
		Acceleration = Vector2.ClampMagnitude(Acceleration,m_fMaxAcceleration);
		m_ecFSM.rigidbody2D.AddForce(Acceleration,ForceMode2D.Force);

		if(!m_bExploding)
		{
			m_ecFSM.RotateToHeading();
		}
		else if(m_bExploding)
		{
			m_ecFSM.RandomRotation(0.75f);
		}
	}

	public override void Exit()
	{
		m_bExplodeCorountineStart = false;
		m_ecFSM.rigidbody2D.drag = 0f;
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;

		//if the landmine has not exploded and its going to die, it self-destruct instantly
		if(!m_bExploded)
		{
			MainCamera.CameraShake();
			m_ecFSM.StartChildCorountine(ExplodeCorountine());//ExplodeDestroy();
			ExplodeDestroy();
		}
		
		if(ECTracker.Instance.LandmineCells.Count <= 1)
		{
			m_bGatherTogether = false;
			m_EndPosition = Vector2.zero;
			m_Target = null;
			m_PathToTarget = null;
			m_CurrentTargetPoint = null;
			m_nCurrentTargetIndex = 0;
			m_LeaderMine = null;
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
		if(m_Target.name.Contains("Captain"))
		{
			return Spread.Tight;
		}

		if(m_Target.GetComponent<Node_Manager>() != null && m_Target.GetComponent<Node_Manager>().activeChildCount > 6)
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
			Collider2D[] m_NeighbourChilds = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x * 120f,Constants.s_onlyEnemeyChildLayer);//Physics2D.OverlapAreaAll(m_SpreadTopLeft,m_SpreadBotRight,LayerMask.NameToLayer ("EnemyChild"));

			for(int i = 0; i < m_NeighbourChilds.Length; i++)
			{
				if(m_NeighbourChilds[i] != null && m_NeighbourChilds[i].gameObject != m_Child && m_NeighbourChilds[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
				{
					NeighbouringLandmine.Add(m_NeighbourChilds[i].gameObject);
				}
			}
		}

		//Debug.Log("Neighbour landmine count: " + NeighbouringLandmine.Count);

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

	//A function that return a boolean as to whether the landmine is reaching a wall
	private bool IsCellReachingWall()
	{
		float CellRadius = m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2;
		float WallX = 4.5f;

		return (m_Child.transform.position.x + CellRadius < -WallX || m_Child.transform.position.x - CellRadius > WallX) ? true : false;
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
	
	private GameObject ObtainLeaderFrmMines()
	{
		int ClosestIndex = 0;
		float ClosestDist = Mathf.Infinity;
		
		for(int i = 0; i < ECTracker.Instance.LandmineCells.Count; i++)
		{
			if(m_CurrentTargetPoint != null && Vector2.Distance(ECTracker.Instance.LandmineCells[i].transform.position,m_CurrentTargetPoint.Position) < ClosestDist)
			{
				ClosestDist = Vector2.Distance(ECTracker.Instance.LandmineCells[i].transform.position,m_CurrentTargetPoint.Position);
				ClosestIndex = i;
			}
			else if(m_CurrentTargetPoint == null && Vector2.Distance(ECTracker.Instance.LandmineCells[i].transform.position,m_ecFSM.m_PMain.transform.position) < ClosestDist)
			{
				ClosestDist = Vector2.Distance(ECTracker.Instance.LandmineCells[i].transform.position,m_Main.transform.position);
				ClosestIndex = i;
			}
		}
		
		return ECTracker.Instance.LandmineCells[ClosestIndex].gameObject;
	}

	private Vector2 GetBlastAwayForce(float _Distance)
	{
		Vector2 Direction = Random.insideUnitCircle.normalized;
		float Force = _Distance /(m_fExplosiveRange - m_fKillRange) * 80f;
		return Direction * Force;
	}

	//A function that drive the landmine to grow and shrink when its going through the exploding process
	private void ExplodingGrowShrink()
	{
		Vector2 CurrentScale = m_Child.transform.localScale;

		if(CurrentScale.x >= m_ExpansionLimit.x && CurrentScale.y >= m_ExpansionLimit.y)
		{
			m_bExpanding = false;
		}
		else if(CurrentScale.x <= m_ShrinkLimit.x && CurrentScale.y <= m_ShrinkLimit.y)
		{
			m_bExpanding = true;
		}

		if(m_bExpanding)
		{
			CurrentScale += new Vector2(m_fExpansionSpeed,m_fExpansionSpeed);
			m_Child.transform.localScale = CurrentScale;
		}
		else if(!m_bExpanding)
		{
			CurrentScale -= new Vector2(m_fExpansionSpeed,m_fExpansionSpeed);
			m_Child.transform.localScale = CurrentScale;
		}
	}

	//a function for any potential sound effects or visual effects to be played
	//during explosions
	private void ExplodeSetup()
	{
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;
		m_bExploding = false;
		m_Child.transform.localScale = Vector3.one;
		m_Animator.ExpandContract(0.0f,0,0.0f,true,0.0f);
	
		AudioManager.PlayECSoundEffect(EnemyChildSFX.LandmineExplode,m_ecFSM.Audio);
	}

	//Go through all the surround cells, destroy any player child cells and damaing the player main cell if in range
	private void ExplodeDestroy()
	{
		MainCamera.CameraShake();

		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position,m_fExplosiveRange);
		float DistanceFromCenterOfBlast = 0f;

		//Utility.DrawCircleCross(m_Child.transform.position,fExplosiveRange,Color.green);
		//Utility.DrawCircleCross(m_Child.transform.position,fKillRange,Color.red);
			
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			//if the player child cell is within the exploding range, kill the player child
			if(m_SurroundingObjects[i] != null && m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerChildTag)
			{
				DistanceFromCenterOfBlast = Utility.Distance(m_Child.transform.position,m_SurroundingObjects[i].transform.position);
				
				if(DistanceFromCenterOfBlast > m_fKillRange)
				{
					m_SurroundingObjects[i].GetComponent<Rigidbody2D>().AddForce(GetBlastAwayForce(DistanceFromCenterOfBlast - m_fKillRange));
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
				if(DistanceFromCenterOfBlast > m_fKillRange)
				{
					m_SurroundingObjects[i].GetComponent<Rigidbody2D>().AddForce(GetBlastAwayForce(DistanceFromCenterOfBlast - m_fKillRange));
				}
				else
				{
					m_SurroundingObjects[i].GetComponent<SquadChildFSM>().KillSquadChild();
				}
			}
			m_ecFSM.StopChildCorountine(ExplodeCorountine());
		}

		//After all the damaging and killing is done, transition the enemy child to a dead state
		m_bExploded = true;
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
		
		m_Child.transform.localScale = Vector3.one;
	}
}
