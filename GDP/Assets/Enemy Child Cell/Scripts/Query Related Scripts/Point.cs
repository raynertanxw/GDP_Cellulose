using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point 
{
	private string m_strIndex;
	private string m_strLIndex;
	private string m_strHIndex;
	
	private bool m_bWalkable;
	
	private Vector2 m_Position;
	private List<Edge> m_EdgeList;
	
	public Point(string _Index, Vector2 _Pos, bool _Walkable) 
	{
		m_strIndex = _Index;
		m_Position = _Pos;
		m_bWalkable = _Walkable;
		m_EdgeList = new List<Edge>();
		
		if(_Index.Contains("-"))
		{
			string[] keys = _Index.Split('-');
			m_strLIndex = keys[0];
			m_strHIndex = keys[1];
		}
	}
	
	public string Index
	{
		get{ return m_strIndex;}
	}
	
	public string LIndex
	{
		get{ return m_strLIndex;}
	}
	
	public string HIndex
	{
		get{ return m_strHIndex;}
	}
	
	public Vector2 Position
	{
		get{ return m_Position;}
		set{ m_Position = value;}
	}
	
	public bool Walkable
	{
		get{ return m_bWalkable;}
		set{ m_bWalkable = value;}
	}
	
	public List<Edge> Edges
	{
		get{ return m_EdgeList;}
		set{ m_EdgeList = value;}
	}

}
