using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectionDatabase
{
	private static DirectionDatabase s_Instance;
	
	private Dictionary<int,Vector2> m_Database;
	private Dictionary<int,bool> m_Usage;
	
	public DirectionDatabase()
	{
		m_Database = new Dictionary<int, Vector2>();
		m_Usage = new Dictionary<int, bool>();
		Initilize();
	}
	
	public static DirectionDatabase Instance
	{
		get
		{
			if(s_Instance == null)
			{
				s_Instance = new DirectionDatabase();
			}
			return s_Instance;
		}
	}
	
	public Dictionary<int,Vector2> Database
	{
		get{ return m_Database;}
	}
	
	public Dictionary<int,bool> Usage
	{
		get{ return m_Usage;}
	}
	
	private void Initilize()
	{
		for(int i = 0; i < 50; i++)
		{
			m_Usage.Add(i + 1, false);
		}
		
		float AngleInterval = 360f/50f;
		float CurrentAngle = 0f;
		Vector2 FirstDirection = new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f));
		Vector2 TargetDirection = Vector2.zero;
		m_Database.Add(1,FirstDirection);
		
		for(int i = 2; i <= 50; i++)
		{
			CurrentAngle += AngleInterval;
			Vector2 PreviousDirection = m_Database[i - 1];
			TargetDirection = Quaternion.Euler(0f,0f,CurrentAngle) * PreviousDirection;
			m_Database.Add(i,TargetDirection);
		}
		
		//PrintDatabase();
	}
	
	public Vector2 Extract()
	{
		List<int> Keys = new List<int>(m_Database.Keys);
		int ExtractIndex = 0;
		Vector2 ExtractVector = Vector2.zero;
		
		foreach(int Key in Keys)
		{
			if(m_Usage[Key] == false)
			{
				ExtractIndex = Key;
				ExtractVector = m_Database[ExtractIndex];
				break;
			}
		}
		
		m_Usage[ExtractIndex] = true;
		return ExtractVector;
	}
	
	public void Return(Vector2 _Position)
	{
		List<int> Keys = new List<int>(m_Database.Keys);
		foreach(int Key in Keys)
		{
			if(m_Database[Key] == _Position)
			{
				m_Usage[Key] = false;
				break;
			}
		}
	}
	
	private void PrintDatabase()
	{
		List<int> Keys = new List<int>(m_Database.Keys);
		foreach(int Key in Keys)
		{
			Debug.Log(Key + ": " + m_Database[Key].ToString("F2"));
		}
	}
}
