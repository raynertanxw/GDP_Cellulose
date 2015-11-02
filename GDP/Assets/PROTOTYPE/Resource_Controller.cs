using UnityEngine;
using System.Collections;

public class Resource_Controller : MonoBehaviour
{
	private Vector3 endPosition;

	// Use this for initialization
	void Start () 
	{
		endPosition = -transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = Vector2.MoveTowards(transform.position, endPosition, Time.deltaTime);
		if (transform.position == endPosition)
			Destroy(this.gameObject);
	}

	void OnMouseDown()
	{
		player_control.resources += 10;
		Destroy(this.gameObject);
	}
}
