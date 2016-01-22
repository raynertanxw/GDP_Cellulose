using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationDatabase
{
	private static FormationDatabase s_Instance;

	private Dictionary<string,int> FIndexDatabase;
    private Dictionary<int, Vector2> FPositionDatabase;// the vector2 stored will be the position difference from the supposed position to the enemy main cell position
	private Dictionary<int, bool> FAvaliabilityDatabase; //Store the formation index and boolean to see if that specific formation position is avaliable or not

	public Dictionary<string,int> FIDatabase { get{ return FIndexDatabase;} }
	public Dictionary<int, Vector2> FPDatabase { get{ return FPositionDatabase;}}
	
	private Formation CurrentFormation;
	private float fCurrentMainScale;
	
	private GameObject EMain;

	public FormationDatabase()
	{
		InitializeDatabases();
		List<EnemyChildFSM> ECList = GameObject.Find("Enemy_Cell").GetComponent<EnemyMainFSM>().ECList;
		RefreshDatabases(ECList);
		EMain = GameObject.Find("Enemy_Cell");
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
		FIndexDatabase = new Dictionary<string, int>();
		FPositionDatabase = new Dictionary<int, Vector2>();
		FAvaliabilityDatabase = new Dictionary<int, bool>();
		ClearDatabases();
    }

	public void RefreshDatabases(List<EnemyChildFSM> _EnemyChild)
    {
		ClearDatabases();

		int FormationIndex = 0;

		for(int i = 0; i < _EnemyChild.Count; i++)
		{
			if(!FIndexDatabase.ContainsKey(_EnemyChild[i].name))
			{
				FIndexDatabase.Add(_EnemyChild[i].name,FormationIndex);
				FPositionDatabase.Add(FormationIndex,Vector2.zero);
				FAvaliabilityDatabase.Add(FormationIndex,true);
				FormationIndex++;
			}
		}
    }

    public void ClearDatabases()
    {
		FIndexDatabase.Clear();
		FPositionDatabase.Clear();
		FAvaliabilityDatabase.Clear();
    }

	public void UpdateDatabaseFormation(Formation _FormationType, float _MainScale)
    {
		CurrentFormation = _FormationType;
		fCurrentMainScale = _MainScale;

		Vector2 EMPos = GameObject.Find("Enemy_Cell").transform.position;

		if(_FormationType == Formation.QuickCircle)
		{
			int CellsPerCircle = 20;
			int CircleCellCount = 0;
			int CircleCount = 1;
			
			float AngleInterval = 360f/CellsPerCircle;
			float CurrentAngle = 0f;
			float GapBetweenCircle = 2.5f;
			
			Vector2 TargetDirection = new Vector2(Mathf.Cos(CurrentAngle * Mathf.Deg2Rad), Mathf.Sin(CurrentAngle * Mathf.Deg2Rad));
			Vector2 CurrentFormationPosition = TargetDirection * GapBetweenCircle;
			List<int> Keys = new List<int>(FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				TargetDirection.x = Mathf.Cos(CurrentAngle * Mathf.Deg2Rad);
				TargetDirection.y = Mathf.Sin(CurrentAngle * Mathf.Deg2Rad);
				CurrentFormationPosition = TargetDirection.normalized * GapBetweenCircle * CircleCount;
				FPositionDatabase[FIndex] = CurrentFormationPosition;
				FAvaliabilityDatabase[FIndex] = false;
				//ebug.Log("Position " + FIndex + ": " + CurrentFormationPosition);
				
				CurrentAngle += AngleInterval;
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
			List<int> Keys = new List<int>(FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				if(FIndex % 9 == 0)
				{
					RightCount = LeftCount = 0;
					
					if(FIndex == 0)
					{
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						if(FPositionDatabase.ContainsKey(FIndex - 9))
						{
							CurrentFormationPos = new Vector2(FPositionDatabase[FIndex - 9].x, FPositionDatabase[FIndex - 9].y + NextLineInterval);
							StoredFormationPos = CurrentFormationPos;
							FPositionDatabase[FIndex] = StoredFormationPos;
							FAvaliabilityDatabase[FIndex] = false;
						}
						
						continue;
					}
				}
				else if(FIndex % 2 != 0)
				{
					RightCount++;
					
					int CurrentCenterIndex = GetCrescentCenterIndex(FIndex);
					
					if(FPositionDatabase.ContainsKey(CurrentCenterIndex))
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[CurrentCenterIndex].x + RightCount * XInterval, FPositionDatabase[CurrentCenterIndex].y + RightCount * YInterval);
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
					}
					continue;
				}
				else if(FIndex % 2 == 0)
				{
					LeftCount++;
					
					int CurrentCenterIndex = GetCrescentCenterIndex(FIndex);
					
					if(FPositionDatabase.ContainsKey(CurrentCenterIndex))
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[CurrentCenterIndex].x - LeftCount * XInterval, FPositionDatabase[CurrentCenterIndex].y + LeftCount * YInterval);
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
					}
					continue;
				}
			}
		}
		else if(_FormationType == Formation.Turtle)
		{
			int ECAmount = FIndexDatabase.Count;
			int SetAmount = (int) Mathf.Floor(ECAmount/9);
			Vector2 CurrentFormationPos = new Vector2(0f, -2.25f * _MainScale);
			Vector2 StoredFormationPos = Vector2.zero;
			float XInterval = 0.65f * _MainScale;
			float XBlockGap = 0.1f * _MainScale;
			float YInterval = -0.5f * _MainScale;
			int RightCount = 0;
			int LeftCount = 0;
			List<int> Keys = new List<int>(FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				if(FIndex % 9 == 0)
				{
					RightCount = LeftCount = 0;
					
					if(FIndex == 0)
					{
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[FIndex - 9].x, FPositionDatabase[FIndex - 9].y + YInterval);
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
				
				if(FIndex % 2 != 0)
				{
					RightCount++;
					int AreaCenterIndex = GetAreaBlockCenterIndex(FIndex);
					
					if(RightCount == 1)
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[AreaCenterIndex].x + XInterval, FPositionDatabase[AreaCenterIndex].y);
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[AreaCenterIndex].x + RightCount * XInterval + XBlockGap, FPositionDatabase[AreaCenterIndex].y - YInterval);
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
				else if(FIndex % 2 == 0)
				{
					LeftCount++;
					int AreaCenterIndex = GetAreaBlockCenterIndex(FIndex);
					
					if(LeftCount == 1)
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[AreaCenterIndex].x - XInterval, FPositionDatabase[AreaCenterIndex].y);
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[AreaCenterIndex].x - LeftCount * XInterval - XBlockGap, FPositionDatabase[AreaCenterIndex].y - YInterval);
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
			}
		}
		else if(_FormationType == Formation.Ladder)
		{
			int RightCount = 0;
			int LeftCount = 0;
			
			float XInterval = 0.65f * _MainScale;
			float XBlockGap = 0.45f * _MainScale;
			float YInterval = 0.5f * _MainScale;
			
			Vector2 CurrentFormationPos = Vector2.zero;
			List<int> Keys = new List<int>(FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				if(FIndex % 8 == 0)
				{
					RightCount = LeftCount = 0;
					if(FIndex == 0)
					{
						CurrentFormationPos.x = XInterval;
						CurrentFormationPos.y = EMPos.y - 2 * YInterval;
						FPositionDatabase[FIndex] = CurrentFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
					else
					{
						CurrentFormationPos.x = FPositionDatabase[FIndex - 8].x;
						CurrentFormationPos.y -= YInterval;
						FPositionDatabase[FIndex] = CurrentFormationPos;
						FAvaliabilityDatabase[FIndex] = false;
						continue;
					}
				}
				else if(FIndex % 8 != 0)
				{
					if(FIndex % 2 != 0)
					{
						RightCount++;
						if(RightCount == 1)
						{
							CurrentFormationPos.x = XBlockGap;
							FPositionDatabase[FIndex] = CurrentFormationPos;
							FAvaliabilityDatabase[FIndex] = false;
							continue;
						}
						else 
						{
							CurrentFormationPos.x = FPositionDatabase[FIndex - 2].x + XInterval;
							FPositionDatabase[FIndex] = CurrentFormationPos;
							FAvaliabilityDatabase[FIndex] = false;
							continue;
						}
					}
					else if(FIndex % 2 == 0)
					{
						LeftCount++; 
						if(LeftCount == 1)
						{
							CurrentFormationPos.x = -XBlockGap;
							FPositionDatabase[FIndex] = CurrentFormationPos;
							FAvaliabilityDatabase[FIndex] = false;
							continue;
						}
						else 
						{
							CurrentFormationPos.x = FPositionDatabase[FIndex - 2].x - XInterval;
							FPositionDatabase[FIndex] = CurrentFormationPos;
							FAvaliabilityDatabase[FIndex] = false;
							continue;
						}
					}
				}
			}
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
			FAvaliabilityDatabase[AvaliableIndex] = false;
			
			//Remove the entry whereby the previous defender occupy this formation index and rewrite with the new defender's name
			foreach(string Key in FIndexDatabase.Keys)
			{
				if(FIndexDatabase[Key] == AvaliableIndex)
				{
					FIndexDatabase.Remove(Key);
					
					if(FIndexDatabase.ContainsKey(_NewDefender.name))
					{
						FIndexDatabase[_NewDefender.name] = AvaliableIndex;
						break;
					}
					
					FIndexDatabase.Add(_NewDefender.name, AvaliableIndex);
					break;
				}
			}
		}
		//If not,
		else
		{
			//Add the new defender into the index database with a new index
			if(FIndexDatabase.ContainsKey(_NewDefender.name))
			{
				FIndexDatabase[_NewDefender.name] = FIndexDatabase.Count + 1;
			}
			else
			{
				FIndexDatabase.Add(_NewDefender.name,FIndexDatabase.Count + 1);
			}
			
			if(FPositionDatabase.ContainsKey(FIndexDatabase[_NewDefender.name]))
			{
				FPositionDatabase[FIndexDatabase[_NewDefender.name]] = Vector2.zero;
			}
			else
			{
				FPositionDatabase.Add(FIndexDatabase[_NewDefender.name],Vector2.zero);
			}

			if(FAvaliabilityDatabase.ContainsKey(FIndexDatabase[_NewDefender.name]))
			{
				FAvaliabilityDatabase[FIndexDatabase[_NewDefender.name]] = false;
			}
			else
			{
				FAvaliabilityDatabase.Add(FIndexDatabase[_NewDefender.name], false);
			}
			
			//Update the database to recalculate the position for all indexes
			UpdateDatabaseFormation(CurrentFormation,fCurrentMainScale);
		}
	}
	
	public void ReturnFormationPos(GameObject _ObjectReturn)
	{
		if(FIndexDatabase.ContainsKey(_ObjectReturn.name))
		{
			int IndexReturned = FIndexDatabase[_ObjectReturn.name];
			FAvaliabilityDatabase[IndexReturned] = true;
		}
	}
	
	private bool IsThereAvaliableIndex()
	{
		foreach(int Key in FAvaliabilityDatabase.Keys)
		{
			if(FAvaliabilityDatabase[Key])
			{
				return true;
			}
		}
		return false;
	}
	
	private int GetFirstAvaliableIndex()
	{
		foreach(int Key in FAvaliabilityDatabase.Keys)
		{
			if(FAvaliabilityDatabase[Key])
			{
				return Key;
			}
		}
		return 999;
	}

    public Vector2 GetTargetFormationPosition(Formation _Formation, GameObject _EnemyCell)
    {
		if(!FIndexDatabase.ContainsKey(_EnemyCell.name))
		{
			MessageDispatcher.Instance.DispatchMessage(_EnemyCell,_EnemyCell,MessageType.Idle,0);
			return Vector2.zero;
		}
    
		int TargetFIndex = FIndexDatabase[_EnemyCell.name];

		//Debug.Log(TargetFIndex);
		Vector2 PosDifference = FPositionDatabase[TargetFIndex];
		Vector2 EMPosition = GameObject.Find("Enemy_Cell").transform.position;
		Vector2 TargetPosition = Vector2.zero;
		
		if(_Formation == Formation.QuickCircle)
		{
			TargetPosition = new Vector2(EMPosition.x + PosDifference.x, EMPosition.y + PosDifference.y);
			return TargetPosition;
		}
		
		TargetPosition = new Vector2(PosDifference.x, EMPosition.y + PosDifference.y);
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

    private int[] GetCircularCenterIndexes(int _CurrentIndex)
    {
		int[] Indexes = new int[2];
		int CenterStart = 0;
		int CenterEnd = 11;

		if(_CurrentIndex > 11)
		{
			CenterStart = _CurrentIndex;

			while(CenterStart % 12 != 0)
			{
				CenterStart--;
			}

			CenterEnd = CenterStart + 11;
		}

		Indexes[0] = CenterStart;
		Indexes[1] = CenterEnd;

		return Indexes;
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
		List<string> Keys = new List<string>(FIndexDatabase.Keys);
		foreach(string Key in Keys)
		{
			Debug.Log("Name: " + Key + " Index: " + FIndexDatabase[Key]);
		}
    }

	private void PrintPositionDatabase()
	{
		List<int> Keys = new List<int>(FPositionDatabase.Keys);
		foreach(int Key in Keys)
		{
			Debug.Log("Index: " + Key + " Position: " + FPositionDatabase[Key]);
		}
	}
	
	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
