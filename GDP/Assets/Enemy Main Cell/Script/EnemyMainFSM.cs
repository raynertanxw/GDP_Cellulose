using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMainFSM : MonoBehaviour 
{
	IEMState m_CurrentState = null;

	public GameObject enemyMain;
	public GameObject ecPrefab;
	List<GameObject> ecList = new List<GameObject>();
	private int nAvailableChild;

	private int nSize;
	private Vector2 initialScale;
	private Vector2 currentScale;
	private int nDamageNum;

	public bool bCanSpawn;

	EMController emController;

	void Start()
	{
		m_CurrentState = EMProductionState.Instance ();

		nAvailableChild = 0;

		nSize = 50;
		initialScale = gameObject.transform.localScale;
		currentScale = initialScale * nSize;
		nDamageNum = 0;

		bCanSpawn = true;

		emController = enemyMain.GetComponent<EMController> ();
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

			bCanSpawn = false;
			yield return new WaitForSeconds (2);
			bCanSpawn = true;
		}
	}

	void ProductionTransition ()
	{
		if (nAvailableChild < 10) 
		{
			if (m_CurrentState != null)
				m_CurrentState.Exit ();

			m_CurrentState = EMProductionState.Instance();
			m_CurrentState.Enter ();
		}
	}

	void StunnedTransition ()
	{
		if (emController.bStunned) 
		{
			if (m_CurrentState != null)
				m_CurrentState.Exit ();
			
			m_CurrentState = EMStunnedState.Instance();
			m_CurrentState.Enter ();
		}
	}

	void DieTransition ()
	{
		if (nSize <= 0) 
		{
			if (m_CurrentState != null)
				m_CurrentState.Exit ();
			
			m_CurrentState = EMDieState.Instance();
			m_CurrentState.Enter ();
		}
	}
}