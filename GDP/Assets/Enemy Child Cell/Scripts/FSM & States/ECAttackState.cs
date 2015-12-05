using UnityEngine;
using System.Collections;

public class ECAttackState : IECState {
	
	private Node_Manager m_TopNode;
	private Node_Manager m_LeftNode;
	private Node_Manager m_RightNode;
	
	public ECAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.eMain;
		
		m_TopNode = GameObject.Find("Node_Top").GetComponent<Node_Manager>();
		m_LeftNode = GameObject.Find("Node_Left").GetComponent<Node_Manager>();
		m_RightNode = GameObject.Find("Node_Right").GetComponent<Node_Manager>();
	}
	
	public override void Enter()
	{
		
	}
	
	public override void Execute()
	{
		DecideAttackType();
	}
	
	public override void Exit()
	{
		
	}
	
	private void DecideAttackType()
	{
		//if player has cell nodes, focus attacks on the cell nodes
		//else, go for the player main cell
		
		if(IsThereThreatToMain())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.ChargeChild, 0);
		}
		else
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.ChargeMain, 0);
		}
		
	}
	
	private bool IsThereThreatToMain()
	{
		int PlayerTotalCells = m_TopNode.GetNodeChildList().Count + m_LeftNode.GetNodeChildList().Count + m_RightNode.GetNodeChildList().Count;
		int OwnTotalCells = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag).Length; //m_Main.GetComponent<EnemyMainFSM>().ECList.Count;
		
		if(PlayerTotalCells > OwnTotalCells)
		{
			return true;
		}
		else if (m_TopNode.GetNodeChildList().Count > 5 || m_LeftNode.GetNodeChildList().Count > 5 || m_RightNode.GetNodeChildList().Count > 5)
		{
			return true;
		}
		else
		{
			return false;
		}
	} 
}
