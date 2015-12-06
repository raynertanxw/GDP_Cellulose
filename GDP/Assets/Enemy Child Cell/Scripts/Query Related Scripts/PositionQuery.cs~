using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionQuery
{
	//Declare an instance of PositionQuery for Singleton purpose
	private static PositionQuery s_Instance;
	
	//Declare an instance of database to refer back to the singleton instance of the database
	private PointDatabase m_PDatabase;
	
	//An integer that serve as an index for the even spreading of generated position in the Landmine state
	//of the enemy child cell
	private int nIndexOrder;
	
	//Constructor for PositionQuery
	public PositionQuery()
	{
		m_PDatabase = PointDatabase.Instance;
		nIndexOrder = 0;
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
	
	//Based on the type of position and the situation of the enemy main and player main cell, it will make an
	//appropriate decision and return the best tactical osition
	public Vector2 RequestLandminePos (PositionType _LandmineType, GameObject _EnemyMain, GameObject _PlayerMain)
	{
		Vector2 IdealMinePos = new Vector2(0f,0f);
		
		List<Vector2> PossiblePositions = GeneratePossiblePositions(_EnemyMain, _PlayerMain);
		IdealMinePos = EvaluateForBestPos(PossiblePositions, _LandmineType, _PlayerMain, _EnemyMain);
		
		return IdealMinePos;
	}
	
	//Return a list of positions based on the enemy main cell and player main cell
	private List<Vector2> GeneratePossiblePositions(GameObject _EnemyMain, GameObject _PlayerMain) 
	{
		List<Vector2> PossiblePosition = new List<Vector2>();
		
		PossiblePosition = m_PDatabase.ExtractPosYRange(_PlayerMain.transform.position.y, _EnemyMain.transform.position.y);
	
		return PossiblePosition;
	}
	
	//Evaluate a list of positions based on the positioning type and the game environment information, returning an ideal position
	private Vector2 EvaluateForBestPos(List<Vector2> _PositionList, PositionType _RequestType, GameObject _PlayerMain, GameObject _EnemyMain)
	{
		Vector2 BestPos = new Vector2(0f,0f);
		List<GameObject> NodeList = GetListOfNodes(_PlayerMain);//sequence should be left, top, right nodes
		
		//Depending on the request given, it will evaluate the list of positions differently.
		//Aggressive positioning for landmines (focus at the weak spot of the 3 nodes formation)
		if(_RequestType == PositionType.Aggressive)
		{
			//if there is no empty nodes, find the weakest node to aim
			if(!IsAllNodesEmpty(NodeList))
			{
				GameObject _TargetNode = GetMostWeakNode(NodeList);
				BestPos = GetClosestPositionToNode(_PositionList, _TargetNode);
			}
			//else if all is empty node, aim the main node
			else
			{
				BestPos = _PlayerMain.transform.position;
			}
		}
		//Focus positioning towards the threatening node (Aimed at the threatening player node)
		else if(_RequestType == PositionType.Neutral)
		{
			if(!IsAllNodesEmpty(NodeList))
			{
				GameObject _TargetNode = GetMostThreateningNode(NodeList);
				BestPos = GetClosestPositionToNode(_PositionList,_TargetNode);
			}
			//else if all is empty node, aim the main node
			else
			{
				BestPos = _PlayerMain.transform.position;
			}
		}
		//Even spread position across all nodes (divide all the different landmines evenly)
		else if(_RequestType == PositionType.Defensive)
		{
			GameObject _TargetNode = NodeList[nIndexOrder];
			BestPos = GetClosestPositionToNode(_PositionList,_TargetNode);
			nIndexOrder++;
			if(nIndexOrder > 2)
			{
				nIndexOrder = 0;
			}
		}
		
		//Add noise to the position given
		BestPos = InduceNoiseToPosition(BestPos);
		
		return BestPos;
	}
	
	//A function that return a list of all player's nodes based on the player main cell
	private List<GameObject> GetListOfNodes(GameObject _PlayerMain)
	{
		List<GameObject> NodeList = new List<GameObject>();
		
		foreach(Transform Node in _PlayerMain.transform)
		{
			NodeList.Add(Node.gameObject);
		}
		
		return NodeList;
	}
	
	//A function that evaluate all the nodes of the player and return the most threatening node
	private GameObject GetMostThreateningNode(List<GameObject> _NodeList)
	{
		int nIndexForMostThreating = 0;
		int nHighestScore = 0;
		
		for(int i = 0; i < _NodeList.Count; i++)
		{
			if(EvaluateNode(_NodeList[i]) > nHighestScore)
			{
				nIndexForMostThreating = i;
				nHighestScore = EvaluateNode(_NodeList[i]);
			}
		}
		
		return _NodeList[nIndexForMostThreating];
	}
	
	//A function to check whether the node has any cells within it
	private bool IsAllNodesEmpty (List<GameObject> _NodeList)
	{
		bool bResult = false;
		for(int i = 0; i < _NodeList.Count; i++)
		{
			if(_NodeList[i].GetComponent<Node_Manager>().GetNodeChildList().Count <= 0)
			{
				return false;
			}
		}
		return true;
	}
	
	//A function that evaluate all the nodes of the player and return the most weak node
	private GameObject GetMostWeakNode (List<GameObject> _NodeList)
	{
		int nIndexForMostWeak = 0;
		int nLowestScore = 0;
		
		for(int i = 0; i < _NodeList.Count; i++)
		{
			if(EvaluateNode(_NodeList[i]) < nLowestScore)
			{
				nIndexForMostWeak = i;
				nLowestScore = EvaluateNode(_NodeList[i]);
			}
		}
		
		return _NodeList[nIndexForMostWeak];
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
		if(_Node.GetComponent<SquadCaptain>() != null)
		{
			nthreatLevel+= 50;
		}
		
		return nthreatLevel;
	}
	
	//A function that based on the given node and a list of information to return the closest position to 
	//the node
	private Vector2 GetClosestPositionToNode (List<Vector2> _PositionList ,GameObject _Node)
	{
		Vector2 ClosestPosition = new Vector2(0f,0f);
		float fClosestDistance = Mathf.Infinity;
		
		for(int i = 0; i < _PositionList.Count; i++)
		{
			float fDistance = Vector2.Distance(_Node.transform.position, _PositionList[i]);
			if(fDistance < fClosestDistance)
			{
				ClosestPosition = _PositionList[i];
				fClosestDistance = fDistance;
			}
		}
		
		return ClosestPosition;
	}
	
	//A function that add values to the position given through the perimeter and returh the new position
	private Vector2 InduceNoiseToPosition (Vector2 _TargetPos)
	{
		float fRandomXNeg = Random.Range(-0.5f, 0f);
		float fRandomXPos = Random.Range(0f, 0.5f);
		float fRandomYNeg = Random.Range(-0.5f, 0f);
		float fRandomYPos = Random.Range(0f, 0.5f);
		
		float fNoiseToX = Mathf.PerlinNoise(fRandomXNeg,fRandomXPos);
		float fNoiseToY = Mathf.PerlinNoise(fRandomYNeg,fRandomYPos);
		Vector2 RandomResult = new Vector2(_TargetPos.x + fNoiseToX, _TargetPos.y + fNoiseToY);
		
		return RandomResult;
	}
}
