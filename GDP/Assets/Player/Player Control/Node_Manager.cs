using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node_Manager : MonoBehaviour
{
	private static Node_Manager nodeLeft, nodeMiddle, nodeRight;
	
	private player_control m_playerCtrl;
	public int m_nNodeIndex;
	
	private List<PlayerChildFSM> m_playerChildInNode;
	
	void Awake()
	{
		m_playerCtrl = transform.parent.GetComponent<player_control>();
		m_playerChildInNode = new List<PlayerChildFSM>();
		
		switch (m_nNodeIndex)
		{
		case 0:
			Node_Manager.nodeLeft = this;
			break;
		case 1:
			Node_Manager.nodeMiddle = this;
			break;
		case 2:
			Node_Manager.nodeRight = this;
			break;
		default:
			Debug.Log("Error: No such node index");
			break;
		}
	}
	
	void OnMouseDown()
	{
		m_playerCtrl.ChangeActiveNode(m_nNodeIndex);
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
			return Node_Manager.nodeMiddle;
		case 2:
			return Node_Manager.nodeRight;
		default:
			Debug.Log("Error: No such node index");
            return null;
        }
	}
	#endregion

}
