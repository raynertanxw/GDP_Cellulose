using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EnemyMainFSM))]
[RequireComponent (typeof (EMController))]
public class EMTransition : MonoBehaviour 
{
	// Instance of the class
	private static EMTransition instance;
	// Singleton
	public static EMTransition Instance()
	{
		return instance;
	}

	EnemyMainFSM m_EMFSM;
	private EMController controller;

	// Transition availability
	[Header("Transition Availability")]
	[Tooltip("Transition avvailability of the Enemy Main Cell")]
	[SerializeField] private bool bCanTransit;
	public bool CanTransit { get { return bCanTransit; } set { bCanTransit = value; } }

	void Start ()
	{
		if (instance == null)
			instance = this;

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

	/// <summary>
	/// Constructor of Transition function
	/// </summary>
	public bool Transition (float nChanceFactor, EMState state)
	{
		return Transition (nChanceFactor, state, 1.0f);
	}

	/// <summary>
	/// Universal transition probability checking function
	/// <param name="nChanceFactor">Equal to one tenth of the persentage.</param>
	/// <param name="state">State trying to transition to.</param>
	/// <param name="fExtraChanceFactor">Extra chance if any (1.0f by default).</param>
	/// </summary>
	public bool Transition (float nChanceFactor, EMState state, float fExtraChanceFactor)
	{
		float nChance = 0f;

		if (nChanceFactor * fExtraChanceFactor <= 10f)
		{
			m_EMFSM.ChangeState (state);
			return true;
		}
		else
		{
			nChance = Random.Range (0f, nChanceFactor);
			if (nChance <= 1f)
				m_EMFSM.ChangeState (state);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Universal function for pausing transition availability
	/// <param name="fTIme">Used to indicate pause time.</param>
	/// </summary>
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
			m_EMFSM.ChangeState (EMState.Stunned);
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

	public static void ResetStatics()
	{
		instance = null;
	}
}