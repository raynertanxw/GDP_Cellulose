using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class player_control : MonoBehaviour
{
	private static player_control s_Instance;
	public static player_control Instance { get { return s_Instance; } }

	public int s_nResources;
	private Transform m_SquadCaptainNode;
	
	private Node m_nActiveNode = Node.RightNode;

	private GameObject spwnCptBtnGO;
	private CanvasGroup leftNodeCanvasGrp, rightNodeCanavsGrp, spawnCtrlCanvasGrp, playerHurtTintCanvasGrp, enemyWarningTintCanvasGrp;
	private RectTransform[] btnRectTransform;
	private Text leftNodeChildText, rightNodeChildText, nutrientText;
	private Vector3 mainCellPos;
	private Vector3[] btnPos;
	private const float s_UIFadeOutDelay = 1.5f;
	private const float s_UIFadeOutSpeed = 0.8f;
	private const float s_UIPopInSpeed = 3.5f;
	private const float s_UIPopOutSpeed = 5.0f;
	private const float s_UIHurtTintFadeSpeed = 2.0f;
	
	void Awake()
	{
		if (Instance == null)
			s_Instance = this;
		else
			Destroy(this.gameObject);

		s_nResources = Settings.s_nPlayerInitialResourceCount;

		m_SquadCaptainNode = GameObject.Find("Node_Captain").transform;
		spwnCptBtnGO = transform.GetChild(3).GetChild(1).gameObject;
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
		playerHurtTintCanvasGrp = transform.GetChild(7).GetChild(0).GetComponent<CanvasGroup>();
		enemyWarningTintCanvasGrp = transform.GetChild(7).GetChild(1).GetComponent<CanvasGroup>();
		leftNodeChildText = transform.GetChild(7).GetChild(2).GetChild(0).GetComponent<Text>();
		rightNodeChildText = transform.GetChild(7).GetChild(2).GetChild(1).GetComponent<Text>();
		nutrientText = transform.GetChild(7).GetChild(3).GetComponent<Text>();

		// Hide both left and right node.
		leftNodeCanvasGrp.alpha = 0f;
		rightNodeCanavsGrp.alpha = 0f;
		spawnCtrlCanvasGrp.alpha = 0f;
		SetLeftNodeControlVisibility(false);
		SetRightNodeControlVisibility(false);
		SetSpawnCtrlVisibility(false);

		// Hide tints
		playerHurtTintCanvasGrp.alpha = 0f;
		enemyWarningTintCanvasGrp.alpha = 0f;
	}

	void Start()
	{
		// Update UI
		UpdateUI_nutrients();
		UpdateUI_nodeChildCountText();
	}

	void Update()
	{
		if (PlayerSquadFSM.Instance.bIsAlive == true)
		{
			if (spwnCptBtnGO.activeSelf == true)
				spwnCptBtnGO.SetActive(false);
		}
		else
		{
			if (spwnCptBtnGO.activeSelf == false)
				spwnCptBtnGO.SetActive(true);
		}

		playerHurtTintCanvasGrp.alpha -= s_UIHurtTintFadeSpeed * Time.deltaTime;
	}

	#region UI HUD update functions
	public void UpdateUI_nutrients()
	{
		nutrientText.text = s_nResources.ToString();
	}

	public void UpdateUI_nodeChildCountText()
	{
		leftNodeChildText.text = Node_Manager.GetNode(Node.LeftNode).activeChildCount.ToString();
		rightNodeChildText.text = Node_Manager.GetNode(Node.RightNode).activeChildCount.ToString();
	}

	public void FlashPlayerHurtTint()
	{
		playerHurtTintCanvasGrp.alpha = 1f;
	}
	#endregion

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
		if (s_nResources >= Settings.s_nPlayerChildSpawnCost && PlayerChildFSM.GetActiveChildCount() < Settings.s_nPlayerMaxChildCount)
		{
			Node _selectedNode = (Node) _nodeIndex;

			// Call a child cell from object pool and set its m_assignedNode to assigned node.
			PlayerChildFSM currentChild = PlayerChildFSM.Spawn(PlayerMain.Instance.transform.position + (Vector3)Random.insideUnitCircle*0.25f);
			currentChild.m_assignedNode = Node_Manager.GetNode(_selectedNode);
			currentChild.m_bIsDefending = Node_Manager.GetNode(_selectedNode).m_bIsDefending;
            currentChild.m_assignedNode.AddChildToNode(currentChild.poolIndex);

			s_nResources -= Settings.s_nPlayerChildSpawnCost;
			UpdateUI_nutrients();
			UpdateUI_nodeChildCountText();

			PlayerMain.Instance.animate.ExpandContract(0.5f, 1, 1.2f, true, 0.2f);
		}
		else
		{
			Debug.Log("Not enough resources " + s_nResources);
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
		Node_Manager selectedNode = Node_Manager.GetNode(_selectedNode);

		if (selectedNode.activeChildCount < Settings.s_nPlayerActionBurstShotChildCost)
		{
			Debug.Log("Not enough child cells for Burst Shot in this node");
		}
		else
		{
			PlayerChildFSM[] formationCells = new PlayerChildFSM[Settings.s_nPlayerActionBurstShotChildCost];
			int fcIndex = 0;
			for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
			{
				if (PlayerChildFSM.s_playerChildStatus[i] == selectedNode.nodePCStatus)
				{
					formationCells[fcIndex] = PlayerChildFSM.playerChildPool[i];
					formationCells[fcIndex].m_formationCells = formationCells; // arrays are reference types.
					formationCells[fcIndex].attackMode = PlayerAttackMode.BurstShot;

					// TEMP FORCED PLACEMENT OF LEADER.
					formationCells[0].rigidbody2D.MovePosition(selectedNode.transform.position + new Vector3(0f, 3f, 0f));
					formationCells[fcIndex].m_assignedNode.SendChildToAttack(i);
					formationCells[fcIndex].DeferredChangeState(PCState.ChargeMain);

					fcIndex++;
					if (fcIndex == formationCells.Length)
						break;
				}
			}
		}

		RestartFadeOut(_selectedNode);
		UpdateUI_nodeChildCountText();
	}

	public void ActionSwarmTarget(int _nodeIndex)
	{
		Node _selectedNode = (Node) _nodeIndex;
        Node_Manager selectedNode = Node_Manager.GetNode(_selectedNode);

        if (selectedNode.activeChildCount < Settings.s_nPlayerActionSwarmTargetChildCost)
        {
            Debug.Log("Not enough child cells for Swarm Target in this node");
        }
        else
        {
			PlayerChildFSM[] formationCells = new PlayerChildFSM[Settings.s_nPlayerActionSwarmTargetChildCost];
			int fcIndex = 0;
			for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
			{
				if (PlayerChildFSM.s_playerChildStatus[i] == selectedNode.nodePCStatus)
				{
					formationCells[fcIndex] = PlayerChildFSM.playerChildPool[i];
					formationCells[fcIndex].m_formationCells = formationCells; // arrays are reference types.
					formationCells[fcIndex].attackMode = PlayerAttackMode.SwarmTarget;
					formationCells[fcIndex].m_assignedNode.SendChildToAttack(i);
					formationCells[fcIndex].DeferredChangeState(PCState.ChargeChild);

					fcIndex++;
					if (fcIndex == formationCells.Length)
						break;
				}
			}
        }

		RestartFadeOut(_selectedNode);
		UpdateUI_nodeChildCountText();
	}

	public void ActionScatterShot(int _nodeIndex)
	{
		Node _selectedNode = (Node) _nodeIndex;
		Node_Manager selectedNode = Node_Manager.GetNode(_selectedNode);

		if (selectedNode.activeChildCount < Settings.s_nPlayerActionScatterShotChildCost)
		{
			Debug.Log("Not enough child cells for ScatterShot in this node");
		}
		else
		{
			PlayerChildFSM[] formationCells = new PlayerChildFSM[Settings.s_nPlayerActionScatterShotChildCost];
			int fcIndex = 0;
			for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
			{
				if (PlayerChildFSM.s_playerChildStatus[i] == selectedNode.nodePCStatus)
				{
					formationCells[fcIndex] = PlayerChildFSM.playerChildPool[i];
					formationCells[fcIndex].m_formationCells = formationCells; // arrays are reference types.
					formationCells[fcIndex].attackMode = PlayerAttackMode.ScatterShot;
					formationCells[fcIndex].m_assignedNode.SendChildToAttack(i);
					formationCells[fcIndex].DeferredChangeState(PCState.ChargeChild);

					fcIndex++;
					if (fcIndex == formationCells.Length)
						break;
				}
			}
		}

		RestartFadeOut(_selectedNode);
		UpdateUI_nodeChildCountText();
	}

	public void ActionSpawnCaptain()
	{
		// Squad Cpt can only spawn one instance.
		if (PlayerSquadFSM.Instance.bIsAlive == true) return;

		// Child count criteria met.
		if (TotalActiveChildInNodes() >= Settings.s_nPlayerSqaudCaptainChildCost)
		{
			int childrenLeftToConsume = Settings.s_nPlayerSqaudCaptainChildCost;
			Node_Manager leftNode = Node_Manager.GetNode(Node.LeftNode);
			Node_Manager rightNode = Node_Manager.GetNode(Node.RightNode);

			// Left node more than right.
			if (leftNode.activeChildCount > rightNode.activeChildCount)
			{
				// Use up cells in smaller node, OR till half of spawn cost.
				for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
				{
					// Move to center.

					// Converge to center.

					// Kill them.
					if (PlayerChildFSM.s_playerChildStatus[i] == pcStatus.InRightNode)
					{
						PlayerChildFSM.playerChildPool[i].SacrificeToSquadCpt();
						rightNode.SendChildToAttack(i);
						childrenLeftToConsume--;

						if (childrenLeftToConsume <= Settings.s_nPlayerSqaudCaptainChildCost / 2)
							break;
					}
				}

				// Consume the remaining needed children.
				for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
				{
					// Move to center.
					
					// Converge to center.
					
					// Kill them.
					if (PlayerChildFSM.s_playerChildStatus[i] == pcStatus.InLeftNode)
					{
						PlayerChildFSM.playerChildPool[i].SacrificeToSquadCpt();
						leftNode.SendChildToAttack(i);
						childrenLeftToConsume--;
						
						if (childrenLeftToConsume == 0)
							break;
					}
				}
			}

			// Right node more than left or equal numbers.
			else
			{
				// Use up cells in smaller node, OR till half of spawn cost.
				for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
				{
					// Move to center.
					
					// Converge to center.
					
					// Kill them.
					if (PlayerChildFSM.s_playerChildStatus[i] == pcStatus.InLeftNode)
					{
						PlayerChildFSM.playerChildPool[i].SacrificeToSquadCpt();
						leftNode.SendChildToAttack(i);
						childrenLeftToConsume--;
						
						if (childrenLeftToConsume <= Settings.s_nPlayerSqaudCaptainChildCost / 2)
							break;
					}
				}
				
				// Consume the remaining needed children.
				for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
				{
					// Move to center.
					
					// Converge to center.
					
					// Kill them.
					if (PlayerChildFSM.s_playerChildStatus[i] == pcStatus.InRightNode)
					{
						PlayerChildFSM.playerChildPool[i].SacrificeToSquadCpt();
						rightNode.SendChildToAttack(i);
						childrenLeftToConsume--;
						
						if (childrenLeftToConsume == 0)
							break;
					}
				}
			}
			
			// Spawn in the Squad Captain.
			Vector3 spawnPos = m_SquadCaptainNode.position;
			spawnPos.z = 0.0f;
			PlayerSquadFSM.Instance.Initialise(spawnPos);

			PlayerMain.Instance.animate.ExpandContract(1.0f, 1, 1.75f);
		}
		else
		{
			Debug.Log("Not enough child at node to convert");
		}

		RestartSpawnCtrlFadeOut();
		UpdateUI_nodeChildCountText();
	}
	#endregion








	#region Helper Functions
	private int TotalActiveChildInNodes()
	{
		int numChild = 0;
		numChild += Node_Manager.GetNode(Node.LeftNode).activeChildCount;
		numChild += Node_Manager.GetNode(Node.RightNode).activeChildCount;

		return numChild;
	}
	#endregion
















	public static void ResetStatics()
	{
		s_Instance = null;
	}
}
