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
		if (m_nActiveNode == Node_Manager.s_nNodeIdexWithSquadCaptain)
		{
			Debug.Log("Node is squad manager, can only disperse");
			return;
		}

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
		if (m_nActiveNode == Node_Manager.s_nNodeIdexWithSquadCaptain)
		{
			Debug.Log("Node is squad manager, can only disperse");
			return;
		}

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
		if (m_nActiveNode == Node_Manager.s_nNodeIdexWithSquadCaptain)
		{
			Debug.Log("Node is squad manager, can only disperse");
			return;
		}

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
		if (m_nActiveNode == Node_Manager.s_nNodeIdexWithSquadCaptain)
		{
			Debug.Log("Node is squad manager, can only disperse");
			return;
		}

		List<PlayerChildFSM> listOfChildCells = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();
		for (int i = 0; i < listOfChildCells.Count; i++)
		{
			listOfChildCells[i].DeferredChangeState(PCState.ChargeMain);
		}
	}

	public void ActionChargeChild()
	{
		if (m_nActiveNode == Node_Manager.s_nNodeIdexWithSquadCaptain)
		{
			Debug.Log("Node is squad manager, can only disperse");
			return;
		}

		List<PlayerChildFSM> listOfChildCells = Node_Manager.GetNode(m_nActiveNode).GetNodeChildList();
		for (int i = 0; i < listOfChildCells.Count; i++)
		{
			listOfChildCells[i].DeferredChangeState(PCState.ChargeChild);
		}
	}

	public void ActionSpawnCaptain()
	{
		// Need to check if no current captain is spawned and also check if node conditions are met.
		if (Node_Manager.s_nNodeIdexWithSquadCaptain == -1)
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
				int nTargetNode;
				if (Node_Manager.s_nNodeIdexWithSquadCaptain == 1)
					nTargetNode = 2;
				else
					nTargetNode = 1;

				for (int i = m_nSqaudCaptainChildCost; i < childList.Count; i++)
				{
					PlayerChildFSM child = childList[i];
					child.m_assignedNode = Node_Manager.GetNode(nTargetNode);
					child.m_assignedNode.AddChildToNodeList(child);
					Node_Manager.GetNode(m_nActiveNode).RemoveChildFromNodeList(child);
					child.RefreshState();
				}

				// Spawn in the Squad Captain.
				Vector3 spawnPos = Node_Manager.GetNode(m_nActiveNode).transform.position;
				spawnPos.z = 0.0f;
				PlayerSquadFSM.Instance.Initialise(spawnPos);


				// Set the s_nNodeIndexWithSquadCaptain to the active node.
				Node_Manager.s_nNodeIdexWithSquadCaptain = m_nActiveNode;
			}
			else
			{
				Debug.Log("Not enough child at node to convert");
			}
		}
		else
		{
			Debug.Log("There is another squad captain active");
		}
	}

	public void ActionDisperse()
	{
		if (Node_Manager.s_nNodeIdexWithSquadCaptain == -1)
		{
			Debug.Log("No squad captain is active, nothing to disperse");
		}
		else if (Node_Manager.s_nNodeIdexWithSquadCaptain == m_nActiveNode)
		{
			// Despawn the Squad Captain and it's child cells.
			// CODE NOT DONE HERE YET.

			// Spawn in child cells
			// TEMP SOLUTION!!!
			for (int i = 0; i < m_nSqaudCaptainChildCost; i++)
			{
				ActionSpawn();
			}

			// Set back to -1 to indicate that there is no more squad captain active.
			Node_Manager.s_nNodeIdexWithSquadCaptain = -1;
		}
		else
		{
			Debug.Log("Active squad captain is in another node");
		}

		Debug.Log("Disperse Action called");
	}
	#endregion
}
