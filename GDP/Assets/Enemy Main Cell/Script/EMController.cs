using UnityEngine;
using System.Collections;

public class EMController : MonoBehaviour
{
	public float fSpeed;
	public float fSpeedFactor;
	public float fSpeedTemp;
	private Vector2 velocity;

	public int nDamageNum;
	public bool bPushed;
	public bool bStunned;
	private bool bCanStun;

	private Rigidbody2D thisRB;

	void Start()
	{
		thisRB = GetComponent<Rigidbody2D> ();
		fSpeed = .25f;
		fSpeedFactor = 1f;
		fSpeedTemp = fSpeed;
		velocity = new Vector2 (0, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
		nDamageNum = 0;
		bPushed = false;
		bStunned = false;
		bCanStun = true;
	}

	void Update()
	{
		if (nDamageNum > 0 && !bPushed) 
		{
			StartCoroutine(ForceBack());
		}

		if (nDamageNum > 5 && !bStunned && bCanStun) 
		{
			StartCoroutine(Stun ());
		}

		if (thisRB.velocity.y < fSpeed * fSpeedFactor && !bStunned) 
		{

		}
	}

	IEnumerator ForceBack()
	{
		Vector2 velocityTemp = new Vector2 (0f, -velocity.y * 2.5f);
		thisRB.velocity = velocityTemp;
		bPushed = true;
		yield return new WaitForSeconds (.4f);
		
		nDamageNum--;
		if (!bStunned)
			RegainVelocity ();

		yield return new WaitForSeconds (.5f);
		bPushed = false;
	}

	void RegainVelocity ()
	{
		velocity = new Vector2(0, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
		fSpeedTemp = fSpeed;
	}

	IEnumerator Stun()
	{
		bStunned = true;
		bCanStun = false;
		yield return new WaitForSeconds (2f);
		bStunned = false;
		yield return new WaitForSeconds (3f);
		bCanStun = true;
	}

	public void ChangeSpeed(float changeInSpeed)
	{
		fSpeedTemp += changeInSpeed;
		velocity = new Vector2(0, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
	}
	
	public void ChangeSpeedFactor(float changeInSpeedF)
	{
		fSpeedFactor += changeInSpeedF;
		velocity = new Vector2(0, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
	}

	void OnTriggerEnter2D (Collider2D collision)
	{
		if (collision.gameObject.tag == "Player") 
		{
			nDamageNum++;
			Destroy (collision.gameObject);
		}
	}
}