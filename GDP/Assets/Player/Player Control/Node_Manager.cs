using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Node_Manager : MonoBehaviour
{
	private static Node_Manager nodeLeft, nodeRight;
	private Image defendAvoidButtonImage;
	
	private player_control m_playerCtrl;
	public int m_nNodeIndex;
	public bool m_bIsDefending = true;
	public Sprite defendSprite, avoidSprite;
	
	private List<PlayerChildFSM> m_playerChildInNode;
	
	void Awake()
	{
		m_playerCtrl = transform.parent.GetComponent<player_control>();
		m_playerChildInNode = new List<PlayerChildFSM>();
		
		switch (m_nNodeIndex)
		{
		case 0:
			Node_Manager.nodeLeft = this;
			defendAvoidButtonImage = GameObject.Find("UI_Player_LeftNode_DefendAvoid").GetComponent<Image>();
			break;
		case 1:
			Node_Manager.nodeRight = this;
			defendAvoidButtonImage = GameObject.Find("UI_Player_RightNode_DefendAvoid").GetComponent<Image>();
			break;
		default:
			Debug.Log("Error: No such node index");
			break;
		}
	}
	
	public void SelectNode()
	{
		m_playerCtrl.ChangeActiveNode(m_nNodeIndex);
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
			defendAvoidButtonImage.sprite = avoidSprite;
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
			defendAvoidButtonImage.sprite = defendSprite;
		}
	}
	
	#region Getter and Setters
	public List<PlayerChildFSM> GetNodeChildList() { return m_playerChildInNode; }
	public void AddChildToNodeList(PlayerChildFSM child)
	{
		m_playerChildInNode.Add(child);
	}
	public void RemoveChildFromNodeList(PlayerChildFSM child)
	{
		m_playerChildInNode.Remove(child);
	}
	
	public static Node_Manager GetNode(int n_nodeIndex)
	{
		switch (n_nodeIndex)
		{
		case 0:
			return Node_Manager.nodeLeft;
		case 1:
			return Node_Manager.nodeRight;
		default:
			Debug.Log("Error: No such node index");
            return null;
        }
	}
	#endregion

}
