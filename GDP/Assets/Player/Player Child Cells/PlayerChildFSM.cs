using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerChildFSM : MonoBehaviour
{
	public static EnemyMainFSM s_eMain;
	public static PlayerMain s_pMain;
	
	private PCState m_currentEnumState;
	private IPCState m_currentState;
	private Dictionary<PCState, IPCState> m_statesDictionary;
	private int m_nIndex;
	private float m_fSpeed;
	public bool m_bIsDefending = true;
	public Transform m_assignedNode;
	public Transform m_currentTarget;

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
