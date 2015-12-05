using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECChargeCState : IECState {
	
	private float fChargeSpeed;
	
	public ECChargeCState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.eMain;
		m_Child = _childCell;
		fChargeSpeed = 0.3f;
	}
	
	public override void Enter()
	{
		//Debug.Log("Enter charge child");
		m_ecFSM.chargeTarget = FindTargetChild();
	}
	
	public override void Execute()
	{
		if(!HasCellReachTargetPos(m_ecFSM.chargeTarget.transform.position))
		{
			ChargeTowards(m_ecFSM.chargeTarget);
		}
		else
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child.gameObject, m_Child.gameObject, MessageType.Dead, 0);
		}
	}
	
	public override void Exit()
	{
		m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
	}
	
	private bool CheckIfTargetIsAvailable(GameObject _Target)
	{
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
		Node_Manager m_TargetNode = GetMostThreateningNode();
		List<PlayerChildFSM> m_PotentialTargets = m_TargetNode.GetNodeChildList();

		float fDistanceBetween = Mathf.Infinity;
		int nAvaliableEnemyChildCells = m_Main.GetComponent<EnemyMainFSM>().ECList.Count;
		GameObject m_TargetCell = m_PotentialTargets[0].gameObject;
		
		for (int i = 0; i < m_PotentialTargets.Count; i++)
		{
			if (CheckIfTargetIsAvailable(m_PotentialTargets[i].gameObject) && (Vector2.Distance(m_Child.transform.position, m_PotentialTargets[i].transform.position)) < fDistanceBetween)
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
			
			//increase score by the amount of nutrients that node has
			
		}
		
		return nthreatLevel;
	}
	
	private bool HasCellReachTargetPos(Vector2 _Pos)
	{
		if (Vector2.Distance(m_Child.transform.position, _Pos) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2 + m_ecFSM.chargeTarget.GetComponent<SpriteRenderer>().bounds.size.x/2)
		{
			return true;
		}
		return false;
	}
	
	private void ChargeTowards(GameObject _PC)
	{
		Vector2 m_TargetPos = _PC.transform.position;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fChargeSpeed;
		fChargeSpeed += 0.2f;
		fChargeSpeed = Mathf.Clamp(fChargeSpeed,1f,12f);
	}
}
