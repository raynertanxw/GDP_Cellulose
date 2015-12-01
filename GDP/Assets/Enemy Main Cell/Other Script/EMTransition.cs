using UnityEngine;
using System.Collections;

public class EMTransition : MonoBehaviour 
{
	EnemyMainFSM m_EMFSM;
	private EMController controller;

	// Transition availability
	public bool bCanTransit;
	public bool CanTransit { get { return bCanTransit; } set { bCanTransit = value; } }

	void Start ()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
		controller = GetComponent<EMController> ();

		bCanTransit = true;
	}

	void Update ()
	{
        // External transition
		ProductionTransition ();
		StunnedTransition ();
		DieTransition ();
	}

	// Universal transition probability checking function
	public void Transition (float nChanceFactor, IEMState state)
	{
		float nChance = 0f;
		float nEnemyChildFactor = m_EMFSM.AvailableChildNum / 10;

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

	#region Exteral Transitions
	// Immediate transition to ProductionState
	void ProductionTransition ()
	{
		if (m_EMFSM.AvailableChildNum <= 10 && m_EMFSM.CurrentState != m_EMFSM.ProductionState) 
		{
			m_EMFSM.ChangeState (m_EMFSM.ProductionState);
		}

		// Reset transition availability
		bCanTransit = true;
	}
	// Immediate transition to StunnedState
	void StunnedTransition ()
	{
		if (controller.Stunned && m_EMFSM.CurrentState != m_EMFSM.StunnedState) 
		{
			m_EMFSM.ChangeState (m_EMFSM.ProductionState);
		}

		// Reset transition availability
		bCanTransit = true;
	}
	// Transition to DieState
	void DieTransition ()
	{
		if (m_EMFSM.Health <= 0 && m_EMFSM.CurrentState != m_EMFSM.DieState) 
		{
			m_EMFSM.ChangeState (m_EMFSM.DieState);
		}
	}
	#endregion
}