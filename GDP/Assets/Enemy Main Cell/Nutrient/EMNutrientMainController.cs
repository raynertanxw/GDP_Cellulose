using UnityEngine;
using System.Collections;

public class EMNutrientMainController : MonoBehaviour 
{
	// Size
	private int nSize;
	private Vector2 initialScale;
	private Vector2 currentScale;
	// Speed
	float fInitialSpeed;
	float fCurrentSpeed;
	private Vector2 velocity;
	bool bCanMove;

	private Rigidbody2D thisRB;

	void Start ()
	{
		// GetComponent
		thisRB = GetComponent<Rigidbody2D> ();

		// Size
		nSize = 20;
		initialScale = gameObject.transform.localScale;
		currentScale = initialScale * Mathf.Sqrt(nSize);
		// Speed
		fInitialSpeed = 2f;
		fCurrentSpeed = fInitialSpeed / Mathf.Sqrt(nSize);
		bCanMove = true;
		#region initialize direction
		int nLorR = Random.Range (1, 3);
		if (nLorR == 1)
			velocity.x = -fCurrentSpeed;
		else
			velocity.x = fCurrentSpeed;
		#endregion
	}

	void Update ()
	{
		CameraLimit ();

		if (bCanMove)
			StartCoroutine (Move ());
	}

	IEnumerator Move ()
	{
		// Move the resource main for 2 seconds
		fCurrentSpeed = fInitialSpeed / Mathf.Sqrt(nSize);
		velocity = new Vector2(fCurrentSpeed, 0f);
		thisRB.velocity = velocity;
		bCanMove = false;
		yield return new WaitForSeconds (2f);
		// Pause movement for 2 seconds
		velocity.x = 0f;
		thisRB.velocity = velocity;
		yield return new WaitForSeconds (2f);
		// Set move to available again
		bCanMove = true;
	}

	void CameraLimit ()
	{
		if (transform.position.x <= EMHelper.leftLimit || transform.position.x >= EMHelper.rightLimit) 
		{
			float fNewVel = -velocity.x;
			velocity.x = fNewVel;
		}
	}
}