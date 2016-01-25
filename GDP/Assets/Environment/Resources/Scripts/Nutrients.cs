using UnityEngine;
using System.Collections;

// Resource.cs: The main controller for each resource node
public class Nutrients : MonoBehaviour
{
	#region Pool Control
	// singleton list to hold all out playerNutrientPoolControllers.
	static private int s_nPoolPointerIndex = 0;
	static private Nutrients[] s_playerNutrientPool;
	static public Nutrients[] playerNutrientPool { get { return s_playerNutrientPool; } }
	
	static public Nutrients Spawn(Vector3 spawnPoint)
	{
		for (int i = 0; i < Settings.s_nPlayerMaxNutrientCount; i++)
		{
			if (s_playerNutrientPool[i].bIsInPool == true)
			{
				s_playerNutrientPool[i].transform.position = spawnPoint;
				s_playerNutrientPool[i].endPosition = new Vector2 (-s_playerNutrientPool[i].transform.position.x, s_playerNutrientPool[i].transform.position.y + Random.Range (-5f, 5f) * s_playerNutrientPool[i].fMaximumOffset);
				s_playerNutrientPool[i].bIsCollectable = true;
				s_playerNutrientPool[i].spriteRen.enabled = true;
				s_playerNutrientPool[i].bIsInPool = false;

				return s_playerNutrientPool[i];
			}
		}
		
		// If we get here we haven't pooled enough.
		Debug.LogWarning("Exhausted Player Nutrient pool --> Increase pool size");
		
		return null;
	}
	#endregion

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
	private bool bIsCollectable = true;     // isCollected: Determines if the current resource can be collected by the player
	private bool bIsInPool = true;
	private Vector3 endPosition;
	private float fClickMagnitude;          // fClickMagnitude: The distance between the resource AT THE POINT OF CLICKING and the player's position

	// GameObject and Component References
	private Transform playerMainTransform;  // playerMainTransform: The transform of the player
	private SpriteRenderer spriteRen;


	// Private Functions
	// Awake(): is called before first frame
	void Awake()
	{
		// Does the pool exist yet
		if (s_playerNutrientPool == null)
		{
			// If not, lazy initiliase it
			s_playerNutrientPool = new Nutrients[Settings.s_nPlayerMaxNutrientCount];
			s_nPoolPointerIndex = 0;
			s_nNutrients = Settings.s_nPlayerNutrientPerBlock;
		}
		// Add myself
		s_playerNutrientPool[s_nPoolPointerIndex] = this;
		s_nPoolPointerIndex++;

		spriteRen = GetComponent<SpriteRenderer>();
		SendBackToPool();
	}

	// Start(): Use this for initialization
	void Start () 
	{
		// Definition of variables
		playerMainTransform = PlayerMain.Instance.transform;
	}
	
	// Update(): is called once per frame
	void Update () 
	{
		if (bIsInPool)
			return;

		// if: Checks if the resource is clicked
		if (bIsCollectable)
		{
			// Animation of player absorbing the resource
			Vector2 vectorToTarget = endPosition - transform.position;
			transform.position = Vector2.MoveTowards(transform.position, endPosition, (vectorToTarget.magnitude * fTimeTaken + fMinimumSpeed) * Time.deltaTime);
			transform.rotation = Quaternion.AngleAxis(vectorToTarget.magnitude * 60f, Vector3.forward);
			if (transform.position == endPosition)
				SendBackToPool();
		}
		else
		{
			// distanceMagnitude: The magnitude between the resource and the player's main cell's position
			float distanceMagnitude = (playerMainTransform.position - transform.position).magnitude;
			transform.position = Vector2.MoveTowards(transform.position, playerMainTransform.position, (distanceMagnitude * fTimeTaken + fMinimumSpeed) * 5.0f * Time.deltaTime);

			transform.localScale = Vector3.one * (distanceMagnitude / fClickMagnitude);

			if (distanceMagnitude < 0.1f)
				SendBackToPool();
		}
	}

	void SendBackToPool()
	{
		spriteRen.enabled = false;
		transform.localScale = Vector3.one;
		bIsInPool = true;
	}

	// OnMouseDown(): Detects when the object is clicked
	void OnMouseDown()
	{
		if (bIsCollectable)
		{
			player_control.Instance.s_nResources += s_nNutrients;
			player_control.Instance.UpdateUI_nutrients();
			fClickMagnitude = (transform.position - playerMainTransform.position).magnitude;
			bIsCollectable = false;
		}
	}

	// Public Static Functions
	public static int Nutrient { get { return s_nNutrients; } }

	// Getter-Setter Functions
	public bool IsInPool { get { return bIsInPool; } }
	public bool IsCollectable { get { return bIsCollectable; } }









	public static void ResetStatics()
	{
		s_playerNutrientPool = null;
	}
}
