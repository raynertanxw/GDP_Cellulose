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
	private float fChargeSpeed;
	
	//an array of gameobject to store the player nodes
	private GameObject[] m_Nodes;
	private GameObject m_SquadCaptain;
	
	private List<Point> PathToTarget;
	private int CurrentTargetIndex;
	private Point CurrentTargetPoint;
	private bool bTeleported;
	
	//constructor
	public ECTrickAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		m_Nodes = new GameObject[3];
	}
	
	//initialize the array and various variables for the trick attack
	public override void Enter()
	{
		m_Nodes[0] = GameObject.Find("Node_Left");
		m_Nodes[1] = GameObject.Find("Node_Right");

		m_SquadCaptain = GameObject.Find("Squad_Captain_Cell");

		m_AttackTarget = GetTarget();
		bReachStart = false;
		fChargeSpeed = 1f;
		
		List<Vector2> m_Positions = CalculateKeyPositions();
		m_StartTelePos = m_Positions[0];
		m_EndTelePos = m_Positions[1];
		m_TargetPos = m_Positions[2];
		
		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_StartTelePos,true);
		PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.Mid);
		CurrentTargetIndex = 0;
		CurrentTargetPoint = PathToTarget[0];
		bTeleported = false;
	}
	
	public override void Execute()
	{
		//if the cell have not reach the teleport starting position, continue charge forward. If the cell
		//reach the teleport starting position, start the teleport corountine. The coroutine will disable the
		//enemy child cell for 1.5 second become teleport the enemy child cell to the next position and then
		//move towards the player main cell
		m_ecFSM.RotateToHeading();
		
		if(!HasCellReachTargetPos(CurrentTargetPoint.Position))
		{
			ChargeTowards(CurrentTargetPoint.Position);
		}
		else if(CurrentTargetIndex + 1 < PathToTarget.Count)
		{
			CurrentTargetIndex++;
			CurrentTargetPoint = PathToTarget[CurrentTargetIndex];
			Utility.DrawCross(PathToTarget[CurrentTargetIndex].Position,Color.cyan,0.1f);
			//Debug.Log("point: " + CurrentTargetPoint.Index);
		}
		else if(HasCellReachTargetPos(PathToTarget[PathToTarget.Count - 1].Position) && bTeleported == false)
		{
			bTeleported = true;
			m_ecFSM.StartChildCorountine(Teleport());
		}
	}
	
	public override void Exit()
	{
		
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
	private GameObject GetWeakestPoint()
	{
		int ScoreLeft = EvaluteNode(m_Nodes[0]);
		int ScoreRight = EvaluteNode(m_Nodes[1]);

		if(m_SquadCaptain != null)
		{
			int ScoreSquad = m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
			int[] Scores = new int[3];
			Scores[0] = ScoreLeft;
			Scores[1] = ScoreRight;
			Scores[2] = ScoreSquad;
			int nLowestScore = 999;
			int nLowestIndex = 0;
			for(int i = 0; i < Scores.Length; i++)
			{
				if(Scores[i] < nLowestScore)
				{
					nLowestScore = Scores[i];
					nLowestIndex = i;
				}
			}
			if(nLowestIndex == 0)
			{
				return m_Nodes[0];
			}
			else if(nLowestIndex == 1)
			{
				return m_Nodes[1];
			}

			return m_SquadCaptain;
		}

		return ScoreLeft < ScoreRight ? m_Nodes[0] : m_Nodes[1];
	}
	
	//a function that evluate a specific node based on several conditions and return an integer that represent the score of threat
	private int EvaluteNode(GameObject _Node)
	{
		//if the node contains no cell, it serve no threat to the enemy main cell
		if(_Node.GetComponent<Node_Manager>().GetNodeChildList().Count == 0)
		{
			return 0;
		}
		
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
			return GetWeakestPoint();
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
		GameObject m_InitialWall = GetWall();
		
		//calculate the appropriate position to start teleporting 
		Vector2 m_StartPos = new Vector2(m_InitialWall.transform.position.x, fInitialY);
		
		//End Tele Position//
		//Use the corresponding Y from start tele or decrease it for the end tele position
		float Noise = Random.Range (-0.5f,0f);
		float fNextY = fInitialY + Noise;
		
		//Use the opposide side wall from the previous one
		GameObject m_NextWall = null;
		if(m_InitialWall == GameObject.Find("Left Wall"))
		{
			m_NextWall = GameObject.Find("Right Wall");
		}
		else
		{
			m_NextWall = GameObject.Find("Left Wall");
		}
		
		//Generate the appropriate position to appear from the teleport
		Vector2 m_EndPos = new Vector2(m_NextWall.transform.position.x, fNextY);
		
		Positions.Add(m_StartPos);
		Positions.Add(m_EndPos);
		Positions.Add(m_AttackTarget.transform.position);
		
		return Positions;
	}
	
	//a function that return the wall that is closest to the given position in the perimeter
	private GameObject GetClosestWall (Vector2 _Pos)
	{
		GameObject m_LWall = GameObject.Find("Left Wall");
		GameObject m_RWall = GameObject.Find("Right Wall");
		
		if(Vector2.Distance(_Pos, m_LWall.transform.position) < Vector2.Distance(_Pos, m_RWall.transform.position))
		{
			return m_LWall;
		}
		return m_RWall;
	}
	
	private GameObject GetWall()
	{
		GameObject m_LeftWall = GameObject.Find("Left Wall");
		GameObject m_RightWall = GameObject.Find("Right Wall");
		int random = Random.Range(0,2);
		if(random == 0)
		{
			return m_LeftWall;
		}
		return m_RightWall;
	}
	
	//a function that direct the enemy child cell towards a gameObject by changing its velocity through calculation
	private void ChargeTowards(Vector2 _Pos)
	{
		Vector2 m_TargetPos = _Pos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,8f);
	}
	
	//a function that direct the enemy child cell towards a gameObject by maintaining its velocity through calculation
	private void MoveTowards(Vector2 _Pos)
	{
		Vector2 m_TargetPos = _Pos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * 8f;
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 + GameObject.Find("Left Wall").GetComponent<SpriteRenderer>().bounds.size.x/2)
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
		yield return new WaitForSeconds(1f);
		
		//reenable the enemy child cell
		m_Child.GetComponent<SpriteRenderer>().enabled = true;
		m_Child.GetComponent<BoxCollider2D>().enabled = true;
		m_Child.GetComponent<Rigidbody2D>().isKinematic = false;
		
		//and, change the position of the enemy child cell to the end of teleport position
		m_Child.transform.position = m_EndTelePos;
		
		bReachStart = true;
		
		PathQuery.Instance.AStarSearch(m_Child.transform.position,m_TargetPos,true);
		PathToTarget = PathQuery.Instance.GetPathToTarget(Directness.Mid);
		CurrentTargetIndex = 0;
		CurrentTargetPoint = PathToTarget[0];
	}
}
