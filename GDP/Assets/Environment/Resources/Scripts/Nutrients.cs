using UnityEngine;
using System.Collections;

// Resource.cs: The main controller for each resource node
public class Nutrients : MonoBehaviour
{
	// Static Variables
	private static int s_nNutrients = 10;   // s_nNutrients: The default amount of nutrients of each nutrients node

	// Editable Variables
	[Tooltip("The time taken for the resource to travel from the start to end IF THERE IS NO DECELERATION")]
	[SerializeField] private float fTimeTaken = 0.5f;
	[Tooltip("The ending speed a.k.a the minimum speed it can move")]
	[SerializeField] private float fMinimumSpeed = 0.5f;	// fMinimumSpeed: This to prevent the resource from moving to a complete hault when reaching the endng point
	[Tooltip("The amount of horizontal offset of the resources when instantiated (The greater the number, the higher the offset)")]
	[SerializeField] private float fMaximumOffset = 0.5f;

	// Uneditable Variables
	private Vector3 endPosition;
	private bool bIsCollectable = true;     // isCollected: Determines if the current resource can be collected by the player
	private float fClickMagnitude;          // fClickMagnitude: The distance between the resource AT THE POINT OF CLICKING and the player's position

	// GameObject and Component References
	private Transform playerMainTransform;  // playerMainTransform: The transform of the player

	// Private Functions
	// Start(): Use this for initialization
	void Start () 
	{
		// Definition of variables
		endPosition = new Vector2 (-transform.position.x, transform.position.y + Random.Range (-5f, 5f) * fMaximumOffset);
		playerMainTransform = GameObject.Find("Player_Cell").transform;
	}
	
	// Update(): is called once per frame
	void Update () 
	{
		// if: Checks if the resource is clicked
		if (bIsCollectable)
		{
			// Animation of player absorbing the resource
			Vector2 vectorToTarget = endPosition - transform.position;
			transform.position = Vector2.MoveTowards(transform.position, endPosition, (vectorToTarget.magnitude * fTimeTaken + fMinimumSpeed) * Time.deltaTime);
			transform.rotation = Quaternion.AngleAxis(vectorToTarget.magnitude * 60f, Vector3.forward);
			if (transform.position == endPosition)
				Destroy(this.gameObject);
		}
		else
		{
			// distanceMagnitude: The magnitude between the resource and the player's main cell's position
			float distanceMagnitude = (playerMainTransform.position - transform.position).magnitude;
			transform.position = Vector2.MoveTowards(transform.position, playerMainTransform.position, (distanceMagnitude * fTimeTaken + fMinimumSpeed) * 5.0f * Time.deltaTime);

			transform.localScale = Vector3.one * (distanceMagnitude / fClickMagnitude);

			if (distanceMagnitude < 0.1f)
				Destroy(this.gameObject);
		}
	}

	// OnMouseDown(): Detects when the object is clicked
	void OnMouseDown()
	{
		if (bIsCollectable)
		{
			player_control.s_nResources += s_nNutrients;
			fClickMagnitude = (transform.position - playerMainTransform.position).magnitude;
			bIsCollectable = false;
		}
	}

	// Getter-Setter Functions
	public static int Nutrient { get { return s_nNutrients; } }
}
