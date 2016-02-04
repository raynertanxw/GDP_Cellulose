using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECTrickAttackState : IECState {

	private GameObject m_AttackTarget;
	private Point m_CurrentTargetPoint;
	
	private Vector2 m_StartTelePos;
	private Vector2 m_EndTelePos;
	private Vector2 m_TargetPos;

	private static float m_fMaxAcceleration;
	private static GameObject m_SquadCaptain;
	
	private GameObject[] m_Nodes;
	private List<Point> m_PathToTarget;

	private int m_nCurrentTargetIndex;
	private bool m_bReachStart;
	
	private bool m_bTeleported;
	private bool m_bTeleporting;
	private bool m_bReachTarget;
	private bool m_bSqueezeToggle;
	private bool m_bSqueezeDone;

	private static Vector3 m_ShrinkRate;
	private static BoxCollider2D m_LeftWall;
	private static BoxCollider2D m_RightWall;
	private static EnemyMainFSM m_EMFSM;

	//constructor
	public ECTrickAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;

		m_Nodes = new GameObject[3];
		m_ShrinkRate = new Vector3(-0.225f, 0.225f, 0.0f);
		m_fMaxAcceleration = 40f;

		m_EMFSM = m_Main.GetComponent<EnemyMainFSM>();
		m_Nodes[0] = Node_Manager.GetNode(Node.LeftNode).gameObject;
		m_Nodes[1] = Node_Manager.GetNode(Node.RightNode).gameObject;
		m_SquadCaptain = PlayerSquadFSM.Instance.gameObject;
		
		BoxCollider2D[] Walls = GameObject.Find("Wall").GetComponents<BoxCollider2D>();
		for(int i = 0 ; i < Walls.Length; i++)
		{
			if(Walls[i].offset.y > 39f && Walls[i].offset.x > 7f)
			{
				m_RightWall = Walls[i];
			}
			else if(Walls[i].offset.y > 39f && Walls[i].offset.x < -7f)
			{
				m_LeftWall = Walls[i];
			}
		}
	}

	//initialize the array and various variables for the trick attack
	public override void Enter()
	{
		if(m_ecFSM.m_AttackTarget == null){m_ecFSM.m_AttackTarget = m_ecFSM.m_PMain;}
	
		m_AttackTarget = m_ecFSM.m_AttackTarget;
		m_bReachStart = false;
		m_bReachTarget = false;
		
		if(m_ecFSM.m_AttackTarget != null){CheckIfEnoughCells();};

		List<Vector2> m_Positions = CalculateKeyPositions();
		m_StartTelePos = m_Positions[0];
		m_EndTelePos = m_Positions[1];
		m_TargetPos = m_Positions[2];

		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_StartTelePos,true);
		m_PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.Mid);
		m_PathToTarget = PathQuery.Instance.RefinePathForTA(m_PathToTarget,m_StartTelePos);
		m_nCurrentTargetIndex = 0;
		m_CurrentTargetPoint = m_PathToTarget[0];
		m_bTeleported = false;

		m_ecFSM.rigidbody2D.drag = 6f;
		ECTracker.s_Instance.TrickAttackCells.Add(m_ecFSM);
	}

	public override void Execute()
	{
		//If the cell had teleported and reach the final point of its travel, let it continue travel a short amount of distance before killing it
		if(!m_bSqueezeDone)
		{
			if(!m_bSqueezeToggle)
			{
				m_ecFSM.StartChildCorountine(SqueezeBeforeCharge(m_Main.transform.position));
				m_ecFSM.GetComponent<BoxCollider2D>().isTrigger = true;
				m_bSqueezeToggle = true;
			}
		}
		else if(m_bSqueezeDone && m_ecFSM.HitBottomOfScreen())
		{
			m_ecFSM.StopChildCorountine(m_ecFSM.PassThroughDeath(1f));
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0.0f);
		}
		else if(m_bSqueezeDone && HasCellReachTargetPos(m_PathToTarget[m_PathToTarget.Count - 1].Position) && m_bTeleported && m_bReachStart)
		{
			m_bReachTarget = true;
			m_ecFSM.StartChildCorountine(m_ecFSM.PassThroughDeath(1f));
		}
	}

	public override void FixedExecute()
	{
		//if the cell have not reach the teleport starting position, continue charge forward. If the cell
		//reach the teleport starting position, start the teleport corountine. The coroutine will disable the
		//enemy child cell for 1.5 second become teleport the enemy child cell to the next position and then
		//move towards the player main cell
		Vector2 Acceleration = Vector2.zero;

		if(m_bSqueezeDone && m_bTeleporting && !HasCellReachTargetPos(m_StartTelePos))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_StartTelePos, 45f);
			if(m_Child.transform.localScale.y < 1f && m_Child.transform.localScale.x > 0.5f){m_Child.transform.localScale += m_ShrinkRate;}
		}
		else if(m_bSqueezeDone && m_bTeleporting && HasCellReachTargetPos(m_StartTelePos))
		{
			m_bTeleported = true;
			m_bTeleporting = false;
			m_ecFSM.StartChildCorountine(Teleport());
		}
		//If the cell has not reached the current target position, continue seek towards that target position and remain seperate from rest of the enemy child cells
		else if(m_bSqueezeDone && !HasCellReachTargetPos(m_CurrentTargetPoint.Position) && !m_bReachTarget)
		{
			Acceleration += SteeringBehavior.Arrive(m_Child,m_CurrentTargetPoint.Position, 0.03f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 30f;
			if(m_Child.transform.localScale.y < 1f && m_Child.transform.localScale.x > 0.5f){m_Child.transform.localScale += m_ShrinkRate;}
		}
		else if(m_bSqueezeDone && m_nCurrentTargetIndex + 1 < m_PathToTarget.Count && !m_bReachTarget)
		{
			m_nCurrentTargetIndex++;
			m_CurrentTargetPoint = m_PathToTarget[m_nCurrentTargetIndex];
		}
		//If the cell had reached the teleporting position and it hasn't teleported, start teleporting the cell to the calculated position
		else if(m_bSqueezeDone && HasCellReachTargetPos(m_PathToTarget[m_PathToTarget.Count - 1].Position) && m_bTeleporting == false && m_bTeleported == false)
		{
			m_bTeleporting = true;
		}

		//Clamp the acceleration of the enemy child cell to a maximum value and then add that acceleration force to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,m_fMaxAcceleration);
		m_ecFSM.rigidbody2D.AddForce(Acceleration,ForceMode2D.Force);

		//Rotate the enemy child cell based on the direction of travel
		m_ecFSM.RotateToHeading();
	}

	public override void Exit()
	{
		//Reset the force acting on the enemy child cell
		m_ecFSM.rigidbody2D.drag = 0f;
		m_ecFSM.rigidbody2D.velocity = Vector2.zero;
		ECTracker.s_Instance.TrickAttackCells.Remove(m_ecFSM);
	}

	//a function that evluate a specific node based on several conditions and return an integer that represent the score of threat
	private int EvaluteNode(GameObject _Node)
	{
		int nthreatLevel = 0;

		//increase score based on amount of cells in that node
		nthreatLevel += _Node.GetComponent<Node_Manager>().activeChildCount;

		return nthreatLevel;
	}

	//a functon that return a list of the 3 key position used to perform the trick attack
	private List<Vector2> CalculateKeyPositions()
	{
		List<Vector2> Positions = new List<Vector2>();

		//Start Tele Position//
		//Generate a random Y to start teleporting
		float fYLimit = (m_Main.transform.position.y + m_ecFSM.m_PMain.transform.position.y) / 2;
		float fInitialY = Random.Range(fYLimit/2,fYLimit);

		//find a side of the Wall to teleport from
		Vector2 m_InitialWall = GetWallPos();

		//calculate the appropriate position to start teleporting
		Vector2 m_StartPos = new Vector2(m_InitialWall.x, fInitialY);

		//End Tele Position//
		//Use the corresponding Y from start tele or decrease it for the end tele position
		float Noise = Random.Range (-0.5f,0f);
		float fNextY = fInitialY + Noise;

		//Randomize the next wall at which the cells will come out from
		Vector2 m_NextWall = Vector2.zero;
		int i = Random.Range(0,2);
		if(i == 0)
		{
			m_NextWall = new Vector2(5.0f,m_Child.transform.position.y);
		}
		else if(i == 1)
		{
			m_NextWall =new Vector2(-5.0f,m_Child.transform.position.y);
		}

		//Generate the appropriate position to appear from the teleport
		Vector2 m_EndPos = new Vector2(m_NextWall.x, fNextY);

		Positions.Add(m_StartPos);
		Positions.Add(m_EndPos);
		Positions.Add(m_AttackTarget.transform.position);
		
		return Positions;
	}

	private Vector2 GetWallPos()
	{
		Vector2 LeftSide = new Vector2(-5.0f,m_Child.transform.position.y);
		Vector2 RightSide = new Vector2(5.0f,m_Child.transform.position.y);
		int random = Random.Range(0,2);
		if(random == 0)
		{
			return LeftSide;
		}
		return RightSide;
	}

	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Utility.Distance(m_Child.transform.position	, _Pos) <= 0.4f)
		{
			return true;
		}
		return false;
	}

	//A corountine that perform the teleporting process for the enemy child cell
	IEnumerator Teleport()
	{
		yield return new WaitForSeconds(0.05f);

		//disable the enemy child cell
		m_Child.GetComponent<SpriteRenderer>().enabled = false;
		m_Child.GetComponent<BoxCollider2D>().enabled = false;
		m_ecFSM.rigidbody2D.isKinematic = true;

		if(m_EMFSM.CurrentStateIndex == EMState.Die)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Dead,0f);
			yield break;
		}
		
		//wait for 1 second
		yield return new WaitForSeconds(2.5f);

		//reenable the enemy child cell
		m_Child.GetComponent<SpriteRenderer>().enabled = true;
		m_Child.GetComponent<BoxCollider2D>().enabled = true;
		m_ecFSM.rigidbody2D.isKinematic = false;

		//and, change the position of the enemy child cell to the end of teleport position
		m_Child.transform.position = m_EndTelePos;

		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_TargetPos,true);
		m_PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.Mid);
		m_nCurrentTargetIndex = 0;
		m_CurrentTargetPoint = m_PathToTarget[0];
		
		Physics2D.IgnoreCollision(m_Child.GetComponent<BoxCollider2D>(),m_LeftWall);
		Physics2D.IgnoreCollision(m_Child.GetComponent<BoxCollider2D>(),m_RightWall);

		m_ecFSM.GetComponent<BoxCollider2D>().isTrigger = false;

		m_bReachStart = true;
	}

	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();

		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x);

		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.tag == Constants.s_strEnemyChildTag && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.TrickAttack)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}

		return Neighbours;
	}
	
	private void CheckIfEnoughCells()
	{
		if(m_ecFSM.m_AttackTarget.name.Contains("LeftNode") && ECTracker.s_Instance.TrickAttackCells.Count >= GameObject.Find("UI_Player_LeftNode").GetComponent<Node_Manager>().activeChildCount)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
		else if(m_ecFSM.m_AttackTarget.name.Contains("RightNode") && ECTracker.s_Instance.TrickAttackCells.Count >= GameObject.Find("UI_Player_RightNode").GetComponent<Node_Manager>().activeChildCount)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
		else if(m_ecFSM.m_AttackTarget.name.Contains("Squad") && ECTracker.Instance.TrickAttackCells.Count >= GameObject.Find("Squad_Captain_Cell").GetComponent<PlayerSquadFSM>().AliveChildCount())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Idle,0);
		}
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
			yield return new WaitForSeconds(0.25f);
		}
		
		m_bSqueezeDone = true;
		AudioManager.PlayECSoundEffect(EnemyChildSFX.CellChargeTowards, m_ecFSM.Audio);
	}
}
