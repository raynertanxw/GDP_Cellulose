using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point 
{
	private string strIndex;
	private string strLIndex;
	private string strHIndex;
	private Vector2 m_Position;
	private bool bWalkable;
	private List<Edge> EdgeList;
	
	public Point(string _Index, Vector2 _Pos, bool _Walkable) 
	{
		strIndex = _Index;
		m_Position = _Pos;
		bWalkable = _Walkable;
		EdgeList = new List<Edge>();
		
		if(_Index.Contains("-"))
		{
			string[] keys = _Index.Split('-');
			strLIndex = keys[0];
			strHIndex = keys[1];
		}
		
		Utility.DrawCross(Position,Color.green,0.1f);
	}
	
	public string Index
	{
		get{ return strIndex;}
	}
	
	public string LIndex
	{
		get{ return strLIndex;}
	}
	
	public string HIndex
	{
		get{ return strHIndex;}
	}
	
	public Vector2 Position
	{
		get{ return m_Position;}
		set{ m_Position = value;}
	}
	
	public bool Walkable
	{
		get{ return bWalkable;}
		set{ bWalkable = value;}
	}
	
	public List<Edge> Edges
	{
		get{ return EdgeList;}
		set{ EdgeList = value;}
	}
	
	/*public Point GetSurroundingPoint (Edge _edge)
	{
		if(EdgeList[_edge] != null)
		{
			return EdgeList[_edge];
		}
		return null;
	}*/
}
