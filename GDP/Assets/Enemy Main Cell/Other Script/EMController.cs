using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private float fMinHoriSpeed;
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
	private Vector2 pushBackVel;
	private Vector2 pushForwardVel;
	private Vector2 stunVel;
	// Horizontal movement
	[SerializeField]
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
	public void CauseDamageOne () { nDamageNum++; m_EMFSM.Health--; fAttackElapsedTime = 0.1f; }
	private bool bPushed; 
	public bool Pushed { get { return bPushed; } }
	private bool bStunned;
	public bool Stunned { get { return bStunned; } }
	private bool bCanPush;
	private bool bCanStun;
	private bool bJustAttacked;
	private float fAttackElapsedTime;
	#endregion

	[Header("Stun state value")]
	[Tooltip("Value used in Stun state")]
	#region Value
	[SerializeField]
	private float fDefaultStunTime;
	[SerializeField]
	private float fCurrentStunTime;
	[SerializeField]
	private float fStunCoolDown;
	[SerializeField]
	private float fDefaultStunTolerance;
	[SerializeField]
	private float fCurrentStunTolerance;
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
	private PlayerMain PMain;
	public bool bIsMainBeingAttacked;
	public bool bShouldMainTank;
	public bool bIsAllChildWithinMain;

	void Awake ()
	{
		if (instance == null)
			instance = this;

		// GetComponent
		m_EMFSM = GetComponent<EnemyMainFSM> ();
		thisRB = GetComponent<Rigidbody2D> ();
		PMain = GameObject.Find("Player_Cell").GetComponent<PlayerMain>();

		// Size
		nInitialNutrientNum = Settings.s_nEnemyMainInitialNutrientNum;
		nCurrentNutrientNum = nInitialNutrientNum;
		// Speed
		fSpeed = Settings.s_fEnemyMainInitialVertSpeed;
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
		// Horizontal speed
		fMinHoriSpeed = Settings.s_fEnemyMainMinHiriSpeed;
		fHoriSpeed = Random.Range (fMinHoriSpeed, fMinHoriSpeed * 2f);
		bCanChangeHori = true;
		// Velocity
		velocity = new Vector2 (fHoriSpeed, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
		pushBackVel = new Vector2 (0.0f, -5.0f);
		pushForwardVel = new Vector2 (0.0f, 1.0f);
		stunVel = new Vector2 (0.0f, -0.5f);
		// Damage
		nDamageNum = 0;
		// State
		bPushed = false;
		bStunned = false;
		bCanPush = true;
		bCanStun = true;
		bJustAttacked = false;
		fAttackElapsedTime = 0.1f;
		fDefaultStunTime = 5f;
		fCurrentStunTime = fDefaultStunTime;
		fStunCoolDown = fDefaultStunTime * 2.0f;
		fDefaultStunTolerance = 5f;
		fCurrentStunTolerance = fDefaultStunTolerance;

		bIsMainBeingAttacked = false;
		bIsAllChildWithinMain = false;
		fNumOfDefaultCells = Settings.s_nEnemyMainInitialChildCellNum;
	}

	void Start ()
	{
		// Start with a few child cells
		StartWithChildCells ();
	}

	void Update()
	{
		// Force back the enemy main cell when received damage and not forced back
		if (nDamageNum > 0 && bCanPush && !bPushed && !Stunned && bJustAttacked) 
		{
			StartCoroutine(ForceBack());
		}
		if ((float)nDamageNum > fCurrentStunTolerance && !bStunned && bCanStun && !bPushed) 
		{
			StartCoroutine(Stun ());
		}
		// Keep updating velocity when stunned
		if (bStunned && thisRB.velocity != velocity)
			ResetVelocity ();
		// Update Aggresiveness
		UpdateAggressiveness ();
		// Check whether the Enemy main is being attacked by the player
		IsMainBeingAttacked();
		// Check whether the Enemy main is having enough of an advantage to tank player's attack to deal more damage onto the player's main
		ShouldMainBeTanking();
		// Check Whether all idling enemy child cells had entered the enemy main cell
		HasAllCellsEnterMain();
		// Check whether the enemy main cell was just attacked
		DamageTimer ();
	}

	void FixedUpdate ()
	{
		// Change the horizontal direction if allowed
		if (bCanChangeHori && !bPushed && !bStunned) {
			StartCoroutine (MovingHorizontally ());
		}
		// Check the direction of horizontal movement is correct
		HorizontalCheck();
		// Make sure the horizntal velocity is not lower than its minimum value
		HorizontalVelocityCheck ();
		// Update values according to current dificulty
		DifficultyUpdate ();
	}

	#region Damage behavior
	// Check whether the enemy main cell was just attacked
	void DamageTimer ()
	{
		fAttackElapsedTime -= Time.deltaTime;
		if (fAttackElapsedTime <= 0.0f)
			bJustAttacked = false;
		else
			bJustAttacked = true;
	}

	// Push back the enemy main cell when received attack
	IEnumerator ForceBack()
	{
		// One push at a time
		bCanPush = false;
		bPushed = true;

		// Push forward first
		if (EMHelper.Instance ().Position.y > Settings.s_fEnemyMainMinY && !bStunned)
			thisRB.velocity = pushForwardVel;

		// Wait for 0.1 second
		yield return new WaitForSeconds (.1f);
        // Temporary velocity for enemy main cell when being pushed
		if (EMHelper.Instance ().Position.y > Settings.s_fEnemyMainMinY && !bStunned)
			thisRB.velocity = pushBackVel;
        // Wait for 0.1 second
		yield return new WaitForSeconds (.1f);
		// Reduce damage count by 1
		nDamageNum--;
        // Reset velocity if the enemy main cell is not stunned
		if (!bStunned)
			ResetVelocity ();

		bPushed = false;
        // Wait for 3 second
		yield return new WaitForSeconds (3f);
        // Set back the push status
		bCanPush = true;
		if (!bStunned)
			nDamageNum = 0;
	}

	// Stun the enemy main cell after receiving certain amount of hits within certain period of time
	IEnumerator Stun()
	{
        // When already stunned cannot call this function
		bStunned = true;
		bCanStun = false;
		// Not force back when stunned
		ResetVelocity ();
        // Wait(being stunned)pushBackVel for seconds
		yield return new WaitForSeconds ((float)nDamageNum);
        // Set back the stunned status
		bStunned = false;
		float stunCoolDown = (float)nDamageNum * 1.5f;
		nDamageNum = 0;
		ResetVelocity ();
        // Cannot be stunned within seconds
		yield return new WaitForSeconds (stunCoolDown);
		bCanStun = true;
	}
	#endregion

	#region Movement
	// Auto reset velocity
	void ResetVelocity ()
	{
        // When in Defend state, the default velocity for resetting is different
		if (bIsDefend && !bStunned)
		{
			velocity = new Vector2 (fHoriSpeed, fSpeed * fSpeedFactor * fDefendFactor);
			thisRB.velocity = velocity;
			fSpeedTemp = fSpeed;
		} 
		else if (!bIsDefend && bStunned)
		{
			velocity = stunVel;
			thisRB.velocity = velocity;
		}
		else 
        {
			velocity = new Vector2(fHoriSpeed, fSpeed * fSpeedFactor);
			thisRB.velocity = velocity;
			fSpeedTemp = fSpeed;
		}
	}

	// Make sure the horizntal velocity is not lower than its minimum value
	void HorizontalVelocityCheck ()
	{
		if (Mathf.Abs (thisRB.velocity.x) < fMinHoriSpeed) 
		{
			if (thisRB.velocity.x > 0) {
				fHoriSpeed = fMinHoriSpeed;
				ResetVelocity ();
			} else if (thisRB.velocity.x < 0) {
				fHoriSpeed = -fMinHoriSpeed;
				ResetVelocity ();
			}
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

	// Move the enemy main cell left or right
	IEnumerator MovingHorizontally ()
	{
		if (m_EMFSM.CurrentStateIndex == EMState.Production || m_EMFSM.CurrentStateIndex == EMState.Maintain) 
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
			float speed = Random.Range (.05f, ((float)nDirCount + Mathf.Sqrt (Mathf.Sqrt (m_EMFSM.AvailableChildNum))) / 20f);
			fHoriSpeed = speed;
			
			// Frequency of checking for changing of direction in terms of health of enemy main cell
			float time = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.Health)) * 1.5f, Mathf.Sqrt ((float)m_EMFSM.Health) * 1.5f);
			
			yield return new WaitForSeconds (time);
			bCanChangeHori = true;
		} 
		else if (m_EMFSM.CurrentStateIndex == EMState.Defend) 
		{
			bCanChangeHori = false;
			
			// Change speed based on the number of child cells of the enemy main cell
			float speed = Random.Range (.1f, .5f / Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.AvailableChildNum) + 1f));
			fHoriSpeed = speed;
			
			// Frequency of checking for changing of direction in terms of the number of child cells of the enemy main cell
			float time = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.AvailableChildNum * 1f)) * 1.5f, 
			                            Mathf.Sqrt ((float)m_EMFSM.AvailableChildNum * 2f) * 1.5f);
			
			int direction = Random.Range (0, 2);
			if (direction == 0) 
				bMovingLeft = !bMovingLeft;
			
			yield return new WaitForSeconds (time);
			bCanChangeHori = true;
		} 
		else if (m_EMFSM.CurrentStateIndex == EMState.AggressiveAttack) 
		{
			bCanChangeHori = false;
			
			// Change speed based on the current aggressiveness of the enemy main cell
			float speed = Random.Range (.1f, Mathf.Sqrt (m_EMFSM.CurrentAggressiveness) / 7.5f);
			fHoriSpeed = speed;
			
			// Frequency of checking for changing of direction in terms of the current aggressiveness of the enemy main cell
			float time = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.CurrentAggressiveness * 2f)) * 1.5f, 
			                            Mathf.Sqrt ((float)m_EMFSM.CurrentAggressiveness * 4f) * 1.5f);
			
			int direction = Random.Range (0, 2);
			if (direction == 0) 
				bMovingLeft = !bMovingLeft;
			
			yield return new WaitForSeconds (time);
			bCanChangeHori = true;
		} 
		else if (m_EMFSM.CurrentStateIndex == EMState.CautiousAttack) 
		{
			bCanChangeHori = false;
			
			// Change speed based on the current aggressiveness of the enemy main cell
			float speed = Random.Range (.1f, Mathf.Sqrt (m_EMFSM.CurrentAggressiveness) / 7.5f);
			fHoriSpeed = speed;
			
			// Frequency of checking for changing of direction in terms of the current aggressiveness of the enemy main cell
			float time = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.CurrentAggressiveness * 2f)) * 1.5f, 
			                            Mathf.Sqrt ((float)m_EMFSM.CurrentAggressiveness * 4f) * 1.5f);
			
			int direction = Random.Range (0, 2);
			if (direction == 0) 
				bMovingLeft = !bMovingLeft;
			
			yield return new WaitForSeconds (time);
			bCanChangeHori = true;
		}
		else if (m_EMFSM.CurrentStateIndex == EMState.Landmine)
		{
			bCanChangeHori = false;
			int direction = Random.Range (0, 2);
			// Frequency of checking for changing of direction in terms of health of enemy main cell
			float time = Random.Range (Mathf.Sqrt (Mathf.Sqrt ((float)m_EMFSM.Health / 2f)), Mathf.Sqrt ((float)m_EMFSM.Health));
			// Make sure the frequency of changing direction is not higher not once per second
			if (time <= 1.5f)
				time = Random.Range (1f, 1.5f);
			// Horizontal speed in terms of num of nutrient
			float speed = Random.Range (.05f, 1f / (Mathf.Sqrt ((float)nCurrentNutrientNum) + fMinHoriSpeed));
		
			if (direction == 0) 
				bMovingLeft = !bMovingLeft;

			fHoriSpeed = speed;
			ResetVelocity ();

			yield return new WaitForSeconds (time);
			bCanChangeHori = true;
		}
	}

	public void ChangeDirection ()
	{
		bMovingLeft = !bMovingLeft;
	}
	#endregion

	#region Change aggressiveness due to the existence of squad captain and its child cells
	void UpdateAggressiveness ()
	{
		// Update Aggressiveness factor (Distance)
		m_EMFSM.AggressivenessDistance = EMHelper.Instance().MinToMaxYRatio + 1.0f;
		// Reset Aggressiveness factor (Enemy Child)
		if (m_EMFSM.AvailableChildNum > 0 && m_EMFSM.AggressivenessEnemyChild != Mathf.Sqrt(Mathf.Sqrt((float)m_EMFSM.AvailableChildNum))) {
			float fAggressivenessEnemyChild = Mathf.Sqrt(Mathf.Sqrt((float)m_EMFSM.AvailableChildNum));
			if (fAggressivenessEnemyChild > 4f)
				fAggressivenessEnemyChild = 4f;
			m_EMFSM.AggressivenessEnemyChild = fAggressivenessEnemyChild;
		}
		else if (m_EMFSM.AvailableChildNum == 0 && m_EMFSM.AggressivenessEnemyChild > 0f) {
			m_EMFSM.AggressivenessEnemyChild = 0f;
		}
		// Reset Aggressiveness factor (Squad Captain)
        if (PlayerSquadFSM.Instance.IsAlive && m_EMFSM.AggressivenessSquadCap == 0f) {
			m_EMFSM.AggressivenessSquadCap = Random.Range (1f, 3f);
        }
        else if (!PlayerSquadFSM.Instance.IsAlive && m_EMFSM.CurrentAggressiveness > 0f) {
			m_EMFSM.AggressivenessSquadCap = 0f;
		}
		// Reset Aggressiveness factor (Squad Child)
		if (PlayerSquadFSM.Instance.IsAlive && m_EMFSM.AggressivenessSquadChild != Mathf.Sqrt(Mathf.Sqrt((float)PlayerSquadFSM.Instance.AliveChildCount())) * 2.0f) {
			float fAggressivenessSquadChild = Mathf.Sqrt(Mathf.Sqrt((float)PlayerSquadFSM.Instance.AliveChildCount())) * 2.0f;
			if (fAggressivenessSquadChild > 4f)
				fAggressivenessSquadChild = 4f;
			m_EMFSM.AggressivenessSquadChild = fAggressivenessSquadChild;
		}
		else if (!PlayerSquadFSM.Instance.IsAlive && m_EMFSM.AggressivenessSquadChild > 0f)
			m_EMFSM.AggressivenessSquadChild = 0f;

		// Update Aggressiveness
		if (m_EMFSM.CurrentAggressiveness != (m_EMFSM.InitialAggressiveness + m_EMFSM.AggressivenessEnemyChild + m_EMFSM.AggressivenessSquadCap + m_EMFSM.AggressivenessSquadChild) / m_EMFSM.AggressivenessDistance)
			m_EMFSM.CurrentAggressiveness = (m_EMFSM.InitialAggressiveness + m_EMFSM.AggressivenessEnemyChild + m_EMFSM.AggressivenessSquadCap + m_EMFSM.AggressivenessSquadChild) / m_EMFSM.AggressivenessDistance;

		// Make sure aggressiveness is not below 1
		if (m_EMFSM.CurrentAggressiveness < 1.0f)
			m_EMFSM.CurrentAggressiveness = 1.0f;
	}
	#endregion

	#region Difficulty Update
	// Update values according to current dificulty
	void DifficultyUpdate ()
	{
		// Stun time
		if (fCurrentStunTime != fDefaultStunTime / EMDifficulty.Instance().CurrentDiff)
			fCurrentStunTime = fDefaultStunTime / EMDifficulty.Instance().CurrentDiff;
		// Stun cool down
		if (fStunCoolDown != fCurrentStunTime * 2.0f)
			fStunCoolDown = fCurrentStunTime * 2.0f;
		// Stun tolerance
		if (fCurrentStunTolerance != fDefaultStunTolerance * EMDifficulty.Instance().CurrentDiff)
			fCurrentStunTolerance = fDefaultStunTolerance * EMDifficulty.Instance().CurrentDiff;
	}
	#endregion
    
	// Spawn a few child cells when the game starts
	void StartWithChildCells ()
	{
		for (int i = 0; i < fNumOfDefaultCells; i++) {
			EMHelper.Instance().ECPool.SpawnFromPool (EMHelper.Instance().Position, true);
		}
	}
	
	//Check whether the enemy main cell is being attacked by a chargeChild/chargeMain player child cell, toggle the boolean "bIsMainBeingAttacked" accordingly
	void IsMainBeingAttacked()
	{
		Collider2D[] IncomingObjects = Physics2D.OverlapCircleAll(transform.position, 100f * GetComponent<SpriteRenderer>().bounds.size.x,Constants.s_onlyPlayerChildLayer);
		PCState PCCurrentState = PCState.Dead;
		if(IncomingObjects.Length <= 0)
		{
			bIsMainBeingAttacked = false;
			return;
		}
		
		for(int i = 0; i < IncomingObjects.Length; i++)
		{
			PCCurrentState = IncomingObjects[i].GetComponent<PlayerChildFSM>().GetCurrentState();
			if(PCCurrentState == PCState.ChargeChild || PCCurrentState == PCState.ChargeMain)
			{
				bIsMainBeingAttacked = true;
				return;
			}
		}
		
		bIsMainBeingAttacked = false;
	}
	
	//Check whether the Enemy main is having enough of an advantage to tank player's attack to deal more damage onto the player's main
	void ShouldMainBeTanking()
	{
		if (PlayerChildFSM.s_playerChildStatus != null) {
			int PCCount = 0;
			for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++) {
				if (PlayerChildFSM.s_playerChildStatus [i] == pcStatus.InLeftNode || PlayerChildFSM.s_playerChildStatus [i] == pcStatus.InRightNode) {
					PCCount++;
				}
			}
		
			if (m_EMFSM.Health > PMain.Health && m_EMFSM.ECList.Count > PCCount && PMain.Health < 35) {
				bShouldMainTank = true;
			}
			bShouldMainTank = false;
		}
	}
	
	void HasAllCellsEnterMain()
	{
		if (ECTracker.s_Instance != null) {
			List<EnemyChildFSM> ECList = ECTracker.s_Instance.IdleCells;
			for (int i = 0; i < ECList.Count; i++) {
				if (!ECIdleState.HasChildEnterMain (ECList [i].gameObject)) {
					bIsAllChildWithinMain = false;
					return;
				}
			}
			bIsAllChildWithinMain = true;
		}
	}

	public static void ResetStatics()
	{
		instance = null;
	}
}