using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public class PlayerMain : MonoBehaviour
{
	private static PlayerMain s_Instance;
	public static PlayerMain Instance { get { return s_Instance; } }
	private Animate mAnimate;
	public Animate animate { get { return mAnimate; } }

	private Vector3 m_Scale;
	private int m_nMaxHealth;
	private float m_fMinScale = 0.5f;
	private float m_fMaxScale = 1.25f;
	private float m_fShrinkSpeed = 1.0f;
	private bool m_bNeedsResizing = false;

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
			if (m_surroundingEnemyCells == null)
				return false;

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
		m_bNeedsResizing = false;
		m_nHealth = Settings.s_nPlayerInitialHealth;
		m_nMaxHealth = m_nHealth;
		mAnimate = new Animate(this.transform);
		m_surroundingEnemyCells = null;
		m_Scale = new Vector3(m_fMaxScale, m_fMaxScale, m_fMaxScale);
		transform.localScale = m_Scale;
	}

	void Start()
	{
		mAnimate.Idle(0.15f, 0.25f);
		mAnimate.IdleRotation(20f, 1080f, 1f, 10f, true, false);
	}

	void Update()
	{
		if (m_bIsAlive == false)
		{
			AnimateDie();
		}

		if (mAnimate.IsExpandContract == true)
		{
			m_bNeedsResizing = true;
		}
		else if (m_bNeedsResizing == true)
		{
			ResizeMainCell();
			m_bNeedsResizing = false;
		}
	}

	void FixedUpdate()
	{
		m_surroundingEnemyCells = Physics2D.OverlapCircleAll(transform.position, m_fDetectionRadius, Constants.s_onlyEnemeyChildLayer);
	}

	public void HurtPlayerMain()
	{
		m_nHealth--;
		player_control.Instance.FlashPlayerHurtTint();
		ResizeMainCell();

		if (m_nHealth <= 0)
		{
			m_bIsAlive = false;
		}
	}

	public void AnimateDie()
	{
		if (m_Scale.x == 0)
			return;

		m_Scale.x -= m_fShrinkSpeed * Time.deltaTime;
		m_Scale.y -= m_fShrinkSpeed * Time.deltaTime;
		m_Scale.z -= m_fShrinkSpeed * Time.deltaTime;

		if (m_Scale.x < 0)
		{
			m_Scale = Vector3.zero;
		}

		transform.localScale = m_Scale;
	}

	public void ResizeMainCell()
	{
		float fScale = m_fMinScale + ((m_fMaxScale - m_fMinScale) * ((float)m_nHealth / m_nMaxHealth));
		m_Scale.x = fScale;
		m_Scale.y = fScale;
		m_Scale.z = fScale;

		transform.localScale = m_Scale;
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
