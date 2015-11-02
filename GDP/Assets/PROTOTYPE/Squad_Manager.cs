using UnityEngine;
using System.Collections;

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
