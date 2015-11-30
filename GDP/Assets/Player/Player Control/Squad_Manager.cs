using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad_Manager : MonoBehaviour
{
	private static Squad_Manager squadLeft, squadMiddle, squadRight;

	private player_control m_playerCtrl;
    public int m_nSquadIndex;

	private List<PlayerChildFSM> m_playerChildInSquad;

	void Awake()
	{
		m_playerCtrl = transform.parent.GetComponent<player_control>();
		m_playerChildInSquad = new List<PlayerChildFSM>();

		switch (m_nSquadIndex)
		{
		case 0:
			Squad_Manager.squadLeft = this;
			break;
		case 1:
			Squad_Manager.squadMiddle = this;
			break;
		case 2:
			Squad_Manager.squadRight = this;
			break;
		default:
			Debug.Log("Error: No such squad index");
			break;
		}
    }

	void OnMouseDown()
	{
		m_playerCtrl.ChangeActiveSquad(m_nSquadIndex);
	}

	#region Getter and Setters
	public List<PlayerChildFSM> GetSquadChildList() { return m_playerChildInSquad; }
	public void AddChildToSquadList(PlayerChildFSM child)
	{
		m_playerChildInSquad.Add(child);
	}
	public void RemoveChildFromSquadList(PlayerChildFSM child)
	{
		m_playerChildInSquad.Remove(child);
	}

	public static Squad_Manager GetSquad(int n_squadIndex)
	{
		switch (n_squadIndex)
		{
		case 0:
			return Squad_Manager.squadLeft;
			break;
		case 1:
			return Squad_Manager.squadMiddle;
			break;
		case 2:
			return Squad_Manager.squadRight;
			break;
		default:
			return null;
			Debug.Log("Error: No such squad index");
			break;
		}
	}
	#endregion
}
