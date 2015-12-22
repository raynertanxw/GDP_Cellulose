using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECMineState : IECState {

	//A vector 2 variable to store the target position for the landmine to move to
	private Vector2 m_TargetPosition;
	private GameObject m_Target;
	private List<Point> PathToTarget;
	private Point CurrentTargetPoint;
	
	//2 booleans to track whether the enemy child cell had reach the target landmine position 
	// or the explosion of the enemy child cell had started
	private bool bReachPosition;
	private bool bReachTarget;
	private bool bExplosionStart;
	
	//a float variable that store the speed of movement for the enemy child cell in this state
	private float fSpeed;
	private int CurrentTargetIndex;

	//Constructor
    public ECMineState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
    }

    public override void Enter()
    {
		//Refresh the pointDatabase with the latest information of the game environment

		PointDatabase.Instance.RefreshDatabase();
		
		//Retrieve the best target position to move to based on the request for position type and other game information
		m_Target = PositionQuery.Instance.GetLandmineTarget(DeterminePositionType(),m_Child);
		m_TargetPosition = PositionQuery.Instance.GetLandminePos(DetermineRangeValue(), DeterminePositionType(), m_Child);
		
		PathQuery.Instance.AStarSearch(m_Child.transform.position, m_TargetPosition,false);
		PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.Low);
		CurrentTargetIndex = 0;
		CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		
		fSpeed = 1f;
		bReachPosition = false;
		bReachTarget = false;
		bExplosionStart = false;
    }

    public override void Execute()
    {
		if(HasCellReachTarget(PathToTarget[PathToTarget.Count - 1].Position))
		{
			bReachTarget = true;
			m_ecFSM.StartChildCorountine(m_ecFSM.PassThroughDeath());
		}
    
		if(m_Target.name == "Node_Top" && m_Target.GetComponent<Node_Manager>().GetNodeChildList().Count <= 0)
		{
			m_Target = PositionQuery.Instance.GetLandmineTarget(DeterminePositionType(),m_Child);
			m_TargetPosition = PositionQuery.Instance.GetLandminePos(DetermineRangeValue(),DeterminePositionType(),m_Child);
			PathQuery.Instance.AStarSearch(m_Child.transform.position,m_TargetPosition,false);
			PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.Low);
			CurrentTargetIndex = 0;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		}
    
		if(!HasCellReachTarget(CurrentTargetPoint.Position) && !HasCellReachTarget(PathToTarget[PathToTarget.Count - 1].Position) && bReachTarget == false)
		{
			FloatTowards(CurrentTargetPoint.Position);
		}
		else if(CurrentTargetIndex + 1 < PathToTarget.Count && bReachTarget == false)
		{
			CurrentTargetIndex++;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
			//Utility.DrawCross(PathToTarget[CurrentTargetIndex].Position,Color.cyan,0.1f);
		}
		
		if(HasCollidedWithPlayerCells())
		{
			m_ecFSM.StartChildCorountine(ExplodeCorountine());
		}
    }

    public override void Exit()
    {
		
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
    
	//a function that direct the enemy child cell towards a gameObject by changing its velocity through calculation
	private void FloatTowards(Vector2 _TargetPos)
	{
		Vector2 m_TargetPos = _TargetPos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fSpeed;
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTarget (Vector2 _TargetPos)
	{
		if (Vector2.Distance(m_Child.transform.position, _TargetPos) <= 0.1f)
		{
			return true;
		}
		return false;
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
	
	private GameObject[] TagLandmines()
	{
		Collider2D[] Objects = Physics2D.OverlapCircleAll(m_Child.transform.position, 1f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		GameObject[] NeighbouringMines = new GameObject[Objects.Length];
		int Count = 0;
		
		foreach(Collider2D collision in Objects)
		{
			if(collision.tag == Constants.s_strEnemyChildTag && collision.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Landmine)
			{
				NeighbouringMines[Count] = collision.gameObject;
				Count++;
			}
		}
		
		return NeighbouringMines;
	}
	
	private Vector2 MaintainDistBetweenMines()
	{
		GameObject[] neighbours = TagLandmines();
		int neighbourCount = 0;
		Vector2 steering = new Vector2(0f, 0f);
		
		foreach (GameObject cell in neighbours)
		{
			if (cell != null && cell != m_Child)
			{
				steering.x += 0.5f * (cell.transform.position.x - m_Child.transform.position.x);
				steering.y += cell.transform.position.y - m_Child.transform.position.y;
				neighbourCount++;
			}
		}
		
		if (neighbourCount <= 0)
		{
			return steering;
		}
		else
		{
			steering /= neighbourCount;
			steering *= -1f;
			steering.Normalize();
			return steering;
		}
	}
	
	//a function for any potential sound effects or visual effects to be played 
	//during explosions (to be implemented in the future)
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
