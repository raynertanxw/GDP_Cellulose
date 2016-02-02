using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (CircleCollider2D))]
public class EnemyNutrientMainMenuAgent : MonoBehaviour
{
	private Camera mainCamera;
	Vector2 previousCameraPos;
	// Size
	[SerializeField]
	private int nSize;
	public int Size { get { return nSize; } }
	private Vector2 initialScale;
	private Vector2 currentScale;
	private float fInitialRadius;
	// Velocity
	public float fMaxVelocity = 1;
	public Vector2 currentVelocity;
	[SerializeField]
	private bool bSucked;
	public bool Sucked { get { return bSucked; } }
	private GameObject suckedTarget;
	// Property
	private float fMass;
	public float fFriction;
	public GameObject miniNutrient;
	private bool bCanSpawn;
	private Vector2 position;
	// List stores all activated agents
	public static List<EnemyNutrientMainMenuAgent> AgentList = new List<EnemyNutrientMainMenuAgent>();
	// List stores behavior of the agent
	List<EnemyNutrientMainMenuFlock> behaviours = new List<EnemyNutrientMainMenuFlock>();
	
	public void AddBehaviour(EnemyNutrientMainMenuFlock behaviour)
	{
		behaviours.Add(behaviour);
	}
	
	public void RemoveBehaviour(EnemyNutrientMainMenuFlock behaviour)
	{
		behaviours.Remove(behaviour);
	}
	
	public void ActivateOrDeactivate (bool state)
	{
		this.gameObject.SetActive (state);
	}
	
	void Awake()
	{
		// Initialiation
		mainCamera = Camera.main;
		previousCameraPos = mainCamera.transform.position;
		nSize = Random.Range (4, 9);
		fFriction = 0.02f;
		initialScale = gameObject.transform.localScale;
		transform.localScale = (Vector3)initialScale * Mathf.Sqrt(Mathf.Sqrt(nSize));
		fInitialRadius = GetComponent<CircleCollider2D> ().bounds.size.x / 2;
		
		bSucked = false;
		bCanSpawn = true;
		position = this.gameObject.transform.position;
		
		// Add the agent to the agent list
		if (AgentList != null)
			AgentList.Add(this);
	}
	
	void Start()
	{
		// Initialize the position 
		InitialPosition ();
	}
	
	void Update()
	{
		// Remove destroyed from the list
		if (AgentList != null)
			AgentList.RemoveAll(item => item == null);
		// Follow camera
		FollowCamera ();
		// Deactivate the nutrient if it is empty
		if (nSize == 0)
			ActivateOrDeactivate (false);
		// Different behavior depends on whether the agent is sucked
		if (bSucked)
			Sucking ();
		else 
		{
			// Reset acceleration
			Vector2 acceleration = Vector2.zero;
			// Modify acceleration based on the velocity returned by each behavior attached on the agent and its weight
			foreach (EnemyNutrientMainMenuFlock behaviour in behaviours) {
				if (behaviour.enabled) {
					acceleration += behaviour.GetVelocity () * behaviour.FlockWeight;
					acceleration += behaviour.GetTargetVelocity () * behaviour.SeekWeight;
				}
			}
			// Lower acceleratin for heavier agent
			if (fMass != 0f)
				currentVelocity += acceleration / Mathf.Sqrt (Mathf.Pow(fMass, 3));
			// Reduce based on the friction
			currentVelocity *= (1f - fFriction);
			
			if (currentVelocity.magnitude > fMaxVelocity)
				currentVelocity = currentVelocity.normalized * fMaxVelocity;
			
			if (currentVelocity.magnitude > 0f) {
				float angle = Mathf.Atan2 (currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
				
				transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, angle);
			}
			
			if (currentVelocity.magnitude > 0f)
				GetComponent<Rigidbody2D> ().velocity = currentVelocity;
		}
		
		/*
		// Instantiate mini nutrient
		if (bCanSpawn && MapManager.Instance.IsInBounds ((Vector2)(position * 1.5f)) && gameObject.activeSelf) 
		{
			StartCoroutine (PauseSpawn ());
		}
		*/
	}
	
	void FixedUpdate ()
	{
		// Update the current position of the agent
		position = this.gameObject.transform.position;
		// Update mass of the agent
		if (fMass != (float)nSize * 2f && nSize != 0)
			fMass = (float)nSize * 2f;
		if (fMass == 0f)
			fMass = 4f;
		// Update localScale of the agent
		if ((Vector2)transform.localScale != initialScale * Mathf.Sqrt(Mathf.Sqrt(nSize)))
			transform.localScale = (Vector3)initialScale * Mathf.Sqrt(Mathf.Sqrt(nSize));
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
	
	// Sucking behavior
	void Sucking ()
	{
		transform.Translate (suckedTarget.transform.position * Time.deltaTime * .5f);
	}
	// Initialize the position of all agents so that all agents keep a fair distance from others
	void InitialPosition ()
	{
		for (int i = 0; i < AgentList.Count; i++)
		{
			// Initialize the position of the nutrient
			AgentList[i].gameObject.transform.position = new Vector3 (Random.Range(fInitialRadius * -15f, fInitialRadius * 15f), 
			                                                          Random.Range(fInitialRadius * -15f, fInitialRadius * 15f));
			// Change the position of the nutrient if it is too close to the preceding one
			if (i != 0)
			{
				while (true)
				{
					// Check for all preceding nutrient cells
					bool bNotTooClose = true; 
					
					for (int j = 0; j < i; j++)
					{
						// Only proceed to break the while loop of initializing position when the nutrient is not too close with any preceding nutrient
						if (Vector3.Distance (AgentList[j].gameObject.transform.position, AgentList[i].gameObject.transform.position) <=
						    fInitialRadius * 6f)
						{
							AgentList[i].gameObject.transform.position = new Vector3 (Random.Range(fInitialRadius * -15f, fInitialRadius * 15f), 
							                                                          Random.Range(fInitialRadius * -15f, fInitialRadius * 15f));
							bNotTooClose = false;
							break;
						}
					}
					// If checked all preceding agents are not too close, then exit the loop
					if (bNotTooClose)
						break;
				}
			}
		}
	}
	// Pause the spawning process so that there are not too many mini cells taking too much computing power at the same time
	IEnumerator PauseSpawn ()
	{
		bCanSpawn = false;
		// Pause for random amount of time depending on the size of the agent
		yield return new WaitForSeconds (Random.Range (Mathf.Sqrt (Mathf.Pow (nSize, 1f)), Mathf.Sqrt (Mathf.Pow (nSize, 3f))));
		// Double check if the main nutrient is in the map and not too close to the enemy main cell
		if (MapManager.Instance.IsInBounds ((Vector2)(position * 1.1f)) && 
		    Vector2.Distance (EMHelper.Instance().Position, transform.position) > fInitialRadius * nSize + EMHelper.Instance ().Radius)
		{
			// Instantiate a mini nutrient object
			Instantiate (miniNutrient, position, Quaternion.identity);
			// Calling the Animate class for spawn animation
			Animate mAnimate;
			mAnimate = new Animate (this.transform);
			mAnimate.ExpandContract (0.1f, 1, 1.1f);
			// Reduce the size of the current main nutrient by 1
			nSize--;
		}
		bCanSpawn = true;
	}
	
	void OnDestroy()
	{
		// Remove the agent from the list if it is destroyed
		if (AgentList != null)
			AgentList.Remove(this);
	}
}
