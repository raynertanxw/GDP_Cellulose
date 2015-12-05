using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (CircleCollider2D))]
[RequireComponent (typeof (SpriteRenderer))]
public class PlayerChildFSM : MonoBehaviour
{
	#region Pool Control
	// singleton list to hold all out playerChildPoolControllers.
	static private List<PlayerChildFSM> s_playerChildFSMPool;
	
	static public PlayerChildFSM Spawn(Vector3 spawnPoint)
	{
		foreach (PlayerChildFSM currentPCFSM in s_playerChildFSMPool)
		{
			// If disabled, thn it's available.
			if (currentPCFSM.m_currentEnumState == PCState.Dead)
			{
				// Set it up
				(currentPCFSM.m_statesDictionary[PCState.Dead] as PC_DeadState).CallFromPool(spawnPoint);
				
				// return a reference to the caller.
				return currentPCFSM;
			}
		}
		
		// If we get here we haven't pooled enough.
		Debug.Log("Exhausted Player Child pool --> Increase pool size");
		
		return null;
	}

	void OnDestroy()
	{
		// remove myself from the pool
		s_playerChildFSMPool.Remove(this);
		// was I the last one?
		if (s_playerChildFSMPool.Count == 0)
		{
			s_playerChildFSMPool = null;
		}
	}
	#endregion

	public static EnemyMainFSM s_eMain;
	public static PlayerMain s_pMain;
	private static int s_nActiveChildCount = 0;

	
	private PCState m_currentEnumState;
	private IPCState m_currentState;
	private Dictionary<PCState, IPCState> m_statesDictionary;
	[SerializeField]
	public bool m_bIsDefending = true;
	public bool m_bHasAwaitingDeferredStateChange = false;
	private PCState m_deferredState;
	public Transform m_assignedNode;
	public EnemyChildFSM m_currentEnemyCellTarget;
	public Squad_Manager m_assignedSquad;


	// Component Cache.
	public Rigidbody2D rigidbody2D;
	public CircleCollider2D collider2D;
	public SpriteRenderer spriteRen;

	#region Getter functions
	public PCState GetCurrentState() { return m_currentEnumState; }

	public static int GetActiveChildCount() { return s_nActiveChildCount; }
	#endregion

	#region Setter functions
	public static void SetActiveChildCount(bool increase)
	{
		if (increase)
			s_nActiveChildCount++;
		else
			s_nActiveChildCount--;
	}
	#endregion

	public void ChangeState(PCState newState)
	{
		m_currentState.Exit();
		m_currentState = m_statesDictionary[newState];
		m_currentState.Enter();
		m_currentEnumState = newState;
	}

	public void DeferredChangeState(PCState newState)
	{
		m_deferredState = newState;
		m_bHasAwaitingDeferredStateChange = true;
	}

	public void ExecuteDeferredStateChange()
	{
		ChangeState(m_deferredState);
		m_bHasAwaitingDeferredStateChange = false;
	}


	// For Enemy to call
	public void KillPlayerChildCell()
	{
		DeferredChangeState(PCState.Dead);
	}

	void Awake()
	{
		// does the pool exist yet
		if (s_playerChildFSMPool == null)
		{
			// lazy initialize it
			s_playerChildFSMPool = new List<PlayerChildFSM>();
		}
		// add myself
		s_playerChildFSMPool.Add(this);


		// Cache components
		rigidbody2D = GetComponent<Rigidbody2D>();
		collider2D = GetComponent<CircleCollider2D>();
		spriteRen = GetComponent<SpriteRenderer>();

		// Set up the m_statesDictionary.
		m_statesDictionary = new Dictionary<PCState, IPCState>();
		m_statesDictionary.Add(PCState.Avoid, new PC_AvoidState(this));
		m_statesDictionary.Add(PCState.ChargeChild, new PC_ChargeChildState(this));
		m_statesDictionary.Add(PCState.ChargeMain, new PC_ChargeMainState(this));
		m_statesDictionary.Add(PCState.Dead, new PC_DeadState(this));
		m_statesDictionary.Add(PCState.Defend, new PC_DefendState(this));
		m_statesDictionary.Add(PCState.Idle, new PC_IdleState(this));


		PlayerChildFSM.SetActiveChildCount(true);
		// Default Start currentState is dead state.
		m_currentState = m_statesDictionary[PCState.Dead];
		m_currentState.Enter();
		m_currentEnumState = PCState.Dead;
	}
	
	void Start ()
	{
	
	}

	void Update ()
	{
		m_currentState.Execute();
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		// Hit Enemy Child
		if (col.gameObject.tag == Constants.s_strEnemyChildTag)
		{
			// Kill Enemy.
			col.gameObject.GetComponent<EnemyChildFSM>().KillChildCell();
			// Kill Self.
			KillPlayerChildCell();
		}
		// Hit Enemy Main.
		else if (col.gameObject.tag == Constants.s_strEnemyTag)
		{
			Debug.Log("Hit Enemy Main");
		}
	}
}
