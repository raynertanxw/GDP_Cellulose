using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad_Manager : MonoBehaviour
{
	private Player_Control m_playerCtrl;
    public int m_nSquadIndex;

	void Awake()
	{
		m_playerCtrl = transform.parent.GetComponent<Player_Control>();
    }

	void OnMouseDown()
	{
		m_playerCtrl.ChangeActiveSquad(m_nSquadIndex);
	}
}
