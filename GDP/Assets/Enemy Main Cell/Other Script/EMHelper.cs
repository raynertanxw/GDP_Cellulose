using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class EMHelper : MonoBehaviour 
{
	public static float leftLimit;
	public static float rightLimit;
	public static float topLimit;
	public static float bottomLimit;

	private float width;

	private bool bCanAddDefend;
	public bool CanAddDefend { get { return bCanAddDefend; } set { bCanAddDefend = true; } }
	private bool bCanAddAttack;
	public bool CanAddAttack { get { return bCanAddAttack; } set { bCanAddAttack = true; } }

	private EMTransition transition;
	private EMController controller;

	void Start () 
	{
		width = GetComponent<BoxCollider2D> ().bounds.size.x;

		bCanAddDefend = true;
	}
	
	void Update () 
	{
        // Update camera border
		CameraLimit ();
        // Make sure the enemy main cell do not go outside the screen
		PositionLimit ();
        // Update width of enemy main cell
        widthUpdate();
	}

	public IEnumerator PauseAddDefend (float fTime)
	{
		bCanAddDefend = false;
		yield return new WaitForSeconds (fTime);
		bCanAddDefend = true;
	}

	public IEnumerator PauseAddAttack (float fTime)
	{
		bCanAddAttack = false;
		yield return new WaitForSeconds (fTime);
		bCanAddAttack = true;
	}

	// Camera limit update
	void CameraLimit ()
	{
		EMHelper.leftLimit = Camera.main.ViewportToWorldPoint (Vector3.zero).x;
		EMHelper.rightLimit = Camera.main.ViewportToWorldPoint (Vector3.right).x;
		EMHelper.bottomLimit = Camera.main.ViewportToWorldPoint (Vector3.zero).y;
		EMHelper.topLimit = Camera.main.ViewportToWorldPoint (Vector3.up).y;
	}
    // Make sure the enemy main cell do not go outside the screen
    void PositionLimit ()
	{
		if (transform.position.x <= EMHelper.leftLimit + width / 2 || transform.position.x >= EMHelper.rightLimit - width / 2)
			controller.ChangeDirection ();
	}
    // Update width of enemy main cell
	void widthUpdate ()
	{
		if (width != GetComponent<BoxCollider2D> ().bounds.size.x)
			width = GetComponent<BoxCollider2D> ().bounds.size.x;
	}

	#region Math
	public float Pow (float value, float power)
	{
		return Mathf.Pow (value, power);
	}

	public float Sqrt (float value)
	{
		return Mathf.Sqrt (value);
	}

	public float Abs (float value)
	{
		return Mathf.Abs (value);
	}
	#endregion
}