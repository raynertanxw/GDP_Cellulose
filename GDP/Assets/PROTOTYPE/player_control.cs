using UnityEngine;
using System.Collections;

public class player_control : MonoBehaviour
{
	public static int resources;
	public int spawnCost = 10;

	[SerializeField]
	private GameObject playerCellPrefab;

	private Transform[] squadSpawnPoints;
	private int activeSquad = 1;

	void Awake()
	{
		squadSpawnPoints = new Transform[3];
		squadSpawnPoints[0] = transform.GetChild(0);
		squadSpawnPoints[1] = transform.GetChild(1);
		squadSpawnPoints[2] = transform.GetChild(2);

		resources = 100;
	}

	void OnMouseDown()
	{
		if (resources > spawnCost)
		{
			Instantiate(playerCellPrefab, squadSpawnPoints[activeSquad].position, Quaternion.identity);
			resources -= spawnCost;
		}
		else
		{
			Debug.Log("Not enough resources");
		}
	}

	public void ChangeActiveSquad(int newSquad)
	{
		activeSquad = newSquad;
		Debug.Log("Active Squad: " + activeSquad);
	}
}
