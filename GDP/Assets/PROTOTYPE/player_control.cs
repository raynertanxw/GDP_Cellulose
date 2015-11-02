using UnityEngine;
using System.Collections;

public class player_control : MonoBehaviour
{
	[SerializeField]
	private GameObject playerCellPrefab;

	void OnMouseDown()
	{
		Debug.Log("Touched Player");

	}
}
