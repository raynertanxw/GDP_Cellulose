using UnityEngine;
using System.Collections;

public class EMController : MonoBehaviour 
{
	public GameObject FSM;
	private EnemyMainFSM m_EMFSM;

	public float fSpeed;
	public float fSpeedFactor;
	public float fSpeedTemp;
	private Vector2 velocity;

	public int nDamageNum;
	public bool bPushed;
	public bool bStunned;
	private bool bCanStun;

	// Size
	public int nSize;
	private Vector2 initialScale;
	private Vector2 currentScale;

	private Rigidbody2D thisRB;

	void Start()
	{
		// GetComponent
		m_EMFSM = FSM.GetComponent<EnemyMainFSM> ();
		thisRB = GetComponent<Rigidbody2D> ();

		// Speed
		fSpeed = .25f;
		fSpeedFactor = 1f;
		fSpeedTemp = fSpeed;
		// Velocity
		velocity = new Vector2 (0, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
		// Damage
		nDamageNum = 0;
		// State
		bPushed = false;
		bStunned = false;
		bCanStun = true;
		// Size
		nSize = 50;
		initialScale = gameObject.transform.localScale;
		currentScale = initialScale * Mathf.Sqrt(nSize);
	}

	void Update()
	{
		// Force back the enemy main cell when received damage and not forced back
		if (nDamageNum > 0 && !bPushed) 
		{
			StartCoroutine(ForceBack());
		}

		// Stun the enemy main cell when received certain amount of hits, can be stunned but not stunned
		if (nDamageNum > 5 && !bStunned && bCanStun) 
		{
			StartCoroutine(Stun ());
		}

		if (thisRB.velocity.y < fSpeed * fSpeedFactor && !bStunned) 
		{

		}

		// Check size
		if (currentScale != initialScale * Mathf.Sqrt(nSize)) 
		{
			currentScale = initialScale * Mathf.Sqrt(nSize);
		}
	}

	// Push back the enemy main cell when received attack
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

	// Auto regain velocity
	void RegainVelocity ()
	{
		velocity = new Vector2(0, fSpeed * fSpeedFactor);
		thisRB.velocity = velocity;
		fSpeedTemp = fSpeed;
	}

	// Stun the enemy main cell after receiving certain amount of hits within certain period of time
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