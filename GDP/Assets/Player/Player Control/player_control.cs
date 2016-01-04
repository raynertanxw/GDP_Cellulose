using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class player_control : MonoBehaviour
{
	public static int s_nResources;
	private int m_nSpawnCost = 5;
	private int m_nSqaudCaptainChildCost = 10;

	private SpriteRenderer[] m_nodePointsRen;
	private int m_nActiveNode = 1;

	private Color m_unselectedNodeCol, m_selectedNodeCol;

	void Awake()
	{
		m_unselectedNodeCol = new Color(0, 1, 0.867f, 0.2f);
		m_selectedNodeCol = new Color(0.75f, 1, 0.25f, 0.2f);

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
		//Debug.Log("Active Node: " + m_nActiveNode);
	}

	
	#region Actions for UI Buttons to call
	public void ActionSpawn()
	{
		if (s_nResources > m_nSpawnCost)
		{
			// Call a child cell from object pool and set its m_assignedNode to assigned node.
			PlayerChildFSM currentChild = PlayerChildFSM.Spawn(Node_Manager.GetNode(m_nActiveNode).transform.position + (Vector3)Random.insideUnitCircle*0.5f);
			currentChild.m_assignedNode = Node_Manager.GetNode(m_nActiveNode);
            currentChild.m_assignedNode.AddChildToNodeList(currentChild);
			s_nResources -= m_nSpawnCost;
		}
		else
		{
			Debug.Log("Not enough resources");
		}
	}

	public void ActionDefend()
	{
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
		List<PlayerChildFSM> listOfChildCells = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();
		for (int i = 0; i < listOfChildCells.Count; i++)
		{
			listOfChildCells[i].DeferredChangeState(PCState.ChargeMain);
		}
	}

	public void ActionChargeChild()
	{
		List<PlayerChildFSM> listOfChildCells = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();
		for (int i = 0; i < listOfChildCells.Count; i++)
		{
			listOfChildCells[i].DeferredChangeState(PCState.ChargeChild);
		}
	}

	public void ActionSpawnCaptain()
	{
		List<PlayerChildFSM> childList = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();

		// Child count criteria met.
		if (childList.Count >= m_nSqaudCaptainChildCost)
		{
			// Remove the requred number of child cells.
			for (int i = 0; i < m_nSqaudCaptainChildCost; i++)
			{
				childList[i].KillPlayerChildCell();
			}

			// Move aside the rest of the children.
			for (int i = m_nSqaudCaptainChildCost; i < childList.Count; i++)
			{

			}

			// Spawn in the Squad Captain.
			Vector3 spawnPos = Node_Manager.GetNode(m_nActiveNode).transform.position;
			spawnPos.z = 0.0f;
			PlayerSquadFSM.Instance.Initialise(spawnPos);
		}
		else
		{
			Debug.Log("Not enough child at node to convert");
		}
	}
	#endregion
}
