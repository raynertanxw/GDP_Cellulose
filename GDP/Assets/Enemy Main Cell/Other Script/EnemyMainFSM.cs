using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyMainFSM : MonoBehaviour 
{
	public static EnemyMainFSM instance;
	// Singleton
	public static EnemyMainFSM Instance() {
		if (instance == null)
			instance = new EnemyMainFSM();
		
		return instance;
	}
	#region Classes for help
	public EMController emController;
	public EMHelper emHelper;
	public EMTransition emTransition;
	#endregion
	#region State classes
	IEMState m_ProdctionState = null;
	public IEMState ProductionState { get { return m_ProdctionState; } }
	IEMState m_DefendState = null;
	public IEMState DefendState { get { return m_DefendState; } }
	IEMState m_MaintainState = null;
	public IEMState MaintainState { get { return m_MaintainState; } }
	IEMState m_AggressiveAttackState = null;
	public IEMState AggressiveAttackState { get { return m_AggressiveAttackState; } }
	IEMState m_CautiousAttackState = null;
	public IEMState CautiousAttackState { get { return m_CautiousAttackState; } }
	IEMState m_LandmineState = null;
	public IEMState LandmineState { get { return m_LandmineState; } }
	IEMState m_StunnedState = null;
	public IEMState StunnedState { get { return m_StunnedState; } }
	IEMState m_DieState = null;
	public IEMState DieState { get { return m_DieState; } }
	#endregion
	private Dictionary<EMState, IEMState> m_statesDictionary;
	private IEMState m_CurrentState = null;
	public IEMState CurrentState { get { return m_CurrentState; } }

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

	void Awake ()
	{
		m_statesDictionary = new Dictionary<PCState, IPCState>();
		m_statesDictionary.Add(EMState.Production, new EMProductionState(this));
		m_statesDictionary.Add (EMState.Maintain, new EMMaintainState (this));
	}

	void Start ()
	{
		#region State classes
		m_ProdctionState = new EMProductionState (this);
		m_DefendState = new EMDefendState (this);
		m_MaintainState = new EMMaintainState (this);
		m_AggressiveAttackState = new EMAggressiveAttackState (this);
		m_CautiousAttackState = new EMCautiousAttackState (this);
		m_LandmineState = new EMLandmineState (this);
		m_StunnedState = new EMStunnedState (this);
		m_DieState = new EMDieState (this);
		#endregion

		// Get the enemy main controller, helper class and transition class
		emController = GetComponent<EMController> ();
		emHelper = GetComponent<EMHelper> ();
		emTransition = GetComponent<EMTransition> ();
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
		m_CurrentState = m_ProdctionState;
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

	#region Coroutine functions
	public void StartProduceChild ()
	{
		StartCoroutine (ProduceChild ());
	}

	public void StartPauseTransition (float fTime)
	{
		StartCoroutine (emTransition.TransitionAvailability (fTime));
	}
	#endregion
}