using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Node_Manager : MonoBehaviour
{
	private static Node_Manager nodeLeft, nodeRight;

	private Image m_defendAvoidButtonImage;
	private RectTransform m_rectTransform;
	private Vector3 m_rotationVec;
	private float m_fNodeRotationSpeed = 100.0f;

	public Node m_NodeEnum;
	private pcStatus m_NodePCStatusEnum;
	public bool m_bIsDefending = true;
	public Sprite defendSprite, avoidSprite;
	private int nChildrenInNode;
	
	void Awake()
	{
		nChildrenInNode = 0;

		m_rectTransform = GetComponent<RectTransform>();
		m_rotationVec = Vector3.zero;
		
		switch (m_NodeEnum)
		{
		case Node.LeftNode:
			Node_Manager.nodeLeft = this;
			m_NodePCStatusEnum = pcStatus.InLeftNode;
			m_defendAvoidButtonImage = GameObject.Find("UI_Player_LeftNode_DefendAvoid").GetComponent<Image>();
			break;
		case Node.RightNode:
			Node_Manager.nodeRight = this;
			m_NodePCStatusEnum = pcStatus.InRightNode;
			m_defendAvoidButtonImage = GameObject.Find("UI_Player_RightNode_DefendAvoid").GetComponent<Image>();
			break;
		default:
			Debug.Log("Error: No such node index");
			break;
		}
	}

	void Update()
	{
		// Rotate the node.
		if (m_NodeEnum == 0)
		{
			m_rotationVec.z += -m_fNodeRotationSpeed * Time.deltaTime;
			m_rectTransform.localRotation = Quaternion.Euler(m_rotationVec);
		}
		else
		{
			m_rotationVec.z += m_fNodeRotationSpeed * Time.deltaTime;
			m_rectTransform.localRotation = Quaternion.Euler(m_rotationVec);
		}
	}

	public void ToggleDefenseAvoid()
	{
		// Change to avoid.
		if (m_bIsDefending == true)
		{
			for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
			{
				if (PlayerChildFSM.s_playerChildStatus[i] == nodePCStatus)
				{
					PlayerChildFSM.playerChildPool[i].m_bIsDefending = false;
					if (PlayerChildFSM.playerChildPool[i].GetCurrentState() != PCState.Idle)
						PlayerChildFSM.playerChildPool[i].DeferredChangeState(PCState.Idle); // Will auto change to avoid if detected.
				}
			}

			m_bIsDefending = false;
			m_defendAvoidButtonImage.sprite = avoidSprite;
		}
		// Change to defend.
		else
		{
			for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
			{
				if (PlayerChildFSM.s_playerChildStatus[i] == nodePCStatus)
				{
					PlayerChildFSM.playerChildPool[i].m_bIsDefending = true;
					if (PlayerChildFSM.playerChildPool[i].GetCurrentState() != PCState.Idle)
						PlayerChildFSM.playerChildPool[i].DeferredChangeState(PCState.Idle); // Will auto change to avoid if detected.
				}
			}
			
			m_bIsDefending = true;
			m_defendAvoidButtonImage.sprite = defendSprite;
		}
	}

	public void AddChildToNode(int _poolIndex)
	{
		switch (m_NodeEnum)
		{
		case Node.LeftNode:
			PlayerChildFSM.s_playerChildStatus[_poolIndex] = pcStatus.InLeftNode;
			break;
		case Node.RightNode:
			PlayerChildFSM.s_playerChildStatus[_poolIndex] = pcStatus.InRightNode;
			break;
		}

		CalculateChildCount();
	}

	public void SendChildToAttack(int _poolIndex)
	{
		PlayerChildFSM.s_playerChildStatus[_poolIndex] = pcStatus.Attacking;
		nChildrenInNode--;
	}

	public void RemoveChildFromNode(int _poolIndex)
	{
		PlayerChildFSM.s_playerChildStatus[_poolIndex] = pcStatus.DeadState;
		nChildrenInNode--;
	}

	// Todo: remove the need for this function
	// Currently used to solve bug when spawning too fast on a mobile with both buttons.
	public void CalculateChildCount()
	{
		int childCountInNode = 0;
		pcStatus node = (pcStatus) ((int)m_NodeEnum + 1);

		for (int i = 0; i < Settings.s_nPlayerMaxChildCount; i++)
		{
			if (PlayerChildFSM.s_playerChildStatus[i] == node)
				childCountInNode++;
		}

		nChildrenInNode = childCountInNode;
	}

	#region Getter and Setters
	public int activeChildCount { get { return nChildrenInNode; } }
	public pcStatus nodePCStatus { get { return m_NodePCStatusEnum; } }

	public static Node_Manager GetNode(Node _selectedNode)
	{
		switch (_selectedNode)
		{
		case Node.LeftNode:
			return Node_Manager.nodeLeft;
		case Node.RightNode:
			return Node_Manager.nodeRight;
		default:
			Debug.Log("Error: No such node index");
            return null;
		}
	}
	#endregion

}
