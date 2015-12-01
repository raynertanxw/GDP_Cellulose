using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyMainFSM : MonoBehaviour 
{
	public EMController emController;

	private IEMState m_CurrentState = null;
	public IEMState CurrentState { get { return m_CurrentState; } }

	public GameObject enemyMain;

	// Enemy Child
	private List<EnemyChildFSM> ecList = new List<EnemyChildFSM>();
	public List<EnemyChildFSM> ECList { get { return ecList; } }
	private int nAvailableChildNum;
	public int AvailableChildNum { get { return nAvailableChildNum; } }

	// Health
	private int nHealth;
	public int Health { get { return nHealth; } }
	// Damage
	private int nDamageNum;
	// Aggressiveness
	private int nAggressiveness;
	public int Aggressiveness { get { return nAggressiveness; } }
	// Production State
	private bool bCanSpawn; 
	public bool CanSpawn { get { return bCanSpawn; } }

	void Start()
	{
		// Get the enemy main controller
		emController = enemyMain.GetComponent<EMController> ();
        // Initialise the enemy child list
        /*
		EnemyChildFSM[] ecClasses = (EnemyChildFSM[])Object.FindObjectsOfType (typeof(EnemyChildFSM));
		foreach (EnemyChildFSM ecClass in ecClasses) 
		{
			if (ecClass != null)
				ecList.Add (ecClass);
		}
		ecList = new List<EnemyChildFSM>();
        */
        ecList = GameObject.FindGameObjectsWithTag("EnemyChild").Select(gameObject => gameObject.GetComponent<EnemyChildFSM>()).ToList();    
        // Count the number of child cells in list
        nAvailableChildNum = ecList.Count;
		// Initialise the default to Production
		m_CurrentState = EMProductionState.Instance ();
		// Initialise num of damages and aggressiveness
		nDamageNum = 0;
		nAggressiveness = 10;
		// Initialise status
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
			//GameObject newChild = (GameObject)Instantiate (enemyChild, transform.position, Quaternion.identity);
			//ecList.Add (newChild);
			//emController.nNutrientNum--;

			bCanSpawn = false;
			yield return new WaitForSeconds (2);
			bCanSpawn = true;
		}
	}
}