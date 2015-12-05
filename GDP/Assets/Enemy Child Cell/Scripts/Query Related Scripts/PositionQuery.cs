using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionQuery
{
	private static PositionQuery instance;
	private PointDatabase m_PDatabase;
	private int nIndexOrder;
	
	public PositionQuery()
	{
		m_PDatabase = PointDatabase.Instance;
		nIndexOrder = 0;
	}
	
	//Singleton
	public static PositionQuery Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new PositionQuery();
			}
			return instance;
		}
	}
	
	public Vector2 RequestLandminePos (PositionType _LandmineType, GameObject _EnemyMain, GameObject _PlayerMain)
	{
		Vector2 m_IdealMinePos = new Vector2(0f,0f);
		
		List<Vector2> m_PossiblePositions = GeneratePossiblePositions(_EnemyMain, _PlayerMain);
		m_IdealMinePos = EvaluateForBestPos(m_PossiblePositions, _LandmineType, _PlayerMain, _EnemyMain);
		
		return m_IdealMinePos;
	}
	
	private List<Vector2> GeneratePossiblePositions(GameObject _EnemyMain, GameObject _PlayerMain) 
	{
		List<Vector2> m_PossiblePosition = new List<Vector2>();
		
		m_PossiblePosition = m_PDatabase.ExtractPosYRange(_PlayerMain.transform.position.y, _EnemyMain.transform.position.y);
	
		return m_PossiblePosition;
	}
	
	private Vector2 EvaluateForBestPos(List<Vector2> _PositionList, PositionType _RequestType, GameObject _PlayerMain, GameObject _EnemyMain)
	{
		Vector2 m_BestPos = new Vector2(0f,0f);
		List<GameObject> m_NodeList = GetListOfNodes(_PlayerMain);//sequence should be left, top, right nodes
		
		//Aggressive positioning for landmines (focus at the weak spot of the 3 nodes formation)
		if(_RequestType == PositionType.Aggressive)
		{
			//if there is no empty nodes, find the weakest node to aim
			if(!IsAllNodesEmpty(m_NodeList))
			{
				GameObject _TargetNode = GetMostWeakNode(m_NodeList);
				m_BestPos = GetClosestPositionToNode(_PositionList, _TargetNode);
			}
			//else if all is empty node, aim the main node
			else
			{
				m_BestPos = _PlayerMain.transform.position;
			}
		}
		//Focus positioning towards the threatening node (Aimed at the threatening player node)
		else if(_RequestType == PositionType.Neutral)
		{
			if(!IsAllNodesEmpty(m_NodeList))
			{
				GameObject _TargetNode = GetMostThreateningNode(m_NodeList);
				m_BestPos = GetClosestPositionToNode(_PositionList,_TargetNode);
			}
			//else if all is empty node, aim the main node
			else
			{
				m_BestPos = _PlayerMain.transform.position;
			}
		}
		//Even spread position across all nodes (divide all the different landmines evenly)
		else if(_RequestType == PositionType.Defensive)
		{
			GameObject _TargetNode = m_NodeList[nIndexOrder];
			m_BestPos = GetClosestPositionToNode(_PositionList,_TargetNode);
			nIndexOrder++;
			if(nIndexOrder > 2)
			{
				nIndexOrder = 0;
			}
		}
		
		m_BestPos = InduceNoiseToPosition(m_BestPos);
		
		return m_BestPos;
	}
	
	private List<GameObject> GetListOfNodes(GameObject _PlayerMain)
	{
		List<GameObject> m_NodeList = new List<GameObject>();
		
		foreach(Transform node in _PlayerMain.transform)
		{
			m_NodeList.Add(node.gameObject);
		}
		
		return m_NodeList;
	}
	
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
			
			//increase score by the amount of nutrients that node has
			
		}
		
		return nthreatLevel;
	}
	
	private Vector2 GetClosestPositionToNode (List<Vector2> _PositionList ,GameObject _Node)
	{
		Vector2 m_ClosestPosition = new Vector2(0f,0f);
		float fClosestDistance = Mathf.Infinity;
		
		for(int i = 0; i < _PositionList.Count; i++)
		{
			float fDistance = Vector2.Distance(_Node.transform.position, _PositionList[i]);
			if(fDistance < fClosestDistance)
			{
				m_ClosestPosition = _PositionList[i];
				fClosestDistance = fDistance;
			}
		}
		
		return m_ClosestPosition;
	}
	
	private Vector2 InduceNoiseToPosition (Vector2 _TargetPos)
	{
		float fRandomXNeg = Random.Range(-0.5f, 0f);
		float fRandomXPos = Random.Range(0f, 0.5f);
		float fRandomYNeg = Random.Range(-0.5f, 0f);
		float fRandomYPos = Random.Range(0f, 0.5f);
		
		float fNoiseToX = Mathf.PerlinNoise(fRandomXNeg,fRandomXPos);
		float fNoiseToY = Mathf.PerlinNoise(fRandomYNeg,fRandomYPos);
		Vector2 m_RandomResult = new Vector2(_TargetPos.x + fNoiseToX, _TargetPos.y + fNoiseToY);
		
		return m_RandomResult;
	}
}
