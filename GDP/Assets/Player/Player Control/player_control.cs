using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class player_control : MonoBehaviour
{
	private static player_control s_Instance;
	public static player_control Instance { get { return s_Instance; } }

	public int s_nResources;

	private Transform m_SquadCaptainNode;
	private GameObject spwnCptBtnGO;
	private CanvasGroup leftNodeCanvasGrp, rightNodeCanavsGrp, spawnCtrlCanvasGrp, playerHurtTintCanvasGrp, enemyWarningTintCanvasGrp, infoPanelCanvasGrp;
	private RectTransform spwnCptBtnRectTransform;
	private Text leftNodeChildText, rightNodeChildText, nutrientText, infoText;
	private Vector3 mainCellPos;
	private Vector3 spwnCptBtnPos;

	private const float s_UIInfoPanelFadeDelay = 0.75f;
	private const float s_UIInfoPanelFadeSpeed = 1.25f;
	private const float s_UIPopInSpeed = 5f;
	private const float s_UIHurtTintFadeSpeed = 2.0f;
	private const float s_UITintFlickerSpeed = 1.0f;
	private float s_UITintFlickerAlpha = 0f;
	private bool m_bUITintFlickerIncreasingAlpha = false;

	private const string GOname_LeftNode = "UI_Player_LeftNode";
	private const string GOname_RightNode = "UI_Player_RightNode";
	private Node activeNode = Node.None;
	private Node activeDraggedNode = Node.None;
	private const string GOname_SpawnCptButton = "UI_Player_SpawnCptButton";
	private const string GOname_OtherScreenAreaButton = "OtherScreenArea_Button";
	private const string GOname_LeftNode_DefendAvoid = "UI_Player_LeftNode_DefendAvoid";
	private const string GOname_LeftNode_ScatterShot = "UI_Player_LeftNode_ScatterShot";
	private const string GOname_LeftNode_SwamTarget = "UI_Player_LeftNode_SwamTarget";
	private const string GOname_LeftNode_BurstShot = "UI_Player_LeftNode_BurstShot";
	private const string GOname_RightNode_DefendAvoid = "UI_Player_RightNode_DefendAvoid";
	private const string GOname_RightNode_ScatterShot = "UI_Player_RightNode_ScatterShot";
	private const string GOname_RightNode_SwamTarget = "UI_Player_RightNode_SwamTarget";
	private const string GOname_RightNode_BurstShot = "UI_Player_RightNode_BurstShot";
	
	void Awake()
	{
		if (Instance == null)
			s_Instance = this;
		else
			Destroy(this.gameObject);

		s_nResources = Settings.s_nPlayerInitialResourceCount;

		m_SquadCaptainNode = GameObject.Find("Node_Captain_SpawnPos").transform;
		spwnCptBtnGO = transform.GetChild(3).GetChild(0).gameObject;
		spawnCtrlCanvasGrp = transform.GetChild(3).GetComponent<CanvasGroup>();
		leftNodeCanvasGrp = transform.GetChild(4).GetComponent<CanvasGroup>();
		rightNodeCanavsGrp = transform.GetChild(5).GetComponent<CanvasGroup>();
		mainCellPos = transform.GetChild(6).GetComponent<RectTransform>().localPosition;
		spwnCptBtnRectTransform = spawnCtrlCanvasGrp.transform.GetChild(0).GetComponent<RectTransform>();
		spwnCptBtnPos = spwnCptBtnRectTransform.localPosition;
		playerHurtTintCanvasGrp = transform.GetChild(7).GetChild(0).GetComponent<CanvasGroup>();
		enemyWarningTintCanvasGrp = transform.GetChild(7).GetChild(1).GetComponent<CanvasGroup>();
		leftNodeChildText = transform.GetChild(7).GetChild(2).GetChild(0).GetComponent<Text>();
		rightNodeChildText = transform.GetChild(7).GetChild(2).GetChild(1).GetComponent<Text>();
		nutrientText = transform.GetChild(7).GetChild(3).GetComponent<Text>();
		infoPanelCanvasGrp = transform.GetChild(7).GetChild(4).GetComponent<CanvasGroup>();
		infoText = transform.GetChild(7).GetChild(4).GetChild(0).GetComponent<Text>();

		// Hide controls, tints, and info panel.
		DeselectAllCtrls();
		playerHurtTintCanvasGrp.alpha = 0f;
		enemyWarningTintCanvasGrp.alpha = 0f;
		infoPanelCanvasGrp.alpha = 0f;
	}

	void Start()
	{
		// Update UI
		UpdateUI_nutrients();
		UpdateUI_nodeChildCountText();
	}

	void Update()
	{
		playerHurtTintCanvasGrp.alpha -= s_UIHurtTintFadeSpeed * Time.deltaTime;

		if (EMHelper.Instance().MinToMaxYRatio > 0.85f)
			FlickerWarningTint(0.5f, 2 * s_UITintFlickerSpeed);
		else if (EMHelper.Instance().MinToMaxYRatio > 0.7f)
			FlickerWarningTint(0f, s_UITintFlickerSpeed);
		else
		{
			enemyWarningTintCanvasGrp.alpha -= s_UIHurtTintFadeSpeed * Time.deltaTime;
			s_UITintFlickerAlpha = enemyWarningTintCanvasGrp.alpha;
		}

		if (PlayerSquadFSM.Instance == null)
			return;

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

	private void FlickerWarningTint(float _fMinAlpha, float _fFlickerSpeed)
	{
		if (s_UITintFlickerAlpha > 1f)
		{
			m_bUITintFlickerIncreasingAlpha = false;
			s_UITintFlickerAlpha -= Time.deltaTime * _fFlickerSpeed;
		}
		else if (s_UITintFlickerAlpha < _fMinAlpha)
		{
			m_bUITintFlickerIncreasingAlpha = true;
			s_UITintFlickerAlpha += Time.deltaTime * _fFlickerSpeed;
		}
		else
		{
			if (m_bUITintFlickerIncreasingAlpha)
				s_UITintFlickerAlpha += Time.deltaTime * _fFlickerSpeed;
			else
				s_UITintFlickerAlpha -= Time.deltaTime * _fFlickerSpeed;
		}

		enemyWarningTintCanvasGrp.alpha = s_UITintFlickerAlpha;
	}
	#endregion

	#region Bringing up and hiding control sets
	private void SetLeftNodeControlVisibility(bool _visible)
	{
		SetCanvasGroupVisible(leftNodeCanvasGrp, _visible);
	}

	private void SetRightNodeControlVisibility(bool _visible)
	{
		SetCanvasGroupVisible(rightNodeCanavsGrp, _visible);
	}

	private void SetSpawnCtrlVisibility(bool _visible)
	{
		if (_visible == false)
			StopCoroutine(Constants.s_strAnimateInSpawnCtrl);
		SetCanvasGroupVisible(spawnCtrlCanvasGrp, _visible);
	}

	public void PresentSpawnCtrl()
	{
		StartCoroutine(Constants.s_strAnimateInSpawnCtrl);
	}
	
	public void DeselectAllCtrls()
	{
		SetSpawnCtrlVisibility(false);
		SetLeftNodeControlVisibility(false);
		SetRightNodeControlVisibility(false);
		;
	}
	#endregion


	#region Animation Helper functions
	private void SetCanvasGroupVisible(CanvasGroup _cgrp, bool _visible)
	{
		if (_visible)
		{
			_cgrp.alpha = 1f;
			_cgrp.interactable = true;
			_cgrp.blocksRaycasts = true;
		}
		else
		{
			if(_cgrp.alpha == 1f){AudioManager.PlayPMSoundEffect(PlayerMainSFX.ActionSelectDissapear);}
			_cgrp.alpha = 0f;
			_cgrp.interactable = false;
			_cgrp.blocksRaycasts = false;
		}
	}

	private IEnumerator FadeOutInfoPanel()
	{
		yield return new WaitForSeconds(s_UIInfoPanelFadeDelay);
		while (infoPanelCanvasGrp.alpha > 0)
		{
			infoPanelCanvasGrp.alpha -= s_UIInfoPanelFadeSpeed * Time.deltaTime;
			yield return null;
		}
	}
	
	private void PresentInfoPanel()
	{
		infoPanelCanvasGrp.alpha = 1f;
		StopCoroutine(Constants.s_strFadeOutInfoPanel);
		StartCoroutine(Constants.s_strFadeOutInfoPanel);
	}






	

	private IEnumerator AnimateInSpawnCtrl()
	{
		float t = 0f;
		while (t < 1.0f)
		{
			spwnCptBtnRectTransform.localPosition = Vector3.Lerp(mainCellPos, spwnCptBtnPos, t);
			spwnCptBtnRectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
			spawnCtrlCanvasGrp.alpha = t;
			t += s_UIPopInSpeed * Time.deltaTime;

			yield return null;
		}

		// Ensure it's snapped to final position.
		spwnCptBtnRectTransform.localPosition = spwnCptBtnPos;
		spwnCptBtnRectTransform.localScale = Vector3.one;
		spawnCtrlCanvasGrp.alpha = 1f;

		spawnCtrlCanvasGrp.interactable = true;
		spawnCtrlCanvasGrp.blocksRaycasts = true;
	}
	#endregion



	
	#region Actions for UI Buttons to call
	public void ActionSpawn(int _nodeIndex)
	{
		Node _selectedNode = (Node) _nodeIndex;
		if (activeNode != Node.None)
			return;

		if (PlayerChildFSM.GetActiveChildCount() >= Constants.s_nPlayerMaxChildCount)
		{
			infoText.text = "Reached\nMaximum\nChild Cell\nCount";
			PresentInfoPanel();
			Node_Manager.GetNode(_selectedNode).CalculateChildCount(); // Force calcualtion check.
			return;
		}

		if (s_nResources >= Settings.s_nPlayerChildSpawnCost)
		{	
			// Call a child cell from object pool and set its m_assignedNode to assigned node.
			PlayerChildFSM currentChild = PlayerChildFSM.Spawn(PlayerMain.Instance.transform.position + (Vector3)Random.insideUnitCircle*0.25f);
			currentChild.m_assignedNode = Node_Manager.GetNode(_selectedNode);
			currentChild.m_bIsDefending = Node_Manager.GetNode(_selectedNode).m_bIsDefending;
			currentChild.m_assignedNode.AddChildToNode(currentChild.poolIndex);
			
			s_nResources -= Settings.s_nPlayerChildSpawnCost;
			UpdateUI_nutrients();
			UpdateUI_nodeChildCountText();
			
			PlayerMain.Instance.animate.ExpandContract(0.5f, 1, 1.2f, true, 0.2f);
			AudioManager.PlayPMSoundEffect(PlayerMainSFX.SpawnCell);
		}
		else
		{
			infoText.text = "Not enough\nnutrients\n\nNeeded:\n" + Settings.s_nPlayerChildSpawnCost + " units";
			PresentInfoPanel();
		}
	}

	public void ActionDefendAvoid(Node _selectedNode)
	{
		Node_Manager.GetNode(_selectedNode).ToggleDefenseAvoid();
	}

	public void ActionBurstShot(Node _selectedNode)
	{
		Node_Manager selectedNode = Node_Manager.GetNode(_selectedNode);

		if (selectedNode.activeChildCount < Settings.s_nPlayerActionBurstShotChildCost)
		{
			infoText.text = "Not enough\nchild cells\n\nNeeded:\n" + Settings.s_nPlayerActionBurstShotChildCost + " cells";
			PresentInfoPanel();
		}
		else
		{
			int[] childrenInNode = new int[] {-1};
			switch (_selectedNode)
			{
			case Node.LeftNode: childrenInNode = PlayerChildFSM.childrenInLeftNode; break;
			case Node.RightNode: childrenInNode = PlayerChildFSM.childrenInRightNode; break;
			}

			PlayerChildFSM[] formationCells = new PlayerChildFSM[Settings.s_nPlayerActionBurstShotChildCost];
			int fcIndex = 0;

			// Assumes that there will be at least the required amount of cells
			for (int i = 0; i < childrenInNode.Length; i++)
			{
				int poolId = childrenInNode[i];

				formationCells[fcIndex] = PlayerChildFSM.playerChildPool[poolId];
				formationCells[fcIndex].m_formationCells = formationCells; // arrays are reference types.
				formationCells[fcIndex].attackMode = PlayerAttackMode.BurstShot;
				
				formationCells[fcIndex].m_assignedNode.SendChildToAttack(poolId);
				formationCells[fcIndex].DeferredChangeState(PCState.ChargeMain);
				
				fcIndex++;
				if (fcIndex == formationCells.Length)
					break;
			}

			infoText.text = "BurstShot";
			PresentInfoPanel();
		}

		UpdateUI_nodeChildCountText();
	}

	public void ActionSwarmTarget(Node _selectedNode)
	{
        Node_Manager selectedNode = Node_Manager.GetNode(_selectedNode);

        if (selectedNode.activeChildCount < Settings.s_nPlayerActionSwarmTargetChildCost)
        {
			infoText.text = "Not enough\nchild cells\n\nNeeded:\n" + Settings.s_nPlayerActionSwarmTargetChildCost +  " cells";
			PresentInfoPanel();
        }
        else
        {
			int[] childrenInNode = new int[] {-1};
			switch (_selectedNode)
			{
			case Node.LeftNode: childrenInNode = PlayerChildFSM.childrenInLeftNode; break;
			case Node.RightNode: childrenInNode = PlayerChildFSM.childrenInRightNode; break;
			}

			PlayerChildFSM[] formationCells = new PlayerChildFSM[Settings.s_nPlayerActionSwarmTargetChildCost];
			int fcIndex = 0;

			// Assumes that there will be at least the required amount of cells
			for (int i = 0; i < childrenInNode.Length; i++)
			{
				int poolId = childrenInNode[i];

				formationCells[fcIndex] = PlayerChildFSM.playerChildPool[poolId];
				formationCells[fcIndex].m_formationCells = formationCells; // arrays are reference types.
				formationCells[fcIndex].attackMode = PlayerAttackMode.SwarmTarget;
				formationCells[fcIndex].m_assignedNode.SendChildToAttack(poolId);
				formationCells[fcIndex].DeferredChangeState(PCState.ChargeChild);
				
				fcIndex++;
				if (fcIndex == formationCells.Length)
					break;
			}

			infoText.text = "SwarmTarget";
			PresentInfoPanel();
        }

		UpdateUI_nodeChildCountText();
	}

	public void ActionScatterShot(Node _selectedNode)
	{
		Node_Manager selectedNode = Node_Manager.GetNode(_selectedNode);

		if (selectedNode.activeChildCount < Settings.s_nPlayerActionScatterShotChildCost)
		{
			infoText.text = "Not enough\nchild cells\n\nNeeded:\n" + Settings.s_nPlayerActionScatterShotChildCost + " cells";
			PresentInfoPanel();
		}
		else
		{
			int[] childrenInNode = new int[] {-1};
			switch (_selectedNode)
			{
			case Node.LeftNode: childrenInNode = PlayerChildFSM.childrenInLeftNode; break;
			case Node.RightNode: childrenInNode = PlayerChildFSM.childrenInRightNode; break;
			}

			PlayerChildFSM[] formationCells = new PlayerChildFSM[Settings.s_nPlayerActionScatterShotChildCost];
			int fcIndex = 0;

			// Assumes that there will be at least the required amount of cells
			for (int i = 0; i < childrenInNode.Length; i++)
			{
				int poolId = childrenInNode[i];

				formationCells[fcIndex] = PlayerChildFSM.playerChildPool[poolId];
				formationCells[fcIndex].m_formationCells = formationCells; // arrays are reference types.
				formationCells[fcIndex].attackMode = PlayerAttackMode.ScatterShot;
				formationCells[fcIndex].m_assignedNode.SendChildToAttack(poolId);
				formationCells[fcIndex].DeferredChangeState(PCState.ChargeChild);
				
				fcIndex++;
				if (fcIndex == formationCells.Length)
					break;
			}

			infoText.text = "ScatterShot";
			PresentInfoPanel();
		}

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
				for (int i = 0; i < Settings.s_nPlayerSqaudCaptainChildCost / 2; i++)
				{
					// Kill them.
					int poolId = PlayerChildFSM.childrenInRightNode[i];
					if (poolId == -1)
						break;

					PlayerChildFSM.playerChildPool[poolId].SacrificeToSquadCpt();
					rightNode.SendChildToAttack(poolId);
					childrenLeftToConsume--;
				}

				// Consume the remaining needed children.
				for (int i = 0; i < Settings.s_nPlayerSqaudCaptainChildCost; i++)
				{
					// Kill them.
					int poolId = PlayerChildFSM.childrenInLeftNode[i];
					if (poolId == -1)
						break;

					PlayerChildFSM.playerChildPool[poolId].SacrificeToSquadCpt();
					leftNode.SendChildToAttack(poolId);
					childrenLeftToConsume--;

					if (childrenLeftToConsume == 0)
						break;
				}
			}

			// Right node more than left or equal numbers.
			else
			{
				// Use up cells in smaller node, OR till half of spawn cost.
				for (int i = 0; i < Settings.s_nPlayerSqaudCaptainChildCost / 2; i++)
				{
					// Kill them.
					int poolId = PlayerChildFSM.childrenInLeftNode[i];
					if (poolId == -1)
						break;
					
					PlayerChildFSM.playerChildPool[poolId].SacrificeToSquadCpt();
					leftNode.SendChildToAttack(poolId);
					childrenLeftToConsume--;
				}
				
				// Consume the remaining needed children.
				for (int i = 0; i < Settings.s_nPlayerSqaudCaptainChildCost; i++)
				{
					// Kill them.
					int poolId = PlayerChildFSM.childrenInRightNode[i];
					if (poolId == -1)
						break;
					
					PlayerChildFSM.playerChildPool[poolId].SacrificeToSquadCpt();
					rightNode.SendChildToAttack(poolId);
					childrenLeftToConsume--;
					
					if (childrenLeftToConsume == 0)
						break;
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
			infoText.text = "Not enough\nchild cells\n\nNeeded:\n" + Settings.s_nPlayerSqaudCaptainChildCost + " cells";
			PresentInfoPanel();
			SetSpawnCtrlVisibility(false);
		}

		UpdateUI_nodeChildCountText();
	}
	#endregion

	#region Event Trigger Functions
	public void SpawnBtnDown(int _nodeIndex)
	{
		if (activeNode != Node.None)
			return;

		Node _selectedNode = (Node) _nodeIndex;
		switch (_selectedNode)
		{
		case Node.LeftNode:
			SetRightNodeControlVisibility(false);
			activeNode = Node.LeftNode;
			break;

		case Node.RightNode:
			SetLeftNodeControlVisibility(false);
			activeNode = Node.RightNode;
			break;
		}
	}

	public void SpawnBtnUp(int _nodeIndex)
	{
		Node _selectedNode = (Node) _nodeIndex;

		if (_selectedNode == activeNode)
			activeNode = Node.None;
	}

	public void NodeBeginDrag(BaseEventData _data)
	{
		if (activeDraggedNode != Node.None)
			return;

		PointerEventData pointerData = _data as PointerEventData;
		switch (pointerData.pointerPressRaycast.gameObject.name)
		{
		case GOname_LeftNode:
			activeDraggedNode = Node.LeftNode;
			break;
		case GOname_RightNode:
			activeDraggedNode = Node.RightNode;
			break;
		}
	}

	public void NodeDrag(BaseEventData _data)
	{
		PointerEventData pointerData = _data as PointerEventData;

		if (pointerData == null)
			return;

		switch (activeDraggedNode)
		{
		case Node.LeftNode:
			if (pointerData.pointerPressRaycast.gameObject.name != GOname_LeftNode)
				break;

			if (pointerData.pointerCurrentRaycast.gameObject.name != GOname_LeftNode)
			{
				SetLeftNodeControlVisibility(true);
				AudioManager.PlayPMSoundEffect(PlayerMainSFX.ActionSelectAppear);
			}
			else
				SetLeftNodeControlVisibility(false);
			break;

		case Node.RightNode:
			if (pointerData.pointerPressRaycast.gameObject.name != GOname_RightNode)
				break;

			if (pointerData.pointerCurrentRaycast.gameObject.name != GOname_RightNode)
			{
				SetRightNodeControlVisibility(true);
			    AudioManager.PlayPMSoundEffect(PlayerMainSFX.ActionSelectAppear);
			}
			else
				SetRightNodeControlVisibility(false);
			break;
		}
	}

	public void NodeEndDrag(BaseEventData _data)
	{
		PointerEventData pointerData = _data as PointerEventData;

		switch (activeDraggedNode)
		{
		case Node.LeftNode:
			switch (pointerData.pointerCurrentRaycast.gameObject.name)
			{
			case GOname_LeftNode_BurstShot:
				ActionBurstShot(Node.LeftNode);
				SetLeftNodeControlVisibility(false);
				AudioManager.PlayPMSoundEffect(PlayerMainSFX.ActionSelecteSelected);
				break;
			case GOname_LeftNode_SwamTarget:
				ActionSwarmTarget(Node.LeftNode);
				SetLeftNodeControlVisibility(false);
				AudioManager.PlayPMSoundEffect(PlayerMainSFX.ActionSelecteSelected);
				break;
			case GOname_LeftNode_ScatterShot:
				ActionScatterShot(Node.LeftNode);
				SetLeftNodeControlVisibility(false);
				AudioManager.PlayPMSoundEffect(PlayerMainSFX.ActionSelecteSelected);
				break;
			case GOname_LeftNode_DefendAvoid:
				ActionDefendAvoid(Node.LeftNode);
				SetLeftNodeControlVisibility(false);
				AudioManager.PlayPMSoundEffect(PlayerMainSFX.ActionSelecteSelected);
				break;
			default:
				SetLeftNodeControlVisibility(false);
				break;
			}
			break;

		case Node.RightNode:
			switch (pointerData.pointerCurrentRaycast.gameObject.name)
			{
			case GOname_RightNode_BurstShot:
				ActionBurstShot(Node.RightNode);
				SetRightNodeControlVisibility(false);
				break;
			case GOname_RightNode_SwamTarget:
				ActionSwarmTarget(Node.RightNode);
				SetRightNodeControlVisibility(false);
				break;
			case GOname_RightNode_ScatterShot:
				ActionScatterShot(Node.RightNode);
				SetRightNodeControlVisibility(false);
				break;
			case GOname_RightNode_DefendAvoid:
				ActionDefendAvoid(Node.RightNode);
				SetRightNodeControlVisibility(false);
				break;
			default:
				SetRightNodeControlVisibility(false);
				break;
			}
			break;
		}

		activeDraggedNode = Node.None;
	}
	
	public void MainEndDrag(BaseEventData data)
	{
		PointerEventData pointerData = data as PointerEventData;
		switch (pointerData.pointerCurrentRaycast.gameObject.name)
		{
		case GOname_SpawnCptButton:
			ActionSpawnCaptain();
			AudioManager.PlayPMSoundEffect(PlayerMainSFX.ActionSelectAppear);
			break;
		default:
			SetSpawnCtrlVisibility(false);
			break;
		}
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
