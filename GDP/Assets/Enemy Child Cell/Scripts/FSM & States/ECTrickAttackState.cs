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
		m_Nodes[1] = GameObject.Find("Node_Top");
		m_Nodes[2] = GameObject.Find("Node_Right");
		m_AttackTarget = GetTarget();
		bReachStart = false;
		fChargeSpeed = 1f;
		
		List<Vector2> m_Positions = CalculateKeyPositions();
		m_StartTelePos = m_Positions[0];
		m_EndTelePos = m_Positions[1];
		m_TargetPos = m_Positions[2];
    }

    public override void Execute()
	{
		//if the cell have not reach the teleport starting position, continue charge forward. If the cell
		//reach the teleport starting position, start the teleport corountine. The coroutine will disable the
		//enemy child cell for 1.5 second become teleport the enemy child cell to the next position and then
		//move towards the player main cell
		if(bReachStart == false)
		{
			ChargeTowards(m_StartTelePos);
			if(HasCellReachTargetPos(m_StartTelePos))
			{
				m_ecFSM.StartChildCorountine(Teleport());
			}
		}
		else
		{
			MoveTowards(m_TargetPos);
		}
    }

    public override void Exit()
    {

    }
    
    //a function that return a boolean that state whether all the player nodes is empty
    private bool IsAllNodesEmpty()
    {
		for(int i = 0; i < m_Nodes.Length; i++)
		{
			if(m_Nodes[i].GetComponent<Node_Manager>().GetNodeChildList().Count > 0)
			{
				return false;
			}
		}
		
		return true;
    }
    
    //a function to return the least threatening node from the player
    private GameObject GetWeakestNode()
    {
		int[] m_Scores = new int[3];
		m_Scores[0] = EvaluteNode(m_Nodes[0]);
		m_Scores[1] = EvaluteNode(m_Nodes[1]);
		m_Scores[2] = EvaluteNode(m_Nodes[2]);
		
		int nIndex = 0;
		int nLowestScore = 999;
		for(int i = 0; i < m_Scores.Length; i++)
		{
			if(m_Scores[i] < nLowestScore)
			{
				nIndex = i;
				nLowestScore = m_Scores[i];
			}
		}
		
		return m_Nodes[nIndex];
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
		
		//increase score if that node have formed together and has a node captain
        if (_Node.GetComponent<PlayerSquadFSM>() != null)
		{
			nthreatLevel+= 50;
		}
		
		return nthreatLevel;
    }
    
    //a function to return the target object that the enemy child should aim towards
    private GameObject GetTarget()
    {
		if(IsAllNodesEmpty())
		{
			return m_ecFSM.m_PMain;
		}
		else
		{
			return GetWeakestNode();
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
		GameObject m_InitialWall = GetClosestWall(m_Child.transform.position);
		
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
			return m_RWall;
		}
		return m_LWall;
    }
    
	//a function that direct the enemy child cell towards a gameObject by changing its velocity through calculation
	private void ChargeTowards(Vector2 _Pos)
	{
		Vector2 m_TargetPos = _Pos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,12f);
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
	}
}
