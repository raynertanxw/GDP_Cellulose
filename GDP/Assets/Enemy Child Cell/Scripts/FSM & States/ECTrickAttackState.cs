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
	private float fMaxAcceleration;

	//an array of gameobject to store the player nodes
	private GameObject[] m_Nodes;
	private GameObject m_SquadCaptain;

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

	//constructor
	public ECTrickAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;

		m_Nodes = new GameObject[3];
		fMaxAcceleration = 20f;

		m_Nodes[0] = Node_Manager.GetNode(Node.LeftNode).gameObject;
		m_Nodes[1] = Node_Manager.GetNode(Node.RightNode).gameObject;
		m_SquadCaptain = PlayerSquadFSM.Instance.gameObject;
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

		m_Child.GetComponent<Rigidbody2D>().drag = 6f;
	}

	public override void Execute()
	{
		//If the cell had teleported and reach the final point of its travel, let it continue travel a short amount of distance before killing it
		if(HasCellReachTargetPos(PathToTarget[PathToTarget.Count - 1].Position) && bTeleported && bReachStart)
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

		if(bTeleporting && !HasCellReachTargetPos(m_StartTelePos))
		{
			Acceleration += SteeringBehavior.Seek(m_Child,m_StartTelePos, 45f);
		}
		else if(bTeleporting && HasCellReachTargetPos(m_StartTelePos))
		{
			bTeleported = true;
			bTeleporting = false;
			m_ecFSM.StartChildCorountine(Teleport());
		}
		//If the cell has not reached the current target position, continue seek towards that target position and remain seperate from rest of the enemy child cells
		else if(!HasCellReachTargetPos(CurrentTargetPoint.Position) && !bReachTarget)
		{
			Acceleration += SteeringBehavior.Seek(m_Child, CurrentTargetPoint.Position, 45f);
			Acceleration += SteeringBehavior.Seperation(m_Child,TagNeighbours()) * 30f;
		}
		else if(CurrentTargetIndex + 1 < PathToTarget.Count && !bReachTarget)
		{
			CurrentTargetIndex++;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
		}
		//If the cell had reached the teleporting position and it hasn't teleported, start teleporting the cell to the calculated position
		else if(HasCellReachTargetPos(PathToTarget[PathToTarget.Count - 1].Position) && bTeleporting == false && bTeleported == false)
		{
			bTeleporting = true;
		}

		//Clamp the acceleration of the enemy child cell to a maximum value and then add that acceleration force to the enemy child cell
		Acceleration = Vector2.ClampMagnitude(Acceleration,fMaxAcceleration);
		m_ecFSM.GetComponent<Rigidbody2D>().AddForce(Acceleration,ForceMode2D.Force);

		//Rotate the enemy child cell based on the direction of travel
		m_ecFSM.RotateToHeading();
	}

	public override void Exit()
	{
		//Reset the force acting on the enemy child cell
		m_Child.GetComponent<Rigidbody2D>().drag = 0f;
		m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

	//a function that return a boolean that state whether all the player nodes is empty
	private bool IsAllThreatEmpty ()
	{
		List<GameObject> Threats = new List<GameObject>();
		Threats.Add(m_Nodes[0]);
		Threats.Add(m_Nodes[1]);
		Threats.Add(m_SquadCaptain);

		bool bResult = false;
		for(int i = 0; i < Threats.Count; i++)
		{
			if(Threats[i].GetComponent<Node_Manager>() != null && Threats[i].GetComponent<Node_Manager>().GetNodeChildList().Count > 0)
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
		nthreatLevel += _Node.GetComponent<Node_Manager>().GetNodeChildList().Count;

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
		int i = Random.Range(0,1);
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
		if (Vector2.Distance(m_Child.transform.position	, _Pos) <= 0.4f)
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
		m_Child.GetComponent<Rigidbody2D>().isKinematic = true;

		//wait for 1 second
		yield return new WaitForSeconds(2.5f);

		//reenable the enemy child cell
		m_Child.GetComponent<SpriteRenderer>().enabled = true;
		m_Child.GetComponent<BoxCollider2D>().enabled = true;
		m_Child.GetComponent<Rigidbody2D>().isKinematic = false;

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
}
