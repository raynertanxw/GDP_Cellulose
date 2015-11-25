using UnityEngine;
using System.Collections;

public class EMHelper : MonoBehaviour 
{
	public static float leftLimit;
	public static float rightLimit;
	public static float topLimit;
	public static float bottomLimit;

	private float width;

	public bool bCanAddDefend;

	private EMTransition transition;
	private EMController controller;

	void Start () 
	{
		width = GetComponent<CircleCollider2D> ().bounds.size.x;

		bCanAddDefend = true;
	}
	
	void Update () 
	{
		CameraLimit ();
		PositionLimit ();
		widthUpdate ();
	}

	public IEnumerator PauseAddDefend (float fTime)
	{
		bCanAddDefend = false;
		yield return new WaitForSeconds (fTime);
		bCanAddDefend = true;
	}

	// Camera limit update
	void CameraLimit ()
	{
		EMHelper.leftLimit = Camera.main.ViewportToWorldPoint (Vector3.zero).x;
		EMHelper.rightLimit = Camera.main.ViewportToWorldPoint (Vector3.right).x;
		EMHelper.bottomLimit = Camera.main.ViewportToWorldPoint (Vector3.zero).y;
		EMHelper.topLimit = Camera.main.ViewportToWorldPoint (Vector3.up).y;
	}

	void PositionLimit ()
	{
		if (transform.position.x <= EMHelper.leftLimit + width / 2 || transform.position.x >= EMHelper.rightLimit - width / 2)
			controller.ChangeDirection ();
	}

	void widthUpdate ()
	{
		if (width != GetComponent<CircleCollider2D> ().bounds.size.x)
			width = GetComponent<CircleCollider2D> ().bounds.size.x;
	}
}