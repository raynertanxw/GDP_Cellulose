using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Renderer))]
[RequireComponent (typeof (EMMainMenuController))]
[RequireComponent (typeof (Rigidbody2D))]
public class EMMainMenuAnimation : MonoBehaviour 
{
	private Rigidbody2D thisRB;
	private Renderer thisRend;
	public GameObject halo;

	[SerializeField]
	private float fSizeFactor = 1f;
	[SerializeField]
	private bool bIsExpanding;
	[SerializeField]
	private bool bIsShrinking;
	[SerializeField]
	private float fExpandScale;
	[SerializeField]
	private float fDefaultExpandRate = .05f;
	private float fTargetSize;
	
	#region Status
	private float fLandmineExpandFactor;
	private int nDieAniPhase;

	private bool bCanTransition;

	[SerializeField]
	private bool bCanRotate;
	[SerializeField]
	private bool bCanBlink;
	private float fBlinkElapsedTime;
	[SerializeField]
	private float fAngularVelocity;
	[SerializeField]
	private float fAngularIniFactor = 50f;
	[SerializeField]
	private float fAngularDeclineFactor = .01f;
	[SerializeField]
	private float fMinAngularVelocity = 20f;
	[SerializeField]
	private float fMaxAngularVelocity = 200f;
	[SerializeField]
	private bool bIsRotatingLeft;
	#endregion
	
	#region Scale
	private Vector2 initialScale;
	private Vector2 currentScale;
	#endregion
	
	#region Color
	private Color defaultColor;
	private Color aggressiveColor;
	private Color cautiousColor;
	private Color landmineColor;
	private Color defendColor;
	private Color stunColor;
	private Color dieColor;
	#endregion

	#region State Emulator
	/// <summary>
	/// No of current state
	/// 1 - Production
	/// 2 - Defend
	/// 3 - AggressiveAttack
	/// 4 - CautiousAttack
	/// 5 - Landmine
	/// 6 - Stun
	/// 7 - Die
	/// </summary>
	[SerializeField]
	private int nCurrentStateNo;
	// No of previous state
	private int nPreviousStateNo;
	#endregion

	void Start ()
	{
		// GetComponent
		thisRB = GetComponent<Rigidbody2D> ();
		thisRend = GetComponent<Renderer> ();
		// Initialization of state status
		fExpandScale = 1.25f;
		fLandmineExpandFactor = 3f;
		nDieAniPhase = 0;
		// Initialization of status
		bCanTransition = true;
		bCanRotate = true;
		bCanBlink = false;
		bIsExpanding = false;
		bIsShrinking = false;
		fTargetSize = 0f;
		fBlinkElapsedTime = 0.0f;
		// Initialization of velocity
		fAngularVelocity = fAngularIniFactor * Random.Range (.75f, 1.25f);
		thisRB.angularVelocity = fAngularVelocity;
		if (fAngularVelocity >= 0f)
			bIsRotatingLeft = false;
		else
			bIsRotatingLeft = true;
		// Initialization of scale
		initialScale = gameObject.transform.localScale;
		currentScale = initialScale * fSizeFactor;
		transform.localScale = (Vector3)currentScale;
		// Initialization of color
		defaultColor = thisRend.material.color;
		aggressiveColor = new Vector4 (1f, 0.25f, 0.25f, 1f);
		cautiousColor = new Vector4 (1f, 0.5f, 0.5f, 1f);
		landmineColor = new Vector4 (1f, 0.5f, 0.5f, 1f);
		defendColor = Color.yellow;
		stunColor = new Vector4 (0.6f, 0.6f, 0.6f, 1f);
		dieColor = new Vector4 (0.6f, 0.6f, 0.6f, 1f);
		// Initialization of the first state
		nCurrentStateNo = Random.Range (1, 8);
		nPreviousStateNo = nCurrentStateNo;
	}

	
	void Update () 
	{
		SizeUpdate ();
		if (bCanTransition) 
			StartCoroutine (StateTransitionPause ());
		if (nCurrentStateNo == 2 || nCurrentStateNo == 3 || nCurrentStateNo == 4 || nCurrentStateNo == 5)
			bCanBlink = true;
		else
			bCanBlink = false;
	}
	
	void FixedUpdate ()
	{
		// Update angular velocity of the enemy main cell when rotation is allowed
		RotationUpdate ();
		// Make sure the angular velocity does not exceed its limit
		RotationLimit ();
		// Rotate faster in AggresiveAttack and CautiousAttack states
		FasterRotation ();
		// Angular velocity declines faster as time goes by in Stun state
		FasterRotationDecline ();
		// Enemy main cell expands in size when receives nutrient
		ExpandAnimation ();
		// Color blink animation of enemy main cell
		ColorBlink ();
		// Update the color of halo effect
		HaloColorUpdate ();
		// Die animation called in the Die state
		DieAnimation ();
		// Expand animation in Landmine state
		LandmineAnimation ();
	}

	private void StateUpdate()
	{
		do 
		{
			nCurrentStateNo = Random.Range (1, 8);
		} while (nCurrentStateNo == nPreviousStateNo);

		nPreviousStateNo = nCurrentStateNo;

		// Reset color
		thisRend.material.color = defaultColor;

		// Enable expansion if transition to Landmine state
		if (nCurrentStateNo == 5)
			bIsExpanding = true;
		else
			bIsExpanding = false;
	}

	private IEnumerator StateTransitionPause ()
	{
		// Pause transition
		bCanTransition = false;
		// Call change state
		StateUpdate ();
		yield return new WaitForSeconds (Random.Range (5f, 8f));
		// Enable transition if current state is not DIe
		if (nCurrentStateNo != 7)
			bCanTransition = true;
	}

	// Update the size of enemy main cell according to current health
	private void SizeUpdate ()
	{
		if (EnemyMainFSM.Instance() != null)
		{
			if (currentScale != initialScale * fSizeFactor && 
			    !bIsExpanding &&
			    !bIsShrinking &&
			    nCurrentStateNo != 7)
			{
				currentScale = initialScale * fSizeFactor;
				transform.localScale = (Vector3)currentScale;
			}
		}
	}
	// Update angular velocity of the enemy main cell when rotation is allowed
	private void RotationUpdate ()
	{
		// Angular velocity declines as time goes by in Production state
		if (fAngularVelocity >= 0f && bCanRotate && nCurrentStateNo == 1) {
			if (fAngularVelocity >= fMinAngularVelocity)
				fAngularVelocity -= fAngularDeclineFactor * Mathf.Abs (fAngularVelocity / 3f);
		} else if (fAngularVelocity < 0f && bCanRotate && nCurrentStateNo == 1) {
			if (fAngularVelocity <= -fMinAngularVelocity)
				fAngularVelocity += fAngularDeclineFactor * Mathf.Abs (fAngularVelocity / 3f);
		}
		
		// Make sure the angular velocity is not less than the minimum value
		if (Mathf.Abs (fAngularVelocity) < fMinAngularVelocity && bCanRotate && nCurrentStateNo == 1) 
		{
			if (!bIsRotatingLeft) {
				fAngularVelocity = -fAngularIniFactor * Random.Range (.75f, 1.25f);
				bIsRotatingLeft = true;
			}
			else {
				fAngularVelocity = fAngularIniFactor * Random.Range (.75f, 1.25f);
				bIsRotatingLeft = false;
			}
		}
		
		thisRB.angularVelocity = fAngularVelocity;
	}
	// Make sure the angular velocity does not exceed its limit
	private void RotationLimit ()
	{
		if (thisRB.angularVelocity > fMaxAngularVelocity)
			thisRB.angularVelocity = fMaxAngularVelocity;
	}
	// Pause rotation for seconds
	public IEnumerator RotationPause (float time)
	{
		bCanRotate = false;
		yield return new WaitForSeconds (time);
		bCanRotate = true;
	}
	// Enemy main cell expands in size when receives nutrient
	private void ExpandAnimation ()
	{
		if (!bIsExpanding && !bIsShrinking)
			fTargetSize = currentScale.x * fExpandScale;
		if (nCurrentStateNo != 5 && nCurrentStateNo != 7) {
			if (bIsExpanding) {
				if (currentScale.x <= fTargetSize) {
					currentScale.x += fDefaultExpandRate * Mathf.Sqrt (fTargetSize - currentScale.x);
					currentScale.y += fDefaultExpandRate * Mathf.Sqrt (fTargetSize - currentScale.y);
				} else {
					bIsExpanding = false;
					bIsShrinking = true;
				}
			} else if (!bIsExpanding && 
			           bIsShrinking &&
			           currentScale.x >= initialScale.x * fSizeFactor) {
				currentScale.x -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.x));
				currentScale.y -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.y));
			} else if (!bIsExpanding && 
			           bIsShrinking &&
			           currentScale.x < initialScale.x * fSizeFactor) {
				bIsShrinking = false;
			}
			
			transform.localScale = (Vector3)currentScale;
		}
	}
	
	#region State Animations
	// Die animation called in the Die state
	private void DieAnimation ()
	{
		if (nCurrentStateNo == 7) {
			
			if (nDieAniPhase == 1 && transform.localScale.x > initialScale.x / 2f) {
				bIsExpanding = false;
				bIsShrinking = true;
				currentScale.x -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.x - initialScale.x / 2f)) / 1.5f;
				currentScale.y -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.y - initialScale.y / 2f)) / 1.5f;
			} else if (nDieAniPhase == 1)
				nDieAniPhase = 2;
			
			if (nDieAniPhase == 2 && transform.localScale.x <= initialScale.x) {
				bIsExpanding = true;
				bIsShrinking = false;
				currentScale.x += fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (initialScale.x - currentScale.x)) / 1.5f;
				currentScale.y += fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (initialScale.y - currentScale.y)) / 1.5f;
			} else if (nDieAniPhase == 2)
				nDieAniPhase = 3;
			
			if (nDieAniPhase == 3 && transform.localScale.x > initialScale.x / 2.5f) {
				bIsExpanding = false;
				bIsShrinking = true;
				currentScale.x -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.x - initialScale.x / 2.5f)) / 1.5f;
				currentScale.y -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.y - initialScale.y / 2.5f)) / 1.5f;
			} else if (nDieAniPhase == 3)
				nDieAniPhase = 4;
			
			if (nDieAniPhase == 4 && transform.localScale.x <= initialScale.x / 1.25f) {
				bIsExpanding = true;
				bIsShrinking = false;
				currentScale.x += fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (initialScale.x - currentScale.x)) / 3f;
				currentScale.y += fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (initialScale.y - currentScale.y)) / 3f;
			} else if (nDieAniPhase == 4)
				nDieAniPhase = 5;
			
			if (nDieAniPhase == 5 && transform.localScale.x > initialScale.x / 20f) {
				bIsExpanding = false;
				bIsShrinking = true;
				currentScale.x -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.x / 5f));
				currentScale.y -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.y / 5f));
			} else if (nDieAniPhase == 5) {
				nDieAniPhase = 0;							// Prevent the animation from showing again before the next death
			}

			if (nDieAniPhase == 0)
			{
				currentScale.x += fDefaultExpandRate * (initialScale.x * fSizeFactor - currentScale.x / 1.5f) / 2f;
				currentScale.y += fDefaultExpandRate * (initialScale.y * fSizeFactor - currentScale.y / 1.5f) / 2f;
				
				if (currentScale.x >= initialScale.x * fSizeFactor)
				{
					bCanTransition = true;
				}
			}
			
			transform.localScale = (Vector3)currentScale;
		} else {
			if (nDieAniPhase != 1)
				nDieAniPhase = 1;								// Reset the animation phase after exit Die state
		}
		
		// Another set of code for color change
		if (nCurrentStateNo == 7) {
			thisRend.material.color = dieColor * currentScale.x;
		}
	}
	// Expand animation in Landmine state
	private void LandmineAnimation ()
	{
		if (nCurrentStateNo == 5) 
		{
			if (bIsExpanding) 
			{
				if (currentScale.x <= fTargetSize)
				{
					currentScale.x += fDefaultExpandRate * fLandmineExpandFactor * Mathf.Sqrt (fTargetSize - currentScale.x);
					currentScale.y += fDefaultExpandRate * fLandmineExpandFactor * Mathf.Sqrt (fTargetSize - currentScale.y);
				}
				else 
				{
					bIsExpanding = false;
					bIsShrinking = true;
				}
			} 
			else if (!bIsExpanding && 
			         bIsShrinking &&
			         currentScale.x >= initialScale.x * fSizeFactor) 
			{
				currentScale.x -= fDefaultExpandRate * fLandmineExpandFactor * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.x));
				currentScale.y -= fDefaultExpandRate * fLandmineExpandFactor * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.y));
			}
			else if (!bIsExpanding && 
			         bIsShrinking &&
			         currentScale.x < initialScale.x * fSizeFactor) 
			{
				bIsShrinking = false;
				bIsExpanding = true;
			}
			
			transform.localScale = (Vector3)currentScale;
		}
	}
	// Color blink animation in AggresiveAttack, CautiousAttack, Landmine and defend states
	private void ColorBlink ()
	{
		// Reset elapsed time when blink animation is disabled
		if (!bCanBlink && fBlinkElapsedTime != 0.0f)
			fBlinkElapsedTime = 0.0f;
		
		// Update blink animation
		if (bCanBlink) 
		{
			fBlinkElapsedTime += Time.deltaTime;
			if (nCurrentStateNo == 3)
			{
				if(fBlinkElapsedTime >= 0.8f / Mathf.Sqrt(Mathf.Sqrt((float)Settings.s_nEnemyMainInitialAggressiveness * 2f)))
				{
					if (thisRend.material.color != aggressiveColor)
					{
						thisRend.material.color = aggressiveColor;
					}
					else 
						thisRend.material.color = defaultColor;
					
					fBlinkElapsedTime = 0.0f;
				}
			}
			else if (nCurrentStateNo == 4)
			{
				if(fBlinkElapsedTime >= 1.2f / Mathf.Sqrt(Mathf.Sqrt((float)Settings.s_nEnemyMainInitialAggressiveness * 2f)))
				{
					if (thisRend.material.color != cautiousColor)
					{
						thisRend.material.color = cautiousColor;
					}
					else 
						thisRend.material.color = defaultColor;
					
					fBlinkElapsedTime = 0.0f;
				}
			}
			else if (nCurrentStateNo == 5)
			{
				if(fBlinkElapsedTime >= 1.2f / Mathf.Sqrt(Mathf.Sqrt((float)Settings.s_nEnemyMainInitialAggressiveness * 2f)))
				{
					if (thisRend.material.color != landmineColor)
					{
						thisRend.material.color = landmineColor;
					}
					else 
						thisRend.material.color = defaultColor;
					
					fBlinkElapsedTime = 0.0f;
				}
			}
			else if (nCurrentStateNo == 2)
			{
				if(fBlinkElapsedTime >= 1.2f / (Mathf.Sqrt ((Settings.s_nEnemyMainInitialChildCellNum) / 2.0f)))
				{
					if (thisRend.material.color != defendColor)
					{
						thisRend.material.color = defendColor;
					}
					else 
						thisRend.material.color = defaultColor;
					
					fBlinkElapsedTime = 0.0f;
				}
			}
		}
		// Update color in stun state
		else if (nCurrentStateNo == 6)
		{
			if (thisRend.material.color != stunColor)
				thisRend.material.color = stunColor;
		}
		else 
			thisRend.material.color = defaultColor;
	}
	// Update the color of halo effect
	private void HaloColorUpdate ()
	{
		if (thisRend.material.color != defaultColor) 
		{
			if (!halo.GetComponent<Renderer> ().enabled)
				halo.GetComponent<Renderer> ().enabled = true;
			if (thisRend.material.color == defendColor)
				halo.GetComponent<Renderer> ().material.color = Color.yellow;
			else if (thisRend.material.color == aggressiveColor)
				halo.GetComponent<Renderer> ().material.color = Color.red;
			else if (thisRend.material.color == cautiousColor || thisRend.material.color == landmineColor)
				halo.GetComponent<Renderer> ().material.color = Color.red * 0.8f;
			else if (thisRend.material.color == stunColor)
				halo.GetComponent<Renderer> ().material.color = stunColor;
		}
		else
			halo.GetComponent<Renderer> ().enabled = false;
	}
	// Rotate faster in AggresiveAttack and CautiousAttack states
	private void FasterRotation ()
	{
		if (nCurrentStateNo == 3 || nCurrentStateNo == 4) 
		{
			// Angular velocity increases as time goes by
			if (fAngularVelocity >= 0f && bCanRotate) 
			{
				if (fAngularVelocity >= fMinAngularVelocity)
					fAngularVelocity += fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity)) * 
						Mathf.Sqrt(Mathf.Sqrt(Settings.s_nEnemyMainInitialAggressiveness));
			} 
			else if (fAngularVelocity < 0f && bCanRotate) 
			{
				if (fAngularVelocity <= -fMinAngularVelocity)
					fAngularVelocity -= fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity)) * 
						Mathf.Sqrt(Mathf.Sqrt(Settings.s_nEnemyMainInitialAggressiveness));
			}
			
			// Make sure the angular velocity is not less than the minimum value
			if (Mathf.Abs (fAngularVelocity) < fMinAngularVelocity * 3f && bCanRotate) {
				if (!bIsRotatingLeft) {
					fAngularVelocity = -fAngularIniFactor * Random.Range (.75f, 1.25f) * 1.5f;
					bIsRotatingLeft = true;
				} else {
					fAngularVelocity = fAngularIniFactor * Random.Range (.75f, 1.25f) * 1.5f;
					bIsRotatingLeft = false;
				}
			}
			
			thisRB.angularVelocity = fAngularVelocity;
		}
	}
	// Angular velocity declines faster in Stun state
	private void FasterRotationDecline ()
	{
		if (nCurrentStateNo == 6) 
		{
			if (fAngularVelocity >= 0f) {
				if (fAngularVelocity >= fMinAngularVelocity)
					fAngularVelocity -= fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity) * 5f);
			} else if (fAngularVelocity < 0f) {
				if (fAngularVelocity <= -fMinAngularVelocity)
					fAngularVelocity += fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity) * 5f);
			}
			// If the angular velocity is too small, set it to zero
			if (Mathf.Abs (fAngularVelocity) < fMinAngularVelocity / 2f) 
				fAngularVelocity = 0f;
		}
	}
	#endregion
}