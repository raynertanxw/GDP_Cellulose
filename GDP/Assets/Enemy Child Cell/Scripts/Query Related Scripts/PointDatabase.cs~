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
		float LeftWallX = GameObject.Find("Left Wall").transform.position.x;
		float RightWallX = GameObject.Find("Right Wall").transform.position.x; 
		float RadiusOfCell = GameObject.Find("Enemy_Child_Cell").GetComponent<SpriteRenderer>().bounds.size.x/2;
		float WidthOfWall = GameObject.Find("Left Wall").GetComponent<SpriteRenderer>().bounds.size.x;
		
		if(_Point.Position.x + RadiusOfCell + WidthOfWall/2 > RightWallX || _Point.Position.x - RadiusOfCell - WidthOfWall/2 < LeftWallX)
		{
			return false;
		}
		
		return true;
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
			m_Database[CurrentKey].Walkable = IsPointWalkable(m_Database[CurrentKey]);
			//Utility.DrawCross(currentGeneration,Color.red,0.05f);
		
			Count++;
		}
		
		List<string> Keys = new List<string>(m_Database.Keys);
		float CostBetweenPoints = Vector2.Distance(m_Database["0-0"].Position,m_Database["0-1"].Position);
		foreach(string key in Keys)
		{
			if(m_Database[key] != null)
			{
				if(GetPointNextToGivenPoint("Up",m_Database[key]) != null && !m_Database[key].Index.Contains("0-"));
				{
					Point PointUp = GetPointNextToGivenPoint("Up",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUp,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Down",m_Database[key]) != null && !m_Database[key].Index.Contains("13-"))
				{
					Point PointDown = GetPointNextToGivenPoint("Down",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointDown,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null)
				{
					Point PointLeft = GetPointNextToGivenPoint("Left",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointLeft,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null)
				{
					Point PointRight = GetPointNextToGivenPoint("Right",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointRight,CostBetweenPoints));
				}
				
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null && GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Left",m_Database[key]))) != null && !m_Database[key].Index.Contains("0-"))
				{
					Point PointTopLeft = GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Left",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointTopLeft,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null && GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key]))) != null && !m_Database[key].Index.Contains("0-") && m_Database[key].LIndex != GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key]))).LIndex)
				{
					Point PointTopRight = GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointTopRight,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null && GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Left",m_Database[key]))) != null && !m_Database[key].Index.Contains("13-") && !m_Database[key].Index.Contains("-0"))
				{
					Point PointBotLeft = GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Left",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointBotLeft,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null && GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Right",m_Database[key]))) != null && !m_Database[key].Index.Contains("13-") && !m_Database[key].Index.Contains("-8"))
				{
					Point PointBotRight = GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Right",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointBotRight,CostBetweenPoints));
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

	public Point GetClosestPointToPosition(Vector2 _Pos, bool _UnwalkableAllowed)
	{
		List<string> keys = new List<string>(m_Database.Keys);
		Point ClosestPoint = new Point("",new Vector2(0f,0f),false);
		float ClosestDistance = Mathf.Infinity;
		
		foreach(string key in keys)
		{
			if(Vector2.Distance(_Pos, m_Database[key].Position) < ClosestDistance && (_UnwalkableAllowed == true||m_Database[key].Walkable == true))
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
			if(GivenHKey < 8)
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
	
	public Point GetIdealPoint(Vector2 _Current, Vector2 _Target)
	{
		//Get the current closest point to the agent
		Point InitialPoint = GetClosestPointToPosition(_Current,false);
		float Difference = Vector2.Distance(InitialPoint.Position,_Target);
		
		//Check if there is any nearby point that is more ideal (more direct towards the target)
		Point TargetPoint = InitialPoint;
		
		for(int i = 0; i < InitialPoint.Edges.Count; i++)
		{
			if(Vector2.Distance(InitialPoint.Edges[i].End.Position,_Target) < Difference)
			{
				TargetPoint = InitialPoint.Edges[i].End;
				Difference = Vector2.Distance(InitialPoint.Edges[i].End.Position,_Target);
			}
		} 
		
		return TargetPoint;
	}
}
