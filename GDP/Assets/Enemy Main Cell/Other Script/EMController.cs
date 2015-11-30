using UnityEngine;
using System.Collections;

public class EMController : MonoBehaviour 
{	
	#region Speed & Velocity
	// Speed
	private float fSpeed;
	public float Speed{ get { return fSpeed; } }
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
	float fHoriSpeed;
	#endregion

	#region Status
	private int nDamageNum; // Amount of damages received within certain period of time, determines whether the enemy main cell will be stunned 
	private bool bPushed; 
	private bool bStunned;
	public bool Stunned { get { return bStunned; } }
	private bool bCanStun;
	#endregion

	#region Size
	private int nInitialSize;
	private int nNutrientNum;
	public int NutrientNum { get { return nNutrientNum; } }
	private Vector2 initialScale;
	private Vector2 currentScale;
	#endregion

	private Rigidbody2D thisRB;

	void Start()
	{
		// GetComponent
		thisRB = GetComponent<Rigidbody2D> ();
		// Speed
		fSpeed = .25f;
		fSpeedFactor = 1f;
		fSpeedTemp = fSpeed;
		bIsDefend = false;
		fDefendFactor = 0.9f;
		// Velocity
		velocity = new Vector2 (0, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
		// Horizontal movement
		bMovingLeft = true;
		bCanChangeHori = true;
		fHoriSpeed = 1f / Mathf.Sqrt (nNutrientNum);
		// Damage
		nDamageNum = 0;
		// State
		bPushed = false;
		bStunned = false;
		bCanStun = true;
		// Size
		nInitialSize = 150;
		nNutrientNum = 50;
		initialScale = gameObject.transform.localScale;
		currentScale = initialScale * (nInitialSize - Mathf.Sqrt(50 - nNutrientNum));
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
        // Change the horizontal direction if allowed
        if (bCanChangeHori) {
			StartCoroutine (MovingHorizontally ());
		}
        // Check the direction of horizontal movement is correct
        HorizontalCheck();
		// Check size
		if (currentScale != initialScale * (nInitialSize - Mathf.Sqrt(50 - nNutrientNum))) 
		{
			currentScale = initialScale * (nInitialSize - Mathf.Sqrt(50 - nNutrientNum));
		}
	}

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
        // Wait(being stunned) for 2 seconds
		yield return new WaitForSeconds (2f);
        // Set back the stunned status
		bStunned = false;
        // Cannot be stunned within 3 seconds
		yield return new WaitForSeconds (3f);
		bCanStun = true;
	}

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
		bCanChangeHori = false;
		int bDirection = Random.Range (0, 2);
		float fTime = Random.Range (0f, 5f);
		float fSpeed = Random.Range (0f, 1f / Mathf.Sqrt (nNutrientNum));

		if (bDirection == 0) 
			bMovingLeft = !bMovingLeft;

		fHoriSpeed = fSpeed;
		yield return new WaitForSeconds (fTime);
		bCanChangeHori = true;
	}

	// Check the direction of horizontal movement is correct
	void HorizontalCheck ()
	{
		if (bMovingLeft && fHoriSpeed > 0)
			fHoriSpeed = -fHoriSpeed;
		else if (!bMovingLeft && fHoriSpeed < 0)
			fHoriSpeed = -fHoriSpeed;
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
    // Receive damage from incoming player mini cells and destroy the mini cell
	void OnTriggerEnter2D (Collider2D collision)
	{
		if (collision.gameObject.tag == "Player") 
		{
			nDamageNum++;
			Destroy (collision.gameObject);
		}
	}
    // Recevice nutrient from incoming enemy mini nutrient and destroy the mini nutrient
	void OnCollisionEnter2D (Collision2D collision)
	{
		if (collision.gameObject.tag == "EnemyNutrient")
		{
			nNutrientNum++;
			Destroy (collision.gameObject);
		}
	}
}