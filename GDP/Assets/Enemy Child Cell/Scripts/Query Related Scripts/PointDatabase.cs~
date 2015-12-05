using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointDatabase
{
	private enum Position {A1, A2, A3, B1, B2, B3, C1, C2, C3, D1, D2, D3}; 

	private static PointDatabase instance;
	private Dictionary<Position,Vector2> m_Database;
	
	//Constructor
	public PointDatabase()
	{
		m_Database = new Dictionary<Position,Vector2>();
		InitializeDatabase();
	}
	
	//Singleton
	public static PointDatabase Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new PointDatabase();
			}
			return instance;
		}
	}
	
	private void InitializeDatabase()
	{
		//Add the 12 positions into the database
		m_Database.Add(Position.A1, new Vector2(0f,0f));
		m_Database.Add(Position.A2, new Vector2(0f,0f));
		m_Database.Add(Position.A3, new Vector2(0f,0f));
		m_Database.Add(Position.B1, new Vector2(0f,0f));
		m_Database.Add(Position.B2, new Vector2(0f,0f));
		m_Database.Add(Position.B3, new Vector2(0f,0f));
		m_Database.Add(Position.C1, new Vector2(0f,0f));
		m_Database.Add(Position.C2, new Vector2(0f,0f));
		m_Database.Add(Position.C3, new Vector2(0f,0f));
		m_Database.Add(Position.D1, new Vector2(0f,0f));
		m_Database.Add(Position.D2, new Vector2(0f,0f));
		m_Database.Add(Position.D3, new Vector2(0f,0f));
	}
	
	public void RefreshDatabase(Vector2 _EnemyMainPos, Vector2 _PlayerMainPos, GameObject Wall)
	{
		//length of each vertical section of the screen (6 sections but only 4 at the center will be used as due to the exceeding length of the wall being the two sections)
		float fVertSection = Wall.GetComponent<SpriteRenderer>().bounds.size.y/6;
		
		//length of each horizontal section of the screen
		float fHoriSection = Mathf.Abs(Wall.transform.position.x/2);
		
		//declare variables for calculation purpose
		Vector2 m_PosCalculate = new Vector2(0f, Screen.height);
		m_PosCalculate = Camera.main.ScreenToWorldPoint(m_PosCalculate);
		
		//positions at the centre top and centre bottom of the screen
		Vector2 m_CentreTop = new Vector2(0f,m_PosCalculate.y/2);
		Vector2 m_CentreBot = new Vector2(0f, -m_CentreTop.y);
		
		//Go through every position in the database and refresh it based on player's main position
		m_Database[Position.A1] = new Vector2(_EnemyMainPos.x + -fHoriSection, _EnemyMainPos.y + 1.0f * fVertSection);
		m_Database[Position.A2] = new Vector2(_EnemyMainPos.x, m_Database[Position.A1].y);
		m_Database[Position.A3] = new Vector2(_EnemyMainPos.x + fHoriSection, m_Database[Position.A1].y);
		
		m_Database[Position.B1] = new Vector2(_EnemyMainPos.x + -fHoriSection, _EnemyMainPos.y + 0.5f * fVertSection);
		m_Database[Position.B2] = new Vector2(_EnemyMainPos.x,  m_Database[Position.B1].y);
		m_Database[Position.B3] = new Vector2(_EnemyMainPos.x + fHoriSection, m_Database[Position.B1].y);
		
		m_Database[Position.C1] = new Vector2(_EnemyMainPos.x + -fHoriSection, -_EnemyMainPos.y + -0.5f * fVertSection);
		m_Database[Position.C2] = new Vector2(_EnemyMainPos.x , m_Database[Position.C1].y);
		m_Database[Position.C3] = new Vector2(_EnemyMainPos.x + fHoriSection, m_Database[Position.C1].y);
		
		m_Database[Position.D1] = new Vector2(_EnemyMainPos.x + -fHoriSection, -_EnemyMainPos.y + -1.0f * fVertSection);
		m_Database[Position.D2] = new Vector2(_EnemyMainPos.x, m_Database[Position.D1].y);
		m_Database[Position.D3] = new Vector2(_EnemyMainPos.x + fHoriSection, m_Database[Position.D1].y);
		
		DrawPoints();
	}
	
	public List<Vector2> ExtractPosYRange (float _MinY, float _MaxY)
	{
		List<Vector2> m_PositionsBelowY = new List<Vector2>();
		
		foreach(Vector2 position in m_Database.Values)
		{
			if(position.y >= _MinY && position.y <= _MaxY)
			{
				m_PositionsBelowY.Add(position);
			}
		}
		
		return m_PositionsBelowY;
	}
	
	public List<Vector2> ExtractPosXRange (float _MinX, float _MaxX)
	{
		List<Vector2> m_PositionsSubX = new List<Vector2>();
		
		foreach(Vector2 position in m_Database.Values)
		{
			if(position.x >= _MinX && position.x <= _MaxX)
			{
				m_PositionsSubX.Add(position);
			}
		}
		
		return m_PositionsSubX;
	}
	
	private void DrawPoints()
	{
		
		foreach(Vector2 position in m_Database.Values)
		{
			Debug.Log(position);
			Utility.DrawCross(position,Color.red,0.1f);
		}
	}
}
