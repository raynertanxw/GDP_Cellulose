using UnityEngine;
using System.Collections;

public class EMTransition : MonoBehaviour 
{
	public GameObject FSM;
	EnemyMainFSM m_EMFSM;
	private EMController controller;

	private bool bCanTransit;
	public bool CanTransit { get { return bCanTransit; } }

	void Start ()
	{
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		controller = GetComponent<EMController> ();

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
		float nEnemyChildFactor = m_EMFSM.AvailableChildNum / 10;
		float nPlayerChildFactor;

		if (nChanceFactor <= 10f)
			m_EMFSM.ChangeState (state);
		else
		{
			nChance = Random.Range (0f, nChanceFactor);
			if (nChance <= 1f)
				m_EMFSM.ChangeState (state);
		}
	}

	// Universal function for pausing transition availability
	public IEnumerator TransitionAvailability (float fTIme)
	{
		bCanTransit = false;
		yield return new WaitForSeconds (fTIme);
		bCanTransit = true;
	}

	// Immediate Transitions checking
	// Immediate Transition for ProductionState
	void ProductionTransition ()
	{
		if (m_EMFSM.AvailableChildNum <= 10 && m_EMFSM.CurrentState != EMProductionState.Instance()) 
		{
			m_EMFSM.ChangeState (EMProductionState.Instance());
		}
	}
	// Immediate Transition for StunnedState
	void StunnedTransition ()
	{
		if (m_EMFSM.emController.Stunned && m_EMFSM.CurrentState != EMStunnedState.Instance()) 
		{
			m_EMFSM.ChangeState (EMStunnedState.Instance());
		}
	}
	// Immediate Transition for DieState
	void DieTransition ()
	{
		if (m_EMFSM.Health <= 0 && m_EMFSM.CurrentState != EMDieState.Instance()) 
		{
			m_EMFSM.ChangeState (EMDieState.Instance());
		}
	}
}