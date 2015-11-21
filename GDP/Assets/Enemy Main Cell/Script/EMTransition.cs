using UnityEngine;
using System.Collections;

public class EMTransition : MonoBehaviour 
{
	public GameObject FSM;
	EnemyMainFSM m_EMFSM;

	public bool bCanTransit;

	void Start ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();

		bCanTransit = true;
	}

	void Update ()
	{
		ProductionTransition ();
		StunnedTransition ();
		DieTransition ();
	}

	// Universal transition checking function
	public void Transition (float nChanceFactor, IEMState state)
	{
		float nChance = 0f;
		float nEnemyChildFactor = m_EMFSM.nSize / 10;
		float nPlayerChildFactor;

		if (nChanceFactor <= 0f)
			m_EMFSM.ChangeState (state);
		else
		{
			nChance = Random.Range (0f, nChanceFactor);
			if (nChance == 0f)
				m_EMFSM.ChangeState (state);
		}
	}

	// Universal function for pausing transition availability
	public IEnumerator TransitionAvailability ()
	{
		bCanTransit = false;
		yield return new WaitForSeconds (0.25f);
		bCanTransit = true;
	}

	// Immediate Transitions checking
	// Immediate Transition for ProductionState
	void ProductionTransition ()
	{
		if (m_EMFSM.nAvailableChildNum <= 10 && m_EMFSM.m_CurrentState != EMProductionState.Instance()) 
		{
			m_EMFSM.ChangeState (EMProductionState.Instance());
		}
	}
	// Immediate Transition for StunnedState
	void StunnedTransition ()
	{
		if (m_EMFSM.emController.bStunned && m_EMFSM.m_CurrentState != EMStunnedState.Instance()) 
		{
			m_EMFSM.ChangeState (EMStunnedState.Instance());
		}
	}
	// Immediate Transition for DieState
	void DieTransition ()
	{
		if (m_EMFSM.nSize <= 0 && m_EMFSM.m_CurrentState != EMDieState.Instance()) 
		{
			m_EMFSM.ChangeState (EMDieState.Instance());
		}
	}
}