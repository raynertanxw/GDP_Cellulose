using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public class PlayerMain : MonoBehaviour
{
	private int m_nHealth = 100;
	public int Health { get { return m_nHealth; } }

	public float m_fDetectionRadius = 4.0f;

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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public int GetEnemyCountSurroundingPlayer()
	{
		Collider2D[] surroudingEnemyChildren = Physics2D.OverlapCircleAll(transform.position, m_fDetectionRadius, Constants.s_onlyEnemeyChildLayer);
		return surroudingEnemyChildren.Length;
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
}
