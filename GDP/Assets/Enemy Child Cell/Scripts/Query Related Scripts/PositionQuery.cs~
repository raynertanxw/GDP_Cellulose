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
		List<GameObject> m_SquadList = GetListOfSquads(_PlayerMain);//sequence should be left, top, right squads
		
		//Aggressive positioning for landmines (focus at the weak spot of the 3 squads formation)
		if(_RequestType == PositionType.Aggressive)
		{
			//if there is no empty squads, find the weakest squad to aim
			if(!IsAllSquadsEmpty(m_SquadList))
			{
				GameObject _TargetSquad = GetMostWeakSquad(m_SquadList);
				m_BestPos = GetClosestPositionToSquad(_PositionList, _TargetSquad);
			}
			//else if all is empty squad, aim the main squad
			else
			{
				m_BestPos = _PlayerMain.transform.position;
			}
		}
		//Focus positioning towards the threatening squad (Aimed at the threatening player squad)
		else if(_RequestType == PositionType.Neutral)
		{
			if(!IsAllSquadsEmpty(m_SquadList))
			{
				GameObject _TargetSquad = GetMostThreateningSquad(m_SquadList);
				m_BestPos = GetClosestPositionToSquad(_PositionList,_TargetSquad);
			}
			//else if all is empty squad, aim the main squad
			else
			{
				m_BestPos = _PlayerMain.transform.position;
			}
		}
		//Even spread position across all squads (divide all the different landmines evenly)
		else if(_RequestType == PositionType.Defensive)
		{
			GameObject _TargetSquad = m_SquadList[nIndexOrder];
			m_BestPos = GetClosestPositionToSquad(_PositionList,_TargetSquad);
			nIndexOrder++;
			if(nIndexOrder > 2)
			{
				nIndexOrder = 0;
			}
		}
		
		m_BestPos = InduceNoiseToPosition(m_BestPos);
		
		return m_BestPos;
	}
	
	private List<GameObject> GetListOfSquads(GameObject _PlayerMain)
	{
		List<GameObject> m_SquadList = new List<GameObject>();
		
		foreach(Transform squad in _PlayerMain.transform)
		{
			m_SquadList.Add(squad.gameObject);
		}
		
		return m_SquadList;
	}
	
	private GameObject GetMostThreateningSquad(List<GameObject> _SquadList)
	{
		int nIndexForMostThreating = 0;
		int nHighestScore = 0;
		
		for(int i = 0; i < _SquadList.Count; i++)
		{
			if(EvaluateSquad(_SquadList[i]) > nHighestScore)
			{
				nIndexForMostThreating = i;
				nHighestScore = EvaluateSquad(_SquadList[i]);
			}
		}
		
		return _SquadList[nIndexForMostThreating];
	}
	
	private bool IsAllSquadsEmpty (List<GameObject> _SquadList)
	{
		bool bResult = false;
		for(int i = 0; i < _SquadList.Count; i++)
		{
			if(_SquadList[i].GetComponent<Squad_Manager>().GetSquadChildList().Count <= 0)
			{
				return false;
			}
		}
		return true;
	}
	
	private GameObject GetMostWeakSquad (List<GameObject> _SquadList)
	{
		int nIndexForMostWeak = 0;
		int nLowestScore = 0;
		
		for(int i = 0; i < _SquadList.Count; i++)
		{
			if(EvaluateSquad(_SquadList[i]) < nLowestScore)
			{
				nIndexForMostWeak = i;
				nLowestScore = EvaluateSquad(_SquadList[i]);
			}
		}
		
		return _SquadList[nIndexForMostWeak];
	}
	
	private int EvaluateSquad (GameObject _Squad)
	{
		//if the squad contains no cell, it serve no threat to the enemy main cell
		if(_Squad.GetComponent<Squad_Manager>().GetSquadChildList().Count == 0)
		{
			return 0;
		}
		
		int nthreatLevel = 0;
		
		//increase score based on amount of cells in that squad
		nthreatLevel += _Squad.GetComponent<Squad_Manager>().GetSquadChildList().Count;
		
		//increase score if that squad have formed together and has a squad captain
		if(_Squad.GetComponent<SquadCaptain>() != null)
		{
			nthreatLevel+= 50;
			
			//increase score by the amount of nutrients that squad has
			
		}
		
		return nthreatLevel;
	}
	
	private Vector2 GetClosestPositionToSquad (List<Vector2> _PositionList ,GameObject _Squad)
	{
		Vector2 m_ClosestPosition = new Vector2(0f,0f);
		float fClosestDistance = Mathf.Infinity;
		
		for(int i = 0; i < _PositionList.Count; i++)
		{
			float fDistance = Vector2.Distance(_Squad.transform.position, _PositionList[i]);
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
