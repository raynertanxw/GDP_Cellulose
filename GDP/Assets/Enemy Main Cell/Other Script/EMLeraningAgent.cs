using UnityEngine;
using System.Collections;

public class EMLeraningAgent : MonoBehaviour 
{
	EnemyMainFSM m_EMFSM;

	private bool bChecking;
	private float fCheckFreq;

	void Start ()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();

		bChecking = false;
		fCheckFreq = 1f;
	}

	void Update ()
	{
		if (!bChecking)
			Critic ();
	}

	private void LearningElement (EMState state, int pastEnemyChild, int currentEnemyChild, int pastPlayerChild, int currentPlayerChild, 
	                              int pastSquadChild, int currentSquadChild, bool squadCaptainWasAlive, bool squadCaptainIsAlive)
	{
		#region General transition probability change
		// Initialize the overall score
		float fOverallScore = 0f;
		// Award for increment of the number of enemy child cells, vice versa
		// (float)(currentEnemyChild - pastEnemyChild) * Mathf.Sqrt(5f) / m_EMFSM.InitialAggressiveness
		fOverallScore += Random.Range (((float)(currentEnemyChild - pastEnemyChild) * Mathf.Sqrt(10f) / m_EMFSM.InitialAggressiveness) / 1.5f,
		                               ((float)(currentEnemyChild - pastEnemyChild) * Mathf.Sqrt(10f) / m_EMFSM.InitialAggressiveness) * 1.5f);
		// Penalty for increment of the number of player child cells, vice versa
		// (float)(currentPlayerChild - pastPlayerChild) * m_EMFSM.InitialAggressiveness
		fOverallScore -= Random.Range (((float)(currentPlayerChild - pastPlayerChild) * m_EMFSM.InitialAggressiveness) / 1.5f,
		                               ((float)(currentPlayerChild - pastPlayerChild) * m_EMFSM.InitialAggressiveness) * 1.5f);
		// Penalty for increment of the number of player squad child cells, vice versa
		// (float)(currentSquadChild - pastSquadChild) * m_EMFSM.InitialAggressiveness
		fOverallScore -= Random.Range (((float)(currentSquadChild - pastSquadChild) * m_EMFSM.InitialAggressiveness) / 1.5f,
		                               ((float)(currentSquadChild - pastSquadChild) * m_EMFSM.InitialAggressiveness) * 1.5f);
		// Award for killing the player squad captain based on enemy initial aggressiveness
		if (squadCaptainWasAlive && !squadCaptainIsAlive) {
			fOverallScore += Random.Range (m_EMFSM.InitialAggressiveness, m_EMFSM.InitialAggressiveness * 2f);
		}
		#endregion
		#region Peculiar transition probability change for some of the states
		// Defned state
		if (state == EMState.Defend)
		{
			fOverallScore += (Mathf.Sqrt(10f) / m_EMFSM.InitialAggressiveness) / (Mathf.Abs(pastEnemyChild - currentEnemyChild) + 1f);
		}
		#endregion
	}

	private void Critic ()
	{
		EMState currentStateEnum = m_EMFSM.CurrentStateIndex;
		int nCurrentEnemyChild = m_EMFSM.AvailableChildNum;
		int nCurrentPlayerChild = PlayerChildFSM.GetActiveChildCount ();
		int nCurrentSquadChild = SquadCaptain.Instance.AliveChildCount ();
		bool bSquadCaptainIsAlive = SquadCaptain.Instance.IsAlive;
		// Pause calling the function for checking and wait for the result
		StartCoroutine (ConstantCheck (fCheckFreq));
		// If we are still in the same state then proceed
		if (currentStateEnum == m_EMFSM.CurrentStateIndex && !bChecking) 
		{
			LearningElement (currentStateEnum, nCurrentEnemyChild, m_EMFSM.AvailableChildNum, nCurrentPlayerChild, PlayerChildFSM.GetActiveChildCount(),
			                 nCurrentSquadChild, SquadCaptain.Instance.AliveChildCount (), bSquadCaptainIsAlive, SquadCaptain.Instance.IsAlive);
		}
	}

	IEnumerator ConstantCheck (float checkFreq)
	{
		bChecking = true;
		yield return new WaitForSeconds (checkFreq);
		bChecking = false;
	}
}