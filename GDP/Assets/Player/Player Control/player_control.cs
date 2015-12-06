using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class player_control : MonoBehaviour
{
	public static int s_nResources;
	public int m_nSpawnCost = 10;

	private SpriteRenderer[] m_nodePointsRen;
	private int m_nActiveNode = 1;

	private Color m_unselectedNodeCol, m_selectedNodeCol;

	void Awake()
	{
		m_unselectedNodeCol = new Color(0, 1, 0.867f, 1);
		m_selectedNodeCol = new Color(0.75f, 1, 0.25f, 1);

		m_nodePointsRen = new SpriteRenderer[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
		{
			m_nodePointsRen[i] = Node_Manager.GetNode(i).gameObject.GetComponent<SpriteRenderer>();
		}
		m_nodePointsRen[m_nActiveNode].color = m_selectedNodeCol;

		s_nResources = 100;
	}

	public void ChangeActiveNode(int nNewNode)
	{
		m_nodePointsRen[m_nActiveNode].color = m_unselectedNodeCol;
		m_nActiveNode = nNewNode;
		m_nodePointsRen[m_nActiveNode].color = m_selectedNodeCol;
		Debug.Log("Active Node: " + m_nActiveNode);
	}

	
	#region Actions for UI Buttons to call
	public void ActionSpawn()
	{
		if (s_nResources > m_nSpawnCost)
		{
			// Call a child cell from object pool and set its m_assignedNode to assigned node.
			PlayerChildFSM currentChild = PlayerChildFSM.Spawn(Node_Manager.GetNode(m_nActiveNode).transform.position + (Vector3)Random.insideUnitCircle*0.5f);
			currentChild.m_assignedNode = Node_Manager.GetNode(m_nActiveNode);
			s_nResources -= m_nSpawnCost;
		}
		else
		{
			Debug.Log("Not enough resources");
		}
	}

	public void ActionDefend()
	{
		Debug.Log("Defend Action called");

		List<PlayerChildFSM> listOfChildCells = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();
		for (int i = 0; i < listOfChildCells.Count; i++)
		{
			listOfChildCells[i].m_bIsDefending = true;
			if (listOfChildCells[i].GetCurrentState() != PCState.Idle)
				listOfChildCells[i].DeferredChangeState(PCState.Idle); // Will auto change to defend if deteced.
		}
	}

	public void ActionAvoid()
	{
		Debug.Log("Avoid Action called");

		List<PlayerChildFSM> listOfChildCells = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();
		for (int i = 0; i < listOfChildCells.Count; i++)
		{
			listOfChildCells[i].m_bIsDefending = false;
			if (listOfChildCells[i].GetCurrentState() != PCState.Idle)
				listOfChildCells[i].DeferredChangeState(PCState.Idle); // Will auto change to avoid if detected.
		}
	}

	public void ActionChargeMain()
	{
		Debug.Log("Charge Main Action Called");

		List<PlayerChildFSM> listOfChildCells = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();
		for (int i = 0; i < listOfChildCells.Count; i++)
		{
			listOfChildCells[i].DeferredChangeState(PCState.ChargeMain);
		}
	}

	public void ActionChargeChild()
	{
		Debug.Log("Charge Child Action Called");

		List<PlayerChildFSM> listOfChildCells = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();
		for (int i = 0; i < listOfChildCells.Count; i++)
		{
			listOfChildCells[i].DeferredChangeState(PCState.ChargeChild);
		}
	}

	public void ActionSpawnCaptain()
	{
		// Need to check if no current captain is spawned and also check if node conditions are met.

		Debug.Log("Spawn Squad Captain Action Called");
	}

	public void ActionDisperse()
	{
		Debug.Log("Disperse Action called");
	}
	#endregion
}
