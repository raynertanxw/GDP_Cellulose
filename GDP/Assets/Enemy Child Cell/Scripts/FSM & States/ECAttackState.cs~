using UnityEngine;
using System.Collections;

public class ECAttackState : IECState {

	//A GameObject reference to Squad Captain cell
	private static GameObject m_SquadCaptain;
	
	//Two GameObject references to player's left and right node
	private static Node_Manager m_LeftNode;
	private static Node_Manager m_RightNode;
	
	private static int EnemyMaxHP;
	private static float EnemyAggressiveness;
	
	private static int PlayerTotalCellCount;
	private static int LeftNodeCellCount;
	private static int RightNodeCellCount;
	private static int SquadCaptCount;
	private static int EnemyTotalCellCount;
	
	private static GameObject CurrentAttackTarget;
	private static string CurrentAttackMethod;
	
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
		EnemyMaxHP = 50;//m_Main.GetComponent<EnemyMainFSM>().Health;
		EnemyAggressiveness = m_Main.GetComponent<EnemyMainFSM>().CurrentAggressiveness;
		
		LeftNodeCellCount = m_LeftNode.activeChildCount;
		RightNodeCellCount = m_RightNode.activeChildCount;
		SquadCaptCount = m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
		PlayerTotalCellCount = LeftNodeCellCount + RightNodeCellCount + SquadCaptCount;
		EnemyTotalCellCount = m_Main.GetComponent<EnemyMainFSM>().AvailableChildNum;
	}
	
	public override void Execute()
	{
		/*When the enemy child cell had entered into this state, it decide what attack type the enemy child
		cell it should take based on the game environment and game agents, it will then transition the enemy child
		cell to another type of attack state*/
		
		if(CurrentAttackTarget == null)
		{
			CurrentAttackTarget = DetermineTargetToAttack();
			CurrentAttackMethod = DecideAttackType(CurrentAttackTarget);
		}

		m_ecFSM.m_AttackTarget = CurrentAttackTarget;
		if(CurrentAttackMethod == "ChargeMain"){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.ChargeMain,0);}
		if(CurrentAttackMethod == "ChargeChild"){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.ChargeChild,0);}
		if(CurrentAttackMethod == "Landmine"){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Landmine,0);}
		if(CurrentAttackMethod == "TrickAttack"){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.TrickAttack,0);}
	}
	
	public override void Exit()
	{
		if(ECTracker.Instance.AttackingCells.Count <= 1){CurrentAttackTarget = null; CurrentAttackMethod = "";}
		ECTracker.Instance.AttackingCells.Remove(m_ecFSM);
	}
	
	private GameObject DetermineTargetToAttack()
	{
		if(IsThereThreatToMain())
		{
			float LeftNodeThreatLevel = 0;
			float RightNodeThreatLevel = 0;
			float SquadCaptThreatLevel = 0;
			
			LeftNodeThreatLevel += LeftNodeCellCount - Settings.s_fEnemyTargetLeftNodeRequirement * EnemyMaxHP;
			if(EnemyAggressiveness >= 12){LeftNodeThreatLevel *= 1.15f;}
			
			RightNodeThreatLevel += RightNodeCellCount - Settings.s_fEnemyTargetRightNodeRequirement * EnemyMaxHP;
			if(EnemyAggressiveness >= 12){RightNodeThreatLevel *= 1.15f;}
			
			SquadCaptThreatLevel += SquadCaptCount;
			if(EnemyAggressiveness <= 6){SquadCaptThreatLevel *= 1.25f;}
			
			if(SquadCaptCount > EnemyMaxHP * 0.75f * Settings.s_fEnemyTargetSquadCaptRequirement){return m_SquadCaptain;}
			
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
			if(PlayerTotalCellCount >= 5f * Settings.s_fEnemyAttackLandmineRequirement * EnemyMaxHP)
			{
				return "Landmine";
			}
			
			if((LeftNodeCellCount <= 4 * Settings.s_fEnemyAttackTrickAttackRequirement * EnemyMaxHP || RightNodeCellCount <= 4 * Settings.s_fEnemyAttackTrickAttackRequirement * EnemyMaxHP) && (PlayerTotalCellCount > 8 * Settings.s_fEnemyAttackTrickAttackRequirement * EnemyMaxHP))
			{
				return "TrickAttack";
			}
			return "ChargeMain";
		}
		else if(_Target.name.Contains("Node"))
		{
			if(_Target.GetComponent<Node_Manager>().activeChildCount >= 4 * Settings.s_fEnemyAttackLandmineRequirement * EnemyMaxHP)
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
			
			if(SquadCaptCount >= Settings.s_fEnemyAttackLandmineRequirement * EnemyMaxHP)
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
		if(PlayerTotalCellCount > EnemyTotalCellCount)
		{
			return true;
		}
		else if(LeftNodeCellCount > EnemyMaxHP * Settings.s_fEnemyTargetLeftNodeRequirement || RightNodeCellCount> EnemyMaxHP * Settings.s_fEnemyTargetRightNodeRequirement)
		{
			return true;
		}
		else if(SquadCaptCount > EnemyMaxHP * 0.6f * Settings.s_fEnemyTargetSquadCaptRequirement)
		{
			return true;
		}
		return false;
	} 

	private int GetSquadCellCount()
	{
		return m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
	}
}
