using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECPoolManager : MonoBehaviour {

	private static Queue<GameObject> s_ECPool;
	private ECTracker s_ECTracker;
	
	public GameObject EnemyChildCell;
	
	private int SpawnCount;

	// Use this for initialization
	void Start () 
	{
		if(s_ECPool == null)
		{
			s_ECPool = new Queue<GameObject>();
		}
		
		s_ECTracker = ECTracker.Instance;
		
		//Store all enemy child cells into the pool
		foreach(Transform child in transform)
		{
			if(child.tag == Constants.s_strEnemyChildTag)
			{
				s_ECPool.Enqueue(child.gameObject);
			}
		}
		
		SpawnCount = 0;
	}
	
	public static Queue<GameObject> ECPool
	{
		get{ return s_ECPool;}
	}
	
	//A function to add a Enemy Child Cell to the pool
	public static void AddToPool (GameObject _EnemyChild)
	{
		if(s_ECPool == null)
		{
			s_ECPool = new Queue<GameObject>();
		}
		s_ECPool.Enqueue(_EnemyChild);
	}
	
	//Extract an Enemy Child Cell from the pool and spawn it to the position given in the perimeter
	public GameObject SpawnFromPool(Vector2 _SpawnPos, bool _Default)
	{
		if (!_Default)
			EMHelper.Instance ().StartProduceChild ();
		SpawnCount++;
		
		//Extract the enemy child cell pool from the pool and add it to the enemy child cell list
		GameObject newChild = s_ECPool.Dequeue();
		newChild.transform.position = _SpawnPos;
		MessageDispatcher.Instance.DispatchMessage(this.gameObject,newChild,MessageType.Idle,0);
		EnemyMainFSM.Instance ().ECList.Add (newChild.GetComponent<EnemyChildFSM> ());
		EnemyMainFSM.Instance ().AvailableChildNum++;
		
		if(IsPoolEmpty())
		{
			return null;
		}
		
		return newChild;
	}

	//A function to check if the pool is empty
	private bool IsPoolEmpty()
	{
		return (s_ECPool.Count > 0) ? false : true;
	}
	
	public static void ResetStatics()
	{
		s_ECPool = null;
	}
}
