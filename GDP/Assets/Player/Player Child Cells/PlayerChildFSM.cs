using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerChildFSM : MonoBehaviour
{
	public static EnemyMainFSM s_eMain;
	public static PlayerMain s_pMain;

	private PCState m_currentState;
	private IPCState m_currentStateInstance;
	private Dictionary<string, IPCState> m_statesDictionary;
	private int m_nIndex;
	private float m_fSpeed;
	public bool m_bIsDefending = true;
	public Transform m_assignedNode;
	public Transform m_currentTarget;

	#region Getter functions
	public int GetIndex() { return m_nIndex; }
	public float GetSpeed() { return m_fSpeed; }
	public PCState GetCurrentState() { return m_currentState; }
	#endregion

	public void ChangeState(PCState newState)
	{

	}

	void Awake()
	{

	}
	
	void Start ()
	{
	
	}

	void Update ()
	{
	
	}
}
