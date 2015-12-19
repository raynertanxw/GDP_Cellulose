using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathQuery 
{
	private PointDatabase m_Database;
	private PositionQuery m_PositionQuery;
	
	private GameObject PlayerMain;
	private GameObject EnemyMain;
	
	private Dictionary<string,Edge> SearchFrontier;
	private Dictionary<string,Edge> ShortestPath;
	private Dictionary<string,float> fCost;
	private Dictionary<string,float> gCost;
	
	private string SourceIndex;
	private string TargetIndex;
	
	public PathQuery()
	{
		m_Database = PointDatabase.Instance;
		m_PositionQuery = PositionQuery.Instance;
		PlayerMain = GameObject.Find("Player_Cell");
		EnemyMain = GameObject.Find("Enemy_Cell");
	}
	
	private float CalculateDistanceBetweenPoints(Point _P1, Point _P2)
	{
		return Vector2.Distance(_P1.Position,_P2.Position);
	}
	
	public void AStarSearch(string _Source, string _Target, Directness _Directness)
	{
		SourceIndex = m_Database.Database[_Source].Index;
		TargetIndex = m_Database.Database[_Target].Index;
		
		List<Point> PointList = PointDatabase.Instance.ReturnDatabaseAsList();
		SearchFrontier = new Dictionary<string,Edge>();//store all points that are being searched
		ShortestPath = new Dictionary<string,Edge>();
		fCost = new Dictionary<string,float>();//cumulative cost to current node + heuristic cost from current node to target node
		gCost = new Dictionary<string,float>();//cumulative cost from the source node to the current node
	
		PriorityQueue priorityQueue = new PriorityQueue(fCost);
		priorityQueue.Add(SourceIndex);
		
		while(priorityQueue.Count() != 0)
		{
			string nextClosestPoint = priorityQueue.Dequeue();
			
			if(nextClosestPoint == SourceIndex || nextClosestPoint == TargetIndex)
			{
				Utility.DrawCross(m_Database.Database[nextClosestPoint].Position,Color.green,0.1f);
			}
			else
			{
				Utility.DrawCross(m_Database.Database[nextClosestPoint].Position,Color.magenta,0.1f);
			}
			
			ShortestPath[nextClosestPoint] = SearchFrontier[nextClosestPoint];
			
			if(nextClosestPoint == TargetIndex)
			{
				return;
			}
			
			foreach(Edge edge in m_Database.Database[nextClosestPoint].Edges)
			{
				//heuristic cost from current node to target node
				float HCost = CalculateDistanceBetweenPoints(edge.End,m_Database.Database[TargetIndex]);
				//cumulative cost from the start node to the current node
				float GCost = gCost[nextClosestPoint] + edge.Cost;
				
				if(SearchFrontier[edge.End.Index] == null && edge.End.Walkable == true)
				{
					fCost[edge.End.Index] = HCost + GCost;
					gCost[edge.End.Index] = GCost;
					priorityQueue.Add(edge.End.Index);
					SearchFrontier[edge.End.Index] = edge;
				}
				else if(GCost < gCost[edge.End.Index] && ShortestPath[edge.End.Index] == null && edge.End.Walkable == true)
				{
					fCost[edge.End.Index] = HCost + GCost;
					gCost[edge.End.Index] = GCost;
					priorityQueue.Add(edge.End.Index);
					SearchFrontier[edge.End.Index] = edge;
				}
			}
		}
		return;
	}
	
	public List<Point> GetPathToTarget()
	{
		List<Point> Path = new List<Point>();
		string currentKey = TargetIndex;
		Path.Insert(0,m_Database.Database[currentKey]);
		
		while(currentKey != SourceIndex && ShortestPath[currentKey] != null)
		{
			currentKey = ShortestPath[currentKey].Start.Index;
			Path.Insert(0,m_Database.Database[currentKey]);
		}
		
		return Path;
	}
	
	
}
