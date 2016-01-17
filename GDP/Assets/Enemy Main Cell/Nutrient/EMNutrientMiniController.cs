using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (EMNutrientPathfindingManager))]
public class EMNutrientMiniController : MonoBehaviour
{
	private bool bCanFindPath;
	public bool CanFindPath { get { return bCanFindPath; } }

	Rigidbody2D thisRB;
	float fSpeed;
	// Absorb behaviour
	bool bIsAbsorbed;
	float fAbsorbSpeed;
	float fAbsorbTime;

	EMNutrientPathfindingManager pathfindingManager;
	EnemyNutrientNode currentNode;
	EnemyNutrientNode nextNode;
	
	void Start () 
	{
		// GetComponent on the gameObject
		thisRB = GetComponent<Rigidbody2D> ();
		pathfindingManager = GetComponent<EMNutrientPathfindingManager> ();
		// Initialization
		currentNode = null;
		nextNode = null;
		fSpeed = Random.Range (.4f, .8f);
		bIsAbsorbed = false;
		fAbsorbSpeed = Random.Range (1f, 2f);
		fAbsorbTime = 1f;
		// Not using A* by default
		bCanFindPath = false;
		// Call the PauseAStar function for initial movement
		StartCoroutine (PauseAStar ());
	}

	void Update () 
	{
		// Destroy the nutrient if enemy main cell does not exist
		if (EnemyMainFSM.Instance ().isActiveAndEnabled == false)
			Destroy (this.gameObject);
		// Absorb behaviour
		if (bIsAbsorbed)
			Absorb ();
		else 
		{
			// A* pathfinding
			if (bCanFindPath) {
				// Set the target to the next node
				if (pathfindingManager.pathArray != null) {
					if (pathfindingManager.pathArray.Count > 1) {
						nextNode = (EnemyNutrientNode)pathfindingManager.pathArray [1];
						currentNode = nextNode;
					}
				} else 
					nextNode = null;
				// Move to the target node
				if (currentNode != null)
					thisRB.velocity = (currentNode.position - (Vector2)this.gameObject.transform.position) * fSpeed;
			}
		}
	}

	void FixedUpdate ()
	{
		// Update the rotation of the mini nutrient
		RotationUpdate ();
		// Friction of initial movement
		if (!bCanFindPath) 
		{
			thisRB.velocity *= .99f;
		}
	}

 	IEnumerator PauseAStar ()
	{
		// Make sure the A* pathfinding is not called until the initial movement comes to the end
		if (bCanFindPath)
			bCanFindPath = false;

		InitialMovement ();

		yield return new WaitForSeconds (Random.Range (2f, 3f));

		bCanFindPath = true;
	}

	void InitialMovement ()
	{
		thisRB.AddForce ((EMHelper.Instance().Position - (Vector2)this.gameObject.transform.position) * Random.Range (5f, 10f));
	}

	void RotationUpdate ()
	{
		// Rotate the mini nutrient towards current velocity
		if (thisRB.velocity != Vector2.zero) 
		{
			float angle = Mathf.Atan2(thisRB.velocity.y, thisRB.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	void Absorb ()
	{
		Vector2 vectorToTarget = EMHelper.Instance().Position - (Vector2)transform.position;
		transform.position = Vector2.MoveTowards(transform.position, EMHelper.Instance().Position, (vectorToTarget.magnitude * fAbsorbTime + fAbsorbSpeed) * Time.deltaTime);
		transform.localScale = Vector3.one * 
			(Vector2.Distance ((Vector2)transform.position, EMHelper.Instance().Position)) / EMHelper.Instance ().Radius *
							   Random.Range (.5f, 1f);
		if (Vector2.Distance ((Vector2)transform.position, EMHelper.Instance().Position) < .1f || transform.localScale.x < .1f) 
		{
			EMController.Instance().AddNutrient ();
			if (!EMAnimation.Instance().IsExpanding)
				EMAnimation.Instance().IsExpanding = true;
			Destroy (this.gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D collision)
	{
		bIsAbsorbed = true;
	}
}