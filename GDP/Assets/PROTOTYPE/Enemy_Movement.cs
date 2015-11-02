using UnityEngine;
using System.Collections;

public class Enemy_Movement : MonoBehaviour
{
	private float speed = 0.1f;
	private Vector2 velocity;

	private Rigidbody2D thisRB;

	void Awake()
	{
		thisRB = GetComponent<Rigidbody2D>();
		changeSpeed(0f);
	}

	public void changeSpeed(float changeInSpeed)
	{
		speed += changeInSpeed;
		velocity = new Vector2(0, speed);
		thisRB.velocity = velocity;
	}
}
