using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Node_Manager : MonoBehaviour
{
	private static Node_Manager nodeLeft, nodeRight;

	private Image m_defendAvoidButtonImage;
	private player_control m_playerCtrl;
	private RectTransform m_rectTransform;
	private Vector3 m_rotationVec;
	private float m_fNodeRotationSpeed = 100.0f;

	public Node m_nNodeEnum;
	public bool m_bIsDefending = true;
	public Sprite defendSprite, avoidSprite;
	
	private List<PlayerChildFSM> m_playerChildInNode;
	
	void Awake()
	{
		m_playerCtrl = transform.parent.GetComponent<player_control>();
		m_rectTransform = GetComponent<RectTransform>();
		m_rotationVec = Vector3.zero;
		m_playerChildInNode = new List<PlayerChildFSM>();
		
		switch (m_nNodeEnum)
		{
		case Node.LeftNode:
			Node_Manager.nodeLeft = this;
			m_defendAvoidButtonImage = GameObject.Find("UI_Player_LeftNode_DefendAvoid").GetComponent<Image>();
			break;
		case Node.RightNode:
			Node_Manager.nodeRight = this;
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
		if (m_nNodeEnum == 0)
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
	
	public void SelectNode()
	{
		m_playerCtrl.ChangeActiveNode(m_nNodeEnum);
	}

	public void ToggleDefenseAvoid()
	{
		// Change to avoid.
		if (m_bIsDefending == true)
		{
			for (int i = 0; i < m_playerChildInNode.Count; i++)
			{
				m_playerChildInNode[i].m_bIsDefending = false;
				if (m_playerChildInNode[i].GetCurrentState() != PCState.Idle)
					m_playerChildInNode[i].DeferredChangeState(PCState.Idle); // Will auto change to avoid if detected.
			}

			m_bIsDefending = false;
			m_defendAvoidButtonImage.sprite = avoidSprite;
		}
		// Change to defend.
		else
		{
			for (int i = 0; i < m_playerChildInNode.Count; i++)
			{
				m_playerChildInNode[i].m_bIsDefending = true;
				if (m_playerChildInNode[i].GetCurrentState() != PCState.Idle)
					m_playerChildInNode[i].DeferredChangeState(PCState.Idle); // Will auto change to avoid if detected.
			}
			
			m_bIsDefending = true;
			m_defendAvoidButtonImage.sprite = defendSprite;
		}
	}
	
	#region Getter and Setters
	public List<PlayerChildFSM> GetNodeChildList() { return m_playerChildInNode; }
	public int activeChildCount { get { return m_playerChildInNode.Count; } }
	public void AddChildToNodeList(PlayerChildFSM child)
	{
		m_playerChildInNode.Add(child);
	}
	public void RemoveChildFromNodeList(PlayerChildFSM child)
	{
		m_playerChildInNode.Remove(child);
	}

	public static Node_Manager GetNode(Node _selectedNode)
	{
		switch (_selectedNode)
		{
		case Node.LeftNode:
			return Node_Manager.nodeLeft;
			break;
		case Node.RightNode:
			return Node_Manager.nodeRight;
			break;
		default:
			Debug.Log("Error: No such node index");
            return null;
		}
	}
	#endregion

}
