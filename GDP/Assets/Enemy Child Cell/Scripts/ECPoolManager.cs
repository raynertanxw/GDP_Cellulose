using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECPoolManager : MonoBehaviour {

	private static Queue<GameObject> m_ECPool;
	public GameObject EnemyChildCell;

	// Use this for initialization
	void Start () 
	{
		Debug.Log("start pool");
		if(m_ECPool == null)
		{
			m_ECPool = new Queue<GameObject>();
		}
	
		//Store all enemy child cells into the pool
		foreach(Transform child in transform)
		{
			if(child.tag == Constants.s_strEnemyChildTag)
			{
				m_ECPool.Enqueue(child.gameObject);
			}
		}
	}
	
	public static void AddToPool (GameObject _EnemyChild)
	{
		if(m_ECPool == null)
		{
			m_ECPool = new Queue<GameObject>();
		}
		m_ECPool.Enqueue(_EnemyChild);
	}
	
	public GameObject SpawnFromPool(Vector2 _SpawnPos)
	{
		Debug.Log("spawn from pool");
		EnemyMainFSM.Instance ().StartProduceChild ();
		if(IsPoolEmpty())
		{
			RestockPool();
		}
		
		GameObject newChild = m_ECPool.Dequeue();
		newChild.transform.position = _SpawnPos;
		MessageDispatcher.Instance.DispatchMessage(this.gameObject,newChild,MessageType.Idle,0);
		EnemyMainFSM.Instance ().ECList.Add (newChild.GetComponent<EnemyChildFSM> ());
		EnemyMainFSM.Instance ().AvailableChildNum++;
		
		return newChild;
	}

	private bool IsPoolEmpty()
	{
		if(m_ECPool.Count > 0)
		{
			return false;
		}
		return true;
	}
	
	//a function to extend the size of the pool by adding another enemy child cell
	private void RestockPool()
	{
		GameObject newChild = (GameObject) Instantiate(EnemyChildCell, transform.position, Quaternion.identity);
		MessageDispatcher.Instance.DispatchMessage(gameObject, newChild, MessageType.Dead, 0);
	}
}
