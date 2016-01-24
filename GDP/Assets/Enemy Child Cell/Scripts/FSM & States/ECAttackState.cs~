using UnityEngine;
using System.Collections;

public class ECAttackState : IECState {

	//A GameObject reference to Squad Captain cell
	private static GameObject m_SquadCaptain;
	
	//Two GameObject references to player's left and right node
	private static Node_Manager m_LeftNode;
	private static Node_Manager m_RightNode;
	
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

	}
	
	public override void Execute()
	{
		/*When the enemy child cell had entered into this state, it decide what attack type the enemy child
		cell it should take based on the game environment and game agents, it will then transition the enemy child
		cell to another type of attack state*/
		
		DecideAttackType();
	}
	
	public override void Exit()
	{
	
	}
	
	private GameObject DetermineTargetToAttack()
	{
		int PMDesirability = 0;
		int PCDesirability = 0;
		int SQCDesirability = 0;
		
		int EMHealth = m_Main.GetComponent<EnemyMainFSM>().Health;
		int PMHealth = m_ecFSM.m_PMain.GetComponent<PlayerMain>().Health;
		int HealthDiff = EMHealth - PMHealth;
		int PlayerChildCount = PlayerChildFSM.GetActiveChildCount();
		int ChildCountDiff = m_Main.GetComponent<EnemyMainFSM>().AvailableChildNum - PlayerChildCount;
		int SquadChildCount = m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
		int PlayerDefence = PlayerChildFSM.GetActiveChildCount() + SquadChildCount;
		
		if(SquadChildCount <= 0)
		{
			SQCDesirability -= 2;
		}
		
		if(HealthDiff < 25 && HealthDiff > -25)
		{
			#region Difference between EC cell count and PC cell Count is within a range of 10
			if(ChildCountDiff < 10 && ChildCountDiff > -10)
			{
				if(SquadChildCount >= 5)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 2 && SquadChildCount < 5)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 10  && PlayerDefence <= 25)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC cell count is 10 to 25 more then PC cell count
			else if(ChildCountDiff >= 10 && ChildCountDiff < 25)
			{
				if(SquadChildCount >= 8)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 5 && SquadChildCount < 8)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 10  && PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability += 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 10 to 25 more than EC Cell Count
			else if(ChildCountDiff <= -10 && ChildCountDiff > -25)
			{
				if(SquadChildCount >= 2)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 0 && SquadChildCount < 2)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 10  && PlayerDefence <= 25)
				{
					PMDesirability += 1;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is 25 to 50 more than PC Cell Count
			else if(ChildCountDiff >= 25 && ChildCountDiff < 50)
			{
				if(SquadChildCount >= 10)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 6 && SquadChildCount < 10)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 25 to 50 more than EC Cell Count
			else if(ChildCountDiff <= -25 && ChildCountDiff > -50)
			{
				if(SquadChildCount >= 1)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 1;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count has at least 50 more than PC Cell count
			else if(ChildCountDiff >= 50)
			{
				if(SquadChildCount > 10)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 6 && SquadChildCount < 10)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count has at least 50 more than EC Cell Count
			else if(ChildCountDiff <= -50)
			{
				if(SquadChildCount > 0)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 1;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
			}
			#endregion
		}
		else if(HealthDiff >= 25 && HealthDiff < 50)
		{
			#region Difference between EC Cell Count and PC Cell Count is withing a range of 10
			if(ChildCountDiff < 10 && ChildCountDiff > -10)
			{
				if(SquadChildCount >= 8)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 5 && SquadChildCount < 8)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 10  && PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is 10 to 25 more than PC Cell Count
			else if(ChildCountDiff >= 10 && ChildCountDiff < 25)
			{
				if(SquadChildCount >= 10)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 6 && SquadChildCount < 10)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 15)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 15  && PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 10 to 25 more than EC Cell Count
			else if(ChildCountDiff <= -10 && ChildCountDiff > -25)
			{
				if(SquadChildCount >= 5)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 2 && SquadChildCount < 5)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 10  && PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is 25 to 50 more than PC Cell Count
			else if(ChildCountDiff >= 25 && ChildCountDiff < 50)
			{
				if(SquadChildCount >= 12)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 6 && SquadChildCount < 12)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 30)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 30 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 25 to 50 more than PC Cell Count
			else if(ChildCountDiff <= -25 && ChildCountDiff > -50)
			{
				if(SquadChildCount >= 2)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 10 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is at least 50 more than PC Cell Count
			else if(ChildCountDiff >= 50)
			{
				if(SquadChildCount > 12)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 8 && SquadChildCount < 12)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 35)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 35 && PlayerDefence < 50)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is at least 50 more than EC Cell Count
			else if(ChildCountDiff <= -50)
			{
				if(SquadChildCount > 0)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 1;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
			}
			#endregion
		}
		else if(HealthDiff <= -25 && HealthDiff > -50)
		{
			#region Difference between EC Cell Count and PC Cell Count is within a range of 10
			if(ChildCountDiff < 10 && ChildCountDiff > -10)
			{
				if(SquadChildCount >= 3)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 0 && SquadChildCount < 3)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 1;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 10  && PlayerDefence <= 25)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is 10 to 25 more than PC Cell Count
			else if(ChildCountDiff >= 10 && ChildCountDiff < 25)
			{
				if(SquadChildCount >= 5)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 2 && SquadChildCount < 5)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 15)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 15  && PlayerDefence <= 30)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 10 to 25 more than EC Cell Count
			else if(ChildCountDiff <= -10 && ChildCountDiff > -25)
			{
				if(SquadChildCount >= 2)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 0 && SquadChildCount < 2)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 10  && PlayerDefence <= 25)
				{
					PMDesirability -= 2;
					PCDesirability += 1;
				}
				else if(PlayerDefence > 25)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell COunt is 25 to 50 more than PC Cell Count
			else if(ChildCountDiff >= 25 && ChildCountDiff < 50)
			{
				if(SquadChildCount >= 10)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 5 && SquadChildCount < 10)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 30 && PlayerDefence < 50)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 25 to 50 more than EC Cell Count
			else if(ChildCountDiff <= -25 && ChildCountDiff > -50)
			{
				if(SquadChildCount >= 1)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence > 10 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is at least 50 more than PC Cell Count
			else if(ChildCountDiff >= 50)
			{
				if(SquadChildCount > 12)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 6 && SquadChildCount < 12)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is at least 50 more than EC Cell Count
			else if(ChildCountDiff <= -50)
			{
				if(SquadChildCount > 0)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence > 10 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
		}
		else if(HealthDiff >= 50 && HealthDiff < 75)
		{
			#region Difference between EC Cell Count and PC Cell Count is within a range of 10
			if(ChildCountDiff < 10 && ChildCountDiff > -10)
			{
				if(SquadChildCount >= 10)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 5 && SquadChildCount < 10)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 25  && PlayerDefence <= 50)
				{
					PMDesirability += 1;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is 10 to 25 more than PC Cell Count
			else if(ChildCountDiff >= 10 && ChildCountDiff < 25)
			{
				if(SquadChildCount >= 12)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 8 && SquadChildCount < 12)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 30)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 30  && PlayerDefence <= 50)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 10 to 25 more than EC Cell Count
			else if(ChildCountDiff <= -10 && ChildCountDiff > -25)
			{
				if(SquadChildCount >= 8)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 5 && SquadChildCount < 8)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 15)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 15  && PlayerDefence <= 25)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence > 25)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is 25 to 50 more than PC Cell Count
			else if(ChildCountDiff >= 25 && ChildCountDiff < 50)
			{
				if(SquadChildCount >= 10)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 5 && SquadChildCount < 10)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 30 && PlayerDefence < 50)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 25 to 50 more than EC Cell Count
			else if(ChildCountDiff <= -25 && ChildCountDiff > -50)
			{
				if(SquadChildCount >= 3)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 10 && PlayerDefence < 50)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is at least 50 more than PC Cell Count
			else if(ChildCountDiff >= 50)
			{
				if(SquadChildCount > 15)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 10 && SquadChildCount < 15)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 25)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 25 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is at least 50 more than EC Cell Count
			else if(ChildCountDiff <= -50)
			{
				if(SquadChildCount > 0)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 5)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 5 && PlayerDefence < 25)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 25 && PlayerDefence < 50)
				{
					PMDesirability -= 2;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
		}
		else if(HealthDiff <= -50 && HealthDiff > -75)
		{
			#region Difference between EC Cell Count and PC Cell Count is within a range of 10
			if(ChildCountDiff < 10 && ChildCountDiff > -10)
			{
				if(SquadChildCount >= 2)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 0 && SquadChildCount < 2)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 5)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 5  && PlayerDefence <= 25)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 25)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is 10 to 25 more than PC Cell Count
			else if(ChildCountDiff >= 10 && ChildCountDiff < 25)
			{
				if(SquadChildCount >= 5)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 0 && SquadChildCount < 5)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 10)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 10  && PlayerDefence <= 50)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 10 to 25 more than EC Cell Count
			else if(ChildCountDiff <= -10 && ChildCountDiff > -25)
			{
				if(SquadChildCount > 0)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 15)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence > 15)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is 25 to 50 more than PC Cell Count
			else if(ChildCountDiff >= 25 && ChildCountDiff < 50)
			{
				if(SquadChildCount >= 10)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 5 && SquadChildCount < 10)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 15)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 15 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 50 && PlayerDefence < 75)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is 25 to 50 more than EC Cell Count
			else if(ChildCountDiff <= -25 && ChildCountDiff > -50)
			{
				if(SquadChildCount >= 0)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 5)
				{
					PMDesirability += 2;
					PCDesirability -= 1;
				}
				else if(PlayerDefence > 5 && PlayerDefence < 25)
				{
					PMDesirability -= 1;
					PCDesirability += 2;
				}
				else if(PlayerDefence >= 25)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region EC Cell Count is at least 50 more than PC Cell Count
			else if(ChildCountDiff >= 50)
			{
				if(SquadChildCount > 15)
				{
					SQCDesirability += 2;
				}
				else if(SquadChildCount > 10 && SquadChildCount < 15)
				{
					SQCDesirability ++;
				}
				
				if(PlayerDefence <= 20)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 20 && PlayerDefence < 50)
				{
					PMDesirability += 1;
					PCDesirability -= 1;
				}
				else if(PlayerDefence >= 50)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
			#region PC Cell Count is at least 50 more than EC Cell Count
			else if(ChildCountDiff <= -50)
			{
				if(SquadChildCount > 0)
				{
					SQCDesirability += 2;
				}
				
				if(PlayerDefence <= 5)
				{
					PMDesirability += 2;
					PCDesirability -= 2;
				}
				else if(PlayerDefence > 5 && PlayerDefence < 10)
				{
					PMDesirability -= 1;
					PCDesirability += 1;
				}
				else if(PlayerDefence >= 10)
				{
					PMDesirability -= 2;
					PCDesirability += 2;
				}
			}
			#endregion
		}
		
		//Debug.Log("PCDesirability: " + PCDesirability);
		//Debug.Log("PMDesirability: " + PMDesirability);
		//Debug.Log("SQCDesirability: " + SQCDesirability);
		
		int HighestDesirability = Mathf.Max(PCDesirability,Mathf.Max(PMDesirability,SQCDesirability));
		if(PCDesirability == HighestDesirability){return (m_LeftNode.activeChildCount > m_RightNode.activeChildCount) ? m_LeftNode.gameObject : m_RightNode.gameObject;}
		if(PMDesirability == HighestDesirability){return m_ecFSM.m_PMain;}
		if(SQCDesirability == HighestDesirability){return m_SquadCaptain;}
		return m_ecFSM.m_PMain;
	}
	
	private void DecideAttackType()
	{
		GameObject Target = DetermineTargetToAttack();
		m_ecFSM.m_AttackTarget = Target;
		//Debug.Log("Target determined: " + m_ecFSM.m_AttackTarget.name);

		int PlayerDefence = PlayerChildFSM.GetActiveChildCount() + m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
		int ECToPCDifference = m_Main.GetComponent<EnemyMainFSM>().AvailableChildNum - PlayerChildFSM.GetActiveChildCount();
		
		if(Target.name.Contains("Node"))
		{
			int ChildCount = Target.GetComponent<Node_Manager>().activeChildCount;
		
			int CCDesirability = 0;
			int LMDesirability = 0;
			int TADesirability = 0;
			
			if(PlayerDefence < 20)
			{
				#region There are 0 - 20 child cells in that node
				if(ChildCount <= 20 && ChildCount > 0)
				{
					if(ECToPCDifference < 10 && ECToPCDifference  > -10)
					{
						CCDesirability += 2;
						TADesirability -= 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference >= 10)
					{
						CCDesirability += 1;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -10)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability += 2;
					}
				}
				#endregion
			}
			else if(PlayerDefence >= 20 && PlayerDefence < 50)
			{
				#region There are 0 - 15 child cells in that node
				if(ChildCount <= 15 && ChildCount > 0)
				{
					if(ECToPCDifference < 10 && ECToPCDifference  > -10)
					{
						CCDesirability += 2;
						TADesirability += 1;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference >= 10)
					{
						CCDesirability += 1;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -10)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability += 2;
					}
				}
				#endregion
				#region There are 16 - 25 child cells in that node
				else if(ChildCount <= 25 && ChildCount > 15)
				{
					if(ECToPCDifference < 10 && ECToPCDifference > -10)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference >= 10 && ECToPCDifference < 20)
					{
						CCDesirability += 1;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -10 && ECToPCDifference > -20)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 20)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability += 1;
					}
					else if(ECToPCDifference <= -20)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region There are 26 to 50 child cells in that node
				else if(ChildCount <= 50 && ChildCount > 25)
				{
					if(ECToPCDifference < 10 && ECToPCDifference  > -10)
					{
						CCDesirability += 1;
						TADesirability -= 2;
						LMDesirability += 1;
					}
					else if(ECToPCDifference >= 10 && ECToPCDifference < 20)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 1;
					}
					else if(ECToPCDifference <= -10 && ECToPCDifference > -20)
					{
						CCDesirability -= 1;
						TADesirability += 1;
						LMDesirability += 1;
					}
					else if(ECToPCDifference >= 20 && ECToPCDifference < 35)
					{
						CCDesirability += 2;
						TADesirability += 1;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -20 && ECToPCDifference < 35)
					{
						CCDesirability -= 2;
						TADesirability -= 1;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 35 && ECToPCDifference <= 50)
					{
						CCDesirability += 1;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -35 && ECToPCDifference >= -50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region There are more than 50 child cell in that node
				else if(ChildCount > 50)
				{
					if(ECToPCDifference < 10 && ECToPCDifference  > -10)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability += 1;
					}
					else if(ECToPCDifference >= 10 && ECToPCDifference < 20)
					{
						CCDesirability += 2;
						TADesirability += 1;
						LMDesirability -= 1;
					}
					else if(ECToPCDifference <= -10 && ECToPCDifference > -20)
					{
						CCDesirability += 1;
						TADesirability += 2;
						LMDesirability -= 1;
					}
					else if(ECToPCDifference >= 20 && ECToPCDifference < 35)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -20 && ECToPCDifference < 35)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 35 && ECToPCDifference <= 50)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -35 && ECToPCDifference >= -50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
			}
			else if(PlayerDefence >= 50)
			{
				#region There are 0 - 15 child cells in that node
				if(ChildCount <= 15 && ChildCount > 0)
				{
					if(ECToPCDifference < 10 && ECToPCDifference  > -10)
					{
						CCDesirability -= 2;
						TADesirability -= 1;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 10)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -10)
					{
						CCDesirability -= 1;
						TADesirability -= 1;
						LMDesirability += 2;
					}
				}
				#endregion
				#region There are 16 to 25 child cells in that node
				else if(ChildCount <= 25 && ChildCount > 15)
				{
					if(ECToPCDifference < 10 && ECToPCDifference > -10)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference >= 10 && ECToPCDifference < 20)
					{
						CCDesirability += 1;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -10 && ECToPCDifference > -20)
					{
						CCDesirability -= 1;
						TADesirability += 1;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 20)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability -= 1;
					}
					else if(ECToPCDifference <= -20)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
				}
				#endregion
				#region There are 26 to 50 child cells in that node
				else if(ChildCount <= 50 && ChildCount > 25)
				{
					if(ECToPCDifference < 10 && ECToPCDifference  > -10)
					{
						CCDesirability += 1;
						TADesirability += 2;
						LMDesirability -= 1;
					}
					else if(ECToPCDifference >= 10 && ECToPCDifference < 20)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 1;
					}
					else if(ECToPCDifference <= -10 && ECToPCDifference > -20)
					{
						CCDesirability -= 1;
						TADesirability += 1;
						LMDesirability += 1;
					}
					else if(ECToPCDifference >= 20 && ECToPCDifference < 35)
					{
						CCDesirability += 2;
						TADesirability += 1;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -20 && ECToPCDifference < 35)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 35 && ECToPCDifference <= 50)
					{
						CCDesirability += 1;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -35 && ECToPCDifference >= -50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region There are 50 or more child cells in the node
				else if(ChildCount > 50)
				{
					if(ECToPCDifference < 10 && ECToPCDifference  > -10)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 10 && ECToPCDifference < 20)
					{
						CCDesirability += 2;
						TADesirability -= 1;
						LMDesirability -= 1;
					}
					else if(ECToPCDifference <= -10 && ECToPCDifference > -20)
					{
						CCDesirability -= 1;
						TADesirability -= 2;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 20 && ECToPCDifference < 35)
					{
						CCDesirability += 2;
						TADesirability += 1;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -20 && ECToPCDifference < 35)
					{
						CCDesirability += 1;
						TADesirability += 1;
						LMDesirability += 2;
					}
					else if(ECToPCDifference >= 35 && ECToPCDifference <= 50)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(ECToPCDifference <= -35 && ECToPCDifference >= -50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
			}
			
			//Debug.Log("CCDesirability: " + CCDesirability);
			//Debug.Log("LMDesirability: " + LMDesirability);
			//Debug.Log("TADesirability: " + TADesirability);
			
			int HighestDesirability = Mathf.Max(CCDesirability,Mathf.Max(LMDesirability, TADesirability));
			if(CCDesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.ChargeChild,0);}// Debug.Log("Node - Change to CC");}
			if(LMDesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Landmine,0);}// Debug.Log("Node - Change to LM");}
			if(TADesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.TrickAttack,0);}// Debug.Log("Node - Change to TA");}
		}
		else if(Target.name.Contains("Player_Cell"))
		{
			int CMDesirability = 0;
			int LMDesirability = 0;
			int TADesirability = 0;
			
			int PHealth = m_ecFSM.m_PMain.GetComponent<PlayerMain>().Health;
			int EHPToPHPDiff = m_Main.GetComponent<EnemyMainFSM>().Health - PHealth;
			
			if(PlayerDefence < 20)
			{
				#region Player's Health is below 25
				if(PHealth < 25 && PHealth > 0)
				{
					if(EHPToPHPDiff >= 0 && EHPToPHPDiff <= 25)
					{
						CMDesirability += 2;
						LMDesirability -= 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff < 0 && EHPToPHPDiff >= -25)
					{
						CMDesirability += 1;
						LMDesirability += 1;
						TADesirability -= 2;
					}
				}
				#endregion
				#region Player Health is 25 to 50
				else if(PHealth >= 25 && PHealth < 50)
				{
					if(EHPToPHPDiff >= 0 && EHPToPHPDiff <= 25)
					{
						CMDesirability += 2;
						LMDesirability -= 2;
						TADesirability += 2;
					}
					else if(EHPToPHPDiff < 0 && EHPToPHPDiff >= -25)
					{
						CMDesirability += 1;
						LMDesirability -= 2;
						TADesirability += 1;
					}
					else if(EHPToPHPDiff > 25 && EHPToPHPDiff <= 50)
					{
						CMDesirability += 2;
						LMDesirability -= 1;
						TADesirability += 2;
					}
					else if(EHPToPHPDiff < -25 && EHPToPHPDiff >= -50)
					{
						CMDesirability += 1;
						LMDesirability += 2;
						TADesirability += 1;
					}
				}
				#endregion
				#region Player Health is greater than 50
				else if(PHealth >= 50)
				{
					if(EHPToPHPDiff >= -25 && EHPToPHPDiff <= 25)
					{
						CMDesirability += 2;
						LMDesirability -= 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff > 25 && EHPToPHPDiff <= 50)
					{
						CMDesirability += 2;
						LMDesirability -= 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff < -25 && EHPToPHPDiff >= -50)
					{
						CMDesirability += 1;
						LMDesirability += 2;
						TADesirability += 1;
					}
					else if(EHPToPHPDiff > 50)
					{
						CMDesirability -= 2;
						LMDesirability += 2;
						TADesirability += 2;
					}
					else if(EHPToPHPDiff < -50)
					{
						CMDesirability -= 2;
						LMDesirability += 2;
						TADesirability -= 2;
					}
				}
				#endregion
			}
			else if (PlayerDefence >= 20 && PlayerDefence < 50)
			{
				#region Player's Health is below 25
				if(PHealth < 25 && PHealth > 0)
				{
					if(EHPToPHPDiff >= 0 && EHPToPHPDiff <= 25)
					{
						CMDesirability += 2;
						LMDesirability += 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff < 0 && EHPToPHPDiff >= -25)
					{
						CMDesirability += 1;
						LMDesirability += 2;
						TADesirability -= 2;
					}
				}
				#endregion
				#region Player's Health is 25 to 50
				else if(PHealth >= 25 && PHealth < 50)
				{
					if(EHPToPHPDiff >= 0 && EHPToPHPDiff <= 25)
					{
						CMDesirability += 2;
						LMDesirability -= 2;
						TADesirability += 2;
					}
					else if(EHPToPHPDiff < 0 && EHPToPHPDiff >= -25)
					{
						CMDesirability += 1;
						LMDesirability += 1;
						TADesirability += 1;
					}
					else if(EHPToPHPDiff > 25 && EHPToPHPDiff <= 50)
					{
						CMDesirability += 2;
						LMDesirability -= 1;
						TADesirability += 2;
					}
					else if(EHPToPHPDiff < -25 && EHPToPHPDiff >= -50)
					{
						CMDesirability -= 1;
						LMDesirability += 2;
						TADesirability -= 1;
					}
				}
				#endregion
				#region Player's Health is 50 and aboive
				else if(PHealth >= 50)
				{
					if(EHPToPHPDiff >= -25 && EHPToPHPDiff <= 25)
					{
						CMDesirability += 2;
						LMDesirability -= 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff > 25 && EHPToPHPDiff <= 50)
					{
						CMDesirability += 1;
						LMDesirability -= 2;
						TADesirability += 1;
					}
					else if(EHPToPHPDiff < -25 && EHPToPHPDiff >= -50)
					{
						CMDesirability -= 1;
						LMDesirability += 1;
						TADesirability -= 1;
					}
					else if(EHPToPHPDiff > 50)
					{
						CMDesirability -= 2;
						LMDesirability += 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff < -50)
					{
						CMDesirability += 2;
						LMDesirability -= 2;
						TADesirability -= 2;
					}
				}
				#endregion
			}
			else if (PlayerDefence >= 50)
			{
				#region Player's Health is below 25
				if(PHealth < 25 && PHealth > 0)
				{
					if(EHPToPHPDiff >= 0 && EHPToPHPDiff <= 25)
					{
						CMDesirability += 1;
						LMDesirability += 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff < 0 && EHPToPHPDiff >= -25)
					{
						CMDesirability -= 1;
						LMDesirability += 2;
						TADesirability -= 2;
					}
				}
				#endregion
				#region Player's Health is 25 to 49
				else if(PHealth >= 25 && PHealth < 50)
				{
					if(EHPToPHPDiff >= 0 && EHPToPHPDiff <= 25)
					{
						CMDesirability += 1;
						LMDesirability += 2;
						TADesirability += 1;
					}
					else if(EHPToPHPDiff < 0 && EHPToPHPDiff >= -25)
					{
						CMDesirability -= 1;
						LMDesirability += 2;
						TADesirability -= 1;
					}
					else if(EHPToPHPDiff > 25 && EHPToPHPDiff <= 50)
					{
						CMDesirability -= 2;
						LMDesirability += 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff < -25 && EHPToPHPDiff >= -50)
					{
						CMDesirability -= 2;
						LMDesirability += 2;
						TADesirability -= 2;
					}
				}
				#endregion
				#region Player's Health is 50 and above
				else if(PHealth >= 50)
				{
					if(EHPToPHPDiff >= -25 && EHPToPHPDiff <= 25)
					{
						CMDesirability -= 1;
						LMDesirability += 2;
						TADesirability -= 1;
					}
					else if(EHPToPHPDiff > 25 && EHPToPHPDiff <= 50)
					{
						CMDesirability += 1;
						LMDesirability += 1;
						TADesirability += 1;
					}
					else if(EHPToPHPDiff < -25 && EHPToPHPDiff >= -50)
					{
						CMDesirability -= 2;
						LMDesirability += 2;
						TADesirability -= 2;
					}
					else if(EHPToPHPDiff > 50)
					{
						CMDesirability += 2;
						LMDesirability -= 2;
						TADesirability += 2;
					}
					else if(EHPToPHPDiff < -50)
					{
						CMDesirability -= 2;
						LMDesirability += 2;
						TADesirability -= 2;
					}
				}
				#endregion
			}
			
			//Debug.Log("CMDesirability: " + CMDesirability);
			//Debug.Log("LMDesirability: " + LMDesirability);
			//Debug.Log("TADesirability: " + TADesirability);
			
			int HighestDesirability = Mathf.Max(CMDesirability,Mathf.Max(LMDesirability,TADesirability));
			if(CMDesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.ChargeMain,0);}// Debug.Log("PlayerMain - Change to CM");}
			if(LMDesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Landmine,0);}// Debug.Log("PlayerMain - Change to LM");}
			if(TADesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.TrickAttack,0);}// Debug.Log("PlayerMain - Change to TA");}
		}
		else if(Target.name.Contains("Squad"))
		{
			int CCDesirability = 0;
			int LMDesirability = 0;
			int TADesirability = 0;
			
			int SquadChildCount = m_SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount();
			//int ECToPCDifference = m_Main.GetComponent<EnemyMainFSM>().AvailableChildNum - PlayerChildFSM.GetActiveChildCount();
			
			if(PlayerDefence < 20)
			{
				#region Enemy Child Count is 1 to 10 more than Player Child Count
				if(ECToPCDifference < 10 && ECToPCDifference  > 0)
				{
					if(SquadChildCount > 0 && SquadChildCount <= 20)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
				}
				#endregion
				#region Player Child Count is 1 to 10 more than Enemy Child Count
				else if(ECToPCDifference <= 0 && ECToPCDifference  > -10)
				{
					if(SquadChildCount > 0 && SquadChildCount <= 20)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
			}
			else if(PlayerDefence >= 20 && PlayerDefence < 50)
			{
				#region Enemy Child Count is 1 to 20 more than Player Child Count
				if(ECToPCDifference < 20 && ECToPCDifference  > 0)
				{
					if(SquadChildCount > 0 && SquadChildCount <= 25)
					{
						CCDesirability += 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability -= 2;
						TADesirability += 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region Player Child Count is 1 to 20 more than Enemy Child Count
				else if(ECToPCDifference <= 0 && ECToPCDifference  > -20)
				{
					if(SquadChildCount > 0 && SquadChildCount < 25)
					{
						CCDesirability -= 2;
						TADesirability += 2;
						LMDesirability += 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region Enemy Child Count is 20 to 49 more than Player Child Count
				else if(ECToPCDifference >= 20 && ECToPCDifference < 50)
				{
					if(SquadChildCount > 0 && SquadChildCount < 25)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability -= 2;
						TADesirability += 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region Player Child Count is 20 to 49 more than Enemy Child Count
				else if(ECToPCDifference <= -20 && ECToPCDifference > -50)
				{
					if(SquadChildCount > 0 && SquadChildCount < 25)
					{
						CCDesirability -= 2;
						TADesirability += 2;
						LMDesirability += 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
			}
			else if(PlayerDefence >= 50)
			{
				#region Enemy Child Count is 1 to 20 more than Player Child Count
				if(ECToPCDifference < 20 && ECToPCDifference  > 0)
				{
					if(SquadChildCount > 0 && SquadChildCount <= 25)
					{
						CCDesirability += 2;
						TADesirability -= 2;
						LMDesirability -= 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region Player Child Count is 1 to 20 more than Player Child Count
				else if(ECToPCDifference <= 0 && ECToPCDifference  > -20)
				{
					if(SquadChildCount > 0 && SquadChildCount < 25)
					{
						CCDesirability -= 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability += 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region Enemy Child Count is 20 to 49 more than Player Child Count
				else if(ECToPCDifference >= 20 && ECToPCDifference < 50)
				{
					if(SquadChildCount > 0 && SquadChildCount < 25)
					{
						CCDesirability += 2;
						TADesirability -= 2;
						LMDesirability -= 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
				}
				#endregion
				#region Player Child Count is 20 to 49 more than Enemy Child Count
				else if(ECToPCDifference <= -20 && ECToPCDifference > -50)
				{
					if(SquadChildCount > 0 && SquadChildCount < 25)
					{
						CCDesirability += 2;
						TADesirability -= 2;
						LMDesirability -= 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
				#region Enemy Child Count is at least 50 more than Player Child Count
				else if(ECToPCDifference >= 50)
				{
					if(SquadChildCount > 0 && SquadChildCount < 25)
					{
						CCDesirability += 2;
						TADesirability += 2;
						LMDesirability -= 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability += 2;
						TADesirability -= 2;
						LMDesirability -= 2;
					}
				}
				#endregion
				#region Player Child Count is at least 50 more than Enemy Child Count
				else if(ECToPCDifference <= -50)
				{
					if(SquadChildCount > 0 && SquadChildCount < 25)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
					else if(SquadChildCount >= 25 && SquadChildCount <= 50)
					{
						CCDesirability -= 2;
						TADesirability -= 2;
						LMDesirability += 2;
					}
				}
				#endregion
			}
			
			//Debug.Log("CCDesirability: " + CCDesirability);
			//Debug.Log("LMDesirability: " + LMDesirability);
			//Debug.Log("TADesirability: " + TADesirability);
			
			int HighestDesirability = Mathf.Max(CCDesirability,Mathf.Max(LMDesirability,TADesirability));
			if(CCDesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.ChargeChild,0);}//Debug.Log("Squad - Change to CC");}
			if(LMDesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.Landmine,0);}//Debug.Log("Squad - Change to LM");}
			if(TADesirability == HighestDesirability){MessageDispatcher.Instance.DispatchMessage(m_Child,m_Child,MessageType.TrickAttack,0);}//Debug.Log("Squad - Change to TA");}
		}
		
		/*if the player child cells become a threat to the enemy main cell, transition to the chargeChild state.
		If the player child cells is not a threat to the enemy main cell, transition to the chargeMain state*/
		
		/*if(IsThereThreatToMain())
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.ChargeChild, 0);
		}
		else
		{
			MessageDispatcher.Instance.DispatchMessage(m_Child, m_ecFSM.gameObject, MessageType.ChargeMain, 0);
		}*/
	}
	
	/*A function that a boolean to see whether there is any threat to the enemy main cell based on the amount
	of total player cells and the amount of cells in each node*/
	private bool IsThereThreatToMain()
	{
		int PlayerTotalCells = GetSquadCellCount() + m_LeftNode.activeChildCount + m_RightNode.activeChildCount;
		int OwnTotalCells = GameObject.FindGameObjectsWithTag(Constants.s_strPlayerChildTag).Length;
		
		if(PlayerTotalCells > OwnTotalCells)
		{
			return true;
		}
		else if (GetSquadCellCount() > 5 || m_LeftNode.activeChildCount > 5 || m_RightNode.activeChildCount > 5)
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
