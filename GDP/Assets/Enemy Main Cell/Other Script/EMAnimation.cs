using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EnemyMainFSM))]
[RequireComponent (typeof (Rigidbody2D))]
public class EMAnimation : MonoBehaviour 
{
	private Rigidbody2D thisRB;

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
	}

	void Update () 
	{
		SizeUpdate ();
	}

	void FixedUpdate ()
	{
		RotationUpdate ();
	}

	// Update the size of enemy main cell according to current health
	void SizeUpdate ()
	{
		if (currentScale != initialScale * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().Health))))
		{
			currentScale = initialScale * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(EnemyMainFSM.Instance().Health)));
			transform.localScale = (Vector3)currentScale;
		}
	}
	// Update angular velocity of the enemy main cell
	void RotationUpdate ()
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
}