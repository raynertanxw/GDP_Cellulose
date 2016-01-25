using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECTrickAttackState : IECState {

	//a gameobject variable to store the target to attack
	private GameObject m_AttackTarget;

	//3 positions used for the trick attack to be performed
	private Vector2 m_StartTelePos;
	private Vector2 m_EndTelePos;
	private Vector2 m_TargetPos;

	//boolean that track whether the enemy child cell had reach the target position
	private bool bReachStart;

	//float to store the speed of movement for the enemy child cell in trick attacking
	private static float fMaxAcceleration;

	//an array of gameobject to store the player nodes
	private GameObject[] m_Nodes;
	private static GameObject m_SquadCaptain;

	//A list of Point to store the A* path calculated for the enemy child cell to reach the target position
	private List<Point> PathToTarget;

	//An integer to store the current target point's index that the cell is traveling to
	private int CurrentTargetIndex;

	//A Point variable that store the current target point that the cell is traveling to
	private Point CurrentTargetPoint;

	//A boolean that state whether the enemy child cell had teleported during the trick attack state
	private bool bTeleported;
	private bool bTeleporting;

	//A boolean that state whether the enemy child cell had reached the last point of the calculated path
	private bool bReachTarget;

	private bool bSqueezeToggle;
	
	private bool bSqueezeDone;

	private static Vector3 ShrinkRate;

	//constructor
	public ECTrickAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;

		m_Nodes = new GameObject[3];
		fMaxAcceleration = 40f;

		m_Nodes[0] = Node_Manager.GetNode(Node.LeftNode).gameObject;
		m_Nodes[1] = Node_Manager.GetNode(Node.RightNode).gameObject;
		m_SquadCaptain = PlayerSquadFSM.Instance.gameObject;
		ShrinkRate = new Vector3(-0.225f, 0.225f, 0.0f);
	}

	//initialize the array and various variables for the trick attack
	public override void Enter()
	{
		m_AttackTarget = GetTarget();
		bReachStart = false;
		bReachTarget = false;

		List<Vector2> m_Positions = CalculateKeyPositions();
		m_StartTelePos = m_Positions[0];
		m_EndTelePos = m_Positions[1];
		m_TargetPos = m_Positions[2];

		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_StartTelePos,true);
		PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.Mid);
		PathToTarget = PathQuery.Instance.RefinePathForTA(PathToTarget,m_StartTelePos);
		CurrentTargetIndex = 0;
		CurrentTargetPoint = PathToTarget[0];
		bTeleported = false;

		m_ecFSM.rigidbody2D.drag = 6f;
		ECTracker.s_Instance.TrickAttackCells.Add(m_ecFSM);
	}

	public override void Execute()
	{
		//If the cell had teleported and reach the final point of its travel, let it continue travel a short amount of distance before killing it
		if(!bSqueezeDone)
		{
			if(!bSqueezeToggle)
			{
				m_ecFSM.StartChildCorountine(SqueezeBeforeCharge(m_Main.transform.position));
				bSqueezeToggle = true;
			}
		}
		else if(bSqueezeDone && HasCellReachTargetPos(PathToTarget[PathToTarget.Count - 1].Position) && bTeleported && bReachStart)
		{
			bReachTarget = true;
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

		if(bSqueezeDone && bTeleporting && !HasCellReachTargetPos(m_StartTelePos))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_StartTelePos, 45f);
			if(m_Child.transform.localScale.y < 1f && m_Child.transform.localScale.x > 0.5f){m_Child.transform.localScale += ShrinkRate;}
		}
		else if(bSqueezeDone && bTeleporting && HasCellReachTargetPos(m_StartTelePos))
		{
			bTeleported = true;
			bTeleporting = false;
			m_ecFSM.StartChildCorountine(Teleport());
		}
		//If the cell has not reached the current target position, continue seek towards that target position and remain seperate from rest of the enemy child cells
		else if(bSqueezeDone && !HasCellReachTargetPos(CurrentTargetPoint.Position) && !bReachTarget)
		{
			Acceleration += SteeringBehavior.Seek(m_Child, CurrentTargetPoint.Position, 45f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 30f;
			if(m_Child.transform.localScale.y < 1f && m_Child.transform.localScale.x > 0.5f){m_Child.transform.localScale += ShrinkRate;}
		}
		else if(bSqueezeDone && CurrentTargetIndex + 1 < PathToTarget.Count && !bReachTarget)
		{
			CurrentTargetIndex++;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		}
		//If the cell had reached the teleporting position and it hasn't teleported, start teleporting the cell to the calculated position
		else if(bSqueezeDone && HasCellReachTargetPos(PathToTarget[PathToTarget.Count - 1].Position) && bTeleporting == false && bTeleported == false)
		{
			bTeleporting = true;
		}

		//Clamp the acceleration of the enemy child cell to a maximum value and then add that acceleration force to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
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

	//a function that return a boolean that state whether all the player nodes is empty
	private bool IsAllThreatEmpty ()
	{
		List<GameObject> Threats = new List<GameObject>();
		Threats.Add(m_Nodes[0]);
		Threats.Add(m_Nodes[1]);
		Threats.Add(m_SquadCaptain);

		for(int i = 0; i < Threats.Count; i++)
		{
			if(Threats[i].GetComponent<Node_Manager>() != null && Threats[i].GetComponent<Node_Manager>().activeChildCount > 0)
			{
				return false;
			}
			else if(Threats[i].GetComponent<PlayerSquadFSM>() != null && Threats[i].GetComponent<PlayerSquadFSM>().AliveChildCount() > 0)
			{
				return false;
			}
		}
		return true;
	}

	//a function to return the least threatening node from the player
	private GameObject GetMostThreatPoint()
	{
		int ScoreLeft = EvaluteNode(m_Nodes[0]);
		int ScoreRight = EvaluteNode(m_Nodes[1]);

		if(m_SquadCaptain != null)
		{
			int ScoreSquad = m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
			int HighestScore = Mathf.Max(ScoreSquad,Mathf.Max(ScoreLeft,ScoreRight));

			if(HighestScore == ScoreSquad)
			{
				return m_SquadCaptain;
			}

			return HighestScore == ScoreLeft ? m_Nodes[0] : m_Nodes[1];
		}

		return ScoreLeft > ScoreRight ? m_Nodes[0] : m_Nodes[1];
	}

	//a function that evluate a specific node based on several conditions and return an integer that represent the score of threat
	private int EvaluteNode(GameObject _Node)
	{
		int nthreatLevel = 0;

		//increase score based on amount of cells in that node
		nthreatLevel += _Node.GetComponent<Node_Manager>().activeChildCount;

		return nthreatLevel;
	}

	//a function to return the target object that the enemy child should aim towards
	private GameObject GetTarget()
	{
		if(IsAllThreatEmpty())
		{
			return m_ecFSM.m_PMain;
		}
		else
		{
			return GetMostThreatPoint();
		}
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
			m_NextWall = new Vector2(4.7f,m_Child.transform.position.y);
		}
		else if(i == 1)
		{
			m_NextWall =new Vector2(-4.7f,m_Child.transform.position.y);
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
		Vector2 LeftSide = new Vector2(-4.7f,m_Child.transform.position.y);
		Vector2 RightSide = new Vector2(4.7f,m_Child.transform.position.y);
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

		//wait for 1 second
		yield return new WaitForSeconds(2.5f);

		//reenable the enemy child cell
		m_Child.GetComponent<SpriteRenderer>().enabled = true;
		m_Child.GetComponent<BoxCollider2D>().enabled = true;
		m_ecFSM.rigidbody2D.isKinematic = false;

		//and, change the position of the enemy child cell to the end of teleport position
		m_Child.transform.position = m_EndTelePos;

		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_TargetPos,true);
		PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.Mid);
		CurrentTargetIndex = 0;
		CurrentTargetPoint = PathToTarget[0];
		//Utility.DrawPath(PathToTarget,Color.red,0.1f);

		bReachStart = true;
	}

	//A function that return a list of GameObjects that are within a circular range to the enemy child cell
	private List<GameObject> TagNeighbours()
	{
		List<GameObject> Neighbours = new List<GameObject>();

		Collider2D[] Neighbouring = Physics2D.OverlapCircleAll(m_Child.transform.position, m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		//Debug.Log("Neighbouring count: " + Neighbouring.Length);

		for(int i = 0; i < Neighbouring.Length; i++)
		{
			if(Neighbouring[i].gameObject != m_Child && Neighbouring[i].gameObject.tag == Constants.s_strEnemyChildTag && Neighbouring[i].gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.TrickAttack)
			{
				Neighbours.Add(Neighbouring[i].gameObject);
			}
		}

		return Neighbours;
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
		
		bSqueezeDone = true;
	}
}
