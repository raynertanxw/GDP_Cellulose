using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class player_control : MonoBehaviour
{
	public static int s_nResources;
	private int m_nSpawnCost = 5;
	private int m_nSqaudCaptainChildCost = 10;
	private Transform m_SquadCaptainNode;
	
	private int m_nActiveNode = 1;

	private CanvasGroup leftNodeCanvasGrp, rightNodeCanavsGrp;
	
	void Awake()
	{
		s_nResources = 1000;

		m_SquadCaptainNode = GameObject.Find("Node_Captain").transform;
		leftNodeCanvasGrp = transform.GetChild(3).GetComponent<CanvasGroup>();
		rightNodeCanavsGrp = transform.GetChild(4).GetComponent<CanvasGroup>();

		// Hide both left and right node.
		SetLeftNodeControlVisibility(false);
		SetRightNodeControlVisibility(false);
	}

	#region Changing the Active Node
	public void ChangeActiveNode(int nNewNode)
	{
		m_nActiveNode = nNewNode;

		switch (nNewNode)
		{
		case 0: // Left Node Active Now
			SetLeftNodeControlVisibility(true);
			SetRightNodeControlVisibility(false);
			break;
		case 1: // Right Node Active Now
			SetRightNodeControlVisibility(true);
			SetLeftNodeControlVisibility(false);
			break;
		}
	}

	private void SetLeftNodeControlVisibility(bool _visible)
	{
		if (_visible)
		{
			leftNodeCanvasGrp.alpha = 1f;
			leftNodeCanvasGrp.interactable = true;
			leftNodeCanvasGrp.blocksRaycasts = true;
		}
		else
		{
			leftNodeCanvasGrp.alpha = 0f;
			leftNodeCanvasGrp.interactable = false;
			leftNodeCanvasGrp.blocksRaycasts = false;
		}
	}

	private void SetRightNodeControlVisibility(bool _visible)
	{
		if (_visible)
		{
			rightNodeCanavsGrp.alpha = 1f;
			rightNodeCanavsGrp.interactable = true;
			rightNodeCanavsGrp.blocksRaycasts = true;
		}
		else
		{
			rightNodeCanavsGrp.alpha = 0f;
			rightNodeCanavsGrp.interactable = false;
			rightNodeCanavsGrp.blocksRaycasts = false;
		}
	}
	#endregion


	
	#region Actions for UI Buttons to call
	public void ActionSpawn(int _selectedNode)
	{
		if (s_nResources > m_nSpawnCost)
		{
			// Call a child cell from object pool and set its m_assignedNode to assigned node.
			PlayerChildFSM currentChild = PlayerChildFSM.Spawn(Node_Manager.GetNode(_selectedNode).transform.position + (Vector3)Random.insideUnitCircle*0.5f);
			currentChild.m_assignedNode = Node_Manager.GetNode(_selectedNode);
            currentChild.m_assignedNode.AddChildToNodeList(currentChild);
			s_nResources -= m_nSpawnCost;
		}
		else
		{
			Debug.Log("Not enough resources");
		}
	}

	public void ActionDefendAvoid(int _nodeIndex)
	{
		Node_Manager.GetNode(_nodeIndex).ToggleDefenseAvoid();
	}

	public void ActionBurstShot(int _nodeIndex)
	{
		Debug.Log(_nodeIndex + " Node called BurstShot");
		ActionChargeMain(); // TEMP
	}

	public void ActionSwarmTarget(int _nodeIndex)
	{
		Debug.Log(_nodeIndex + " Node called SwarmTarget");
		ActionChargeChild(); // TEMP
	}

	public void ActionScatterShot(int _nodeIndex)
	{
		Debug.Log(_nodeIndex + " Node called ScatterShot");
	}

	public void ActionSpawnCaptain()
	{
		// Squad Cpt can only spawn one instance.
		if (PlayerSquadFSM.Instance.bIsAlive == true) return;

		// Child count criteria met.
		if (totalActiveChild() >= m_nSqaudCaptainChildCost)
		{
			int childrenLeftToConsume = m_nSqaudCaptainChildCost;

			// Left node more than right.
			if (Node_Manager.GetNode(0).activeChildCount > Node_Manager.GetNode(1).activeChildCount)
			{
				List<PlayerChildFSM> childList = Node_Manager.GetNode(1).GetNodeChildList();

				// Use up cells in smaller node, OR till half of spawn cost.
				for (int i = 0; i < childList.Count; i++)
				{
					// Move to center.

					// Converge to center.

					// Kill them.
					childList[i].KillPlayerChildCell();
					childrenLeftToConsume--;

					if (i == childrenLeftToConsume/2)
						break;
				}

				childList = Node_Manager.GetNode(0).GetNodeChildList();

				// Consume the remaining needed children.
				for (int i = 0; i < childrenLeftToConsume; i++)
				{
					// Move to center.
					
					// Converge to center.
					
					// Kill them.
					childList[i].KillPlayerChildCell();
					// DO NOT reduce childreLefToConsume, for loop will terminate prematurely.
				}
			}

			// Right node more than left or equal numbers.
			else
			{
				List<PlayerChildFSM> childList = Node_Manager.GetNode(0).GetNodeChildList();
				
				// Use up cells in smaller node, OR till half of spawn cost.
				for (int i = 0; i < childList.Count; i++)
				{
					// Move to center.
					
					// Converge to center.
					
					// Kill them.
					childList[i].KillPlayerChildCell();
					childrenLeftToConsume--;
					
					if (i == childrenLeftToConsume/2)
						break;
				}
				
				childList = Node_Manager.GetNode(1).GetNodeChildList();
				
				// Consume the remaining needed children.
				for (int i = 0; i < childrenLeftToConsume; i++)
				{
					// Move to center.
					
					// Converge to center.
					
					// Kill them.
					childList[i].KillPlayerChildCell();
					// DO NOT reduce childreLefToConsume, for loop will terminate prematurely.
				}
			}
			
			// Spawn in the Squad Captain.
			Vector3 spawnPos = m_SquadCaptainNode.position;
			spawnPos.z = 0.0f;
			PlayerSquadFSM.Instance.Initialise(spawnPos);
		}
		else
		{
			Debug.Log("Not enough child at node to convert");
		}
	}
	#endregion








	#region Helper Functions
	private int totalActiveChild()
	{
		int numChild = 0;
		numChild += Node_Manager.GetNode(0).activeChildCount;
		numChild += Node_Manager.GetNode(1).activeChildCount;

		return numChild;
	}
	#endregion



















	// Old Actions.
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

}
