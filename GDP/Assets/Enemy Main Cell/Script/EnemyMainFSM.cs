using UnityEngine;
using System.Collections;

public class EnemyMainFSM : MonoBehaviour 
{
	IEMState m_CurrentState = null;

	int nDamageNum;

	void Start()
	{
		m_CurrentState = EMProductionState.Instance ();

		nDamageNum = 0;
	}

	void Update()
	{

	}

	public void ChangeState (IEMState newState)
	{
		if (m_CurrentState != null)
			m_CurrentState.Exit ();
		m_CurrentState = newState;
		m_CurrentState.Enter ();
	}
}