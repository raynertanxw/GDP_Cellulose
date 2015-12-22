using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
public class EMNutrientMiniController : MonoBehaviour
{
	private bool bCanFindPath;
	public bool CanFindPath { get { return bCanFindPath; } }

	Rigidbody2D thisRB;
	
	void Start () 
	{
		thisRB = GetComponent<Rigidbody2D> ();
		bCanFindPath = false;
		StartCoroutine (PauseAStar ());
	}

	void Update () 
	{
		if (!bCanFindPath)
			thisRB.velocity *= .99f;
	}

 	IEnumerator PauseAStar ()
	{
		// Make sure the A* pathfinding is not called until the initial movement comes to the end
		if (bCanFindPath)
			bCanFindPath = false;

		InitialMovement ();

		yield return new WaitForSeconds (Random.Range (3f, 5f));

		bCanFindPath = true;
	}

	void InitialMovement ()
	{
		thisRB.AddRelativeForce (Vector2.up * Random.Range (30f, 50f));
	}
}