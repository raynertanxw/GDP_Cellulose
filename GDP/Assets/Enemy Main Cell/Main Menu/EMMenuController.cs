using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (CircleCollider2D))]
[RequireComponent (typeof (EMMenuAnimation))]
public class EMMenuController : MonoBehaviour 
{
	private Rigidbody2D thisRB;
	private Camera mainCamera;

	float leftLimit;
	float rightLimit;
	float topLimit;
	float bottomLimit;

	Vector2 previousCameraPos;

	private float fRadius;
	private float width;

	[SerializeField]
	private Vector2 velocity;
	[SerializeField] 
	private bool bCanChangeDir;
	private float fChangeRate;
	
	void Start ()
	{
		thisRB = GetComponent<Rigidbody2D> ();
		mainCamera = Camera.main;

		width = GetComponent<CircleCollider2D> ().bounds.size.x / 2f;
		fRadius = GetComponent<CircleCollider2D> ().bounds.size.x;

		previousCameraPos = mainCamera.transform.position;

		bCanChangeDir = true;
		fChangeRate = Random.Range (3f, 6f);

		// Update camera border
		CameraLimit ();
		// Randomize initial position
		InitialPosition ();
	}

	void Update () 
	{
		// Update camera border
		//CameraLimit ();
		// Follow camera
		//FollowCamera ();
		// Update Radius
		if (fRadius != GetComponent<CircleCollider2D> ().bounds.size.x / 2f)
			fRadius = GetComponent<CircleCollider2D> ().bounds.size.x / 2f;
		// Update velocity
		if (thisRB.velocity != velocity)
			thisRB.velocity = velocity;
		// Limit the area that the enemy main cell can move
		PositionLimit ();
		// Randomize velocity if allowed
		if (bCanChangeDir)
			StartCoroutine (RandomizeVelocity ());
	}

	// Camera limit update
	void CameraLimit ()
	{
		leftLimit = Camera.main.ViewportToWorldPoint (Vector3.zero).x + fRadius;
		rightLimit = Camera.main.ViewportToWorldPoint (Vector3.right).x - fRadius;
		bottomLimit = Camera.main.ViewportToWorldPoint (Vector3.zero).y + fRadius;
		topLimit = Camera.main.ViewportToWorldPoint (Vector3.up).y - fRadius;
	}
	// Limit the area that the enemy main cell can move
	void PositionLimit ()
	{
		if (transform.position.x > rightLimit) {
			transform.position = new Vector2 (rightLimit, transform.position.y);
			if (velocity.x > 0f) {
				velocity.x *= -1f;
				thisRB.velocity = velocity;
			}
		} else if (transform.position.x < leftLimit) {
			transform.position = new Vector2 (leftLimit, transform.position.y);
			if (velocity.x < 0f) {
				velocity.x *= -1f;
				thisRB.velocity = velocity;
			}
		} else if (transform.position.y > topLimit) {
			transform.position = new Vector2 (transform.position.x, topLimit);
			if (velocity.y > 0f) {
				velocity.y *= -1f;
				thisRB.velocity = velocity;
			}
		} else if (transform.position.y < bottomLimit) {
			transform.position = new Vector2 (transform.position.x, bottomLimit);
			if (velocity.y < 0f) {
				velocity.y *= -1f;
				thisRB.velocity = velocity;
			}
		}
	}
	// Follow camera
	private void FollowCamera ()
	{
		//thisRB.AddForce (Vector2.one * 10f);

		if (mainCamera.transform.position.x != previousCameraPos.x || mainCamera.transform.position.y != previousCameraPos.y)
		{
			transform.position = new Vector2 (transform.position.x - (previousCameraPos.x - mainCamera.transform.position.x),
			                                  transform.position.y - (previousCameraPos.y - mainCamera.transform.position.y));

			previousCameraPos = mainCamera.transform.position;
		}
	}
	// Randomize initial position
	void InitialPosition ()
	{
		Vector2 position = new Vector2 (Random.Range (leftLimit, rightLimit), Random.Range (bottomLimit, topLimit));
		transform.position = position;
	}

	// Randomize velocity if allowed
	IEnumerator RandomizeVelocity ()
	{
		// Prohibit calls of this function
		bCanChangeDir = false;

		// Randomize velocity
		int bXPositive = Random.Range (0, 2);
		int bYPositive = Random.Range (0, 2);
		velocity.x = Random.Range (Settings.s_fEnemyMainMinHiriSpeed * 2f, Settings.s_fEnemyMainMinHiriSpeed * 5f);
		velocity.y = Random.Range (Settings.s_fEnemyMainMinHiriSpeed * 2f, Settings.s_fEnemyMainMinHiriSpeed * 5f);
		if (bXPositive == 0)
			velocity.x *= -1f;
		if (bYPositive == 0)
			velocity.y *= -1f;
		thisRB.velocity = velocity;

		// Randomize the changing rate 
		fChangeRate = Random.Range (3f, 6f);
		// Pause changing of direction for a few seconds
		yield return new WaitForSeconds (fChangeRate);
		// Allows changing of direction
		bCanChangeDir = true;
	}
}