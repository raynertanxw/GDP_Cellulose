using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (CircleCollider2D))]
[RequireComponent (typeof (SpriteRenderer))]
public class PlayerChildFSM : MonoBehaviour
{
	public static EnemyMainFSM s_eMain;
	public static PlayerMain s_pMain;
	private static int s_nIndex = 0;
	
	private PCState m_currentEnumState;
	private IPCState m_currentState;
	private Dictionary<PCState, IPCState> m_statesDictionary;
	[SerializeField]
	private int m_nIndex;
	private float m_fSpeed;
	public bool m_bIsDefending = true;
	public Transform m_assignedNode;
	public Transform m_currentTarget;


	// Component Cache.
	public Rigidbody2D rigidbody2D;
	public CircleCollider2D collider2D;
	public SpriteRenderer spriteRen;

	#region Getter functions
	public int GetIndex() { return m_nIndex; }
	public float GetSpeed() { return m_fSpeed; }
	public PCState GetCurrentState() { return m_currentEnumState; }
	#endregion

	public void ChangeState(PCState newState)
	{
		m_currentState.Exit();
		m_currentState = m_statesDictionary[newState];
		m_currentState.Enter();
		m_currentEnumState = newState;
	}

	void Awake()
	{
		// Initialise the index.
		m_nIndex = s_nIndex++; // Assigns the variable first before incrementing it.

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
}
