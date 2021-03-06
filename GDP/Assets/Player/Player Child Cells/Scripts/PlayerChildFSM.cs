﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (CircleCollider2D))]
[RequireComponent (typeof (SpriteRenderer))]
public class PlayerChildFSM : MonoBehaviour
{
	#region Pool Control
	// singleton list to hold all out playerChildPoolControllers.
	private static int s_nPoolPointerIndex = 0;
	private static PlayerChildFSM[] s_playerChildFSMPool;
	public static PlayerChildFSM[] playerChildPool { get { return s_playerChildFSMPool; } }
	public static pcStatus[] s_playerChildStatus;
	private static int[] s_childrenInLeftNode;
	private static int[] s_childrenInRightNode;
	private static int[] s_childrenInAttack;

	public static int GetActiveChildCount() { return s_nActiveChildCount; }
	public static int[] childrenInLeftNode { get { return s_childrenInLeftNode; } }
	public static int[] childrenInRightNode { get { return s_childrenInRightNode; } }
	public static int[] childrenInAttack { get { return  s_childrenInAttack; } }

	public static PlayerChildFSM Spawn(Vector3 spawnPoint)
	{
		for (int i = 0; i < Constants.s_nPlayerMaxChildCount; i++)
		{
			// If disabled, thn it's available.
			if (s_playerChildFSMPool[i].m_currentEnumState == PCState.Dead)
			{
                // Set it up
                spawnPoint.z = 0; // Enforce all the spawnpoints to spawn at z = 0.
				(s_playerChildFSMPool[i].m_statesDictionary[PCState.Dead] as PC_DeadState).CallFromPool(spawnPoint);
				
				// return a reference to the caller.
				return s_playerChildFSMPool[i];
			}
		}
		
		// If we get here we haven't pooled enough.
		Debug.Log("Exhausted Player Child pool --> Increase pool size");
		
		return null;
	}

	private static int[] ChildrenPoolIdInStatus(pcStatus _selectedStatus)
	{
		int[] children = new int[Constants.s_nPlayerMaxChildCount + 1];
		int count = 0;
		for (int i = 0; i < Constants.s_nPlayerMaxChildCount; i++)
		{
			if (s_playerChildStatus[i] == _selectedStatus)
			{
				children[count] = i;
				count++;
			}
		}

		children[count] = -1;
		return children;
	}

	// Handled by singleton PlayerMain.
	public static void UpdateChildrenStatusArrays()
	{
		s_childrenInLeftNode = ChildrenPoolIdInStatus(pcStatus.InLeftNode);
		s_childrenInRightNode = ChildrenPoolIdInStatus(pcStatus.InRightNode);
		s_childrenInAttack = ChildrenPoolIdInStatus(pcStatus.Attacking);
	}
	#endregion
	
	private static int s_nActiveChildCount = 0;

	
	private PCState m_currentEnumState;
	private IPCState m_currentState;
	private Dictionary<PCState, IPCState> m_statesDictionary;
	public bool m_bIsDefending = true;
	public bool m_bHasAwaitingDeferredStateChange = false;
	public bool m_bSacrificeToSquadCpt = false;
	public PlayerChildFSM[] m_formationCells;
	public bool m_bIsInFormation;
    public PlayerAttackMode attackMode;
	private PCState m_deferredState;
	public EnemyChildFSM m_currentEnemyCellTarget;
	public Node_Manager m_assignedNode;
	private int m_nPoolIndex;
	private AudioSource m_AudioSource;


	// Component Cache.
	public new Rigidbody2D rigidbody2D;
	public new CircleCollider2D collider2D;
	public SpriteRenderer spriteRen;

	#region Getter functions
	public PCState GetCurrentState() { return m_currentEnumState; }
	public int poolIndex { get { return m_nPoolIndex; } }
	public AudioSource Audio{ get { return m_AudioSource; } }
	#endregion

	#region Setter functions
	public static void IncrementActiveChildCount()
	{
		++s_nActiveChildCount;
	}

	public static void DecrementActiveChildCount()
	{
		--s_nActiveChildCount;
	}
	#endregion

	public void ChangeState(PCState newState)
	{
		m_currentState.Exit();
		m_currentState = m_statesDictionary[newState];
		m_currentState.Enter();
		m_currentEnumState = newState;
	}

	public void RefreshState()
	{
		m_currentState.Exit();
		m_currentState.Enter();
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

	public void SacrificeToSquadCpt()
	{
		m_bSacrificeToSquadCpt = true;
		ChangeState(PCState.Idle);
	}

	// For Enemy to call
	public void KillPlayerChildCell()
	{
		DeferredChangeState(PCState.Dead);
		m_bSacrificeToSquadCpt = false;
	}

	void Awake()
	{
		// does the pool exist yet
		if (s_playerChildFSMPool == null)
		{
			// lazy initialize it
			s_playerChildFSMPool = new PlayerChildFSM[Constants.s_nPlayerMaxChildCount];
			s_playerChildStatus = new pcStatus[Constants.s_nPlayerMaxChildCount];
			s_nPoolPointerIndex = 0;
			s_nActiveChildCount = 0;

			s_childrenInRightNode = new int[] {-1};
			s_childrenInLeftNode = new int[] {-1};
			s_childrenInAttack = new int[] {-1};
		}
		// add myself
		s_playerChildFSMPool[s_nPoolPointerIndex] = this;
		s_playerChildStatus[s_nPoolPointerIndex] = pcStatus.DeadState;
		m_nPoolIndex = s_nPoolPointerIndex;
		s_nPoolPointerIndex++;


		// Cache components
		rigidbody2D = GetComponent<Rigidbody2D>();
		collider2D = GetComponent<CircleCollider2D>();
		spriteRen = GetComponent<SpriteRenderer>();
		m_AudioSource = GetComponent<AudioSource>();

		// Set up the m_statesDictionary.
		m_statesDictionary = new Dictionary<PCState, IPCState>();
		m_statesDictionary.Add(PCState.Avoid, new PC_AvoidState(this));
		m_statesDictionary.Add(PCState.ChargeChild, new PC_ChargeChildState(this));
		m_statesDictionary.Add(PCState.ChargeMain, new PC_ChargeMainState(this));
		m_statesDictionary.Add(PCState.Dead, new PC_DeadState(this));
		m_statesDictionary.Add(PCState.Defend, new PC_DefendState(this));
		m_statesDictionary.Add(PCState.Idle, new PC_IdleState(this));


		PlayerChildFSM.IncrementActiveChildCount();
		// Change to dead state.
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

    void FixedUpdate()
    {
        m_currentState.FixedExecute();
    }

	void OnCollisionEnter2D(Collision2D col)
	{
		// Ignore collisions if is being scrificed for squad captain.
		if (m_bSacrificeToSquadCpt == true)
			return;

		// Hit Enemy Child
		if (col.gameObject.tag == Constants.s_strEnemyChildTag)
		{
			// Kill Enemy.
			col.gameObject.GetComponent<EnemyChildFSM>().KillChildCell();
			// Kill Self.
			KillPlayerChildCell();
			
			AudioManager.PlayPMSoundEffect(PlayerMainSFX.KillChild);
		}
		// Hit Enemy Main.
		else if (col.gameObject.tag == Constants.s_strEnemyTag)
		{
			// Damage Enemy.
			EMController.Instance().CauseDamageOne();
			MainCamera.CameraShake();

			KillPlayerChildCell();
		}
	}

    #if UNITY_EDITOR
    public float fGizmoCohesionRadius = 0f;
    public float fGizmoSeparationRadius = 0f;

    void OnDrawGizmos()
    {
//        m_currentState.ExecuteOnDrawGizmos();
//
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireSphere(transform.position, fGizmoCohesionRadius);
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, fGizmoSeparationRadius);
    }
    #endif









	public static void ResetStatics()
	{
		s_playerChildFSMPool = null;
		s_playerChildStatus = null;
	}
}

