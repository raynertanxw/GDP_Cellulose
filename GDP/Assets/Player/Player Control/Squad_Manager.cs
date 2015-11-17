using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad_Manager : MonoBehaviour
{
	private player_control playerCtrl;
    public int squadIndex;

	void Awake()
	{
		playerCtrl = transform.parent.GetComponent<player_control>();
    }

	void OnMouseDown()
	{
		playerCtrl.ChangeActiveSquad(squadIndex);
	}
}
