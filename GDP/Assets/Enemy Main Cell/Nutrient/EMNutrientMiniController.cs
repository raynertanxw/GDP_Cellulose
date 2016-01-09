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
		fSpeed = Random.Range (.5f, 1f);
		// Not using A* by default
		bCanFindPath = false;
		// Call the PauseAStar function for initial movement
		StartCoroutine (PauseAStar ());
	}

	void Update () 
	{
		if (!bCanFindPath) 
		{
			thisRB.velocity *= .99f;
		}

		if (bCanFindPath) 
		{
			if (pathfindingManager.pathArray != null)
			{
				if (pathfindingManager.pathArray.Count > 1)
				{
					nextNode = (EnemyNutrientNode)pathfindingManager.pathArray[1];
					currentNode = nextNode;
				}
			}
			else 
				nextNode = null;

			if (currentNode != null)
				thisRB.velocity = (currentNode.position - (Vector2)this.gameObject.transform.position) * fSpeed;
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
		thisRB.AddForce ((EnemyMainFSM.Instance().Position - (Vector2)this.gameObject.transform.position) * Random.Range (5f, 10f));
	}

	void OnTriggerEnter2D (Collider2D collision)
	{
		EMController.Instance().AddNutrient ();
		Destroy (this.gameObject);
	}
}