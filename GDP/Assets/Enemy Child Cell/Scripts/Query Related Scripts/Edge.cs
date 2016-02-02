using UnityEngine;
using System.Collections;

public class Edge 
{
	private Point m_Source;
	private Point m_Target;
	private float m_fCost;
	
	public Edge(Point _Start, Point _End, float _Cost)
	{
		m_Source = _Start;
		m_Target = _End;
		m_fCost = _Cost;
	}
	
	public Point Start
	{
		get{ return m_Source;}
		set{ m_Source = value;}
	}
	
	public Point End
	{
		get{ return m_Target;}
		set{ m_Target = value;}
	}
	
	public float Cost
	{
		get{ return m_fCost;}
		set{ m_fCost = value;}
	}
}
