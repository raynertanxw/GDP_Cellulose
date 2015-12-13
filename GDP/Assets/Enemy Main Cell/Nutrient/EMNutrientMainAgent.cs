using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EMNutrientMainAgent : MonoBehaviour 
{
	// Size
	private int nSize;
	public int Size { get { return nSize; } }
	private Vector2 initialScale;
	private Vector2 currentScale;
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

		bSucked = false;
	}

	void Start()
	{
		// Add the agent to the agent list
		AgentList.Add(this);
	}
	
	void Update()
	{
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
	}

	void Sucking ()
	{
		transform.Translate (suckedTarget.transform.position * Time.deltaTime * .5f);
	}

	void OnTriggerEnter2D (Collider2D collision)
	{
		// Assign one nutrient to be sucked during collision
		if (collision.gameObject.layer == LayerMask.NameToLayer ("EnemyNutrient") && !collision.GetComponent<EMNutrientMainAgent>().Sucked) 
		{
			if (collision.gameObject.transform.position.y > this.gameObject.transform.position.y)
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
		if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyNutrient")) 
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