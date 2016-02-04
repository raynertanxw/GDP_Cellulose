using UnityEngine;
using System.Collections;

public class ECAttackState : IECState {

	//A GameObject reference to Squad Captain cell
	private static GameObject m_SquadCaptain;
	
	//Two GameObject references to player's left and right node
	private static Node_Manager m_LeftNode;
	private static Node_Manager m_RightNode;
	
	private static int m_nEnemyMaxHP;
	private static float m_fEnemyAggressiveness;
	
	private static int m_nPlayerTotalCellCount;
	private static int m_nLeftNodeCellCount;
	private static int m_nRightNodeCellCount;
	private static int m_nSquadCaptCount;
	private static int m_nEnemyTotalCellCount;
	
	private static GameObject m_CurrentAttackTarget;
	private static string m_strCurrentAttackMethod;
	
	//Constructor
	public ECAttackState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;

		m_SquadCaptain = GameObject.Find("Squad_Captain_Cell");
		m_LeftNode = Node_Manager.GetNode(Node.LeftNode);
		m_RightNode = Node_Manager.GetNode(Node.RightNode);
	}
	
	public override void Enter()
	{
		ECTracker.Instance.AttackingCells.Add(m_ecFSM);
		
		m_nEnemyMaxHP = m_Main.GetComponent<EnemyMainFSM>().Health;
		m_fEnemyAggressiveness = m_Main.GetComponent<EnemyMainFSM>().CurrentAggressiveness;
		
		m_nLeftNodeCellCount = m_LeftNode.activeChildCount;
		m_nRightNodeCellCount = m_RightNode.activeChildCount;
		
		m_nSquadCaptCount = m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
		
		m_nPlayerTotalCellCount = m_nLeftNodeCellCount + m_nRightNodeCellCount + m_nSquadCaptCount;
		m_nEnemyTotalCellCount = m_Main.GetComponent<EnemyMainFSM>().AvailableChildNum;
	}
	
	public override void Execute()
	{
		/*When the enemy child cell had entered into this state, it decide what attack type the enemy child
		cell it should take based on the game environment and game agents, it will then transition the enemy child
		cell to another type of attack state*/
		
		if(m_CurrentAttackTarget == null)
		{
			m_CurrentAttackTarget = DetermineTargetToAttack();
			m_strCurrentAttackMethod = DecideAttackType(m_CurrentAttackTarget);
		}

		m_ecFSM.m_AttackTarget = m_CurrentAttackTarget;
		if(m_strCurrentAttackMethod == "ChargeMain"){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.ChargeMain,0);}
		if(m_strCurrentAttackMethod == "ChargeChild"){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.ChargeChild,0);}
		if(m_strCurrentAttackMethod == "Landmine"){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Landmine,0);}
		if(m_strCurrentAttackMethod == "TrickAttack"){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.TrickAttack,0);}
	}
	
	public override void Exit()
	{
		if(ECTracker.Instance.AttackingCells.Count <= 1){m_CurrentAttackTarget = null; m_strCurrentAttackMethod = "";}
		ECTracker.Instance.AttackingCells.Remove(m_ecFSM);
	}
	
	private GameObject DetermineTargetToAttack()
	{
		if(IsThereThreatToMain())
		{
			float LeftNodeThreatLevel = 0;
			float RightNodeThreatLevel = 0;
			float SquadCaptThreatLevel = 0;
			
			LeftNodeThreatLevel += m_nLeftNodeCellCount - Settings.s_fEnemyTargetLeftNodeRequirement * m_nEnemyMaxHP;
			if(m_fEnemyAggressiveness >= 12){LeftNodeThreatLevel *= 1.15f;}
			
			RightNodeThreatLevel += m_nRightNodeCellCount - Settings.s_fEnemyTargetRightNodeRequirement * m_nEnemyMaxHP;
			if(m_fEnemyAggressiveness >= 12){RightNodeThreatLevel *= 1.15f;}
			
			SquadCaptThreatLevel += m_nSquadCaptCount;
			if(m_fEnemyAggressiveness <= 6){SquadCaptThreatLevel *= 1.25f;}
			
			if(m_nSquadCaptCount > m_nEnemyMaxHP * 0.75f * Settings.s_fEnemyTargetSquadCaptRequirement){return m_SquadCaptain;}
			
			float HighestThreatLevel = Mathf.Max(SquadCaptThreatLevel,Mathf.Max(LeftNodeThreatLevel,RightNodeThreatLevel));
			if(HighestThreatLevel == LeftNodeThreatLevel){return m_LeftNode.gameObject;}
			if(HighestThreatLevel == RightNodeThreatLevel){return m_RightNode.gameObject;}
			if(HighestThreatLevel == SquadCaptThreatLevel){return m_SquadCaptain;}
			
			return m_ecFSM.m_PMain;
		}
		return m_ecFSM.m_PMain;
	}
	
	private string DecideAttackType(GameObject _Target)
	{
		if(_Target.name == "Player_Cell")
		{
			if(m_nPlayerTotalCellCount >= 5f * Settings.s_fEnemyAttackLandmineRequirement * m_nEnemyMaxHP)
			{
				return "Landmine";
			}
			
			if((m_nLeftNodeCellCount <= 4 * Settings.s_fEnemyAttackTrickAttackRequirement * m_nEnemyMaxHP || m_nRightNodeCellCount <= 4 * Settings.s_fEnemyAttackTrickAttackRequirement * m_nEnemyMaxHP) && (m_nPlayerTotalCellCount > 8 * Settings.s_fEnemyAttackTrickAttackRequirement * m_nEnemyMaxHP))
			{
				return "TrickAttack";
			}
			return "ChargeMain";
		}
		else if(_Target.name.Contains("Node"))
		{
			if(_Target.GetComponent<Node_Manager>().activeChildCount >= 4 * Settings.s_fEnemyAttackLandmineRequirement * m_nEnemyMaxHP)
			{
				return "Landmine";
			}
			
			int RandomMove = Random.Range(0,3);
			if(RandomMove == 0 || RandomMove == 1){return "ChargeChild";}
			if(RandomMove == 2){return "TrickAttack";}
		}
		else if(_Target.name.Contains("Squad"))
		{
			m_ecFSM.m_AttackTarget = _Target;
			
			if(m_nSquadCaptCount >= Settings.s_fEnemyAttackLandmineRequirement * m_nEnemyMaxHP)
			{
				return "Landmine";
			}
			
			int RandomMove = Random.Range(0,3);
			if(RandomMove == 0 || RandomMove == 1){return "ChargeChild";}
			if(RandomMove == 2){return "TrickAttack";}
		}
		
		return "Player_Cell";
	}
	
	/*A function that a boolean to see whether there is any threat to the enemy main cell based on the amount
	of total player cells and the amount of cells in each node*/
	private bool IsThereThreatToMain()
	{
		if(m_nPlayerTotalCellCount > m_nEnemyTotalCellCount)
		{
			return true;
		}
		else if(m_nLeftNodeCellCount > m_nEnemyMaxHP * Settings.s_fEnemyTargetLeftNodeRequirement || m_nRightNodeCellCount > m_nEnemyMaxHP * Settings.s_fEnemyTargetRightNodeRequirement)
		{
			return true;
		}
		else if(m_nSquadCaptCount > m_nEnemyMaxHP * 0.6f * Settings.s_fEnemyTargetSquadCaptRequirement)
		{
			return true;
		}
		return false;
	} 
}
