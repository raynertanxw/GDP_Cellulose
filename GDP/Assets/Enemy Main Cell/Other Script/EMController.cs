using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EnemyMainFSM))]
[RequireComponent (typeof (EMDifficulty))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (CircleCollider2D))]
public class EMController : MonoBehaviour 
{	
	// Instance of the class
	private static EMController instance;
	// Singleton
	public static EMController Instance()
	{
		return instance;
	}

	EnemyMainFSM m_EMFSM;

	#region Speed & Velocity
	// Speed
	private float fSpeed;
	public float Speed{ get { return fSpeed; } }
	private float fHoriSpeed;
	public float HoriSpeed { get { return fHoriSpeed; } }
	// Speed factor that changes speed
	private float fSpeedFactor;
	public float SpeedFactor { get { return fSpeedFactor; } }
	// Temporary speed
	private float fSpeedTemp;
	// Slow down the enemy main cell if in Defend state
	public bool bIsDefend;
	private float fDefendFactor;
	// Velocity
	private Vector2 velocity;
	// Horizontal movement
	private bool bMovingLeft;
	public bool MovingLeft { get { return bMovingLeft; } }
	private bool bCanChangeHori;
	#endregion

	#region Status
	[Header("Number of damage")]
	[Tooltip("Number of enemy damage")]
	[SerializeField] 
	private int nDamageNum; // Amount of damages received within certain period of time, determines whether the enemy main cell will be stunned 
	public int CauseAnyDamage { get { return nDamageNum; } set { nDamageNum = value; } }
	public void CauseDamageOne () { nDamageNum++; m_EMFSM.Health--;}
	private bool bPushed; 
	public bool Pushed { get { return bPushed; } }
	private bool bStunned;
	public bool Stunned { get { return bStunned; } }
	private bool bCanStun;
	[SerializeField]
	private float fStunTime;
	[SerializeField]
	private float fNumOfDefaultCells;
	#endregion

	#region Size
	private int nInitialNutrientNum;
	[Header("Number of nutrient")]
	[Tooltip("Number of enemy nutrient")]
	[SerializeField] 
	private int nCurrentNutrientNum;
	public int NutrientNum { get { return nCurrentNutrientNum; } }
	public void ReduceNutrient () { nCurrentNutrientNum--; }
	public void AddNutrient () { nCurrentNutrientNum++; }
	#endregion

	private Rigidbody2D thisRB;

	void Awake ()
	{
		if (instance == null)
			instance = this;

		// GetComponent
		m_EMFSM = GetComponent<EnemyMainFSM> ();
		thisRB = GetComponent<Rigidbody2D> ();

		// Size
		nInitialNutrientNum = 100;
		nCurrentNutrientNum = nInitialNutrientNum;
		// Speed
		fSpeed = .05f;
		fSpeedFactor = 1f;
		fSpeedTemp = fSpeed;
		bIsDefend = false;
		fDefendFactor = 0.9f;
		// Initialization of horizontal movement
		// Randomize direction
		int nDirection = Random.Range (0, 2);
		if (nDirection == 0)
			bMovingLeft = true;
		else 
			bMovingLeft = false;
		// Randomize speed
		fHoriSpeed = Random.Range (0.06f, 0.12f);
		bCanChangeHori = true;
		// Velocity
		velocity = new Vector2 (fHoriSpeed, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
		// Damage
		nDamageNum = 0;
		// State
		bPushed = false;
		bStunned = false;
		bCanStun = true;
		fStunTime = 3f;
		fNumOfDefaultCells = 5f;
	}

	void Start ()
	{
		// Start with a few child cells
		StartWithChildCells ();
	}

	void Update()
	{
		// Force back the enemy main cell when received damage and not forced back
		if (nDamageNum > 0 && !bPushed) 
		{
			StartCoroutine(ForceBack());
		}
		// Stun the enemy main cell when received certain amount of hits, can be stunned but not stunned
		if (nDamageNum > 5 && !bStunned && bCanStun) 
		{
			StartCoroutine(Stun ());
		}
        // Reset velocity when current velocity is incorrect and the enemy main cell is free to move
        if (!bPushed && !bStunned)
        {
            if (bIsDefend)
            {
                if (thisRB.velocity.y != fSpeed * fSpeedFactor * fDefendFactor)
                    ResetVelocity();
            }
            else
            {
                if (thisRB.velocity.y != fSpeed * fSpeedFactor)
                    ResetVelocity();
            }
        }
		// Update Aggresiveness
		UpdateAggressiveness ();
		// Which whether the player wins
		LoseCheck ();
	}

	void FixedUpdate ()
	{
		// Change the horizontal direction if allowed
		if (bCanChangeHori && !bPushed && !bStunned) {
			StartCoroutine (MovingHorizontally ());
		}
		// Check the direction of horizontal movement is correct
		HorizontalCheck();
	}

	#region Damage behavior
	// Push back the enemy main cell when received attack
	IEnumerator ForceBack()
	{
        // Temporary velocity for enemy main cell when being pushed
		Vector2 velocityTemp = new Vector2 (0f, -velocity.y * 2.5f);
		thisRB.velocity = velocityTemp;
        // Set the push status to true
		bPushed = true;
        // Wait for 0.4 second
		yield return new WaitForSeconds (.4f);
		// Reduce damage count by 1
		nDamageNum--;
        // Reset velocity if the enemy main cell is not stunned
		if (!bStunned)
			ResetVelocity ();
        // Wait for 0.5 second
		yield return new WaitForSeconds (.5f);
        // Set back the push status
		bPushed = false;
	}

	// Stun the enemy main cell after receiving certain amount of hits within certain period of time
	IEnumerator Stun()
	{
        // When already stunned cannot call this function
		bStunned = true;
		bCanStun = false;
		// Pause rotation animation
		StartCoroutine (EMAnimation.Instance ().RotationPause (fStunTime / EMDifficulty.Instance().CurrentDiff));
        // Wait(being stunned) for seconds
		yield return new WaitForSeconds (fStunTime / EMDifficulty.Instance().CurrentDiff);
        // Set back the stunned status
		bStunned = false;
        // Cannot be stunned within seconds
		yield return new WaitForSeconds (fStunTime * EMDifficulty.Instance().CurrentDiff * 1.5f);
		bCanStun = true;
	}
	#endregion

	#region Movement
	// Auto reset velocity
	void ResetVelocity ()
	{
        // When in Defend state, the default velocity for resetting is different
		if (bIsDefend)
        {
			velocity = new Vector2 (fHoriSpeed, fSpeed * fSpeedFactor * fDefendFactor);
			thisRB.velocity = velocity;
			fSpeedTemp = fSpeed;
		}
        else
        {
			velocity = new Vector2(fHoriSpeed, fSpeed * fSpeedFactor);
			thisRB.velocity = velocity;
			fSpeedTemp = fSpeed;
		}
	}

	// Move the enemy main cell left or right
	IEnumerator MovingHorizontally ()
	{
		if (m_EMFSM.CurrentStateIndex == EMState.Production) 
		{
			bCanChangeHori = false;
			// Change direction based on the position of enemy nutrient and enemy main cell
			int nDirCount = 0;
			for (int i = 0; i < EMNutrientMainAgent.AgentList.Count; i++)
			{
				if (EMNutrientMainAgent.AgentList[i] != null)
				{
					if (EMNutrientMainAgent.AgentList[i].transform.position.x > transform.position.x)
						nDirCount ++;
					else if (EMNutrientMainAgent.AgentList[i].transform.position.x < transform.position.x)
						nDirCount --;
				}
			}
			if (nDirCount < 0 && !bMovingLeft) 
				bMovingLeft = true;
			else if (nDirCount > 0 && MovingLeft) 
				bMovingLeft = false;
			
			// Change speed based on num of nutrient on one side of the enemy main cell and number of available child cells
			float fSpeed = Random.Range (.05f, ((float)nDirCount + Mathf.Sqrt (Mathf.Sqrt (m_EMFSM.AvailableChildNum))) / 20f);
			fHoriSpeed = fSpeed;
			
			// Frequency of checking for changing of direction in terms of health of enemy main cell
			float fTime = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.Health / 2f)) * 1.5f, Mathf.Sqrt ((float)m_EMFSM.Health) * 1.5f);
			
			yield return new WaitForSeconds (fTime);
			bCanChangeHori = true;
		} 
		else if (m_EMFSM.CurrentStateIndex == EMState.Defend) 
		{
			bCanChangeHori = false;
			
			// Change speed based on the number of child cells of the enemy main cell
			float fSpeed = Random.Range (.1f, .5f / Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.AvailableChildNum) + 1f));
			fHoriSpeed = fSpeed;
			
			// Frequency of checking for changing of direction in terms of the number of child cells of the enemy main cell
			float fTime = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.AvailableChildNum * 1f)) * 1.5f, 
			                            Mathf.Sqrt ((float)m_EMFSM.AvailableChildNum * 2f) * 1.5f);
			
			int bDirection = Random.Range (0, 2);
			if (bDirection == 0) 
				bMovingLeft = !bMovingLeft;
			
			yield return new WaitForSeconds (fTime);
			bCanChangeHori = true;
		} 
		else if (m_EMFSM.CurrentStateIndex == EMState.AggressiveAttack) 
		{
			bCanChangeHori = false;
			
			// Change speed based on the current aggressiveness of the enemy main cell
			float fSpeed = Random.Range (.1f, Mathf.Sqrt (m_EMFSM.CurrentAggressiveness) / 7.5f);
			fHoriSpeed = fSpeed;
			
			// Frequency of checking for changing of direction in terms of the current aggressiveness of the enemy main cell
			float fTime = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.CurrentAggressiveness * 2f)) * 1.5f, 
			                            Mathf.Sqrt ((float)m_EMFSM.CurrentAggressiveness * 4f) * 1.5f);
			
			int bDirection = Random.Range (0, 2);
			if (bDirection == 0) 
				bMovingLeft = !bMovingLeft;
			
			yield return new WaitForSeconds (fTime);
			bCanChangeHori = true;
		} 
		else if (m_EMFSM.CurrentStateIndex == EMState.CautiousAttack) 
		{
			bCanChangeHori = false;
			
			// Change speed based on the current aggressiveness of the enemy main cell
			float fSpeed = Random.Range (.1f, Mathf.Sqrt (m_EMFSM.CurrentAggressiveness) / 7.5f);
			fHoriSpeed = fSpeed;
			
			// Frequency of checking for changing of direction in terms of the current aggressiveness of the enemy main cell
			float fTime = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.CurrentAggressiveness * 2f)) * 1.5f, 
			                            Mathf.Sqrt ((float)m_EMFSM.CurrentAggressiveness * 4f) * 1.5f);
			
			int bDirection = Random.Range (0, 2);
			if (bDirection == 0) 
				bMovingLeft = !bMovingLeft;
			
			yield return new WaitForSeconds (fTime);
			bCanChangeHori = true;
		}
		else 
		{
			bCanChangeHori = false;
			int bDirection = Random.Range (0, 2);
			// Frequency of checking for changing of direction in terms of health of enemy main cell
			float fTime = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.Health / 2f)), Mathf.Sqrt ((float)m_EMFSM.Health));
			// Make sure the frequency of changing direction is not higher not once per second
			if (fTime <= 1.5f)
				fTime = Random.Range (1f, 1.5f);
			// Horizontal speed in terms of num of nutrient
			float fSpeed = Random.Range (.05f, 1f / Mathf.Sqrt ((float)nCurrentNutrientNum) + 1f);
		
			if (bDirection == 0) 
				bMovingLeft = !bMovingLeft;

			fHoriSpeed = fSpeed;
			ResetVelocity ();

			yield return new WaitForSeconds (fTime);
			bCanChangeHori = true;
		}
	}

	// Check the direction of horizontal movement is correct
	void HorizontalCheck ()
	{
		if (bMovingLeft && fHoriSpeed > 0f) {
			fHoriSpeed *= -1f;
			ResetVelocity ();
		} else if (!bMovingLeft && fHoriSpeed < 0f) {
			fHoriSpeed *= -1f;
			ResetVelocity ();
		}
	}

	public void ChangeSpeed(float changeInSpeed)
	{
		fSpeedTemp += changeInSpeed;
		velocity = new Vector2(fHoriSpeed, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
	}
	
	public void ChangeSpeedFactor(float changeInSpeedF)
	{
		// Change speed factor
		fSpeedFactor += changeInSpeedF;
		// Reset velocity
		velocity = new Vector2(0, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
	}

	public void ChangeDirection ()
	{
		bMovingLeft = !bMovingLeft;
	}
	#endregion

	#region Change aggressiveness due to the existence of squad captain
	void UpdateAggressiveness ()
	{
		// Reset Aggressiveness factor (Captain)
        if (PlayerSquadFSM.Instance.IsAlive && m_EMFSM.AggressivenessSquadCap == 0) {
			m_EMFSM.AggressivenessSquadCap = Random.Range (1f, 3f);
        }
        else if (!PlayerSquadFSM.Instance.IsAlive && m_EMFSM.CurrentAggressiveness > m_EMFSM.InitialAggressiveness) {
			m_EMFSM.AggressivenessSquadCap = 0f;
		}
		// Reset Aggressiveness factor (Child)
        if (PlayerSquadFSM.Instance.IsAlive && m_EMFSM.AggressivenessSquadChild != 10f / Mathf.Sqrt((float)PlayerSquadFSM.Instance.AliveChildCount())) {
            float fAggressivenessSquadChild = 10f / Mathf.Sqrt((float)PlayerSquadFSM.Instance.AliveChildCount());
			if (fAggressivenessSquadChild > 4f)
				fAggressivenessSquadChild = 4f;
			m_EMFSM.AggressivenessSquadChild = fAggressivenessSquadChild;
		}

		// Update Aggressiveness
		if (m_EMFSM.CurrentAggressiveness != m_EMFSM.InitialAggressiveness + m_EMFSM.AggressivenessSquadCap + m_EMFSM.AggressivenessSquadChild)
			m_EMFSM.CurrentAggressiveness = m_EMFSM.InitialAggressiveness + m_EMFSM.AggressivenessSquadCap + m_EMFSM.AggressivenessSquadChild;
	}
	#endregion
    
	void StartWithChildCells ()
	{
		for (int i = 0; i < fNumOfDefaultCells; i++) {
			EMHelper.Instance().ECPool.SpawnFromPool (EMHelper.Instance().Position, true);
		}
	}

	// Checking whether the enemy main cell goes out of the screen or runs out of health 
	// Deactivate enemy main cell gameObject if the player wins
	void LoseCheck ()
	{
		if (transform.position.y - EMHelper.Instance().Radius / 2f > EMHelper.topLimit)
			this.gameObject.SetActive (false);
		else if (m_EMFSM.Health <= 0)
			this.gameObject.SetActive (false);
	}
}