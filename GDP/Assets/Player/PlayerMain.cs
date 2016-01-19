using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public class PlayerMain : MonoBehaviour
{
	[SerializeField]
	private int m_nHealth = 100;
	public int Health { get { return m_nHealth; } }

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

	public float m_fDetectionRadius = 5.0f;

	public static PlayerMain s_Instance;

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
	}

	void FixedUpdate()
	{
		m_surroundingEnemyCells = Physics2D.OverlapCircleAll(transform.position, m_fDetectionRadius, Constants.s_onlyEnemeyChildLayer);
	}

	public void HurtPlayerMain()
	{
		m_nHealth--;
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
}
