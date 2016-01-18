using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EnemyMainFSM))]
public class EMLeraningAgent : MonoBehaviour 
{
	// Instance of the class
	private static EMLeraningAgent instance;
	// Singleton
	public static EMLeraningAgent Instance()
	{
		return instance;
	}

	EnemyMainFSM m_EMFSM;

	private bool bCanStartCheck;
	private float fCheckFreq;

	private bool bCanRegain;
	private float fRegainFreq;

	void Awake ()
	{
		if (instance == null)
			instance = this;
	}

	void Start ()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();

		bCanStartCheck = true;
		fCheckFreq = 1f;

		bCanRegain = true;
		fRegainFreq = 1f;
	}

	void Update ()
	{
		if (bCanStartCheck)
			Critic ();

		if (bCanRegain)
			RegainScoreCall ();
		// // Update the score on the inspector
		EnemyMainFSM.Instance ().ScoreUpdate ();
		// Clamp the hided score value between -100f and 100f
		EnemyMainFSM.Instance ().ScoreLimit ();
	}

	// Learning Element: Correspond with the FSM to make improvements
	private void LearningElement (EMState state, int pastEnemyChild, int currentEnemyChild, int pastPlayerChild, int currentPlayerChild, 
	                              int pastSquadChild, int currentSquadChild, bool squadCaptainWasAlive, bool squadCaptainIsAlive, 
	                              int pastEnemyHealth, int pastPlayerHealth)
	{
		#region General transition probability change
		// Initialize the overall score
		float fOverallScore = 0f;
		// Reward for increment of the number of enemy child cells, vice versa
		// The less aggressive the enemy is, the more it wants to increase the number of its child cells
		fOverallScore += Random.Range (((float)(currentEnemyChild - pastEnemyChild) * Mathf.Pow (3f, 2f) / m_EMFSM.CurrentAggressiveness) * 1.25f,
		                               ((float)(currentEnemyChild - pastEnemyChild) * Mathf.Pow (3f, 2f) / m_EMFSM.CurrentAggressiveness) * .75f);
		// Reward for player child cells killed, vice versa
		if (((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness * .5f) < 
		    ((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness * 1f))
			fOverallScore += Random.Range (((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness) * .5f, 
			                               ((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness) * 1f);
		else 
			fOverallScore += Random.Range (((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness) * 1f, 
			                               ((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness) * .5f);
		// Penalty for increment of the number of player squad child cells, vice versa
		// (float)(currentSquadChild - pastSquadChild) * m_EMFSM.InitialAggressiveness
		fOverallScore -= Random.Range (((float)(currentSquadChild - pastSquadChild) * m_EMFSM.CurrentAggressiveness) * .5f,
		                               ((float)(currentSquadChild - pastSquadChild) * m_EMFSM.CurrentAggressiveness) * 1f);
		// Reward for killing the player squad captain based on enemy initial aggressiveness
		if (squadCaptainWasAlive && !squadCaptainIsAlive) {
			fOverallScore += Random.Range (m_EMFSM.CurrentAggressiveness, m_EMFSM.CurrentAggressiveness * 2f);
		}
		// Penalty for loss of health of the enemy main cell
		if (pastEnemyHealth > m_EMFSM.Health) {
			fOverallScore -= Random.Range ((float)(pastEnemyHealth - m_EMFSM.Health) * Mathf.Sqrt(m_EMFSM.CurrentAggressiveness), 
			                               (float)(pastEnemyHealth - m_EMFSM.Health) * m_EMFSM.CurrentAggressiveness);
		}

		#endregion
		#region Peculiar transition probability changes
		// Production state
		if (state == EMState.Production)
		{
			fOverallScore += Random.Range (((float)(currentEnemyChild - pastEnemyChild) / m_EMFSM.CurrentAggressiveness) * 15f,
			                               ((float)(currentEnemyChild - pastEnemyChild) / m_EMFSM.CurrentAggressiveness) * 30f);
		}
		// Defned state
		if (state == EMState.Defend && Mathf.Abs(pastEnemyChild - currentEnemyChild) != 0)
		{
			fOverallScore += (Mathf.Sqrt(10f) / m_EMFSM.CurrentAggressiveness) / (Mathf.Abs(pastEnemyChild - currentEnemyChild));
		}
		// AggressiveAttack, CautiousAttack and Landmine state
		if (state == EMState.AggressiveAttack || state == EMState.CautiousAttack || state == EMState.Landmine)
		{
			// Reward for player child cells killed, vice versa
			if (((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness * .25f) < 
			    ((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness * .5f))
				fOverallScore += Random.Range (((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness) * .25f, 
				                               ((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness) * .5f);
			else 
				fOverallScore += Random.Range (((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness) * .5f, 
				                               ((float)(pastPlayerChild - currentPlayerChild) * m_EMFSM.CurrentAggressiveness) * .25f);
			// Reward for squad child cells killed, vice versa
			if (((float)(pastSquadChild - currentSquadChild) * m_EMFSM.CurrentAggressiveness * 1.5f) < 
			    ((float)(pastSquadChild - currentSquadChild) * m_EMFSM.CurrentAggressiveness * 3f))
				fOverallScore += Random.Range (((float)(pastSquadChild - currentSquadChild) * m_EMFSM.CurrentAggressiveness) * 1.5f,
				                               ((float)(pastSquadChild - currentSquadChild) * m_EMFSM.CurrentAggressiveness) * 3f);
			else 
				fOverallScore += Random.Range (((float)(pastSquadChild - currentSquadChild) * m_EMFSM.CurrentAggressiveness) * 3f,
				                               ((float)(pastSquadChild - currentSquadChild) * m_EMFSM.CurrentAggressiveness) * 1.5f);

			// Penalty for loss of health of the enemy main cell
			if (pastEnemyHealth > m_EMFSM.Health) {
				if (((pastEnemyHealth - m_EMFSM.Health) * 3f / Mathf.Sqrt(m_EMFSM.CurrentAggressiveness)) < 
				    (pastEnemyHealth - m_EMFSM.Health))
					fOverallScore -= Random.Range ((pastEnemyHealth - m_EMFSM.Health) * 3f / Mathf.Sqrt(m_EMFSM.CurrentAggressiveness), 
					                               (pastEnemyHealth - m_EMFSM.Health));
				else 
					fOverallScore -= Random.Range ((pastEnemyHealth - m_EMFSM.Health), 
					                               (pastEnemyHealth - m_EMFSM.Health) * 3f / Mathf.Sqrt(m_EMFSM.CurrentAggressiveness));
			}

			// Reward for loss of health of the player main cell
			if (((pastPlayerHealth - PlayerMain.s_Instance.Health) * Mathf.Sqrt(m_EMFSM.CurrentAggressiveness)) * 4f < 
			    (pastPlayerHealth - PlayerMain.s_Instance.Health) * Mathf.Sqrt(m_EMFSM.CurrentAggressiveness) * 8f)
			{
				fOverallScore += Random.Range ((pastPlayerHealth - PlayerMain.s_Instance.Health) * Mathf.Sqrt(m_EMFSM.CurrentAggressiveness) * 4f, 
				                               (pastPlayerHealth - PlayerMain.s_Instance.Health) * Mathf.Sqrt(m_EMFSM.CurrentAggressiveness) * 8f);
			}
			else 
				fOverallScore += Random.Range ((pastPlayerHealth - PlayerMain.s_Instance.Health) * Mathf.Sqrt(m_EMFSM.CurrentAggressiveness) * 8f, 
				                               (pastPlayerHealth - PlayerMain.s_Instance.Health) * Mathf.Sqrt(m_EMFSM.CurrentAggressiveness) * 4f);
		}
		// Landmine state
		if (state == EMState.Landmine)
		{
			fOverallScore += Mathf.Sqrt ((float)currentEnemyChild);
			fOverallScore += Random.Range (Mathf.Sqrt ((float)currentPlayerChild) * m_EMFSM.InitialAggressiveness / 5f, 
			                               Mathf.Sqrt ((float)currentPlayerChild) * m_EMFSM.InitialAggressiveness / 2.5f);
		}
		// Main state
		if (fOverallScore < 0f && state == EMState.Maintain)
		{
			EnemyMainFSM.Instance().LearningDictionary[EMState.Maintain] += Mathf.Sqrt(Mathf.Abs (fOverallScore) * 2f);
		}
		#endregion

		fOverallScore = ScoreCompressor (state, fOverallScore);
		EnemyMainFSM.Instance ().LearningDictionary [state] += fOverallScore;
	}
	// Critic Tells the learning element how well the agent is doing with respect to a fixed performance measure
	private void Critic ()
	{
		EMState currentStateEnum = m_EMFSM.CurrentStateIndex;
		int nCurrentEnemyChild = m_EMFSM.AvailableChildNum;
		int nCurrentPlayerChild = PlayerChildFSM.GetActiveChildCount ();
		int nCurrentSquadChild = PlayerSquadFSM.Instance.AliveChildCount ();
		bool bSquadCaptainIsAlive = PlayerSquadFSM.Instance.IsAlive;
		int nCurrentEnemyHealth = m_EMFSM.Health;
		int nCurrentPlayerHealth = PlayerMain.s_Instance.Health;
		// Pause calling the function for checking and wait for the result
		fCheckFreq = Random.Range (0.25f, .5f);
		StartCoroutine (PauseCheck (fCheckFreq, currentStateEnum, nCurrentEnemyChild, nCurrentPlayerChild, nCurrentSquadChild, bSquadCaptainIsAlive, 
		                            nCurrentEnemyHealth, nCurrentPlayerHealth));
	}
	// Check only once within a given perio of time
	IEnumerator PauseCheck (float checkFreq, EMState pastState,int pastEnemyChild, int pastPlayerChild, int pastSquadChild, bool squadCaptainWasAlive, 
	                        int pastEnemyHealth, int pastPlayerHealth)
	{
		bCanStartCheck = false;		// One check at a time
		yield return new WaitForSeconds (checkFreq);

		// If we are still in the same state then proceed
		if (pastState == m_EMFSM.CurrentStateIndex)
			LearningElement (pastState, pastEnemyChild, m_EMFSM.AvailableChildNum, pastPlayerChild, PlayerChildFSM.GetActiveChildCount(),
			                 pastSquadChild, PlayerSquadFSM.Instance.AliveChildCount (), squadCaptainWasAlive, PlayerSquadFSM.Instance.IsAlive, 
			                 pastEnemyHealth, pastPlayerHealth);
	
		bCanStartCheck = true;		//Reset the checking availability
	}

	float ScoreCompressor (EMState state, float score)
	{
		if (EnemyMainFSM.Instance ().LearningDictionary [state] > 0f) {
			return score / Mathf.Sqrt (Mathf.Sqrt (Mathf.Abs(EnemyMainFSM.Instance ().LearningDictionary [state])));
		} 
		else 
			return score;
	}

	public float OriginalScore (EMState state)
	{
		return EnemyMainFSM.Instance().LearningDictionary[state];
	}

	public float RealScore (EMState state)
	{
		return 2f * Mathf.Sqrt (EnemyMainFSM.Instance().LearningDictionary[state]);
	}

	void RegainScoreCall ()
	{
		fRegainFreq = Random.Range (2f, 4f);
		StartCoroutine (RegainPause (fRegainFreq));
	}

	IEnumerator RegainPause (float regainFreq)
	{
		bCanRegain = false;
		yield return new WaitForSeconds (regainFreq);
		RegainFunction ();
		bCanRegain = true;
	}

	void RegainFunction ()
	{
		// Production state
		if (EnemyMainFSM.Instance ().LearningDictionary [EMState.Production] > 1f || EnemyMainFSM.Instance ().LearningDictionary [EMState.Production] < -1f)
			EnemyMainFSM.Instance ().LearningDictionary [EMState.Production] *= 0.95f;
		else 
			EnemyMainFSM.Instance ().LearningDictionary [EMState.Production] = 0f;
		// Maintain state
		if (EnemyMainFSM.Instance ().LearningDictionary [EMState.Maintain] > 1f || EnemyMainFSM.Instance ().LearningDictionary [EMState.Maintain] < -1f)
			EnemyMainFSM.Instance ().LearningDictionary [EMState.Maintain] *= 0.95f;
		else 
			EnemyMainFSM.Instance ().LearningDictionary [EMState.Maintain] = 0f;
		// Defend state
		if (EnemyMainFSM.Instance ().LearningDictionary [EMState.Defend] > 1f || EnemyMainFSM.Instance ().LearningDictionary [EMState.Defend] < -1f)
			EnemyMainFSM.Instance ().LearningDictionary [EMState.Defend] *= 0.95f;
		else 
			EnemyMainFSM.Instance ().LearningDictionary [EMState.Defend] = 0f;
		// AggressiveAttack state
		if (EnemyMainFSM.Instance ().LearningDictionary [EMState.AggressiveAttack] > 1f || EnemyMainFSM.Instance ().LearningDictionary [EMState.AggressiveAttack] < -1f)
			EnemyMainFSM.Instance ().LearningDictionary [EMState.AggressiveAttack] *= 0.95f;
		else 
			EnemyMainFSM.Instance ().LearningDictionary [EMState.AggressiveAttack] = 0f;
		// CautiousAttack state
		if (EnemyMainFSM.Instance ().LearningDictionary [EMState.CautiousAttack] > 1f || EnemyMainFSM.Instance ().LearningDictionary [EMState.CautiousAttack] < -1f)
			EnemyMainFSM.Instance ().LearningDictionary [EMState.CautiousAttack] *= 0.95f;
		else 
			EnemyMainFSM.Instance ().LearningDictionary [EMState.CautiousAttack] = 0f;
		// Landmine state
		if (EnemyMainFSM.Instance ().LearningDictionary [EMState.Landmine] > 1f || EnemyMainFSM.Instance ().LearningDictionary [EMState.Landmine] < -1f)
			EnemyMainFSM.Instance ().LearningDictionary [EMState.Landmine] *= 0.95f;
		else 
			EnemyMainFSM.Instance ().LearningDictionary [EMState.Landmine] = 0f;
	}
}