using UnityEngine;
using System.Collections;

public class EMTransition : MonoBehaviour 
{
	EnemyMainFSM m_EMFSM;
	private EMController controller;

	// Transition availability
	[Header("Transition Availability")]
	[Tooltip("Transition avvailability of the Enemy Main Cell")]
	[SerializeField] private bool bCanTransit;
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
	public void Transition (float nChanceFactor, EMState state)
	{
		float nChance = 0f;

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
	public IEnumerator TransitionAvailability (float fTime)
	{
		EMState currentStateIndex = m_EMFSM.CurrentStateIndex;
		bCanTransit = false;
		yield return new WaitForSeconds (fTime);
		// Reset transition availability only when it is still in the state which the call is from
		if (currentStateIndex == m_EMFSM.CurrentStateIndex)
			bCanTransit = true;
	}

	#region Exteral Transitions
	// Immediate transition to ProductionState
	void ProductionTransition ()
	{
		if (m_EMFSM.AvailableChildNum <= 10 && m_EMFSM.CurrentStateIndex != EMState.Production && bCanTransit && !controller.Pushed && !controller.Stunned) 
		{
			m_EMFSM.ChangeState (EMState.Production);
		}
	}
	// Immediate transition to StunnedState
	void StunnedTransition ()
	{
		if (controller.Stunned && m_EMFSM.CurrentStateIndex != EMState.Stunned) 
		{
			m_EMFSM.ChangeState (EMState.Stunnedtion);
		}
	}
	// Transition to DieState
	void DieTransition ()
	{
		if (m_EMFSM.Health <= 0 && m_EMFSM.CurrentStateIndex != EMState.Die) 
		{
			m_EMFSM.ChangeState (EMState.Die);
		}
	}
	#endregion
}