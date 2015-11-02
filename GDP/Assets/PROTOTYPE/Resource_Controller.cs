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

	// Use this for initialization
	void Start () 
	{
		endPosition = new Vector2 (-transform.position.x, transform.position.y + Random.Range (-5f, 5f) * offsetInfluence);
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = Vector2.MoveTowards(transform.position, endPosition, ((endPosition - transform.position).magnitude * timeTaken + endingSpeed) * Time.deltaTime);
		if (transform.position == endPosition)
			Destroy(this.gameObject);
	}

	void OnMouseDown()
	{
		player_control.resources += 10;
		Destroy(this.gameObject);
	}
}
