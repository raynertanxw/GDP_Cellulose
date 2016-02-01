using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionQuery
{
	//Declare an instance of PositionQuery for Singleton purpose
	private static PositionQuery s_Instance;
	private GameObject PlayerMain;
	private GameObject EnemyMain;
	private GameObject SquadCaptain;
	
	//Constructor for PositionQuery
	public PositionQuery()
	{
		PlayerMain = GameObject.Find("Player_Cell");
		EnemyMain = GameObject.Find("Enemy_Cell");
		SquadCaptain = GameObject.Find("Squad_Captain_Cell");
	}
	
	//Singleton and Getter function for Position Query
	public static PositionQuery Instance
	{
		get
		{
			if(s_Instance == null)
			{
				s_Instance = new PositionQuery();
			}
			return s_Instance;
		}
	}
	
	private bool IsAllThreatEmpty ()
	{
		List<GameObject> Threats = new List<GameObject>();
		Threats.Add(Node_Manager.GetNode(Node.LeftNode).gameObject);
        Threats.Add(Node_Manager.GetNode(Node.RightNode).gameObject);
		Threats.Add(GameObject.Find("Squad_Captain_Cell"));

		for(int i = 0; i < Threats.Count; i++)
		{
			if(Threats[i].GetComponent<Node_Manager>() != null && Threats[i].GetComponent<Node_Manager>().activeChildCount > 0)
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
	
	//A function that evaluate all the nodes of the player and return the most threatening node
	private GameObject GetMostThreateningThreat()
	{
		List<GameObject> Threats = new List<GameObject>();
		Threats.Add(Node_Manager.GetNode(Node.LeftNode).gameObject);
		Threats.Add(Node_Manager.GetNode(Node.RightNode).gameObject);
		Threats.Add(GameObject.Find("Squad_Captain_Cell"));
		
		int nIndexForMostThreating = 0;
		int nHighestScore = 0;
		
		for(int i = 0; i < Threats.Count; i++)
		{
			if(Threats[i].name.Contains("Node") && EvaluateNode(Threats[i]) > nHighestScore)
			{
				nIndexForMostThreating = i;
				nHighestScore = EvaluateNode(Threats[i]);
			}
			else if(Threats[i].name.Contains("Squad") && Threats[i].GetComponent<PlayerSquadFSM>().AliveChildCount() > nHighestScore)
			{
				nIndexForMostThreating = i;
				nHighestScore = Threats[i].GetComponent<PlayerSquadFSM>().AliveChildCount();
			}
		}

		return Threats[nIndexForMostThreating];
	}
	
	//A function that evaluate all the nodes of the player and return the most weak node
	private GameObject GetMostWeakNode ()
	{
		List<GameObject> Threats = new List<GameObject>();
		Threats.Add(Node_Manager.GetNode(Node.LeftNode).gameObject);
		Threats.Add(Node_Manager.GetNode(Node.RightNode).gameObject);
		Threats.Add(GameObject.Find("Squad_Captain_Cell"));
		int nIndexForMostWeak = 0;
		int nLowestScore = 999;
		
		for(int i = 0; i < Threats.Count; i++)
		{
			if(Threats[i].name.Contains("Node") && EvaluateNode(Threats[i]) < nLowestScore)
			{
				nIndexForMostWeak = i;
				nLowestScore = EvaluateNode(Threats[i]);
			}
			else if(Threats[i].name.Contains("Squad") && Threats[i].GetComponent<PlayerSquadFSM>().AliveChildCount() < nLowestScore)
			{
				nIndexForMostWeak = i;
				nLowestScore = Threats[i].GetComponent<PlayerSquadFSM>().AliveChildCount();
			}
		}
		
		return Threats[nIndexForMostWeak];
	}
	
	//a function that evalute the given node and return a score
	private int EvaluateNode (GameObject _Node)
	{
		//if the node contains no cell, it serve no threat to the enemy main cell
		if(_Node.GetComponent<Node_Manager>().activeChildCount == 0)
		{
			return 0;
		}
		
		int nthreatLevel = 0;
		
		//increase score based on amount of cells in that node
		nthreatLevel += _Node.GetComponent<Node_Manager>().activeChildCount;
		
		//increase score if that node have formed together and has a node captain
		if(_Node.GetComponent<PlayerSquadFSM>() != null)
		{
			nthreatLevel+= 50;
		}
		
		return nthreatLevel;
	}
	
	private List<Vector2> GetPointRangeBetweenPlayerEnemy()
	{
		Point PlayerClosestPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
		Point EnemyClosestPoint = PointDatabase.Instance.GetClosestPointToPosition(EnemyMain.transform.position,false);
		
		float PosDifferenceX = EnemyClosestPoint.Position.x - PlayerClosestPoint.Position.x;
		float PosDifferenceY = EnemyClosestPoint.Position.y - PlayerClosestPoint.Position.y;
		
		int PointDifferenceX = Mathf.RoundToInt(PosDifferenceX/PointDatabase.Instance.PointIntervalX);
		int PointDifferenceY = Mathf.RoundToInt(PosDifferenceY/PointDatabase.Instance.PointIntervalY);
		
		List<Vector2> PointRange = new List<Vector2>();
		PointRange.Add(new Vector2(Mathf.RoundToInt(0.6f * PointDifferenceX), Mathf.RoundToInt(0.6f * PointDifferenceY)));
		PointRange.Add(new Vector2(Mathf.RoundToInt(0.5f * PointDifferenceX), Mathf.RoundToInt(0.5f * PointDifferenceY)));
		PointRange.Add(new Vector2(Mathf.RoundToInt(0.25f * PointDifferenceX), Mathf.RoundToInt(0.25f * PointDifferenceY)));
		
		return PointRange;
	}
	
	private Point GetPointAfterMoving(Point CurrentPoint, int ChangeInX, int ChangeInY, string YDirection)
	{
		if(YDirection == "Up" || YDirection == "up")
		{
			for(int i = 0; i < ChangeInY; i++)
			{
				CurrentPoint = PointDatabase.Instance.GetPointNextToGivenPoint("Up", CurrentPoint);
			}
		}
		else
		{
			for(int i = 0; i < Mathf.Abs(ChangeInY); i++)
			{
				CurrentPoint = PointDatabase.Instance.GetPointNextToGivenPoint("Down", CurrentPoint);
			}
			
		}
		
		if(ChangeInX >= 0)
		{
			for(int i = 0; i < ChangeInX; i++)
			{
				CurrentPoint = PointDatabase.Instance.GetPointNextToGivenPoint("Right", CurrentPoint);
			}
		}
		else
		{
			for(int i = 0; i < Mathf.Abs(ChangeInX); i++)
			{
				CurrentPoint = PointDatabase.Instance.GetPointNextToGivenPoint("Left", CurrentPoint);
			}
		}
		
		return CurrentPoint;
	}
	
	public Vector2 InduceNoiseToPosition (Vector2 _TargetPos)
	{
		float fRandomXNeg = Random.Range(-0.5f, 0f);
		float fRandomXPos = Random.Range(0f, 0.5f);
		float fRandomYNeg = Random.Range(-0.5f, 0f);
		float fRandomYPos = Random.Range(0f, 0.75f);
		
		float fNoiseToX = Random.Range(fRandomXNeg,fRandomXPos);
		float fNoiseToY = Random.Range(fRandomYNeg,fRandomYPos);
		Vector2 RandomResult = new Vector2(_TargetPos.x + fNoiseToX, _TargetPos.y + fNoiseToY);
		
		return RandomResult;
	}
	
	public GameObject GetLandmineTarget(PositionType PType, GameObject Agent)
	{
		GameObject target = null;
		
		if(PType == PositionType.Aggressive)
		{
			if(IsAllThreatEmpty())
			{
				return PlayerMain;
			}
			else
			{
				if(SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount() > 0)
				{
					return SquadCaptain;
				}
				else
				{
					return PlayerMain;
				}
			}
		}
		else if(PType == PositionType.Defensive)
		{
			if(IsAllThreatEmpty())
			{
				return PlayerMain;
			}
			else
			{
				return GetMostThreateningThreat();
			}
		}
		else if(PType == PositionType.Neutral)
		{
			return PlayerMain;
		}
		
		return null;
	}
	
	public Vector2 GetLandminePos(RangeValue Range, PositionType PType, GameObject Agent)
	{
		Vector2 targetPos = Vector2.zero;
		
		if(PType == PositionType.Aggressive)
		{
			if(IsAllThreatEmpty())
			{
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
				targetPos = CurrentPoint.Position;
			}
			else
			{
				Point CurrentPoint = null;
				if(SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount() > 0)
				{
					CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(SquadCaptain.transform.position,false);
				}
				else
				{
					CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
				}
				targetPos = CurrentPoint.Position;
			}
		}
		else if(PType == PositionType.Defensive)
		{
			if(IsAllThreatEmpty())
			{
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
				targetPos = CurrentPoint.Position;
			}
			else
			{
				GameObject Target = GetMostThreateningThreat();
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(Target.transform.position,false);
				targetPos = CurrentPoint.Position;
			}
		}
		else if(PType == PositionType.Neutral)
		{
			Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
			targetPos = CurrentPoint.Position;
		}
		
	   // Utility.DrawCross(targetPos,Color.green,0.5f);
		return targetPos;
	}

	public Formation GetDefensiveFormation()
	{
		// Factors - Types of Attack, Amount of Defenders, Attack Source
		float QCDesirability = 0;
		float RCDesirability = 0;
		float TurtleDesirability = 0;
		float LadderDesirability = 0;
		
		PlayerAttackMode AttackToDefendAgainst = GetMostSignificantAttack();
		int DefendingCellsCount = ECTracker.Instance.DefendCells.Count;
		GameObject AttackSource = GetAttackSource(AttackToDefendAgainst);

		if(AttackSource == null){return Formation.ReverseCircular;}
		
		float EnemyToAttackSourceXDifference = GameObject.Find("Enemy_Cell").transform.position.x - AttackSource.transform.position.x;
		//Player left node x - -3f Player Right Node - 3f
		
		if(AttackToDefendAgainst == PlayerAttackMode.BurstShot)
		{
			if(EnemyToAttackSourceXDifference >= 1.5f || EnemyToAttackSourceXDifference <= -1.5f)
			{
				if(DefendingCellsCount <= 20)
				{
					TurtleDesirability += 1;
				}
				else
				{
					RCDesirability += 1;
				}
			}
			else
			{
				if(DefendingCellsCount <= 20)
				{
					TurtleDesirability += 1;
				}
				else if(DefendingCellsCount <= 20 && DefendingCellsCount > 10)
				{
					RCDesirability += 1;
				}
				else
				{
					QCDesirability += 1;
				}
			}
		}
		else if(AttackToDefendAgainst == PlayerAttackMode.SwarmTarget)
		{
			if(EnemyToAttackSourceXDifference >= 1.5f || EnemyToAttackSourceXDifference <= -1.5f)
			{
				if(DefendingCellsCount <= 10)
				{
					QCDesirability += 1;
				}
				else if(DefendingCellsCount <= 20 && DefendingCellsCount > 10)
				{
					QCDesirability += 1;
				}
				else
				{
					RCDesirability += 1;
				}
			}
			else
			{
				QCDesirability += 1;
			}
		}
		else if(AttackToDefendAgainst == PlayerAttackMode.ScatterShot)
		{
			if(EnemyToAttackSourceXDifference >= 1.5f || EnemyToAttackSourceXDifference <= -1.5f)
			{
				if(DefendingCellsCount <= 10)
				{
					LadderDesirability += 1;
				}
				else if(DefendingCellsCount <= 20 && DefendingCellsCount > 10)
				{
					TurtleDesirability += 1;
				}
				else
				{
					QCDesirability += 1;
				}
			}
			else
			{
				TurtleDesirability += 1;
			}
		}
		
		QCDesirability *= Settings.s_fEnemyDefendQCWeight;
		RCDesirability *= Settings.s_fEnemyDefendRCWeight;
		TurtleDesirability *= Settings.s_fEnemyDefendTurtleWeight;
		LadderDesirability *= Settings.s_fEnemyDefendLadderWeight;
		
		float HighestDesirability = Mathf.Max(QCDesirability,Mathf.Max(RCDesirability,Mathf.Max(TurtleDesirability,LadderDesirability)));
		if(HighestDesirability == QCDesirability){return Formation.QuickCircle;}
		if(HighestDesirability == RCDesirability){return Formation.ReverseCircular;}
		if(HighestDesirability == TurtleDesirability){return Formation.Turtle;}
		if(HighestDesirability == LadderDesirability){return Formation.Ladder;}
		
		return Formation.ReverseCircular;
	}

	public PlayerAttackMode GetMostSignificantAttack()
	{
		float BurstCount = 0;
		float SwarmCount = 0;
		float ScatterCount = 0;
		
		for(int i = 0; i < PlayerChildFSM.childrenInAttack.Length - 1; i++)
		{
			if(PlayerChildFSM.childrenInAttack[i] == -1){break;}
			
			if(PlayerChildFSM.playerChildPool[PlayerChildFSM.childrenInAttack[i]].attackMode == PlayerAttackMode.BurstShot)
			{
				BurstCount++;
			}
			else if(PlayerChildFSM.playerChildPool[PlayerChildFSM.childrenInAttack[i]].attackMode == PlayerAttackMode.SwarmTarget)
			{
				SwarmCount++;
			}
			else if(PlayerChildFSM.playerChildPool[PlayerChildFSM.childrenInAttack[i]].attackMode == PlayerAttackMode.ScatterShot)
			{
				ScatterCount++;
			}
		}
		
		/*for(int i = 0; i < PlayerChildFSM.playerChildPool.Length; i++)
		{
			if(PlayerChildFSM.s_playerChildStatus[i] == pcStatus.Attacking)
			{
				if(PlayerChildFSM.playerChildPool[i].attackMode == PlayerAttackMode.BurstShot)
				{
					BurstCount++;
				}
				else if(PlayerChildFSM.playerChildPool[i].attackMode == PlayerAttackMode.SwarmTarget)
				{
					SwarmCount++;
				}
				else if(PlayerChildFSM.playerChildPool[i].attackMode == PlayerAttackMode.ScatterShot)
				{
					ScatterCount++;
				}
			}
		}*/
		
		BurstCount *= Settings.s_fEnemyDefendBurstSignificancy;
		SwarmCount *= Settings.s_fEnemyDefendSwarmSignificancy;
		ScatterCount *= Settings.s_fEnemyDefendScatterSignificancy;
		
		float HighestCount = Mathf.Max(BurstCount, Mathf.Max(SwarmCount,ScatterCount));
		if(HighestCount == BurstCount){return PlayerAttackMode.BurstShot;}
		if(HighestCount == SwarmCount){return PlayerAttackMode.SwarmTarget;}
		if(HighestCount == ScatterCount){return PlayerAttackMode.ScatterShot;}
		
		return PlayerAttackMode.ScatterShot;
	}
	
	private GameObject GetAttackSource(PlayerAttackMode _Attack)
	{
		for(int i = 0; i < PlayerChildFSM.childrenInAttack.Length - 1; i++)
		{
			if(PlayerChildFSM.childrenInAttack[i] == -1){break;}
			
			if(PlayerChildFSM.playerChildPool[PlayerChildFSM.childrenInAttack[i]].attackMode == _Attack)
			{
				return PlayerChildFSM.playerChildPool[PlayerChildFSM.childrenInAttack[i]].gameObject;
			}
		}
	
		/*for(int i = 0; i < PlayerChildFSM.playerChildPool.Length; i++)
		{
			if(PlayerChildFSM.playerChildPool[i].attackMode == _Attack)
			{
				return PlayerChildFSM.playerChildPool[i].gameObject;
			}
		}*/
		return null;
	}

	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
