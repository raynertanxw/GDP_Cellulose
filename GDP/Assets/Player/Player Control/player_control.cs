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

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space) && squadSpawnPoints[activeSquad].GetComponent<PROTOTYPE_Squad_Manager>().Empty() == false)
        {
			squadSpawnPoints[activeSquad].GetComponent<PROTOTYPE_Squad_Manager>().CommandAttack();
        }
    }

	void OnMouseDown()
	{
		if (resources > spawnCost)
		{
			GameObject childCell = (GameObject) Instantiate(playerCellPrefab, squadSpawnPoints[activeSquad].position, Quaternion.identity);
			squadSpawnPoints[activeSquad].gameObject.GetComponent<PROTOTYPE_Squad_Manager>().AddChild(childCell.GetComponent<PROTOTYPE_ChildController>());
			childCell.GetComponent<PROTOTYPE_ChildController>().setSquadManager(squadSpawnPoints[activeSquad].gameObject.GetComponent<PROTOTYPE_Squad_Manager>());
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
