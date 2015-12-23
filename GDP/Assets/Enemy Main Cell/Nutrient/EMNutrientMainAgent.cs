using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (CircleCollider2D))]
public class EMNutrientMainAgent : MonoBehaviour 
{
	// Size
	private int nSize;
	public int Size { get { return nSize; } }
	private Vector2 initialScale;
	private Vector2 currentScale;
	private float fInitialRadius;
	// Velocity
	public float fMaxVelocity = 1;
	public Vector2 currentVelocity;
	private bool bSucked;
	public bool Sucked { get { return bSucked; } }
	private GameObject suckedTarget;
	// Property
	public float fMass = 10;
	public float fFriction = .05f;
	public bool bRotating = true;
	public GameObject miniNutrient;
	private bool bCanSpawn;
	private Vector2 position;
	
	public static List<EMNutrientMainAgent> AgentList = new List<EMNutrientMainAgent>();
	
	List<EMNutrientMainFlock> behaviours = new List<EMNutrientMainFlock>();
	
	public void AddBehaviour(EMNutrientMainFlock behaviour)
	{
		behaviours.Add(behaviour);
	}
	
	public void RemoveBehaviour(EMNutrientMainFlock behaviour)
	{
		behaviours.Remove(behaviour);
	}

	void Awake()
	{
		// Initialiation
		nSize = 5;
		initialScale = gameObject.transform.localScale;
		transform.localScale = (Vector3)initialScale * Mathf.Sqrt(Mathf.Sqrt(nSize));
		fInitialRadius = GetComponent<CircleCollider2D> ().bounds.size.x / 2;

		bSucked = false;
		bCanSpawn = true;
		position = this.gameObject.transform.position;
	}

	void Start()
	{
		// Add the agent to the agent list
		AgentList.Add(this);
		// Initialize the position 
		InitialPosition ();
	}
	
	void Update()
	{
		// Remove destroyed from the list
		AgentList.RemoveAll(item => item == null);

		position = this.gameObject.transform.position;

		if ((Vector2)transform.localScale != initialScale * Mathf.Sqrt(Mathf.Sqrt(nSize)))
		    transform.localScale = (Vector3)initialScale * Mathf.Sqrt(Mathf.Sqrt(nSize));

		if (bSucked)
			Sucking ();
		else 
		{
			Vector2 acceleration = Vector2.zero;
			
			foreach (EMNutrientMainFlock behaviour in behaviours) {
				if (behaviour.enabled) {
					acceleration += behaviour.GetVelocity () * behaviour.FlockWeight;
					acceleration += behaviour.GetTargetVelocity () * behaviour.SeekWeight;
				}
			}
			
			currentVelocity += acceleration / fMass;
			
			currentVelocity -= currentVelocity * fFriction;
			
			if (currentVelocity.magnitude > fMaxVelocity)
				currentVelocity = currentVelocity.normalized * fMaxVelocity;
			
			transform.position = new Vector2 (transform.position.x + currentVelocity.x * Time.deltaTime, transform.position.y + currentVelocity.y * Time.deltaTime);

			if (bRotating && currentVelocity.magnitude > 0.0001f) {
				float angle = Mathf.Atan2 (currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
				
				transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, angle);
			}
		}

		// Instantiate mini nutrient
		if (bCanSpawn && MapManager.instance.IsInBounds ((Vector2)(position * 1.5f))) 
		{
			StartCoroutine (PauseSpawn ());
		}
	}

	void Sucking ()
	{
		transform.Translate (suckedTarget.transform.position * Time.deltaTime * .5f);
	}

	void InitialPosition ()
	{
		for (int i = 0; i < AgentList.Count; i++)
		{
			// Initialize the position of the nutrient
			AgentList[i].gameObject.transform.position = new Vector3 (Random.Range(fInitialRadius * -10f, fInitialRadius * 10f), 
			                                                          Random.Range(fInitialRadius * -10f, fInitialRadius * 10f));
			// Change the position of the nutrient if it is too close to the preceding one
			if (i != 0)
			{
				while (true)
				{
					// Check for all preceding nutrient cells
					for (int j = 0; j < i; j++)
					{
						// Only proceed to break the while loop of initializing position when the nutrient is not too close with any preceding nutrient
						if (Vector3.Distance (AgentList[i-1].gameObject.transform.position, AgentList[i].gameObject.transform.position) <=
						    fInitialRadius * 5f)
						{
							AgentList[i].gameObject.transform.position = new Vector3 (Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
							break;
						}
					}
					break;
				}
			}
		}
	}

	IEnumerator PauseSpawn ()
	{
		bCanSpawn = false;
		yield return new WaitForSeconds (Random.Range (Mathf.Sqrt (Mathf.Pow (nSize, 3f)), Mathf.Pow (nSize, 2f)));
		if (MapManager.instance.IsInBounds ((Vector2)(position * 1.5f)))
			Instantiate (miniNutrient, position, Quaternion.identity);
		bCanSpawn = true;
	}

	void OnTriggerEnter2D (Collider2D collision)
	{
		// Assign one nutrient to be sucked during collision
		if (collision.gameObject.tag == Constants.s_strEnemyMainNutrient && !collision.GetComponent<EMNutrientMainAgent>().Sucked) 
		{
			if (collision.gameObject.transform.position.y > position.y)
			{
				suckedTarget = collision.gameObject;
				bSucked = true;
				// Unregister the sucked nutrient from the list
				AgentList.Remove(this);
			}
		}
	}

	void OnTriggerStay2D (Collider2D collision)
	{
		// Check collisions with other enemy nutrient
		if (collision.gameObject.tag == Constants.s_strEnemyMainNutrient)
		{
			// Only one of the two nutrient on collision can perform actions
			if (!bSucked)
			{
				if (Vector2.Distance (transform.position, collision.gameObject.transform.position) < 0.1f)
				{
					// Add the size of the nutrient destroyed to the one stays
					nSize += collision.gameObject.GetComponent<EMNutrientMainAgent>().Size;
					Destroy(collision.gameObject);
				}
			}
		}
	}

	void OnTriggerExit2D (Collider2D collision)
	{
		bSucked = false;
		// Assign the nutrient back to the list
		AgentList.Add(this);
	}

	void OnDestroy()
	{
		AgentList.Remove(this);
	}
}