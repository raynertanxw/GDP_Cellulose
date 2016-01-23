using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public class PlayerMain : MonoBehaviour
{
	public static PlayerMain s_Instance;
	private Animate mAnimate;
	public Animate animate { get { return mAnimate; } }

	[SerializeField]
	private int m_nHealth = 100;
	public int Health { get { return m_nHealth; } }
	private bool m_bIsAlive = true;
	public bool IsAlive { get { return m_bIsAlive; } }

	public float m_fDetectionRadius = 5.0f;
	private Collider2D[] m_surroundingEnemyCells;
	public Collider2D[] surroundingEnemyCells { get { return m_surroundingEnemyCells; } }
	public bool hasSurroundingEnemyCells
	{
		get
		{
			if (m_surroundingEnemyCells.Length > 0)
				return true;
			else
				return false;
		}
	}



	void Awake()
	{
		if (PlayerMain.s_Instance == null)
		{
			PlayerMain.s_Instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}

		m_bIsAlive = true;
		m_nHealth = Settings.s_nPlayerInitialHealth;
		mAnimate = new Animate(this.transform);
	}

	void Start()
	{
		mAnimate.Idle(0.15f, 0.25f);
	}

	void FixedUpdate()
	{
		m_surroundingEnemyCells = Physics2D.OverlapCircleAll(transform.position, m_fDetectionRadius, Constants.s_onlyEnemeyChildLayer);
	}

	public void HurtPlayerMain()
	{
		m_nHealth--;

		if (m_nHealth <= 0)
		{
			m_bIsAlive = false;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == Constants.s_strEnemyChildTag)
		{
			// Kill the child cell.
			col.gameObject.GetComponent<EnemyChildFSM>().KillChildCell();
			// Reduce health.
			HurtPlayerMain();
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, m_fDetectionRadius);
	}
	#endif
















	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
