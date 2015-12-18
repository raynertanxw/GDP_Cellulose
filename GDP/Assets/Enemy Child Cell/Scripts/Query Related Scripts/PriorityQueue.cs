using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue
{
	private Queue<int> queue;
	private float[] cost;
	
	public PriorityQueue(float[] fCost)
	{
		queue = new Queue<int>();
		cost = fCost;
	}
	
	public void Add(int index)
	{
		if(CheckLowestCost(index))
		{
			EnqueueToFront(index);
		}
		else
		{
			queue.Enqueue(index);
		}
	}
	
	void PrintCosts()
	{
		for(int i = 0; i < cost.Length; i++)
		{
			Debug.Log("cost index: " + i + " , element: " + cost[i]);
		}
	}
	
	bool CheckLowestCost(int index)
	{
		var items = queue.ToArray();
		for(int i = 0; i < items.Length; i++)
		{
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
			int current = queue.Dequeue();
			Debug.Log(current);
			queue.Enqueue(current);
		}
		Debug.Log("after count: " + queue.Count);
	}
	
	void EnqueueToFront(int index)
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
	
	public void ChangePriority(int index)
	{
		if(CheckLowestCost(index))
		{
			EnqueueToFront(index);
		}
	}
	
	public int Dequeue()
	{
		int dq = queue.Dequeue();
		return dq;
	}
	
	public int Count()
	{
		return queue.Count;
	}
	
	bool IsIndexSameAsFirst(int index)
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
