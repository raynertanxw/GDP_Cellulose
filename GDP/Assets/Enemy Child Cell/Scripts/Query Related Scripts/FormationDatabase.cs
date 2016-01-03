﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationDatabase
{
	private static FormationDatabase s_Instance;

	private Dictionary<string,int> FIndexDatabase;
    private Dictionary<int, Vector2> FPositionDatabase;// the vector2 stored will be the position difference from the supposed position to the enemy main cell position
    
	public Dictionary<string,int> FIDatabase { get{ return FIndexDatabase;} }
	public Dictionary<int, Vector2> FPDatabase { get{ return FPositionDatabase;}}
    
	public FormationDatabase()
	{
		InitializeDatabases();
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
		Debug.Log("Initialize");
		FIndexDatabase = new Dictionary<string, int>();
		FPositionDatabase = new Dictionary<int, Vector2>();
		ClearDatabases();
    }
    
	public void RefreshDatabases(List<EnemyChildFSM> _EnemyChild)
    {
		ClearDatabases();

		int FormationIndex = 0;
    
		foreach(EnemyChildFSM Child in _EnemyChild)
		{
			FIndexDatabase.Add(Child.gameObject.name,FormationIndex);
			FPositionDatabase.Add(FormationIndex,new Vector2(0f,0f));
			FormationIndex++;
		}
		
		//PrintIndexDatabase();
    }
    
    public void ClearDatabases()
    {
		//Debug.Log("Clear");
		FIndexDatabase.Clear();
		FPositionDatabase.Clear();
    }
    
	public void UpdateDatabaseFormation(Formation _FormationType, List<EnemyChildFSM> _EnemyChild)
    {
		RefreshDatabases(_EnemyChild);

		//Debug.Log(_EnemyChild.Count);
		
		Vector2 EMPos = GameObject.Find("Enemy_Cell").transform.position;
    
		if(_FormationType == Formation.Crescent)
		{
			Vector2 CurrentFormationPos = new Vector2(0f,-1.4f);//Although it is positive 1.4f here, it should be negative. It's left positive as it will be minus away from the main pos
			Vector2 StoredFormationPos = new Vector2(0f,0f);
			float XInterval = 0.55f;// and -0.65 for left side
			float YInterval = 0.3f;//add -0.6f to the next line
			float NextLineInterval = -0.6f;
			int RightCount = 0;
			int LeftCount = 0;
			List<int> Keys = new List<int>(FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				//if the current formation cell is the start of a new formation row
				if(FIndex % 9 == 0)//if the formation index is to the factor of 9 (9 Cells are required to make one row in the formation
				{
					RightCount = 0;
					LeftCount = 0;
				
					if(FIndex == 0)
					{
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						continue;
					}
					else
					{
						Debug.Log("Previous one: " + FPositionDatabase[FIndex - 9]);
						CurrentFormationPos = new Vector2(FPositionDatabase[FIndex - 9].x, FPositionDatabase[FIndex - 9].y + NextLineInterval);
						StoredFormationPos = CurrentFormationPos;
						FPositionDatabase[FIndex] = StoredFormationPos;
						continue;
					}
				}
				else if(FIndex % 2 != 0)//if the formation index is odd (All cells in the right wing of the formation has odd fIndex)
				{	
					RightCount++;
					
					int CurrentCenterIndex = GetCrescentCenterIndex(FIndex);
					
					Debug.Log("CurrentCenter = " + CurrentCenterIndex);
					//Debug.Log("empos.y: " + EMPos.y + " fposDatabase.y: " + FPositionDatabase[CurrentCenterIndex].y + " rightCount * yInterval: " + RightCount * YInterval); 
					
					CurrentFormationPos = new Vector2(FPositionDatabase[CurrentCenterIndex].x + RightCount * XInterval, FPositionDatabase[CurrentCenterIndex].y + RightCount * YInterval);
					StoredFormationPos = CurrentFormationPos;
					FPositionDatabase[FIndex] = StoredFormationPos;
					
					continue;
				}
				else if(FIndex % 2 == 0)//if the formation index is even (All cells in the left wing of the formation has even fIndex)
				{
					LeftCount++;
					
					int CurrentCenterIndex = GetCrescentCenterIndex(FIndex);
					
					Debug.Log("CurrentCenter = " + CurrentCenterIndex);
					
					CurrentFormationPos = new Vector2(FPositionDatabase[CurrentCenterIndex].x - LeftCount * XInterval,FPositionDatabase[CurrentCenterIndex].y + LeftCount * YInterval);
					StoredFormationPos = CurrentFormationPos;
					FPositionDatabase[FIndex] = StoredFormationPos;
					
					continue;
				}
			}
		}
		else if(_FormationType == Formation.ReverseCrescent)
		{
			Vector2 CurrentFormationPos = new Vector2(EMPos.x,EMPos.y - 2f);
			Vector2 StoredFormationPos = new Vector2(0f,0f);
			float XInterval = 0.65f;// and -0.65 for left side
			float YInterval = -0.3f;
			float NextLineInterval = -0.6f;
			int RightCount = 0;
			int LeftCount = 0;
			List<int> Keys = new List<int>(FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				if(FIndex % 9 == 0)
				{
					if(FIndex == 0)
					{
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase.Add(FIndex,StoredFormationPos);
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(EMPos.x, FPositionDatabase[FIndex - 9].y + NextLineInterval);
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase.Add(FIndex,StoredFormationPos);
						continue;
					}
				}
				else if(FIndex % 2 != 0)
				{
					RightCount++;
					
					int CurrentCenterIndex = GetCrescentCenterIndex(FIndex);
					
					CurrentFormationPos = new Vector2(FPositionDatabase[CurrentCenterIndex].x + RightCount * XInterval, FPositionDatabase[CurrentCenterIndex].y + RightCount * YInterval);
					StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
					FPositionDatabase.Add(FIndex,StoredFormationPos);
					
					continue;
				}
				else if(FIndex % 2 == 0)
				{
					LeftCount++;
					
					int CurrentCenterIndex = GetCrescentCenterIndex(FIndex);
					
					CurrentFormationPos = new Vector2(FPositionDatabase[CurrentCenterIndex].x - LeftCount * XInterval, FPositionDatabase[CurrentCenterIndex].y + LeftCount * YInterval);
					StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
					FPositionDatabase[FIndex] = StoredFormationPos;
					
					continue;
				}
			}
		}
		else if(_FormationType == Formation.CircularSurround)
		{
			Vector2 CurrentFormationPos = new Vector2(EMPos.x,EMPos.y - 0.68f);
			Vector2 StoredFormationPos = new Vector2(0f,0f);
			float XInterval = 0.34f;
			float YInterval = 0.34f;
			float NextLineInterval = -0.4f;
			int RightCount = 0;
			int LeftCount = 0;
			List<int> Keys = new List<int>(FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				//front central cell
				if(FIndex % 12 == 0)
				{
					if(FIndex == 0)
					{
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase[FIndex] = StoredFormationPos;
						continue;
					}
					else
					{
						//The circle will grow larger as more are being stack
						XInterval *= 1.5f;
						YInterval *= 1.5f;
					
						CurrentFormationPos = new Vector2(FPositionDatabase[FIndex - 9].x, FPositionDatabase[FIndex - 9].y + NextLineInterval);
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase[FIndex] = StoredFormationPos;
						continue;
					}
				}
				
				//Back central cell
				if(FIndex % 11 == 0)
				{
					Vector2 RightCellToCurrent = FPositionDatabase[FIndex - 2];
					Vector2 LeftCellToCurrent = FPositionDatabase[FIndex - 1];
					
					CurrentFormationPos = new Vector2((RightCellToCurrent.x + LeftCellToCurrent.x)/2, (RightCellToCurrent.y + LeftCellToCurrent.y)/2);
					StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
					FPositionDatabase[FIndex] = StoredFormationPos;
					continue;
				}
				
				if(FIndex % 2 != 0)
				{
					RightCount++;
					int[] CircularCentral = GetCircularCenterIndexes(FIndex);
					Vector2 StartPoint = FPositionDatabase[CircularCentral[0]];
					Vector2 EndPoint = FPositionDatabase[CircularCentral[1]];
					
					if(RightCount == 1)
					{
						CurrentFormationPos = new Vector2(StartPoint.x + XInterval, StartPoint.y);
					}
					else if(RightCount == 5)
					{
						CurrentFormationPos = new Vector2(EndPoint.x + XInterval, EndPoint.y);
					}
					else
					{
						CurrentFormationPos = new Vector2(StartPoint.x + 2 * XInterval, StartPoint.y + (FIndex - 1) * YInterval);
					}
					
					StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
					FPositionDatabase[FIndex] = StoredFormationPos;
					continue;
				}
				else if(FIndex % 2 == 0)
				{
					LeftCount++;
					int[] CircularCentral = GetCircularCenterIndexes(FIndex);
					Vector2 StartPoint = FPositionDatabase[CircularCentral[0]];
					Vector2 EndPoint = FPositionDatabase[CircularCentral[1]];
					
					if(LeftCount == 1)
					{
						CurrentFormationPos = new Vector2(StartPoint.x - XInterval, StartPoint.y);
					}
					else if(LeftCount == 5)
					{
						CurrentFormationPos = new Vector2(EndPoint.x - XInterval, EndPoint.y);
					}
					else
					{
						CurrentFormationPos = new Vector2(StartPoint.x - 2 * XInterval, StartPoint.y + (FIndex - 1) * YInterval);
					}
					
					StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
					FPositionDatabase[FIndex] = StoredFormationPos;
					continue;
				}
			}
		}
		else if(_FormationType == Formation.AreaBlock)
		{
			int ECAmount = FIndexDatabase.Count;
			int SetAmount = (int) Mathf.Floor(ECAmount/9);
			Vector2 CurrentFormationPos = new Vector2(EMPos.x,EMPos.y - 0.75f);
			Vector2 StoredFormationPos = new Vector2(0f,0f);
			float XInterval = 0.34f;
			float YInterval = 0.3f;
			int RightCount = 0;
			int LeftCount = 0;
			List<int> Keys = new List<int>(FPositionDatabase.Keys);
			
			foreach(int FIndex in Keys)
			{
				if(FIndex % 9 == 0)
				{
					if(FIndex == 0)
					{
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase[FIndex] = StoredFormationPos;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[FIndex - 9].x, FPositionDatabase[FIndex - 9].y + YInterval);
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase[FIndex] = StoredFormationPos;
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
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase[FIndex] = StoredFormationPos;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[AreaCenterIndex].x + RightCount * XInterval, FPositionDatabase[AreaCenterIndex].y + YInterval);
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase[FIndex] = StoredFormationPos;
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
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase[FIndex] = StoredFormationPos;
						continue;
					}
					else
					{
						CurrentFormationPos = new Vector2(FPositionDatabase[AreaCenterIndex].x - LeftCount * XInterval, FPositionDatabase[AreaCenterIndex].y + YInterval);
						StoredFormationPos = new Vector2(EMPos.x - CurrentFormationPos.x, EMPos.y - CurrentFormationPos.y);
						FPositionDatabase[FIndex] = StoredFormationPos;
						continue;
					}
				}
			}
		}
		
		//PrintPositionDatabase();
    }
    
    public Vector2 GetTargetFormationPosition(GameObject _EnemyCell)
    {
		int TargetFIndex = FIndexDatabase[_EnemyCell.name];
		Vector2 PosDifference = FPositionDatabase[TargetFIndex];
		
		Vector2 EMPosition = GameObject.Find("Enemy_Cell").transform.position;
		Vector2 TargetPosition = new Vector2(EMPosition.x + PosDifference.x, EMPosition.y + PosDifference.y);

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
		int Current = _CurrentIndex;
		
		while(Current % 11 != 0)
		{
			Current++;
		}
		
		Indexes[1] = Current;
		
		Current = _CurrentIndex;
		
		while(Current % 11 != 0)
		{
			Current--;
		}
		
		Indexes[0] = Current;
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
    
    public bool IsAllCellInPosition()
    {
		return true;
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

}
