using UnityEngine;
using System.Collections;

public class Misc_UI : MonoBehaviour
{
	private static Misc_UI s_miscUI = null;
	public static Misc_UI Instance { get { return s_miscUI; } }
	private SpriteRenderer EnemyStunnedTextSpriteRen;
	private float EnemyStunnedTextOffsetY;
	private float stunDisplayDuration;

	private Animate m_EnemyStunnedTextAnimate;

	void Awake()
	{
		if (s_miscUI == null)
			s_miscUI = this;
		else
			Destroy(this.gameObject);

		EnemyStunnedTextSpriteRen = transform.GetChild(0).GetComponent<SpriteRenderer>();
		
		EnemyStunnedTextSpriteRen.enabled = false;
		stunDisplayDuration = 0f;

		m_EnemyStunnedTextAnimate = new Animate(EnemyStunnedTextSpriteRen.transform);
	}
	
	void Update()
	{
		if (stunDisplayDuration > 0f)
		{
			stunDisplayDuration -= Time.deltaTime;
			EnemyStunnedTextOffsetY = 1.0f + EnemyMainFSM.Instance().Health / 100.0f;

			Vector3 textPos = EnemyMainFSM.Instance().transform.position;
			textPos.y -= EnemyStunnedTextOffsetY;
			EnemyStunnedTextSpriteRen.transform.position = textPos; 
		}
		else if (EnemyStunnedTextSpriteRen.enabled == true)
		{
			m_EnemyStunnedTextAnimate.StopExpandContract(false);
			EnemyStunnedTextSpriteRen.enabled = false;
		}
	}

	public void DisplayEnemyStun(float _stunDuration)
	{
		stunDisplayDuration = _stunDuration;
		EnemyStunnedTextSpriteRen.enabled = true;
		m_EnemyStunnedTextAnimate.ExpandContract(500.0f, 1000, 1.25f, true, 0f);
	}

	void OnDestroy()
	{
		s_miscUI = null;
	}
}
