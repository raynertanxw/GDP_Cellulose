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

	// Learning Element: Correspond with the FSM to make improvements
	private void LearningElement (EMState state, int pastEnemyChild, int currentEnemyChild, int pastPlayerChild, int currentPlayerChild, 
	                              int pastSquadChild, int currentSquadChild, bool squadCaptainWasAlive, bool squadCaptainIsAlive)
	{
		#region General transition probability change
		// Initialize the overall score
		float fOverallScore = 0f;
		// Reward for increment of the number of enemy child cells, vice versa
		// (float)(currentEnemyChild - pastEnemyChild) * Mathf.Sqrt(15f) / m_EMFSM.InitialAggressiveness
		fOverallScore += Random.Range (((float)(currentEnemyChild - pastEnemyChild) * Mathf.Sqrt(15f) / m_EMFSM.InitialAggressiveness) / 1.5f,
		                               ((float)(currentEnemyChild - pastEnemyChild) * Mathf.Sqrt(15f) / m_EMFSM.InitialAggressiveness) * 1.5f);
		// Penalty for increment of the number of player child cells, vice versa
		// (float)(currentPlayerChild - pastPlayerChild) * m_EMFSM.InitialAggressiveness
		fOverallScore -= Random.Range (((float)(currentPlayerChild - pastPlayerChild) * m_EMFSM.InitialAggressiveness) / 3f,
		                               ((float)(currentPlayerChild - pastPlayerChild) * m_EMFSM.InitialAggressiveness) * 1f);
		// Penalty for increment of the number of player squad child cells, vice versa
		// (float)(currentSquadChild - pastSquadChild) * m_EMFSM.InitialAggressiveness
		fOverallScore -= Random.Range (((float)(currentSquadChild - pastSquadChild) * m_EMFSM.InitialAggressiveness) / 3f,
		                               ((float)(currentSquadChild - pastSquadChild) * m_EMFSM.InitialAggressiveness) * 1f);
		// Reward for killing the player squad captain based on enemy initial aggressiveness
		if (squadCaptainWasAlive && !squadCaptainIsAlive) {
			fOverallScore += Random.Range (m_EMFSM.InitialAggressiveness, m_EMFSM.InitialAggressiveness * 2f);
		}
		#endregion
		#region Peculiar transition probability changes
		// Production state
		if (state== EMState.Production)
		{
			fOverallScore += Random.Range (((float)(currentEnemyChild - pastEnemyChild) * Mathf.Sqrt(10f) / m_EMFSM.InitialAggressiveness) / 1.5f,
			                               ((float)(currentEnemyChild - pastEnemyChild) * Mathf.Sqrt(10f) / m_EMFSM.InitialAggressiveness) * 1.5f);
		}
		// Defned state
		if (state == EMState.Defend && Mathf.Abs(pastEnemyChild - currentEnemyChild) != 0)
		{
			fOverallScore += (Mathf.Sqrt(10f) / m_EMFSM.InitialAggressiveness) / (Mathf.Abs(pastEnemyChild - currentEnemyChild));
		}
		// AggressiveAttack, CautiousAttack and Landmine state
		if (state == EMState.AggressiveAttack || state == EMState.CautiousAttack || state == EMState.Landmine)
		{
			if (fOverallScore > 0f)
				EnemyMainFSM.Instance().CurrentAggressiveness += 1f;
			else if (fOverallScore < 0f)
				EnemyMainFSM.Instance().CurrentAggressiveness -= 1f;
		}
		// Main state
		if (fOverallScore < 0f && state != EMState.Maintain)
		{
			EnemyMainFSM.Instance().LearningDictionary[EMState.Maintain] += Mathf.Sqrt(Mathf.Abs (fOverallScore) * 2f);
		}
		#endregion
		fOverallScore = ScoreCompressor (state, fOverallScore);
	}
	// Critic Tells the learning element how well the agent is doing with respect to a fixed performance measure
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
	// Check only once within a given perio of time
	IEnumerator ConstantCheck (float checkFreq)
	{
		bChecking = true;
		yield return new WaitForSeconds (checkFreq);
		bChecking = false;
	}

	float ScoreCompressor (EMState state, float score)
	{
		if (EnemyMainFSM.Instance ().LearningDictionary [state] != 0f) {
			return score / Mathf.Sqrt (EnemyMainFSM.Instance ().LearningDictionary [state]);
		}
		else 
			return score;
	}
}