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

		bIsExpanding = false;
		bIsShrinking = false;
		fTargetSize = 0f;
	}

	void Update () 
	{
		SizeUpdate ();
	}

	void FixedUpdate ()
	{
		RotationUpdate ();
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
	// Update angular velocity of the enemy main cell
	private void RotationUpdate ()
	{
		// Angular velocity declines as time goes by
		if (fAngularVelocity >= 0f) {
			if (fAngularVelocity >= fMinAngularVelocity)
				fAngularVelocity -= fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity));
		} else if (fAngularVelocity < 0f) {
			if (fAngularVelocity <= -fMinAngularVelocity)
				fAngularVelocity += fAngularDeclineFactor * Mathf.Sqrt (Mathf.Abs (fAngularVelocity));
		}

		// Make sure the angular velocity is not less than the minimum value
		if (Mathf.Abs (fAngularVelocity) < fMinAngularVelocity) 
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
	// 
	public void ExpandAnimation ()
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