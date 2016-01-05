using UnityEngine;
using System.Collections;

public class ECAttackState : IECState {

	private GameObject m_SquadCaptain;

	//Three Node_Manager variables to store the 3 individial nodes of the player;
	private Node_Manager m_LeftNode;
	private Node_Manager m_RightNode;
	
	//Constructor
	public ECAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;

		m_SquadCaptain = GameObject.Find("Squad_Captain_Cell");

		//Initialize the 2 nodes of the player
		m_LeftNode = GameObject.Find("Node_Left").GetComponent<Node_Manager>();
		m_RightNode = GameObject.Find("Node_Right").GetComponent<Node_Manager>();
	}
	
	public override void Enter()
	{
		
	}
	
	public override void Execute()
	{
		//When the enemy child cell had entered into this state, it decide what attack type the enemy child
		//cell it should take based on the game environment and other agents and transition the enemy child
		//cell to another type of attack state 
		DecideAttackType();
	}
	
	public override void Exit()
	{
		
	}
	
	private void DecideAttackType()
	{
		//if the player child cells become a threat to the enemy main cell, transition to the chargeChild state.
		//If the player child cells is not a threat to the enemy main cell, transition to the chargeMain state
		/*if(IsThereThreatToMain())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.ChargeChild, 0);
		}
		else
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.ChargeMain, 0);
		}*/
		MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.ChargeMain, 0);
	}
	
	//A function that a boolean to see whether there is any threat to the enemy main cell based on the amount
	//of total player cells and the amount of cells in each node
	private bool IsThereThreatToMain()
	{
		int PlayerTotalCells = GetSquadCellCount() + m_LeftNode.GetNodeChildList().Count + m_RightNode.GetNodeChildList().Count;
		int OwnTotalCells = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag).Length; //m_Main.GetComponent<EnemyMainFSM>().ECList.Count;
		
		if(PlayerTotalCells > OwnTotalCells)
		{
			return true;
		}
		else if (GetSquadCellCount() > 5 || m_LeftNode.GetNodeChildList().Count > 5 || m_RightNode.GetNodeChildList().Count > 5)
		{
			return true;
		}
		else
		{
			return false;
		}
	} 

	private int GetSquadCellCount()
	{
		return m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
	}
}
