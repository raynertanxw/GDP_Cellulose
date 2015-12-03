using UnityEngine;
using System.Collections;

public class ECAttackState : IECState {
	
	private Squad_Manager m_TopSquad;
	private Squad_Manager m_LeftSquad;
	private Squad_Manager m_RightSquad;
	
	public ECAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.eMain;
		
		m_TopSquad = GameObject.Find("Squad_Top").GetComponent<Squad_Manager>();
		m_LeftSquad = GameObject.Find("Squad_Left").GetComponent<Squad_Manager>();
		m_RightSquad = GameObject.Find("Squad_Right").GetComponent<Squad_Manager>();
	}
	
	public override void Enter()
	{
		Debug.Log("Enter Attack");
		DecideAttackType();
	}
	
	public override void Execute()
	{
		
	}
	
	public override void Exit()
	{
		
	}
	
	private void DecideAttackType()
	{
		//if player has cell squads, focus attacks on the cell squads
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
		int PlayerTotalCells = m_TopSquad.GetSquadChildList().Count + m_LeftSquad.GetSquadChildList().Count + m_RightSquad.GetSquadChildList().Count;
		int OwnTotalCells = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag).Length; //m_Main.GetComponent<EnemyMainFSM>().ECList.Count;
		
		if(PlayerTotalCells > OwnTotalCells)
		{
			return true;
		}
		else if (m_TopSquad.GetSquadChildList().Count > 5 || m_LeftSquad.GetSquadChildList().Count > 5 || m_RightSquad.GetSquadChildList().Count > 5)
		{
			return true;
		}
		else
		{
			return false;
		}
	} 
}
