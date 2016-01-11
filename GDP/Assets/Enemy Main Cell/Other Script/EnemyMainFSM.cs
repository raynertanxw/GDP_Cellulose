using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyMainFSM : MonoBehaviour 
{
	// Instance of the class
	private static EnemyMainFSM instance;
	// Singleton
	public static EnemyMainFSM Instance()
	{
		return instance;
	}

	// Current position of the enemy main cell
	private Vector2 m_position;
	public Vector2 Position { get { return m_position; } set { m_position = value; } }
	[SerializeField]
	private GameObject enemyMainObject;
	[SerializeField]
	private ECPoolManager ECPool;
	public GameObject EnemyMainObject { get { return enemyMainObject; } }
	#region Classes for help
	[HideInInspector]
	public EMController emController;
	[HideInInspector]
	public EMHelper emHelper;
	[HideInInspector]
	public EMTransition emTransition;
	#endregion
	/*#region State classes
	IEMState m_ProductionState = null;
	public IEMState ProductionState { get { return m_ProductionState; } }
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
	#endregion*/

	// Dictionary of states of enemy main cell
	private Dictionary<EMState, IEMState> m_statesDictionary;
	public Dictionary<EMState, IEMState> StatesDictionary { get { return m_statesDictionary; } }
	// Current State
	private IEMState m_CurrentState = null;
	public IEMState CurrentState { get { return m_CurrentState; } }
	private EMState m_currentStateIndex;
	public EMState CurrentStateIndex { get { return m_currentStateIndex; } }

	// Enemy Child
	private List<EnemyChildFSM> ecList = new List<EnemyChildFSM>();
	public List<EnemyChildFSM> ECList { get { return ecList; } }
	// Available child number
	[Header("Available child number")]
	[Tooltip("Available child number of the Enemy Main Cell")]
	[SerializeField] private int nAvailableChildNum;
	public int AvailableChildNum { get { return nAvailableChildNum; } set { nAvailableChildNum = value; } }

	#region Health
	[Header("Health")]
	[Tooltip("Health of the Enemy Main Cell")]
	[SerializeField] private int nHealth;
	public int Health { get { return nHealth; } }
	#endregion
	#region Aggressiveness
	[Header("Aggressiveness")]
	[Tooltip("Initial Aggressiveness of the Enemy Main Cell")]
	[SerializeField] private float nInitialAggressiveness;
	public float InitialAggressiveness { get { return nInitialAggressiveness; } }
	[Tooltip("Current Aggressiveness of the Enemy Main Cell")]
	[SerializeField] private float nCurrentAggressiveness;
	public float CurrentAggressiveness { get { return nCurrentAggressiveness; } set { nCurrentAggressiveness = value; } }
	[Tooltip("Current Squad Captain Aggressiveness of the Enemy Main Cell")]
	[SerializeField] private float nAggressivenessSquadCap;
	public float AggressivenessSquadCap { get { return nAggressivenessSquadCap; } set { nAggressivenessSquadCap = value; } }
	[Tooltip("Current Squad Child Aggressiveness of the Enemy Main Cell")]
	[SerializeField] private float nAggressivenessSquadChild;
	public float AggressivenessSquadChild { get { return nAggressivenessSquadChild; } set { nAggressivenessSquadChild = value; } }
	#endregion
	#region Learning Element
	[Header("Learning Element")]
	[Tooltip("Score of Enemy Main Production State")]
	[SerializeField] private float fProductionScore;
	public float ProductionScore { get { return fProductionScore; } set { fProductionScore = value; } }
	[Tooltip("Score of Enemy Main Maintain State")]
	[SerializeField] private float fMaintainScore;
	public float MaintainScore { get { return fMaintainScore; } set { fMaintainScore = value; } }
	[Tooltip("Score of Enemy Main Defend State")]
	[SerializeField] private float fDefendScore;
	public float DefendScore { get { return fDefendScore; } set { fDefendScore = value; } }
	[Tooltip("Score of Enemy Main Aggressive Attack State")]
	[SerializeField] private float fAggressiveAttackScore;
	public float AggressiveAttackScore { get { return fAggressiveAttackScore; } set { fAggressiveAttackScore = value; } }
	[Tooltip("Score of Enemy Main Cautious Attack State")]
	[SerializeField] private float fCautiousAttackScore;
	public float CautiousAttackScore { get { return fCautiousAttackScore; } set { fCautiousAttackScore = value; } }
	[Tooltip("Score of Enemy Main Landmine State")]
	[SerializeField] private float fLandmineScore;
	public float LandmineScore { get { return fLandmineScore; } set { fLandmineScore = value; } }
	// Dictionary of Learning Element
	private Dictionary<EMState, float> m_learningDictionary;
	public Dictionary<EMState, float> LearningDictionary{ get { return m_learningDictionary; } }
	public void SetLearningScore (EMState key, float value) {m_learningDictionary [key] = value;}
	#endregion
	// Production status
	private bool bCanSpawn; 
	public bool CanSpawn { get { return bCanSpawn; } }
	public ECPoolManager Pool { get { return ECPool; } }

	void Awake ()
	{
		if (instance == null)
			instance = this;

		m_position = gameObject.transform.position;

		nAvailableChildNum = 0;
		// Initialise num of health and aggressiveness
		nHealth = 50;
		nInitialAggressiveness = 5;
		nCurrentAggressiveness = nInitialAggressiveness;
		nAggressivenessSquadCap = 0;
		nAggressivenessSquadChild = 0;
		// Initialise status
		bCanSpawn = true;
		//ECPool = GameObject.Find("Enemy Child Cell Pool").GetComponent<ECPoolManager>();
	}

	void Start ()
	{
		enemyMainObject = this.gameObject;
		#region Initialize state dictionary
		m_statesDictionary = new Dictionary<EMState, IEMState>();
		m_statesDictionary.Add (EMState.Production, new EMProductionState (this));
		m_statesDictionary.Add (EMState.Maintain, new EMMaintainState (this));
		m_statesDictionary.Add (EMState.Defend, new EMDefendState (this));
		m_statesDictionary.Add (EMState.AggressiveAttack, new EMAggressiveAttackState (this));
		m_statesDictionary.Add (EMState.CautiousAttack, new EMCautiousAttackState (this));
		m_statesDictionary.Add (EMState.Landmine, new EMLandmineState (this));
		m_statesDictionary.Add (EMState.Stunned, new EMStunnedState (this));
		m_statesDictionary.Add (EMState.Die, new EMDieState (this));
		#endregion

		#region Initialize the Learning Element dictionary
		m_learningDictionary = new Dictionary<EMState, float>();
		m_learningDictionary.Add (EMState.Production, 0f);
		m_learningDictionary.Add (EMState.Maintain, 0f);
		m_learningDictionary.Add (EMState.Defend, 0f);
		m_learningDictionary.Add (EMState.AggressiveAttack, 0f);
		m_learningDictionary.Add (EMState.CautiousAttack, 0f);
		m_learningDictionary.Add (EMState.Landmine, 0f);
		m_learningDictionary.Add (EMState.Stunned, 0f);
		m_learningDictionary.Add (EMState.Die, 0f);
		#endregion

		// Initialize the default to Production
		m_CurrentState = m_statesDictionary [EMState.Production];
		m_currentStateIndex = EMState.Production;

		// Get the enemy main controller, helper class and transition class
		emController = GetComponent<EMController> ();
		emHelper = GetComponent<EMHelper> ();
		emTransition = GetComponent<EMTransition> ();
		
		//Initialize and refresh the point database for Enemy Child Cells
		PointDatabase.Instance.InitializeDatabase();
		PointDatabase.Instance.RefreshDatabase();
		FormationDatabase.Instance.RefreshDatabases(ECList);
		
        // Initialise the enemy child list
        /*
		EnemyChildFSM[] ecClasses = (EnemyChildFSM[])GameObject.FindObjectsOfType (typeof(EnemyChildFSM));
		foreach (EnemyChildFSM ecClass in ecClasses) 
		{
			if (ecClass.CurrentStateEnum != ECState.Dead)
				ecList.Add (ecClass);
		}
        */
        // ecList = GameObject.FindGameObjectsWithTag("EnemyChild").Select(gameObject => gameObject.GetComponent<EnemyChildFSM>()).ToList();    
        // Count the number of child cells in list
		/*
		for (int i = 0; i < ecList.Count; i++) {
			if(ecList[i].CurrentStateEnum != ECState.Dead)
				nAvailableChildNum++;
		}
		*/
	}

	void Update()
	{
		m_CurrentState.Execute ();
	}

	// Change current state, perform exit and enter functions
	public void ChangeState (EMState newState)
	{
		if (m_CurrentState != null)
			m_CurrentState.Exit ();

		m_currentStateIndex = newState;
		m_CurrentState = m_statesDictionary [newState];
		m_CurrentState.Enter ();
	}

	public IEnumerator ProduceChild ()
	{
		if (bCanSpawn) 
		{
			bCanSpawn = false;
			/*
			for (int i = 0; i < ecList.Count; i++)
			{
				if (ecList[i].CurrentStateEnum == ECState.Dead)
				{
					ecList[i].ChangeState(ECState.Idle);
					break;
				}
			}
			nAvailableChildNum++;
			emController.ReduceNutrient ();

			bCanSpawn = false;
			*/
			/*GameObject newChild = ECPool.SpawnFromPool(gameObject.transform.position);	
			newChild.transform.SetParent(this.transform);
			ecList.Add (newChild.GetComponent<EnemyChildFSM> ());
			nAvailableChildNum++;
			newChild.GetComponent<Rigidbody2D>().velocity = emController.Rigibody.velocity;
			emController.ReduceNutrient ();*/
			/*
			GameObject newChild = (GameObject) Instantiate(enemyMiniPrefab, transform.position, Quaternion.identity);
			newChild.transform.SetParent(this.transform);
			ecList.Add (newChild.GetComponent<EnemyChildFSM> ());
			nAvailableChildNum++;
			newChild.GetComponent<Rigidbody2D>().velocity = emController.Rigibody.velocity;
			emController.ReduceNutrient ();
			*/
			emController.ReduceNutrient ();
			// Randomize the interval time between spawns of child cells in terms of num of available child cells
			yield return new WaitForSeconds (UnityEngine.Random.Range (Mathf.Sqrt(Mathf.Sqrt((float)nAvailableChildNum)), 
			                                               Mathf.Sqrt ((float)nAvailableChildNum)));
			bCanSpawn = true;
		}
	}

	#region Coroutine functions
	// Things needed when produce child cells
	public void StartProduceChild ()
	{
		StartCoroutine (ProduceChild ());
	}
	// Disable transition for fTime
	public void StartPauseTransition (float fTime)
	{
		StartCoroutine (emTransition.TransitionAvailability (fTime));
	}
	// Stop commanding child cells to Attack state for fTime
	public void StartPauseAddAttack (float fTime)
	{
		StartCoroutine (emHelper.PauseAddAttack (fTime));
	}
	// Stop commanding child cells to Defend state for fTime
	public void StartPauseAddDefend (float fTime)
	{
		StartCoroutine (emHelper.PauseAddDefend (fTime));
	}
	// Stop commanding child cells to Landmine state for fTime
	public void StartPauseAddLandmine (float fTime)
	{
		StartCoroutine (emHelper.PauseAddLandmine (fTime));
	}
	#endregion

	// Update the score on the inspector
	public void ScoreUpdate ()
	{
		fProductionScore = LearningDictionary [EMState.Production];
		fMaintainScore = LearningDictionary [EMState.Maintain];
		fDefendScore = LearningDictionary [EMState.Defend];
		fAggressiveAttackScore = LearningDictionary [EMState.AggressiveAttack];
		fCautiousAttackScore = LearningDictionary [EMState.CautiousAttack];
		fLandmineScore = LearningDictionary [EMState.Landmine];
	}

	public void ScoreLimit ()
	{
		Mathf.Clamp (LearningDictionary [EMState.Production], -100f, 100f);
		Mathf.Clamp (LearningDictionary [EMState.Maintain], -100f, 100f);
		Mathf.Clamp (LearningDictionary [EMState.Defend], -100f, 100f);
		Mathf.Clamp (LearningDictionary [EMState.AggressiveAttack], -100f, 100f);
		Mathf.Clamp (LearningDictionary [EMState.CautiousAttack], -100f, 100f);
		Mathf.Clamp (LearningDictionary [EMState.Landmine], -100f, 100f);
	}
}