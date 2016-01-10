using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECDefendState : IECState {
	
	//a boolean to track whether the position of the enemy child cell had reach the guiding position for 
	//the defence formation
	private bool bReachPos;
	private static bool bGathered;
	
	private bool bAdjustNeeded;
	
	//a float to store the movement speed of the enemy child towards the defending position
	private float fMoveSpeed;
	
	//a vector2 to store the defending position that the enemy child cell need to move to
    private Vector2 m_TargetPos;

    private float fDefendTime;
    private float fLeftLimit;
    private float fRightLimit;
    private float fMaxAcceleration;
    
    private GameObject m_LeftWall;
    private GameObject m_RightWall;
    private static Formation CurrentFormation;

	//Constructor
    public ECDefendState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = _ecFSM.m_EMain;
		m_LeftWall = GameObject.Find("Left Wall");
		m_RightWall = GameObject.Find("Right Wall");
		fMoveSpeed = 3f;
		fMaxAcceleration = 30f;
		fDefendTime = 0f;
		//fLeftLimit = Vector2.Distance(new Vector2(0f,0f),GameObject.Find("Left Wall").transform.position) - m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 - GameObject.Find("Left Wall").GetComponent<SpriteRenderer>().bounds.size.x/2;
		//fRightLimit = Vector2.Distance(new Vector2(0f,0f),GameObject.Find("Right Wall").transform.position) - m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 - GameObject.Find("Right Wall").GetComponent<SpriteRenderer>().bounds.size.x/2;
		CurrentFormation = Formation.Empty;
    }

    public override void Enter()
    {
		bReachPos = false;
		bAdjustNeeded = false;
		fDefendTime = 0f;
		
		m_Child.GetComponent<Rigidbody2D>().drag = 2.6f;
    }

    public override void Execute()
    {
		if(CurrentFormation == Formation.Empty)
		{
			CurrentFormation = PositionQuery.Instance.GetDefensiveFormation();
			FormationDatabase.Instance.UpdateDatabaseFormation(CurrentFormation,GetDefendingCellsFSM());
		}
    
		m_TargetPos = FormationDatabase.Instance.GetTargetFormationPosition(CurrentFormation, m_Child);
		
		if(HasAllCellsGathered())
		{
			bGathered = true;
		}
    }
    
    public override void FixedExecute()
    {
		Vector2 Acceleration = Vector2.zero;
		
		if(!HasCellReachTargetPos(m_Main.transform.position) && !HasAllCellsGathered() && !bGathered)
		{
			m_ecFSM.RotateToHeading();
			Acceleration += SteeringBehavior.Seek(m_Child,m_Main.transform.position,24f);
		}
		
		if(!HasCellReachTargetPos(m_TargetPos) && bGathered && !bReachPos && !bAdjustNeeded && !IsCellReachingWall())
		{
			m_Child.GetComponent<Rigidbody2D>().drag = 5f;
			Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,15f);
		}
		else if(!HasCellReachTargetPos(m_TargetPos) && bGathered && !bReachPos && bAdjustNeeded && !IsCellReachingWall()) 
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,24f);
		}
		else if(!bReachPos && IsCellReachingWall())
		{
			bAdjustNeeded = true;
			Acceleration += GetAwayFromWall() * 24f;
		}
		else if(HasCellReachTargetPos(m_TargetPos) && bGathered && !bReachPos)
		{
			bReachPos = true;
		}
		
		
		if(bReachPos && !IsCellReachingWall())
		{
			//Debug.Log("vibrate point 1");
			Acceleration += new Vector2(0f, m_Main.GetComponent<Rigidbody2D>().velocity.y) * 24f;
			Acceleration += SteeringBehavior.ShakeOnSpot(m_Child,1f,8f);
			
			if(!HasCellReachTargetPos(m_TargetPos))
			{
				//Debug.Log("vibrate point 2");
				Acceleration += SteeringBehavior.Seek(m_Child,m_TargetPos,24f);
			}
		}
		else if(bReachPos && IsCellReachingWall() && CurrentFormation == Formation.CircularSurround)
		{
			//Debug.Log("vibrate point 3");
			Acceleration += GetAwayFromWall() * 24f;
		}
		
		
		if(!IsThereNoAttackers() && IsPlayerChildPassingBy())
		{
			//Debug.Log("vibrate point 1");
			Acceleration += SteeringBehavior.Seek(m_Child,GetClosestAttacker().transform.position,24f);
		}
		else if(IsThereNoAttackers())
		{
			fDefendTime += Time.deltaTime;
			if(fDefendTime >= 2f)
			{
				MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0f);
			}
		}
		
		
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);
		
		if(bReachPos)
		{
			m_ecFSM.RandomRotation(0.85f);
		}
		else
		{
			m_ecFSM.RotateToHeading();
		}
    }

    public override void Exit()
    {
		fDefendTime = 0f;
		if(GetDefendingCells().Count <= 1)
		{
			CurrentFormation = Formation.Empty;
		}
    }

		//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= 0.05f)
		{
			return true;
		}
		return false;
	}

    private bool IsPlayerChildPassingBy()
    {
		Collider2D[] PasserBy = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		foreach(Collider2D obj in PasserBy)
		{
			if(obj != null && obj.tag == Constants.s_strPlayerChildTag)
			{
				return true;
			}
		}
		return false;
    }
    
    private bool IsThereNoAttackers()
	{
		GameObject[] PlayerChilds = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag);
		foreach(GameObject child in PlayerChilds)
		{
			if(child.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeChild || child.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain)
			{
				return false;
			}
		}
		return true;
	}
	
	private GameObject GetClosestAttacker()
	{
		GameObject[] PlayerChilds = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag);
		GameObject ClosestAttacker = PlayerChilds[0];
		float distance = Mathf.Infinity;
		
		foreach(GameObject child in PlayerChilds)
		{
			if((child.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeChild || child.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.ChargeMain) && Vector2.Distance(child.transform.position,m_Child.transform.position) < distance)
			{
				ClosestAttacker = child;
				distance = Vector2.Distance(child.transform.position,m_Child.transform.position);
			}
		}
		
		return ClosestAttacker;
	}
	
	private List<GameObject> GetDefendingCells()
	{
		GameObject[] EnemyChild = GameObject.FindGameObjectsWithTag(Constants.s_strEnemyChildTag);
		List<GameObject> Defenders = new List<GameObject>();
		foreach(GameObject Child in EnemyChild)
		{
			if(Child.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Defend)
			{
				Defenders.Add(Child);
			}
		}
		return Defenders;
	}
	
	private List<EnemyChildFSM> GetDefendingCellsFSM()
	{
		GameObject[] EnemyChild = GameObject.FindGameObjectsWithTag(Constants.s_strEnemyChildTag);
		List<EnemyChildFSM> Defenders = new List<EnemyChildFSM>();
		foreach(GameObject Child in EnemyChild)
		{
			if(Child.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Defend)
			{
				Defenders.Add(Child.GetComponent<EnemyChildFSM>());
			}
		}
		return Defenders;
	}
	
	private bool HasAllCellsGathered()
	{
		List<GameObject> DefendingCells = GetDefendingCells();
		foreach(GameObject DefendingCell in DefendingCells)
		{
			if(!HasCellReachTargetPos(m_Main.transform.position))
			{
				return false;
			}
		}
		return true;
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
			return new Vector2(0f, m_Main.GetComponent<Rigidbody2D>().velocity.y);
		}
		else if(ClosestWall.transform.position.x > 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x < 0)
		{
			return m_Main.GetComponent<Rigidbody2D>().velocity;
		}
		else if(ClosestWall.transform.position.x < 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x < 0)
		{
			return new Vector2(0f, m_Main.GetComponent<Rigidbody2D>().velocity.y);
		}
		else if(ClosestWall.transform.position.x < 0 && m_Main.GetComponent<Rigidbody2D>().velocity.x > 0)
		{
			return m_Main.GetComponent<Rigidbody2D>().velocity;
		}
		
		return Vector2.zero;
	}
	
	private Vector2 GetNoise()
	{
		Vector2 Noise = Random.insideUnitCircle * 10f;
		return Noise;
	}
}
