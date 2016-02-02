using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointDatabase
{
	//Declare an instance of the PointDatabase to make Database Singleton
	private static PointDatabase s_Instance;
	
	private Vector2 m_InitialPlayerPos;
	private GameObject m_PlayerMain;
	private Dictionary<string,Point> m_Database;
	
	public float m_PointIntervalX;
	public float m_PointIntervalY;
	
	//Constructor for the PointDatabase
	public PointDatabase()
	{
		m_Database = new Dictionary<string, Point>();
		m_PlayerMain = GameObject.Find("Player_Cell");
		m_InitialPlayerPos = m_PlayerMain.transform.position;
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
		float LeftWallX = (-Screen.width/2) + 0.5f;
		float RightWallX = (Screen.width/2) - 0.5f;
		float RadiusOfCell = m_PlayerMain.GetComponent<SpriteRenderer>().bounds.size.x/2;
		
		return (_Point.Position.x + RadiusOfCell > RightWallX || _Point.Position.x - RadiusOfCell < LeftWallX) ? false : true;
	}
	
	public void InitializeDatabase()
	{
		float fScreenWidth = Screen.width;
		float fScreenHeight = Screen.height;
		
		Vector2 topLeft = Camera.main.ScreenToWorldPoint(new Vector2(0,fScreenHeight));
		Vector2 botRight = Camera.main.ScreenToWorldPoint(new Vector2(fScreenWidth,0));
		
		//width: 9 Height : 14
		m_PointIntervalX = (botRight.x - topLeft.x)/8;
		m_PointIntervalY = (topLeft.y - botRight.y)/14; 
		
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
				currentGeneration.x += m_PointIntervalX;
				HKey++;
			}
			
			if(HKey >= 9)
			{
				currentGeneration.y -= m_PointIntervalY;
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
		float CostBetweenPoints = Utility.Distance(m_Database["0-0"].Position,m_Database["0-1"].Position);
		foreach(string key in Keys)
		{
			if(m_Database[key] != null)
			{
				Point PointUsed = null;
				
				if(GetPointNextToGivenPoint("Up",m_Database[key]) != null && !m_Database[key].Index.Contains("0-"));
				{
					PointUsed = GetPointNextToGivenPoint("Up",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUsed,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Down",m_Database[key]) != null && !m_Database[key].Index.Contains("13-"))
				{
					PointUsed = GetPointNextToGivenPoint("Down",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUsed,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null)
				{
					PointUsed = GetPointNextToGivenPoint("Left",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUsed,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null)
				{
					PointUsed = GetPointNextToGivenPoint("Right",m_Database[key]);
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUsed,CostBetweenPoints));
				}
				
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null && GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Left",m_Database[key]))) != null && !m_Database[key].Index.Contains("0-"))
				{
					PointUsed = GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Left",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUsed,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null && GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key]))) != null && !m_Database[key].Index.Contains("0-") && m_Database[key].LIndex != GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key]))).LIndex)
				{
					PointUsed = GetPointNextToGivenPoint("Up",(GetPointNextToGivenPoint("Right",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUsed,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Left",m_Database[key]) != null && GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Left",m_Database[key]))) != null && !m_Database[key].Index.Contains("13-") && !m_Database[key].Index.Contains("-0"))
				{
					PointUsed = GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Left",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUsed,CostBetweenPoints));
				}
				if(GetPointNextToGivenPoint("Right",m_Database[key]) != null && GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Right",m_Database[key]))) != null && !m_Database[key].Index.Contains("13-") && !m_Database[key].Index.Contains("-8"))
				{
					PointUsed = GetPointNextToGivenPoint("Down",(GetPointNextToGivenPoint("Right",m_Database[key])));
					m_Database[key].Edges.Add(new Edge(m_Database[key],PointUsed,CostBetweenPoints));
				}
			}
		}
	}
	
	public void RefreshDatabase()
	{
		//CREATE LIST OF STRINGS TO STORE THE KEY OF THE DICTIONARY
		float fDifferenceY = m_PlayerMain.transform.position.y - m_InitialPlayerPos.y;
		List<string> keys = new List<string>(m_Database.Keys);
		
		foreach(string key in keys)
		{
			m_Database[key].Position = new Vector2(m_Database[key].Position.x, m_Database[key].Position.y + fDifferenceY);
			m_Database[key].Walkable = IsPointWalkable(m_Database[key]);
		}
	}

	public Point GetClosestPointToPosition(Vector2 _Pos, bool _UnwalkableAllowed)
	{
		List<string> keys = new List<string>(m_Database.Keys);
		Point ClosestPoint = new Point("",Vector2.zero,false);
		float ClosestDistance = Mathf.Infinity;
		
		foreach(string key in keys)
		{
			if(Utility.Distance(_Pos, m_Database[key].Position) < ClosestDistance && (_UnwalkableAllowed == true||m_Database[key].Walkable == true))
			{
				ClosestPoint = m_Database[key];
				ClosestDistance = Utility.Distance(_Pos, m_Database[key].Position);
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
		float Difference = Utility.Distance(InitialPoint.Position,_Target);
		
		//Check if there is any nearby point that is more ideal (more direct towards the target)
		Point TargetPoint = InitialPoint;
		
		for(int i = 0; i < InitialPoint.Edges.Count; i++)
		{
			if(Utility.Distance(InitialPoint.Edges[i].End.Position,_Target) < Difference)
			{
				TargetPoint = InitialPoint.Edges[i].End;
				Difference = Utility.Distance(InitialPoint.Edges[i].End.Position,_Target);
			}
		} 
		
		return TargetPoint;
	}
	
	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
