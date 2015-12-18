using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointDatabase
{
	//Declare an instance of the PointDatabase to make Database Singleton
	private static PointDatabase s_Instance;
	
	//Declare a dictionary to store the various tactical position in the database
	private Dictionary<string,Point> m_Database;
	
	private Vector2 m_InitialPlayerPos;
	public float PointIntervalX;
	public float PointIntervalY;
	
	//Constructor for the PointDatabase
	public PointDatabase()
	{
		m_Database = new Dictionary<string, Point>();
		m_InitialPlayerPos = GameObject.Find("Player_Cell").transform.position;
	}
	
	//Singleton and Get function
	public static PointDatabase Instance
	{
		get
		{
			if(s_Instance == null)
			{
				Debug.Log("initialize");
				s_Instance = new PointDatabase();
			}
			return s_Instance;
		}
	}
	
	public Dictionary<string,Point> Database
	{
		get{ return m_Database;}
	}
	
	private bool IsPointWalkable(Point _Point)
	{
		Collider2D[] PointCollisions = Physics2D.OverlapPointAll(_Point.Position);
		foreach(Collider2D collision in PointCollisions)
		{
			if(collision.tag == "Wall")
			{
				return true;
			}
		}
		return false;
	}
	
	public void InitializeDatabase()
	{
		float fScreenWidth = Screen.width;
		float fScreenHeight = Screen.height;
		
		Vector2 topLeft = Camera.main.ScreenToWorldPoint(new Vector2(0,fScreenHeight));
		Vector2 botRight = Camera.main.ScreenToWorldPoint(new Vector2(fScreenWidth,0));
		
		//width: 9 Height : 14
		PointIntervalX = (botRight.x - topLeft.x)/8;
		PointIntervalY = (topLeft.y - botRight.y)/14; 
		
		Vector2 currentGeneration = topLeft;
		int HKey = 0;
		int LKey = 0;
		int Count = 0;
		
		m_Database.Add(LKey.ToString() + "-" + HKey.ToString(), new Point(LKey.ToString() + "-" + HKey.ToString(),currentGeneration,true));
		//Utility.DrawCross(currentGeneration,Color.red,0.05f);
		
		while(Count < 125)
		{
			if(HKey <= 8)
			{
				currentGeneration.x += PointIntervalX;
				HKey++;
			}
			
			if(HKey >= 9)
			{
				currentGeneration.y -= PointIntervalY;
				currentGeneration.x = topLeft.x;
				LKey++;
				HKey = 0;
			}
			
			string CurrentKey = LKey.ToString() + "-" + HKey.ToString();
			m_Database.Add(CurrentKey, new Point(LKey.ToString() + "-" + HKey.ToString(),currentGeneration,true));
			Debug.Log("generate: " + LKey.ToString() + "-" + HKey.ToString());
			m_Database[CurrentKey].Walkable = IsPointWalkable(m_Database[CurrentKey]);
			//Utility.DrawCross(currentGeneration,Color.red,0.05f);
		
			Count++;
		}
		
		List<string> Keys = new List<string>(m_Database.Keys);
		foreach(string key in Keys)
		{
			if(m_Database[key] != null)
			{
				if(GetPointNextToGivenPoint("Up",m_Database[key]) != null && !m_Database[key].Index.Contains("0-"));
				{
					Point PointUp = GetPointNextToGivenPoint("Up",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUp,1.0f));
					PointUp.Edges.Add(new Edge(PointUp,m_Database[key],1.0f));
				}
				if(GetPointNextToGivenPoint("Down",m_Database[key]) != null && !m_Database[key].Index.Contains("13-"))
				{
					Point PointDown = GetPointNextToGivenPoint("Down",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointDown,1.0f));
					PointDown.Edges.Add(new Edge(PointDown,m_Database[key],1.0f));
				}
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null)
				{
					Point PointLeft = GetPointNextToGivenPoint("Left",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointLeft,1.0f));
					PointLeft.Edges.Add(new Edge(PointLeft,m_Database[key],1.0f));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null)
				{
					Point PointRight = GetPointNextToGivenPoint("Right",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointRight,1.0f));
					PointRight.Edges.Add(new Edge(PointRight,m_Database[key],1.0f));
				}
				
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null && GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Left",m_Database[key]))) != null && !m_Database[key].Index.Contains("0-"))
				{
					Point PointTopLeft = GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Left",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointTopLeft,1.0f));
					PointTopLeft.Edges.Add(new Edge(PointTopLeft,m_Database[key],1.0f));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null && GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key]))) != null && !m_Database[key].Index.Contains("0-") && m_Database[key].LIndex != GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key]))).LIndex)
				{
					Point PointTopRight = GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointTopRight,1.0f));
					PointTopRight.Edges.Add(new Edge(PointTopRight,m_Database[key],1.0f));
				}
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null && GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Left",m_Database[key]))) != null && !m_Database[key].Index.Contains("13-") && !m_Database[key].Index.Contains("-0"))
				{
					Point PointBotLeft = GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Left",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointBotLeft,1.0f));
					PointBotLeft.Edges.Add(new Edge(PointBotLeft,m_Database[key],1.0f));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null && GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Right",m_Database[key]))) != null && !m_Database[key].Index.Contains("13-") && !m_Database[key].Index.Contains("-8"))
				{
					Point PointBotRight = GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Right",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointBotRight,1.0f));
					PointBotRight.Edges.Add(new Edge(PointBotRight,m_Database[key],1.0f));
				}
			}
		}
	}
	
	public void RefreshDatabase()
	{
		//CREATE LIST OF STRINGS TO STORE THE KEY OF THE DICTIONARY
		float fDifferenceY = GameObject.Find("Player_Cell").transform.position.y - m_InitialPlayerPos.y;
		List<string> keys = new List<string>(m_Database.Keys);
		
		foreach(string key in keys)
		{
			m_Database[key].Position = new Vector2(m_Database[key].Position.x, m_Database[key].Position.y + fDifferenceY);
			m_Database[key].Walkable = IsPointWalkable(m_Database[key]);
		}
	}

	public List<Point> GetPointsBetweenAxis(string _XorY, float _Min, float _Max)
	{
		List<Point> PointsWithinRange = new List<Point>();
		List<string> keys = new List<string>(m_Database.Keys);
		
		if(_XorY == "X" || _XorY == "x")
		{
			foreach(string key in keys)
			{
				if(m_Database[key].Position.x >= _Min && m_Database[key].Position.x <= _Max)
				{
					PointsWithinRange.Add(m_Database[key]);
				}
			}
		}
		else if(_XorY == "Y" || _XorY == "y")
		{
			foreach(string key in keys)
			{
				if(m_Database[key].Position.y >= _Min && m_Database[key].Position.y <= _Max)
				{
					PointsWithinRange.Add(m_Database[key]);
				}
			}
		}
		
		return PointsWithinRange;
	}

	public Point GetClosestPointToPosition(Vector2 _Pos)
	{
		List<string> keys = new List<string>(m_Database.Keys);
		Point ClosestPoint = new Point("",new Vector2(0f,0f),false);
		float ClosestDistance = Mathf.Infinity;
		
		foreach(string key in keys)
		{
			if(Vector2.Distance(_Pos, m_Database[key].Position) < ClosestDistance)
			{
				ClosestPoint = m_Database[key];
				ClosestDistance = Vector2.Distance(_Pos, m_Database[key].Position);
			}
		}
		
		return ClosestPoint;
	}
	
	public Point GetPointNextToGivenPoint (string _Direction, Point _Given)
	{
		string[] keys = _Given.Index.Split('-');
		int GivenLKey = int.Parse(keys[0]);
		int GivenHKey = int.Parse(keys[1]);
		
		if(GivenLKey == 13)
		{
			Debug.Log ("Meow");
		}
		
		if(_Direction == "Up" || _Direction == "up")
		{
			int TargetLKey = GivenLKey - 1;
			if(TargetLKey < 0)
			{
				TargetLKey = 0;
			}
			return m_Database[TargetLKey.ToString() + "-" + GivenHKey.ToString()];
		}
		else if(_Direction == "Down" || _Direction == "down")
		{
			int TargetLKey = GivenLKey + 1;
			if(TargetLKey > 13)
			{
				TargetLKey = 13;
			}
			return m_Database[TargetLKey.ToString() + "-" + GivenHKey.ToString()]; 
		}
		else if(_Direction == "Left" || _Direction == "left")
		{
			if(GivenHKey > 0)
			{
				int TargetHKey = GivenHKey - 1;
				return m_Database[GivenLKey.ToString() + "-" + TargetHKey.ToString()]; 
			}
		}
		else if(_Direction == "Right" || _Direction == "right")
		{
			if(GivenHKey < 7)
			{
				int TargetHKey = GivenHKey + 1;
				return m_Database[GivenLKey.ToString() + "-" + TargetHKey.ToString()];
			}
		}
		return null;
	}
	
	public List<Point> ReturnDatabaseAsList()
	{
		List<string> Keys = new List<string>(m_Database.Keys);
		List<Point> Points = new List<Point>();
		foreach(string key in Keys)
		{
			Points.Add(m_Database[key]);
		}
		return Points;
	}
}
