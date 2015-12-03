using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECPoolManager : MonoBehaviour {

	private static List<GameObject> m_ECPool;

	// Use this for initialization
	void Start () 
	{
		m_ECPool = new List<GameObject>();
	
		//Store all enemy child cells into the pool
		foreach(Transform child in transform)
		{
			if(child.tag == Constants.s_strEnemyChildTag)
			{
				m_ECPool.Add(child.gameObject);
			}
		}
	}
	
	public static void AddToPool (GameObject _EnemyChild)
	{
		m_ECPool.Add(_EnemyChild);
	}
	
	public GameObject SpawnFromPool(Vector2 _SpawnPos)
	{
		//Retrieve the first child in the pool
		GameObject m_Child = m_ECPool[0];
		m_ECPool.RemoveAt(0);
	
		//Spawn it at the specified location	
		m_Child.transform.position = _SpawnPos;
		MessageDispatcher.Instance.DispatchMessage(this.gameObject, m_Child, MessageType.Idle, 0);
		
		return m_Child;
	}
	
	
}
