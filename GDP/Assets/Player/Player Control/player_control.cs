using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class player_control : MonoBehaviour
{
	public static int s_nResources;
	private Transform m_SquadCaptainNode;
	
	private Node m_nActiveNode = Node.RightNode;

	private CanvasGroup leftNodeCanvasGrp, rightNodeCanavsGrp;
	
	void Awake()
	{
		s_nResources = Settings.s_nPlayerInitialResourceCount;

		m_SquadCaptainNode = GameObject.Find("Node_Captain").transform;
		leftNodeCanvasGrp = transform.GetChild(3).GetComponent<CanvasGroup>();
		rightNodeCanavsGrp = transform.GetChild(4).GetComponent<CanvasGroup>();

		// Hide both left and right node.
		SetLeftNodeControlVisibility(false);
		SetRightNodeControlVisibility(false);
	}

	#region Changing the Active Node
	public void ChangeActiveNode(Node nNewNode)
	{
		m_nActiveNode = nNewNode;

		switch (nNewNode)
		{
		case Node.LeftNode:
			SetLeftNodeControlVisibility(true);
			SetRightNodeControlVisibility(false);
			break;
		case Node.RightNode:
			SetRightNodeControlVisibility(true);
			SetLeftNodeControlVisibility(false);
			break;
		}
	}

	private void SetLeftNodeControlVisibility(bool _visible)
	{
		if (_visible)
		{
			leftNodeCanvasGrp.interactable = true;
			leftNodeCanvasGrp.blocksRaycasts = true;
			RestartFadeOut(Node.LeftNode);
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
			rightNodeCanavsGrp.interactable = true;
			rightNodeCanavsGrp.blocksRaycasts = true;
			RestartFadeOut(Node.RightNode);
		}
		else
		{
			rightNodeCanavsGrp.alpha = 0f;
			rightNodeCanavsGrp.interactable = false;
			rightNodeCanavsGrp.blocksRaycasts = false;
		}
	}

	private IEnumerator FadeOutCanvasGroup(CanvasGroup cgrp)
	{
		yield return new WaitForSeconds(1.5f);
		while (cgrp.alpha > 0)
		{
			cgrp.alpha -= 0.8f * Time.deltaTime;
			yield return null;
		}
		cgrp.interactable = false;
		cgrp.blocksRaycasts = false;
	}

	private void RestartFadeOut(Node _selectedNode)
	{
		switch (_selectedNode)
		{
		case Node.LeftNode:
			StopCoroutine(Constants.s_strFadeOutCanvasGroup);
			leftNodeCanvasGrp.alpha = 1.0f;
			StartCoroutine(Constants.s_strFadeOutCanvasGroup, leftNodeCanvasGrp);
			break;
		case Node.RightNode:
			StopCoroutine(Constants.s_strFadeOutCanvasGroup);
			rightNodeCanavsGrp.alpha = 1.0f;
			StartCoroutine(Constants.s_strFadeOutCanvasGroup, rightNodeCanavsGrp);
			break;
		}
	}
	#endregion


	
	#region Actions for UI Buttons to call
	public void ActionSpawn(int _nodeIndex)
	{
		if (s_nResources > Settings.s_nPlayerChildSpawnCost && totalActiveChild() < Settings.s_nPlayerMaxChildCount)
		{
			Node _selectedNode = (Node) _nodeIndex;

			// Call a child cell from object pool and set its m_assignedNode to assigned node.
			PlayerChildFSM currentChild = PlayerChildFSM.Spawn(PlayerMain.s_Instance.transform.position + (Vector3)Random.insideUnitCircle*0.25f);
			currentChild.m_assignedNode = Node_Manager.GetNode(_selectedNode);
			currentChild.m_bIsDefending = Node_Manager.GetNode(_selectedNode).m_bIsDefending;
            currentChild.m_assignedNode.AddChildToNodeList(currentChild);
			s_nResources -= Settings.s_nPlayerChildSpawnCost;
		}
		else
		{
			Debug.Log("Not enough resources");
		}
	}

	public void ActionDefendAvoid(int _nodeIndex)
	{
		Node _selectedNode = (Node) _nodeIndex;
		Node_Manager.GetNode(_selectedNode).ToggleDefenseAvoid();

		RestartFadeOut(_selectedNode);
	}

	public void ActionBurstShot(int _nodeIndex)
	{
		Node _selectedNode = (Node) _nodeIndex;
		Debug.Log(_nodeIndex + " Node called BurstShot");
		ActionChargeMain(); // TEMP

		RestartFadeOut(_selectedNode);
	}

	public void ActionSwarmTarget(int _nodeIndex)
	{
		Node _selectedNode = (Node) _nodeIndex;
		Debug.Log(_nodeIndex + " Node called SwarmTarget");
		ActionChargeChild(); // TEMP

		RestartFadeOut(_selectedNode);
	}

	public void ActionScatterShot(int _nodeIndex)
	{
		Node _selectedNode = (Node) _nodeIndex;
		Debug.Log(_nodeIndex + " Node called ScatterShot");

		RestartFadeOut(_selectedNode);
	}

	public void ActionSpawnCaptain()
	{
		// Squad Cpt can only spawn one instance.
		if (PlayerSquadFSM.Instance.bIsAlive == true) return;

		// Child count criteria met.
		if (totalActiveChild() >= Settings.s_nPlayerSqaudCaptainChildCost)
		{
			int childrenLeftToConsume = Settings.s_nPlayerSqaudCaptainChildCost;

			// Left node more than right.
			if (Node_Manager.GetNode(Node.LeftNode).activeChildCount > Node_Manager.GetNode(Node.RightNode).activeChildCount)
			{
				List<PlayerChildFSM> childList = Node_Manager.GetNode(Node.RightNode).GetNodeChildList();

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

				childList = Node_Manager.GetNode(Node.LeftNode).GetNodeChildList();

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
				List<PlayerChildFSM> childList = Node_Manager.GetNode(Node.LeftNode).GetNodeChildList();
				
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
				
				childList = Node_Manager.GetNode(Node.RightNode).GetNodeChildList();
				
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
		numChild += Node_Manager.GetNode(Node.LeftNode).activeChildCount;
		numChild += Node_Manager.GetNode(Node.RightNode).activeChildCount;

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
