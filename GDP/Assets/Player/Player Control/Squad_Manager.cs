using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad_Manager : MonoBehaviour
{
	private player_control m_playerCtrl;
    public int m_nSquadIndex;

	void Awake()
	{
		m_playerCtrl = transform.parent.GetComponent<player_control>();
    }

	void OnMouseDown()
	{
		m_playerCtrl.ChangeActiveSquad(m_nSquadIndex);
	}
}
