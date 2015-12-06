using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeCState : IECState {
	
	//a float that determine the speed to charge towards a player child cell
	private float fChargeSpeed;
	
	//Constructor
	public ECChargeCState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
		m_Child = _childCell;
		fChargeSpeed = 0.3f;
	}
	
	
	public override void Enter()
	{
		//Set the charge target to be one of the player child cell
		m_ecFSM.m_ChargeTarget = FindTargetChild();
	}
	
	public override void Execute()
	{
		//If the Enemy child cell had not reach the charge target, continue charge towards the target.
		//if the target is dead during the charging, change the target for the enemy child cell
		if(!HasCellReachTargetPos(m_ecFSM.m_ChargeTarget.transform.position))
		{
			ChargeTowards(m_ecFSM.m_ChargeTarget);
			if(IsTargetDead(m_ecFSM.m_ChargeTarget))
			{
				m_ecFSM.m_ChargeTarget = FindTargetChild();
			}
		}
	}
	
	public override void Exit()
	{
		//stop the child cell from moving after it charges finish
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
	}
	
	//a function that check whether the target is still avaliable to be attacked and return a boolean to
	//represent the result
	private bool CheckIfTargetIsAvailable(GameObject _Target)
	{
		//Loop through all the enemy child cells and check whether another child cell had targeted the same
		//target. Then, return a boolean to represent the result
		
		GameObject[] m_Childs = GameObject.FindGameObjectsWithTag(Constants.s_strEnemyChildTag);
		for(int i = 0; i < m_Childs.Length; i++)
		{
			//Utility.CheckEmpty<EnemyChildFSM>(m_Childs[i].GetComponent<EnemyChildFSM>());
			if(m_Childs[i].GetComponent<EnemyChildFSM>().Target != _Target)
			{
				return false;
			}
		}
		return true;
	}
	
	private GameObject FindTargetChild()
	{
		//Find the node to obtain a target child by evaluating which node is the most threatening
		Node_Manager m_TargetNode = GetMostThreateningNode();
		List<PlayerChildFSM> m_PotentialTargets = m_TargetNode.GetNodeChildList();

		//loop through all the childs of the most threatening squad and return the closest target player cell
		float fDistanceBetween = Mathf.Infinity;
		int nAvaliableEnemyChildCells = m_Main.GetComponent<EnemyMainFSM>().ECList.Count;
		GameObject m_TargetCell = GetFirstAvailableCell(m_PotentialTargets);
		
		for (int i = 0; i < m_PotentialTargets.Count; i++)
		{
			if (CheckIfTargetIsAvailable(m_PotentialTargets[i].gameObject) && (Vector2.Distance(m_Child.transform.position, m_PotentialTargets[i].transform.position)) < fDistanceBetween && m_PotentialTargets[i].GetCurrentState() != PCState.Dead)
			{
				fDistanceBetween = Vector2.Distance(m_Child.transform.position, m_PotentialTargets[i].transform.position);
				m_TargetCell = m_PotentialTargets[i].gameObject;
			}
		}

		//If there is no more available player child to be targeted, stop this state and shift back to idle state
		if(m_TargetCell == null)
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_Child, MessageType.Idle, 0);
			return null;
		}

		return m_TargetCell;
	}
	
	private Node_Manager GetMostThreateningNode()
	{
		//3 different player nodes
		GameObject m_TopNode = GameObject.Find("Node_Top");
		GameObject m_LeftNode = GameObject.Find("Node_Left");
		GameObject m_RightNode = GameObject.Find("Node_Right");
		
		//Scores for the 3 different nodes
		int nTopScore = EvaluateNode(m_TopNode);
		int nLeftScore = EvaluateNode(m_LeftNode);
		int nRightScore = EvaluateNode(m_RightNode);
		
		//Compare the score of the nodes and obtain the highest threat score, then return the most threatening
		//node
		int nHighestThreat = Mathf.Max(Mathf.Max(nTopScore,nLeftScore),nRightScore);
		
		if(nHighestThreat == nTopScore)
		{
			return m_TopNode.GetComponent<Node_Manager>();
		}
		else if(nHighestThreat == nLeftScore)
		{
			return m_LeftNode.GetComponent<Node_Manager>();
		}
		else if(nHighestThreat == nRightScore)
		{
			return m_RightNode.GetComponent<Node_Manager>();
		}
		
		return null;
	}
	
	//A function to evaluate the node based on the enemy main cell and player condition to return a score
	private int EvaluateNode (GameObject _Node)
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
		if(_Node.GetComponent<SquadCaptain>() != null)
		{
			nthreatLevel+= 50;
		}
		
		return nthreatLevel;
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 + m_ecFSM.m_ChargeTarget.GetComponent<SpriteRenderer>().bounds.size.x/2)
		{
			return true;
		}
		return false;
	}
	
	//a function that direct the enemy child cell towards a gameObject by changing its velocity through calculation
	private void ChargeTowards(GameObject _PC)
	{
		Vector2 m_TargetPos = _PC.transform.position;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,12f);
	}
	
	//a function that return a boolean to show the target gameobject is dead 
	private bool IsTargetDead(GameObject _Target)
	{
		if(_Target.GetComponent<PlayerChildFSM>().GetCurrentState() == PCState.Dead)
		{
			return true;
		}
		return false;
	}
	
	private GameObject GetFirstAvailableCell(List<PlayerChildFSM> _PotentialTarget)
	{
		for(int i = 0; i < _PotentialTarget.Count; i++)
		{
			if(_PotentialTarget[i] != null)
			{
				return _PotentialTarget[i].gameObject;
			}
		}
		return null;
	}
}
