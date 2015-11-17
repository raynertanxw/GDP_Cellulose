using UnityEngine;
using System.Collections;

public class Resource_Controller : MonoBehaviour
{
	[Tooltip("The time taken for the resource to travel from the start to end IF THERE IS NO DECELERATION")]
	[SerializeField] private float timeTaken = 0.5f;
	[Tooltip("The ending speed a.k.a the minimum speed it can move")]
	[SerializeField] private float endingSpeed = 0.5f;	//endingSpeed: This to prevent the resource from moving to a complete hault when reaching the endng point
	[Tooltip("The amount of horizontal offset of the resources when instantiated (The greater the number, the higher the offset)")]
	[SerializeField] private float offsetInfluence = 0.5f;

	private Vector3 endPosition;
    private bool isCollectable = true;        // isCollected: Determines if the current resource can be collected by the player
    private float clickMagnitude;             // clickMagnitude: The distance between the resource and player when is clicked
    private Vector3 initialScale;             // <------- REMOVE THIS IN MAIN GAME, SPRITE SHOULD VECTOR.ONE SCALE

    private Transform playerMainTransform;

	// Use this for initialization
	void Start () 
	{
		endPosition = new Vector2 (-transform.position.x, transform.position.y + Random.Range (-5f, 5f) * offsetInfluence);
        playerMainTransform = GameObject.Find("Player_Cell").transform;
        initialScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (isCollectable)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPosition, ((endPosition - transform.position).magnitude * timeTaken + endingSpeed) * Time.deltaTime);
            if (transform.position == endPosition)
                Destroy(this.gameObject);
        }
        else
        {
            // distanceMagnitude: The magnitude between the resource and the player's main cell's position
            float distanceMagnitude = (playerMainTransform.position - transform.position).magnitude;
            transform.position = Vector2.MoveTowards(transform.position, playerMainTransform.position, (distanceMagnitude * timeTaken + endingSpeed) * 5.0f * Time.deltaTime);

            transform.localScale = initialScale * (distanceMagnitude / clickMagnitude);

            if (distanceMagnitude < 0.1f)
                Destroy(this.gameObject);
        }
	}

	void OnMouseDown()
	{
        if (isCollectable)
        {
            Player_Control.s_nResources += 10;
            clickMagnitude = (transform.position - playerMainTransform.position).magnitude;
            isCollectable = false;
        }
	}
}
