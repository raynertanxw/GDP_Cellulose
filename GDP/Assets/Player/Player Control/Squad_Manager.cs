using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad_Manager : MonoBehaviour
{
	private Player_Control playerCtrl;
    public int squadIndex;

	void Awake()
	{
		playerCtrl = transform.parent.GetComponent<Player_Control>();
    }

	void OnMouseDown()
	{
		playerCtrl.ChangeActiveSquad(squadIndex);
	}
}
