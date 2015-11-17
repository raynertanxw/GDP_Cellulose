using UnityEngine;
using System.Collections;

public class Player_Control : MonoBehaviour
{
	public static int s_nResources;
	public int m_nSpawnCost = 10;

	[SerializeField]
	private GameObject m_playerCellPrefab;
	
	private Transform[] m_squadPoints;
	private SpriteRenderer[] m_squadPointsRen;
	private int m_nActiveSquad = 1;

	private Color m_unselectedSquadCol, m_selectedSquadCol;

	void Awake()
	{
		m_unselectedSquadCol = new Color(0, 1, 0.867f, 1);
		m_selectedSquadCol = new Color(0.75f, 1, 0.25f, 1);

		m_squadPoints = new Transform[transform.childCount];
		m_squadPointsRen = new SpriteRenderer[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
		{
			m_squadPoints[i] = transform.GetChild(i);
			m_squadPointsRen[i] = transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
		}
		m_squadPointsRen[m_nActiveSquad].color = m_selectedSquadCol;

		s_nResources = 100;
	}

    void Update()
    {

    }

	public void ChangeActiveSquad(int nNewSquad)
	{
		m_squadPointsRen[m_nActiveSquad].color = m_unselectedSquadCol;
		m_nActiveSquad = nNewSquad;
		m_squadPointsRen[m_nActiveSquad].color = m_selectedSquadCol;
		Debug.Log("Active Squad: " + m_nActiveSquad);
	}

	public void ActionSpawn()
	{
		if (s_nResources > m_nSpawnCost)
		{
			GameObject childCell = (GameObject) Instantiate(m_playerCellPrefab, m_squadPoints[m_nActiveSquad].position, Quaternion.identity);
			s_nResources -= m_nSpawnCost;
		}
		else
		{
			Debug.Log("Not enough resources");
		}
	}
}
