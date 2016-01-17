using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EnemyMainFSM))]
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

	[SerializeField]
	private bool bIsExpanding;
	public bool IsExpanding { get { return bIsExpanding; } set { bIsExpanding = value; } }
	[SerializeField]
	private bool bIsShrinking;
	[SerializeField]
	private float fExpandScale = 1.25f;
	[SerializeField]
	private float fExpandRate = .05f;
	private float fTargetSize;

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
	private bool bIsRotatingLeft;

	#region Scale
	private Vector2 initialScale;
	private Vector2 currentScale;
	public Vector2 CurrentScale { get { return currentScale; } }
	#endregion

	void Start () 
	{
		if (instance == null)
			instance = this;
		// GetComponent
		thisRB = GetComponent<Rigidbody2D> ();
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
	}

	void Update () 
	{
		SizeUpdate ();
	}

	void FixedUpdate ()
	{
		// Update angular velocity of the enemy main cell when rotation is allowed
		RotationUpdate ();
		// Angular velocity declines faster as time goes by
		FasterRotationDecline ();
		// Enemy main cell expands in size when receives nutrient
		ExpandAnimation ();
	}

	// Update the size of enemy main cell according to current health
	private void SizeUpdate ()
	{
		if (currentScale != initialScale * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().Health))) && 
		    !bIsExpanding &&
		    !bIsShrinking)
		{
			currentScale = initialScale * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().Health)));
			transform.localScale = (Vector3)currentScale;
		}
	}
	// Update angular velocity of the enemy main cell when rotation is allowed
	private void RotationUpdate ()
	{
		// Angular velocity declines as time goes by
		if (fAngularVelocity >= 0f && bCanRotate) {
			if (fAngularVelocity >= fMinAngularVelocity)
				fAngularVelocity -= fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity));
		} else if (fAngularVelocity < 0f && bCanRotate) {
			if (fAngularVelocity <= -fMinAngularVelocity)
				fAngularVelocity += fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity));
		}

		// Make sure the angular velocity is not less than the minimum value
		if (Mathf.Abs (fAngularVelocity) < fMinAngularVelocity && bCanRotate) 
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
	// Angular velocity declines faster as time goes by
	private void FasterRotationDecline ()
	{
		if (!bCanRotate) 
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
		if (bIsExpanding) 
		{
			if (currentScale.x <= fTargetSize)
			{
				currentScale.x += fExpandRate * Mathf.Sqrt (fTargetSize - currentScale.x);
				currentScale.y += fExpandRate * Mathf.Sqrt (fTargetSize - currentScale.y);
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
			currentScale.x -= fExpandRate * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.x));
			currentScale.y -= fExpandRate * Mathf.Sqrt (Mathf.Abs (fTargetSize - currentScale.y));
		}
		else if (!bIsExpanding && 
		         bIsShrinking &&
		         currentScale.x < initialScale.x * Mathf.Sqrt (Mathf.Sqrt (Mathf.Sqrt (EnemyMainFSM.Instance ().Health)))) 
		{
			bIsShrinking = false;
		}

		transform.localScale = (Vector3)currentScale;
	}
}