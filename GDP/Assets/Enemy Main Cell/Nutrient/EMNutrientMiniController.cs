﻿using UnityEngine;
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
		fSpeed = Random.Range (.4f, .8f);
		// Not using A* by default
		bCanFindPath = false;
		// Call the PauseAStar function for initial movement
		StartCoroutine (PauseAStar ());
	}

	void Update () 
	{
		// Update the rotation of the mini nutrient
		RotationUpdate ();
		// Friction of initial movement
		if (!bCanFindPath) 
		{
			thisRB.velocity *= .99f;
		}
		// A* pathfinding
		if (bCanFindPath) 
		{
			// Set the target to the next node
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
			// Move to the target node
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

	void RotationUpdate ()
	{
		// Rotate the mini nutrient towards current velocity
		if (thisRB.velocity != Vector2.zero) 
		{
			float angle = Mathf.Atan2(thisRB.velocity.y, thisRB.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	void OnTriggerEnter2D (Collider2D collision)
	{
		EMController.Instance().AddNutrient ();
		Destroy (this.gameObject);
	}
}