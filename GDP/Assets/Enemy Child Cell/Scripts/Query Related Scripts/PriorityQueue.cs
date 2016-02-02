using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue
{
	private Queue<string> m_Queue;
	private Dictionary<string,float> m_Cost;
	
	public PriorityQueue(Dictionary<string,float> _fCost)
	{
		m_Queue = new Queue<string>();
		m_Cost = _fCost;
	}
	
	public void Add(string index)
	{
		if(CheckLowestCost(index))
		{
			EnqueueToFront(index);
		}
		else
		{
			m_Queue.Enqueue(index);
		}
	}
	
	bool CheckLowestCost(string _index)
	{
		var items = m_Queue.ToArray();
		for(int i = 0; i < items.Length; i++)
		{
			if(m_Cost[items[i]] < m_Cost[_index])
			{
				return false;
			}
		}
		return true;
	}
	
	public void DebugAllEntries()
	{
		Debug.Log("count: " + m_Queue.Count);
		for(int i = 0; i < m_Queue.Count; i++)
		{
			string current = m_Queue.Dequeue();
			Debug.Log(current);
			m_Queue.Enqueue(current);
		}
		Debug.Log("after count: " + m_Queue.Count);
	}
	
	void EnqueueToFront(string index)
	{
		if(m_Queue.Count >= 1)
		{
			var indexes = m_Queue.ToArray();
			m_Queue.Clear();
			m_Queue.Enqueue(index);
			foreach(var i in indexes)
			{
				m_Queue.Enqueue(i);
			}
		}
		else
		{
			m_Queue.Enqueue(index);
		}
	}
	
	public void ChangePriority(string index)
	{
		if(CheckLowestCost(index))
		{
			EnqueueToFront(index);
		}
	}
	
	public string Dequeue()
	{
		string dq = m_Queue.Dequeue();
		return dq;
	}
	
	public int Count()
	{
		return m_Queue.Count;
	}
}
