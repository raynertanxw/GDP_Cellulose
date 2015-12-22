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
	
	private bool IsAllNodesEmpty ()
	{
		List<GameObject> NodeList = new List<GameObject>();
		NodeList.Add(GameObject.Find("Node_Left"));
		NodeList.Add(GameObject.Find("Node_Right"));
		NodeList.Add(GameObject.Find("Node_Top"));
		
		bool bResult = false;
		for(int i = 0; i < NodeList.Count; i++)
		{
			if(NodeList[i].GetComponent<Node_Manager>().GetNodeChildList().Count > 0)
			{
				return false;
			}
		}
		return true;
	}
	
	//A function that evaluate all the nodes of the player and return the most threatening node
	private GameObject GetMostThreateningNode()
	{
		List<GameObject> NodeList = new List<GameObject>();
		NodeList.Add(GameObject.Find("Node_Left"));
		NodeList.Add(GameObject.Find("Node_Right"));
		NodeList.Add(GameObject.Find("Node_Top"));
		int nIndexForMostThreating = 0;
		int nHighestScore = 0;
		
		for(int i = 0; i < NodeList.Count; i++)
		{
			if(EvaluateNode(NodeList[i]) > nHighestScore)
			{
				nIndexForMostThreating = i;
				nHighestScore = EvaluateNode(NodeList[i]);
			}
		}
		
		return NodeList[nIndexForMostThreating];
	}
	
	//A function that evaluate all the nodes of the player and return the most weak node
	private GameObject GetMostWeakNode ()
	{
		List<GameObject> NodeList = new List<GameObject>();
		NodeList.Add(GameObject.Find("Node_Left"));
		NodeList.Add(GameObject.Find("Node_Right"));
		NodeList.Add(GameObject.Find("Node_Top"));
		int nIndexForMostWeak = 0;
		int nLowestScore = 0;
		
		for(int i = 0; i < NodeList.Count; i++)
		{
			if(EvaluateNode(NodeList[i]) < nLowestScore)
			{
				nIndexForMostWeak = i;
				nLowestScore = EvaluateNode(NodeList[i]);
			}
		}
		
		return NodeList[nIndexForMostWeak];
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
	
	private Vector2 InduceNoiseToPosition (Vector2 _TargetPos)
	{
		float fRandomXNeg = Random.Range(-0.5f, 0f);
		float fRandomXPos = Random.Range(0f, 0.5f);
		float fRandomYNeg = Random.Range(-0.5f, 0f);
		float fRandomYPos = Random.Range(0f, 0.5f);
		
		float fNoiseToX = Random.Range(fRandomXNeg,fRandomXPos);
		Debug.Log(fNoiseToX);
		float fNoiseToY = Random.Range(fRandomYNeg,fRandomYPos);
		Debug.Log(fNoiseToY);
		Vector2 RandomResult = new Vector2(_TargetPos.x + fNoiseToX, _TargetPos.y + fNoiseToY);
		
		return RandomResult;
	}
	
	public GameObject GetLandmineTarget(PositionType PType, GameObject Agent)
	{
		GameObject target = null;
		if(IsAllNodesEmpty())
		{
			return GameObject.Find("Player_Cell");
		}
		
		if(PType == PositionType.Aggressive)
		{
			GameObject TopNode = GameObject.Find("Node_Top");
			if(TopNode.GetComponent<Node_Manager>().GetNodeChildList().Count > 0)
			{
				return TopNode;
			}
			else
			{
				return GameObject.Find("Player_Cell");
			}
		}
		else if(PType == PositionType.Defensive)
		{
			return GetMostThreateningNode();
		}
		else if(PType == PositionType.Neutral)
		{
			if(CurrentNodeTarget == 0)
			{
				return GameObject.Find("Node_Left");
			}
			else if(CurrentNodeTarget == 1)
			{
				return GameObject.Find("Node_Top");
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
			Debug.Log("Aggressive");
			if(IsAllNodesEmpty())
			{
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
				targetPos = CurrentPoint.Position;
			}
			else
			{
				GameObject TopNode = GameObject.Find("Node_Top");
				Point CurrentPoint = null;
				if(TopNode.GetComponent<Node_Manager>().GetNodeChildList().Count > 0)
				{
					CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(TopNode.transform.position,false);
				}
				else
				{
					CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
				}
			}
		}
		else if(PType == PositionType.Defensive)
		{
			Debug.Log("Defensive");
			if(IsAllNodesEmpty())
			{
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
				targetPos = CurrentPoint.Position;
			}
			else
			{
				GameObject TargetNode = GetMostThreateningNode();
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(TargetNode.transform.position,false);
				targetPos = CurrentPoint.Position;
			}
		}
		else if(PType == PositionType.Neutral)
		{
			Debug.Log("Netural");
			if(IsAllNodesEmpty())
			{
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(PlayerMain.transform.position,false);
				targetPos = CurrentPoint.Position;
			}
			else
			{
				GameObject TargetNode = null;
				
				if(CurrentNodeTarget == 0)
				{
					TargetNode = GameObject.Find("Node_Left");
				}
				else if(CurrentNodeTarget == 1)
				{
					TargetNode = GameObject.Find("Node_Top");
				}
				else if(CurrentNodeTarget == 2)
				{
					TargetNode = GameObject.Find("Node_Right");
				}
				
				Point CurrentPoint = PointDatabase.Instance.GetClosestPointToPosition(TargetNode.transform.position,false);
				targetPos = CurrentPoint.Position;
				
				if(CurrentNodeTarget >= 2)
				{
					CurrentNodeTarget = 0;
				}
				else
				{
					CurrentNodeTarget++;
				}
			}
		}
		
		//Utility.DrawCross(targetPos,Color.green,0.5f);
		return targetPos;//InduceNoiseToPosition(targetPos);
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
