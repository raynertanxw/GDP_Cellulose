using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionQuery
{
	//Declare an instance of PositionQuery for Singleton purpose
	private static PositionQuery s_Instance;
	
	//Declare an instance of database to refer back to the singleton instance of the database
	private PointDatabase m_PDatabase;
	
	private GameObject EnemyMain;
	private GameObject PlayerMain;
	private static int CurrentNodeTarget;
	
	//Constructor for PositionQuery
	public PositionQuery()
	{
		m_PDatabase = PointDatabase.Instance;
		PlayerMain = GameObject.Find("Player_Cell");
		EnemyMain = GameObject.Find("Enemey_Cell");
		CurrentNodeTarget = 0;
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
		Threats.Add(GameObject.Find("Node_Left"));
		Threats.Add(GameObject.Find("Node_Right"));
		Threats.Add(GameObject.Find("Squad_Captain_Cell"));
		
		bool bResult = false;
		for(int i = 0; i < Threats.Count; i++)
		{
			if(Threats[i].GetComponent<Node_Manager>() != null && Threats[i].GetComponent<Node_Manager>().GetNodeChildList().Count > 0)
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
		Threats.Add(GameObject.Find("Node_Left"));
		Threats.Add(GameObject.Find("Node_Right"));
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
		Threats.Add(GameObject.Find("Node_Left"));
		Threats.Add(GameObject.Find("Node_Right"));
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
		if(_Node.GetComponent<Node_Manager>().GetNodeChildList().Count == 0)
		{
			return 0;
		}
		
		int nthreatLevel = 0;
		
		//increase score based on amount of cells in that node
		nthreatLevel += _Node.GetComponent<Node_Manager>().GetNodeChildList().Count;
		
		//increase score if that node have formed together and has a node captain
		if(_Node.GetComponent<PlayerSquadFSM>() != null)
		{
			nthreatLevel+= 50;
		}
		
		return nthreatLevel;
	}
	
	private List<Vector2> GetPointRangeBetweenPlayerEnemy()
	{
		Point PlayerClosestPoint = PointDatabase.Instance.GetClosestPointToPosition(GameObject.Find("Player_Cell").transform.position,false);
		Point EnemyClosestPoint = PointDatabase.Instance.GetClosestPointToPosition(GameObject.Find("Enemy_Cell").transform.position,false);
		
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
		if(IsAllThreatEmpty())
		{
			return GameObject.Find("Player_Cell");
		}
		
		if(PType == PositionType.Aggressive)
		{
			GameObject SquadCaptain = GameObject.Find("Squad_Captain_Cell");
			if(SquadCaptain.GetComponent<PlayerSquadFSM>().AliveChildCount() > 0)
			{
				return SquadCaptain;
			}
			else
			{
				return GameObject.Find("Player_Cell");
			}
		}
		else if(PType == PositionType.Defensive)
		{
			return GetMostThreateningThreat();
		}
		else if(PType == PositionType.Neutral)
		{
			if(CurrentNodeTarget == 0)
			{
				return GameObject.Find("Node_Left");
			}
			else if(CurrentNodeTarget == 1)
			{
				return GameObject.Find("Squad_Captain_Cell");
			}
			else if(CurrentNodeTarget == 2)
			{
				return GameObject.Find("Node_Right");
			}
		}
		
		return null;
	}
	
	public Vector2 GetLandminePos(RangeValue Range, PositionType PType, GameObject Agent)
	{
		Vector2 targetPos = new Vector2(0f,0f);
		
		if(PType == PositionType.Aggressive)
		{
			if(IsAllThreatEmpty())
			{
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
				targetPos = CurrentPoint.Position;
			}
			else
			{
				GameObject SquadCaptain = GameObject.Find("Squad_Captain_Cell");
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
			//Debug.Log("Defensive");
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
		/*GameObject EMain = GameObject.Find("Enemy_Cell");
		if(EMain.transform.position.x >= -0.05f && EMain.transform.position.x <= 0.05f)
		{
			int Choice = Random.Range(0, 4);
			if(Choice == 0)
			{
				return Formation.Crescent;
			}
			else if(Choice == 1)
			{
				return Formation.ReverseCrescent;
			}
			else if(Choice == 2)
			{
				return Formation.CircularSurround;
			}
			else if(Choice == 3)
			{
				return Formation.AreaBlock;
			}
		}*/
		return Formation.AreaBlock;
	}
		
	/*
	public static Vector2[] GetPathToPM(QueryType Qtype, TargetType Ttype, RangeValue Range, Directness Direct)
	{
	
	}
	
	public static Vector2[] GetPathToPC(QueryType Qtype, TargetType Ttype, RangeValue Range, Directness Direct)
	{
		
	}
	
	public static Vector2[] GetTrickAttackPathToPM(QueryType Qtype, TargetType Ttype, RangeValue Range, Directness Direct)
	{
	
	}
	
	public static Vector2 GetECPosAwayFromPC(QueryType Qtype, TargetType Ttype, RangeValue Range, Directness Direct)
	{
	
	}*/
}
