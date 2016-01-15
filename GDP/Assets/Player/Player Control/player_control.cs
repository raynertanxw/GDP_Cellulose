using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class player_control : MonoBehaviour
{
	public static int s_nResources;
	private Transform m_SquadCaptainNode;
	
	private Node m_nActiveNode = Node.RightNode;

	private CanvasGroup leftNodeCanvasGrp, rightNodeCanavsGrp, spawnCtrlCanvasGrp;
	private RectTransform[] btnRectTransform;
	private Vector3 mainCellPos;
	private Vector3[] btnPos;
	private const float s_UIFadeOutDelay = 1.5f;
	private const float s_UIFadeOutSpeed = 0.8f;
	private const float s_UIPopInSpeed = 3.5f;
	private const float s_UIPopOutSpeed = 5.0f;
	
	void Awake()
	{
		s_nResources = Settings.s_nPlayerInitialResourceCount;

		m_SquadCaptainNode = GameObject.Find("Node_Captain").transform;
		spawnCtrlCanvasGrp = transform.GetChild(3).GetComponent<CanvasGroup>();
		leftNodeCanvasGrp = transform.GetChild(4).GetComponent<CanvasGroup>();
		rightNodeCanavsGrp = transform.GetChild(5).GetComponent<CanvasGroup>();
		mainCellPos = transform.GetChild(6).GetComponent<RectTransform>().localPosition;
		btnRectTransform = new RectTransform[3];
		btnRectTransform[0] = spawnCtrlCanvasGrp.transform.GetChild(0).GetComponent<RectTransform>();
		btnRectTransform[1] = spawnCtrlCanvasGrp.transform.GetChild(1).GetComponent<RectTransform>();
		btnRectTransform[2] = spawnCtrlCanvasGrp.transform.GetChild(2).GetComponent<RectTransform>();
		btnPos = new Vector3[3];
		for (int i = 0; i < btnRectTransform.Length; i++)
			btnPos[i] = btnRectTransform[i].localPosition;

		// Hide both left and right node.
		leftNodeCanvasGrp.alpha = 0f;
		rightNodeCanavsGrp.alpha = 0f;
		spawnCtrlCanvasGrp.alpha = 0f;
		SetLeftNodeControlVisibility(false);
		SetRightNodeControlVisibility(false);
		SetSpawnCtrlVisibility(false);
	}

	#region Bringing up and hiding control sets
	public void ChangeActiveNode(Node nNewNode)
	{
		m_nActiveNode = nNewNode;

		switch (nNewNode)
		{
		case Node.LeftNode:
			SetLeftNodeControlVisibility(true);
			SetRightNodeControlVisibility(false);
			SetSpawnCtrlVisibility(false);
			break;
		case Node.RightNode:
			SetRightNodeControlVisibility(true);
			SetLeftNodeControlVisibility(false);
			SetSpawnCtrlVisibility(false);
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
			StopCoroutine(Constants.s_strFadeOutCanvasGroup);
			StartCoroutine(QuickFadeOutCanvasGroup(leftNodeCanvasGrp));
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
			StopCoroutine(Constants.s_strFadeOutCanvasGroup);
			StartCoroutine(QuickFadeOutCanvasGroup(rightNodeCanavsGrp));
		}
	}

	public void PresentSpawnCtrl()
	{
		SetSpawnCtrlVisibility(true);
		SetRightNodeControlVisibility(false);
		SetLeftNodeControlVisibility(false);
	}

	private void SetSpawnCtrlVisibility(bool _visible)
	{
		if (_visible)
		{
			// Only pop in if spawn control has completely faded.
			if (spawnCtrlCanvasGrp.alpha == 0f)
				StartCoroutine(Constants.s_strAnimateInSpawnCtrl);

			RestartSpawnCtrlFadeOut();
		}
		else
		{
			StopCoroutine(Constants.s_strFadeOutCanvasGroup);
			StartCoroutine(QuickFadeOutCanvasGroup(spawnCtrlCanvasGrp));
		}
	}

	public void DeselectAllCtrls()
	{
		SetSpawnCtrlVisibility(false);
		SetLeftNodeControlVisibility(false);
		SetRightNodeControlVisibility(false);
	}
	#endregion


	#region Animation Helper functions
	private IEnumerator FadeOutCanvasGroup(CanvasGroup cgrp)
	{
		yield return new WaitForSeconds(s_UIFadeOutDelay);
		while (cgrp.alpha > 0)
		{
			cgrp.alpha -= s_UIFadeOutSpeed * Time.deltaTime;
			yield return null;
		}
		cgrp.interactable = false;
		cgrp.blocksRaycasts = false;
	}

	private IEnumerator QuickFadeOutCanvasGroup(CanvasGroup cgrp)
	{
		while (cgrp.alpha > 0)
		{
			cgrp.alpha -= s_UIPopOutSpeed * Time.deltaTime;
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

	private void RestartSpawnCtrlFadeOut()
	{
		StopCoroutine(Constants.s_strFadeOutCanvasGroup);
		spawnCtrlCanvasGrp.alpha = 1f;
		StartCoroutine(Constants.s_strFadeOutCanvasGroup, spawnCtrlCanvasGrp);
	}

	private IEnumerator AnimateInSpawnCtrl()
	{
		float t = 0f;
		while (t < 1.0f)
		{
			for (int i = 0; i < btnRectTransform.Length; i++)
			{
				btnRectTransform[i].localPosition = Vector3.Lerp(mainCellPos, btnPos[i], t);
				btnRectTransform[i].localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
			}
			spawnCtrlCanvasGrp.alpha = t;
			t += s_UIPopInSpeed * Time.deltaTime;

			yield return null;
		}

		// Ensure it's snapped to final position.
		for (int i = 0; i < btnRectTransform.Length; i++)
		{
			btnRectTransform[i].localPosition = btnPos[i];
			btnRectTransform[i].localScale = Vector3.one;
		}
		spawnCtrlCanvasGrp.alpha = 1f;

		spawnCtrlCanvasGrp.interactable = true;
		spawnCtrlCanvasGrp.blocksRaycasts = true;
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

		RestartSpawnCtrlFadeOut();
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

		RestartSpawnCtrlFadeOut();
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
