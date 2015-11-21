using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMainFSM : MonoBehaviour 
{
	public EMController emController;

	public IEMState m_CurrentState = null;

	public GameObject enemyMain;
	// Enemy Child
	public GameObject ecPrefab;
	List<GameObject> ecList = new List<GameObject>();
	public int nAvailableChildNum;

	// Damage
	private int nDamageNum;
	// Aggressiveness
	public int nAggressiveness;
	// Production State
	public bool bCanSpawn; 

	void Start()
	{
		emController = enemyMain.GetComponent<EMController> ();

		m_CurrentState = EMProductionState.Instance ();

		nAvailableChildNum = 0;

		nDamageNum = 0;
		nAggressiveness = 10;

		bCanSpawn = true;
	}

	void Update()
	{
		m_CurrentState.Execute ();
	}

	public void ChangeState (IEMState newState)
	{
		if (m_CurrentState != null)
			m_CurrentState.Exit ();

		m_CurrentState = newState;
		m_CurrentState.Enter ();
	}

	public IEnumerator ProduceChild ()
	{
		if (bCanSpawn) 
		{
			GameObject newChild = (GameObject)Instantiate (ecPrefab, transform.position, Quaternion.identity);
			ecList.Add (newChild);
			emController.nSize--;

			bCanSpawn = false;
			yield return new WaitForSeconds (2);
			bCanSpawn = true;
		}
	}
}