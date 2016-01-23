using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EnemyMainFSM))]
[RequireComponent (typeof (Renderer))]
[RequireComponent (typeof (Rigidbody2D))]
public class EMAnimation : MonoBehaviour 
{
	// Instance of the class
	private static EMAnimation instance;
	// Singleton
	public static EMAnimation Instance()
	{
		return instance;
	}

	private Rigidbody2D thisRB;
	private Renderer thisRend;

	[SerializeField]
	private bool bIsExpanding;
	public bool IsExpanding { get { return bIsExpanding; } set { bIsExpanding = value; } }
	[SerializeField]
	private bool bIsShrinking;
	[SerializeField]
	private float fExpandScale = 1.25f;
	[SerializeField]
	private float fDefaultExpandRate = .05f;
	private float fTargetSize;

	#region Status
	private float fLandmineExpandFactor;
	private int nDieAniPhase;

	[SerializeField]
	bool bCanRotate;
	[SerializeField]
	private float fAngularVelocity;
	public float AngularVelocity { get { return fAngularVelocity; } set { fAngularVelocity = value; } }
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
	public Vector2 InitialScale { get { return initialScale; } }
	private Vector2 currentScale;
	public Vector2 CurrentScale { get { return currentScale; } }
	#endregion

	#region Color
	private Color defaultColor;
	private Color aggressieColor;
	private Color cautiousColor;
	private Color landmineColor;
	#endregion

	void Start () 
	{
		if (instance == null)
			instance = this;
		// GetComponent
		thisRB = GetComponent<Rigidbody2D> ();
		thisRend = GetComponent<Renderer> ();
		// Initialization of state status
		fLandmineExpandFactor = 3f;
		nDieAniPhase = 0;
		// Initialization of status
		bCanRotate = true;
		bIsExpanding = false;
		bIsShrinking = false;
		fTargetSize = 0f;
		// Initialization of velocity
		fAngularVelocity = fAngularIniFactor * Random.Range (.75f, 1.25f);
		thisRB.angularVelocity = fAngularVelocity;
		if (fAngularVelocity >= 0f)
			bIsRotatingLeft = false;
		else
			bIsRotatingLeft = true;
		// Initialization of scale
		initialScale = gameObject.transform.localScale;
		currentScale = initialScale * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().Health)));
		transform.localScale = (Vector3)currentScale;
		// Initialization of color
		defaultColor = thisRend.material.color;
		aggressieColor = new Vector4 (1f, 0.25f, 0.25f, 1f);
		cautiousColor = new Vector4 (1f, 0.5f, 0.5f, 1f);
		landmineColor = new Vector4 (1f, 0.5f, 0.5f, 1f);
	}

	void Update () 
	{
		SizeUpdate ();
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
		// Update the color of enemy main cell
		ColorUpdate ();
		// Die animation called in the Die state
		DieAnimation ();
		// Expand animation in Landmine state
		LandmineAnimation ();
	}

	// Update the size of enemy main cell according to current health
	private void SizeUpdate ()
	{
		if (currentScale != initialScale * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().Health))) && 
		    !bIsExpanding &&
		    !bIsShrinking &&
		    EnemyMainFSM.Instance().CurrentStateIndex != EMState.Die)
		{
			currentScale = initialScale * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().Health)));
			transform.localScale = (Vector3)currentScale;
		}
	}
	// Update angular velocity of the enemy main cell when rotation is allowed
	private void RotationUpdate ()
	{
		// Angular velocity declines as time goes by in Production state
		if (fAngularVelocity >= 0f && bCanRotate && EnemyMainFSM.Instance().CurrentStateIndex == EMState.Production) {
			if (fAngularVelocity >= fMinAngularVelocity)
				fAngularVelocity -= fAngularDeclineFactor * Mathf.Abs (fAngularVelocity / 3f);
		} else if (fAngularVelocity < 0f && bCanRotate && EnemyMainFSM.Instance().CurrentStateIndex == EMState.Production) {
			if (fAngularVelocity <= -fMinAngularVelocity)
				fAngularVelocity += fAngularDeclineFactor * Mathf.Abs (fAngularVelocity / 3f);
		}

		// Make sure the angular velocity is not less than the minimum value
		if (Mathf.Abs (fAngularVelocity) < fMinAngularVelocity && bCanRotate && EnemyMainFSM.Instance().CurrentStateIndex == EMState.Production) 
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
		if (EnemyMainFSM.Instance().CurrentStateIndex != EMState.Landmine &&
		    EnemyMainFSM.Instance().CurrentStateIndex != EMState.Die) {
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
				currentScale.x >= initialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (EnemyMainFSM.Instance ().Health)))) {
				currentScale.x -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.x));
				currentScale.y -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.y));
			} else if (!bIsExpanding && 
				bIsShrinking &&
				currentScale.x < initialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (EnemyMainFSM.Instance ().Health)))) {
				bIsShrinking = false;
			}

			transform.localScale = (Vector3)currentScale;
		}
	}

	#region State Animations
	// Die animation called in the Die state
	private void DieAnimation ()
	{
		if (EnemyMainFSM.Instance ().CurrentStateIndex == EMState.Die) {

			if (nDieAniPhase == 1 && transform.localScale.x > initialScale.x / 2f)
			{
				bIsExpanding = false;
				bIsShrinking = true;
				currentScale.x -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.x - initialScale.x / 2f));
				currentScale.y -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.y - initialScale.y / 2f));
			}
			else if (nDieAniPhase == 1)
				nDieAniPhase = 2;

			if (nDieAniPhase == 2 && transform.localScale.x <= initialScale.x)
			{
				bIsExpanding = true;
				bIsShrinking = false;
				currentScale.x += fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (initialScale.x - currentScale.x));
				currentScale.y += fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (initialScale.y - currentScale.y));
			}
			else if (nDieAniPhase == 2)
				nDieAniPhase = 3;

			if (nDieAniPhase == 3 && transform.localScale.x > initialScale.x / 2.5f)
			{
				bIsExpanding = false;
				bIsShrinking = true;
				currentScale.x -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.x - initialScale.x / 2f));
				currentScale.y -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.y - initialScale.y / 2f));
			}
			else if (nDieAniPhase == 3)
				nDieAniPhase = 4;

			if (nDieAniPhase == 4 && transform.localScale.x <= initialScale.x / 1.25f)
			{
				bIsExpanding = true;
				bIsShrinking = false;
				currentScale.x += fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (initialScale.x / 1.25f - currentScale.x));
				currentScale.y += fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (initialScale.y / 1.25f - currentScale.y));
			}
			else if (nDieAniPhase == 4)
				nDieAniPhase = 5;

			if (nDieAniPhase == 5 && transform.localScale.x > initialScale.x / 20f)
			{
				bIsExpanding = false;
				bIsShrinking = true;
				currentScale.x -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.x / 3f));
				currentScale.y -= fDefaultExpandRate * Mathf.Sqrt (Mathf.Abs (currentScale.y / 3f));
			}
			else if (nDieAniPhase == 5)
			{
				nDieAniPhase = 0;							// Prevent the animation from showing again before the next death
				EMHelper.Instance ().Visibility (false); 	// Make the enemy main cell invisible
			}

			transform.localScale = (Vector3)currentScale;
		} else
			nDieAniPhase = 1;								// Reset the animation phase after exit Die state
	}
	// Expand animation in Landmine state
	private void LandmineAnimation ()
	{
		if (EnemyMainFSM.Instance().CurrentStateIndex == EMState.Landmine) 
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
			         currentScale.x >= initialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (EnemyMainFSM.Instance ().Health)))) 
			{
				currentScale.x -= fDefaultExpandRate * fLandmineExpandFactor * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.x));
				currentScale.y -= fDefaultExpandRate * fLandmineExpandFactor * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.y));
			}
			else if (!bIsExpanding && 
			         bIsShrinking &&
			         currentScale.x < initialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (EnemyMainFSM.Instance ().Health)))) 
			{
				bIsShrinking = false;
				bIsExpanding = true;
			}
			
			transform.localScale = (Vector3)currentScale;
		}
	}
	// Color change in AggresiveAttack, CautiousAttack and Landmine states
	private void ColorUpdate ()
	{
		if (EnemyMainFSM.Instance().CurrentStateIndex == EMState.AggressiveAttack)
		{
			thisRend.material.color = aggressieColor;
		}
		else if (EnemyMainFSM.Instance().CurrentStateIndex == EMState.CautiousAttack)
		{
			thisRend.material.color = cautiousColor;
		}
		else if (EnemyMainFSM.Instance().CurrentStateIndex == EMState.Landmine)
		{
			thisRend.material.color = landmineColor;
		}
		else if (EnemyMainFSM.Instance().CurrentStateIndex != EMState.AggressiveAttack && 
		         EnemyMainFSM.Instance().CurrentStateIndex != EMState.CautiousAttack && 
		         EnemyMainFSM.Instance().CurrentStateIndex != EMState.Landmine)
		{
			thisRend.material.color = defaultColor;
		}
	}
	// Rotate faster in AggresiveAttack and CautiousAttack states
	private void FasterRotation ()
	{
		if (EnemyMainFSM.Instance().CurrentStateIndex == EMState.AggressiveAttack || 
		    EnemyMainFSM.Instance().CurrentStateIndex == EMState.CautiousAttack) 
		{
			// Angular velocity increases as time goes by
			if (fAngularVelocity >= 0f && bCanRotate) 
			{
				if (fAngularVelocity >= fMinAngularVelocity)
					fAngularVelocity += fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity)) * 
						Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().CurrentAggressiveness));
			} 
			else if (fAngularVelocity < 0f && bCanRotate) 
			{
				if (fAngularVelocity <= -fMinAngularVelocity)
					fAngularVelocity -= fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity)) * 
						Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().CurrentAggressiveness));
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
		if (bCanRotate && EnemyMainFSM.Instance().CurrentStateIndex == EMState.Stunned) 
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

	public static void ResetStatics()
	{
		instance = null;
	}
}