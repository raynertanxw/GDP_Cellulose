using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationDatabase
{
	private static FormationDatabase s_Instance;

	private Dictionary<string,int> m_FIndexDatabase;
    private Dictionary<int, Vector2> m_FPositionDatabase;
	private Dictionary<int, bool> m_FAvaliabilityDatabase;

	public Dictionary<string,int> FIDatabase { get{ return m_FIndexDatabase;} }
	public Dictionary<int, Vector2> FPDatabase { get{ return m_FPositionDatabase;}}
	
	private Formation m_CurrentFormation;
	private float m_fCurrentMainScale;
	
	private GameObject m_EMain;
	private GameObject m_PMain;

	public FormationDatabase()
	{
		m_EMain = GameObject.Find("Enemy_Cell");
		m_PMain = GameObject.Find("Player_Cell");
	
		InitializeDatabases();
		List<EnemyChildFSM> ECList = GameObject.Find("Enemy_Cell").GetComponent<EnemyMainFSM>().ECList;
		RefreshDatabases(ECList);
	}

	//Singleton and Get function
	public static FormationDatabase Instance
	{
		get
		{
			if(s_Instance == null)
			{
				s_Instance = new FormationDatabase();
			}
			return s_Instance;
		}
	}

    private void InitializeDatabases()
    {
		m_FIndexDatabase = new Dictionary<string, int>();
		m_FPositionDatabase = new Dictionary<int, Vector2>();
		m_FAvaliabilityDatabase = new Dictionary<int, bool>();
		ClearDatabases();
    }

	public void RefreshDatabases(List<EnemyChildFSM> _EnemyChild)
    {
		ClearDatabases();

		int FormationIndex = 0;

		for(int i = 0; i < _EnemyChild.Count; i++)
		{
			if(!m_FIndexDatabase.ContainsKey(_EnemyChild[i].name)){m_FIndexDatabase.Add(_EnemyChild[i].name,FormationIndex);}
			if(!m_FPositionDatabase.ContainsKey(FormationIndex)){m_FPositionDatabase.Add(FormationIndex,Vector2.zero);}
			if(!m_FAvaliabilityDatabase.ContainsKey(FormationIndex)){m_FAvaliabilityDatabase.Add(FormationIndex,true);}
			FormationIndex++;
		}
    }

    public void ClearDatabases()
    {
		m_FIndexDatabase.Clear();
		m_FPositionDatabase.Clear();
		m_FAvaliabilityDatabase.Clear();
    }

	public void UpdateDatabaseFormation(Formation _FormationType, float _MainScale)
    {
		m_CurrentFormation = _FormationType;
		m_fCurrentMainScale = _MainScale;

		Vector2 EMPos = GameObject.Find("Enemy_Cell").transform.position;

		if(_FormationType == Formation.QuickCircle)
		{
			int CellsPerCircle = 20;
			int CircleCellCount = 0;
			int CircleCount = 1;
			
			float AngleInterval = 360f/CellsPerCircle;
			float CurrentAngle = 180f;
			float GapBetweenCircle = 2.5f;
			
			Vector2 TargetDirection = new Vector2(Mathf.Cos(CurrentAngle * Mathf.Deg2Rad), Mathf.Sin(CurrentAngle * Mathf.Deg2Rad));
			Vector2 CurrentFormationPosition = TargetDirection * GapBetweenCircle;
			List<int> Keys = new List<int>(m_FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				TargetDirection.x = Mathf.Cos(CurrentAngle * Mathf.Deg2Rad);
				TargetDirection.y = Mathf.Sin(CurrentAngle * Mathf.Deg2Rad);
				CurrentFormationPosition = TargetDirection.normalized * GapBetweenCircle * CircleCount;
				m_FPositionDatabase[FIndex] = CurrentFormationPosition;
				m_FAvaliabilityDatabase[FIndex] = false;
				//ebug.Log("Position " + FIndex + ": " + CurrentFormationPosition);
				
				CurrentAngle += AngleInterval;
				if(CurrentAngle >= 360f){CurrentAngle -= 360.0f;}
				
				CircleCellCount++;
				
				if(CircleCellCount > 20)
				{
					//CircleCount++;
					GapBetweenCircle += 0.75f;
					CircleCellCount = 0;
				}
			}
		}
		else if(_FormationType == Formation.ReverseCircular)
		{
			Vector2 CurrentFormationPos = new Vector2(0f,-1.6f * _MainScale);
			Vector2 StoredFormationPos = Vector2.zero;
			float XInterval = 0.7f * _MainScale;// and -0.65 for left side
			float YInterval = -0.15f * _MainScale;
			float NextLineInterval = -0.35f * _MainScale;
			int RightCount = 0;
			int LeftCount = 0;
			List<int> Keys = new List<int>(m_FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				if(FIndex % 9 == 0)
				{
					RightCount = LeftCount = 0;
					
					if(FIndex == 0)
					{
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						if(m_FPositionDatabase.ContainsKey(FIndex - 9))
						{
							CurrentFormationPos = new Vector2(m_FPositionDatabase[FIndex - 9].x, m_FPositionDatabase[FIndex - 9].y + NextLineInterval);
							StoredFormationPos = CurrentFormationPos;
							m_FPositionDatabase[FIndex] = StoredFormationPos;
							m_FAvaliabilityDatabase[FIndex] = false;
						}
						
						continue;
					}
				}
				else if(FIndex % 2 != 0)
				{
					RightCount++;
					
					int CurrentCenterIndex = GetCrescentCenterIndex(FIndex);
					
					if(m_FPositionDatabase.ContainsKey(CurrentCenterIndex))
					{
						CurrentFormationPos = new Vector2(m_FPositionDatabase[CurrentCenterIndex].x + RightCount * XInterval, m_FPositionDatabase[CurrentCenterIndex].y + RightCount * YInterval);
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
					}
					continue;
				}
				else if(FIndex % 2 == 0)
				{
					LeftCount++;
					
					int CurrentCenterIndex = GetCrescentCenterIndex(FIndex);
					
					if(m_FPositionDatabase.ContainsKey(CurrentCenterIndex))
					{
						CurrentFormationPos = new Vector2(m_FPositionDatabase[CurrentCenterIndex].x - LeftCount * XInterval, m_FPositionDatabase[CurrentCenterIndex].y + LeftCount * YInterval);
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
					}
					continue;
				}
			}
		}
		else if(_FormationType == Formation.Turtle)
		{
			int ECAmount = m_FIndexDatabase.Count;
			int SetAmount = (int) Mathf.Floor(ECAmount/9);
			Vector2 CurrentFormationPos = new Vector2(0f, -2.25f * _MainScale);
			Vector2 StoredFormationPos = Vector2.zero;
			float XInterval = 0.65f * _MainScale;
			float XBlockGap = 0.1f * _MainScale;
			float YInterval = -0.5f * _MainScale;
			int RightCount = 0;
			int LeftCount = 0;
			List<int> Keys = new List<int>(m_FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				if(FIndex % 9 == 0)
				{
					RightCount = LeftCount = 0;
					
					if(FIndex == 0)
					{
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(m_FPositionDatabase[FIndex - 9].x, m_FPositionDatabase[FIndex - 9].y + YInterval);
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
				
				if(FIndex % 2 != 0)
				{
					RightCount++;
					int AreaCenterIndex = GetAreaBlockCenterIndex(FIndex);
					
					if(RightCount == 1)
					{
						CurrentFormationPos = new Vector2(m_FPositionDatabase[AreaCenterIndex].x + XInterval, m_FPositionDatabase[AreaCenterIndex].y);
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(m_FPositionDatabase[AreaCenterIndex].x + RightCount * XInterval + XBlockGap, m_FPositionDatabase[AreaCenterIndex].y - YInterval);
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
				else if(FIndex % 2 == 0)
				{
					LeftCount++;
					int AreaCenterIndex = GetAreaBlockCenterIndex(FIndex);
					
					if(LeftCount == 1)
					{
						CurrentFormationPos = new Vector2(m_FPositionDatabase[AreaCenterIndex].x - XInterval, m_FPositionDatabase[AreaCenterIndex].y);
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(m_FPositionDatabase[AreaCenterIndex].x - LeftCount * XInterval - XBlockGap, m_FPositionDatabase[AreaCenterIndex].y - YInterval);
						StoredFormationPos = CurrentFormationPos;
						m_FPositionDatabase[FIndex] = StoredFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
			}
		}
		else if(_FormationType == Formation.Ladder)
		{
			int RightCount = 0;
			int LeftCount = 0;
			
			float XInterval = 0.45f * _MainScale;
			float XBlockGap = 0.65f * _MainScale;
			float YInterval = 0.5f * _MainScale;
			
			Vector2 CurrentFormationPos = Vector2.zero;
			List<int> Keys = new List<int>(m_FPositionDatabase.Keys);
			foreach(int FIndex in Keys)
			{
				if(FIndex % 12 == 0)
				{
					LeftCount = RightCount = 0;
					if(FIndex == 0)
					{
						RightCount++;
						CurrentFormationPos.x = XBlockGap;
						CurrentFormationPos.y = -YInterval * 4;
						m_FPositionDatabase[FIndex] = CurrentFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						RightCount++;
						CurrentFormationPos.x = m_FPositionDatabase[FIndex - 12].x;
						CurrentFormationPos.y = m_FPositionDatabase[FIndex - 12].y - YInterval;
						m_FPositionDatabase[FIndex] = CurrentFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
				else if(FIndex % 2 != 0)
				{
					if(LeftCount == 0)
					{
						LeftCount++;
						CurrentFormationPos.x = m_FPositionDatabase[FIndex - 1].x - (2 * XBlockGap);
						CurrentFormationPos.y = m_FPositionDatabase[FIndex - 1].y;
						m_FPositionDatabase[FIndex] = CurrentFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						LeftCount++;
						//Debug.Log("left trying to access: " + (FIndex - 2));
						CurrentFormationPos.x = m_FPositionDatabase[FIndex - 2].x - XInterval;
						CurrentFormationPos.y = m_FPositionDatabase[FIndex - 2].y;
						m_FPositionDatabase[FIndex] = CurrentFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
				else if(FIndex % 2 == 0)
				{
					if(RightCount != 0)
					{
						RightCount++;
						CurrentFormationPos.x = m_FPositionDatabase[FIndex - 2].x + XInterval;
						CurrentFormationPos.y = m_FPositionDatabase[FIndex - 2].y;
						m_FPositionDatabase[FIndex] = CurrentFormationPos;
						m_FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
			}
		}
    }
    
    public void CheckOverlapPlayerADRange()
    {
		float YLimit = m_PMain.transform.position.y + PlayerMain.Instance.m_fDetectionRadius;
		List<int> Keys = new List<int>(m_FPositionDatabase.Keys);
		
		float YAdjustment = 0f;
		
		foreach(int FIndex in Keys)
		{
			if(m_FPositionDatabase[FIndex].y < YLimit && YLimit - m_FPositionDatabase[FIndex].y > YAdjustment)
			{
				YAdjustment = YLimit - m_FPositionDatabase[FIndex].y;
			}
		}
		
		if(YAdjustment == 0f){return;}

		Vector2 UpdatedPosition = Vector2.zero;
		foreach(int FIndex in Keys)
		{
			UpdatedPosition = m_FPositionDatabase[FIndex];
			UpdatedPosition.y += YAdjustment/2;
			m_FPositionDatabase[FIndex] = UpdatedPosition;
		}
    }

	public void AddNewDefenderToCurrentFormation(GameObject _NewDefender)
	{
		//Is there any avaliable index at where ? If yes
		if(IsThereAvaliableIndex())
		{
			//Get the first avaliable formation index to replace
			int AvaliableIndex = GetFirstAvaliableIndex();
			
			//Since replaced, that formation index will not be avaliable for other defenders
			m_FAvaliabilityDatabase[AvaliableIndex] = false;
			
			//Remove the entry whereby the previous defender occupy this formation index and rewrite with the new defender's name
			foreach(string Key in m_FIndexDatabase.Keys)
			{
				if(m_FIndexDatabase[Key] == AvaliableIndex)
				{
					m_FIndexDatabase.Remove(Key);
					m_FIndexDatabase[_NewDefender.name] = AvaliableIndex;
					break;
				}
			}
		}
		//If not,
		else
		{
			if(!m_FIndexDatabase.ContainsKey(_NewDefender.name)){m_FIndexDatabase.Add(_NewDefender.name, m_FIndexDatabase.Count);} else{m_FIndexDatabase[_NewDefender.name] = m_FIndexDatabase.Count;}
			if(!m_FPositionDatabase.ContainsKey(m_FIndexDatabase[_NewDefender.name])){m_FPositionDatabase.Add(m_FIndexDatabase[_NewDefender.name], Vector2.zero);} else{m_FPositionDatabase[m_FIndexDatabase[_NewDefender.name]] = Vector2.zero;}
			if(!m_FAvaliabilityDatabase.ContainsKey(m_FIndexDatabase[_NewDefender.name])){m_FAvaliabilityDatabase.Add(m_FIndexDatabase[_NewDefender.name], false);} else{m_FAvaliabilityDatabase[m_FIndexDatabase[_NewDefender.name]] = false;}
		
			UpdateDatabaseFormation(m_CurrentFormation,m_fCurrentMainScale);
		}
		CheckOverlapPlayerADRange();
	}
	
	public void ReturnFormationPos(GameObject _ObjectReturn)
	{
		if(m_FIndexDatabase.ContainsKey(_ObjectReturn.name))
		{
			int IndexReturned = m_FIndexDatabase[_ObjectReturn.name];
			m_FAvaliabilityDatabase[IndexReturned] = true;
		}
	}
	
	private bool IsThereAvaliableIndex()
	{
		foreach(int Key in m_FAvaliabilityDatabase.Keys)
		{
			if(m_FAvaliabilityDatabase[Key])
			{
				return true;
			}
		}
		return false;
	}
	
	private int GetFirstAvaliableIndex()
	{
		foreach(int Key in m_FAvaliabilityDatabase.Keys)
		{
			if(m_FAvaliabilityDatabase[Key])
			{
				return Key;
			}
		}
		return 999;
	}

    public Vector2 GetTargetFormationPosition(Formation _Formation, GameObject _EnemyCell)
    {
		string CellName = _EnemyCell.name;
		if(!m_FIndexDatabase.ContainsKey(CellName))
		{
			MessageDispatcher.Instance.DispatchMessage(_EnemyCell,_EnemyCell,MessageType.Idle,0);
			return Vector2.zero;
		}
    
		int TargetFIndex = m_FIndexDatabase[CellName];

		Vector2 PosDifference = m_FPositionDatabase[TargetFIndex];
		Vector2 EMPosition = m_EMain.transform.position;
		Vector2 TargetPosition = Vector2.zero;
		
		if(_Formation == Formation.Ladder)
		{
			TargetPosition = new Vector2(PosDifference.x, EMPosition.y + PosDifference.y);
			return TargetPosition;
		}
		
		TargetPosition = new Vector2(EMPosition.x + PosDifference.x, EMPosition.y + PosDifference.y);
		return TargetPosition;
    }

    private int GetCrescentCenterIndex(int _CurrentIndex)
    {
		int Current = _CurrentIndex;
		while(Current % 9 != 0)
		{
			Current--;
		}
		return Current;
    }

    private int GetAreaBlockCenterIndex(int _CurrentIndex)
    {
		int Current = _CurrentIndex;
		while(Current % 9 != 0)
		{
			Current--;
		}
		return Current;
    }

    private void PrintIndexDatabase()
    {
		List<string> Keys = new List<string>(m_FIndexDatabase.Keys);
		foreach(string Key in Keys)
		{
			Debug.Log("Name: " + Key + " Index: " + m_FIndexDatabase[Key]);
		}
    }

	private void PrintPositionDatabase()
	{
		List<int> Keys = new List<int>(m_FPositionDatabase.Keys);
		foreach(int Key in Keys)
		{
			Debug.Log("Index: " + Key + " Position: " + m_FPositionDatabase[Key]);
		}
	}
	
	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
