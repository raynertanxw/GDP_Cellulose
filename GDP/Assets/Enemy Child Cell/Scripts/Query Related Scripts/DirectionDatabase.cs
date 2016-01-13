using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectionDatabase
{
	private static DirectionDatabase s_Instance;
	
	private Dictionary<int,Vector2> m_Database;
	
	private Dictionary<int,bool> m_Usage;
	
	private int MaximumECCells;
	
	public DirectionDatabase()
	{
		m_Database = new Dictionary<int, Vector2>();
		m_Usage = new Dictionary<int, bool>();
		MaximumECCells = ECPoolManager.ECPool.Count;
		
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
		for(int i = 0; i < MaximumECCells; i++)
		{
			m_Usage.Add(i + 1, false);
		}
		
		int CellPerCircle = 15;
		float AngleInterval = 360f/CellPerCircle;
		float CurrentAngle = Random.Range(0f,360f);
		float AngleTraveled = 0f;
		
		Vector2 FirstDirection = GetDirectionFromAngle(CurrentAngle);
		Vector2 TargetDirection = Vector2.zero;
		
		m_Database.Add(1,FirstDirection);
		for(int i = 2; i <= MaximumECCells; i++)
		{
			CurrentAngle += AngleInterval;
			if(CurrentAngle > 360f)
			{
				CurrentAngle -= 360f;
			}
			
			AngleTraveled += AngleInterval;
			
			TargetDirection = GetDirectionFromAngle(CurrentAngle);
			m_Database.Add(i,TargetDirection);
		}
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
	
	private Vector2 GetDirectionFromAngle(float _Angle)
	{
		_Angle *= Mathf.Deg2Rad;
		return new Vector2(Mathf.Cos(_Angle),Mathf.Sin(_Angle));
	}
}
