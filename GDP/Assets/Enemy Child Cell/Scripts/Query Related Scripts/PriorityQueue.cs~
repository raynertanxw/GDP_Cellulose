using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue
{
	private Queue<string> queue;
	private Dictionary<string,float> cost;
	
	public PriorityQueue(Dictionary<string,float> fCost)
	{
		queue = new Queue<string>();
		cost = fCost;
	}
	
	public void Add(string index)
	{
		if(CheckLowestCost(index))
		{
			//Debug.Log("enqueue to front");
			EnqueueToFront(index);
		}
		else
		{
			queue.Enqueue(index);
		}
		
		//DebugAllEntries();
	}
	
	bool CheckLowestCost(string index)
	{
		var items = queue.ToArray();
		for(int i = 0; i < items.Length; i++)
		{
			//Debug.Log(items[i] + ": " + cost[items[i]] + " " + index + ": " + cost[index]);
			
			if(cost[items[i]] < cost[index])
			{
				return false;
			}
		}
		return true;
	}
	
	public void DebugAllEntries()
	{
		Debug.Log("count: " + queue.Count);
		for(int i = 0; i < queue.Count; i++)
		{
			string current = queue.Dequeue();
			Debug.Log(current);
			queue.Enqueue(current);
		}
		Debug.Log("after count: " + queue.Count);
	}
	
	void EnqueueToFront(string index)
	{
		if(queue.Count >= 1)
		{
			var indexes = queue.ToArray();
			queue.Clear();
			queue.Enqueue(index);
			foreach(var i in indexes)
			{
				queue.Enqueue(i);
			}
		}
		else
		{
			queue.Enqueue(index);
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
		string dq = queue.Dequeue();
		return dq;
	}
	
	public int Count()
	{
		return queue.Count;
	}
	
	bool IsIndexSameAsFirst(string index)
	{
		Debug.Log("index: " + index);
		Debug.Log("peek: " + queue.Peek());
		if(index == queue.Peek())
		{
			return true;
		}
		return false;
	}
}
