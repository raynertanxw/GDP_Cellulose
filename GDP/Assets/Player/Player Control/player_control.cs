using UnityEngine;
using System.Collections;

public class player_control : MonoBehaviour
{
	public static int resources;
	public int spawnCost = 10;

	[SerializeField]
	private GameObject playerCellPrefab;
	
	private Transform[] squadPoints;
	private SpriteRenderer[] squadPointsRen;
	private int activeSquad = 1;

	private Color unselectedSquadCol, selectedSquadCol;

	void Awake()
	{
		unselectedSquadCol = new Color(0, 1, 0.867f, 1);
		selectedSquadCol = new Color(0.75f, 1, 0.25f, 1);

		squadPoints = new Transform[transform.childCount];
		squadPointsRen = new SpriteRenderer[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
		{
			squadPoints[i] = transform.GetChild(i);
			squadPointsRen[i] = transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
		}
		squadPointsRen[activeSquad].color = selectedSquadCol;

		resources = 100;
	}

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space) && squadPoints[activeSquad].GetComponent<PROTOTYPE_Squad_Manager>().Empty() == false)
        {
			squadPoints[activeSquad].GetComponent<PROTOTYPE_Squad_Manager>().CommandAttack();
        }
    }

	void OnMouseDown()
	{
		if (resources > spawnCost)
		{
			GameObject childCell = (GameObject) Instantiate(playerCellPrefab, squadPoints[activeSquad].position, Quaternion.identity);
			squadPoints[activeSquad].gameObject.GetComponent<PROTOTYPE_Squad_Manager>().AddChild(childCell.GetComponent<PROTOTYPE_ChildController>());
			childCell.GetComponent<PROTOTYPE_ChildController>().setSquadManager(squadPoints[activeSquad].gameObject.GetComponent<PROTOTYPE_Squad_Manager>());
            resources -= spawnCost;
		}
		else
		{
			Debug.Log("Not enough resources");
		}
	}

	public void ChangeActiveSquad(int newSquad)
	{
		squadPointsRen[activeSquad].color = unselectedSquadCol;
		activeSquad = newSquad;
		squadPointsRen[activeSquad].color = selectedSquadCol;
		Debug.Log("Active Squad: " + activeSquad);
	}
}
