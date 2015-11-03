using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad_Manager : MonoBehaviour
{
	private player_control playerCtrl;
    private ChildController leader;
    private GameObject enemy;
    private float speed;
    private bool inCommand;
    [SerializeField]
    private string currentCommand;

    [SerializeField]
    private List<ChildController> children;

    public int squadIndex;

	void Awake()
	{
		playerCtrl = transform.parent.GetComponent<player_control>();
        children = new List<ChildController>();
        enemy = GameObject.Find("Enemy_Cell");
        speed = 0.2f;
        currentCommand = "";
        inCommand = false;
    }

	void OnMouseDown()
	{
		playerCtrl.ChangeActiveSquad(squadIndex);
	}

    void Update()
    {
        ExecuteCommand();
    }

    void ExecuteCommand()
    {
        if (currentCommand == "Attack")
        {
            Attack();
        }
        
    }

    bool CheckSquadForLeader()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].ReturnIsLeader() == true)
            {
                return true;
            }
        }
        return false;
    }

    public void ReassignLeader()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].ReturnIsLeader() == false)
            {
                children[i].SetLeader();
                break;
            }
        }
        SetLeader();
    }

    public void AddChild(ChildController child)
    {
        children.Add(child);
        if (CheckSquadForLeader() == false)
        {
            child.SetLeader();
        }
    }

    bool CheckChildrenEmpty()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                return false;
            }
        }
        return true;
    }

    ChildController ReturnLeader()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].ReturnIsLeader() == true)
            {
                return children[i];
            }
        }
        return null;
    }

    bool CheckCellReachTarget(ChildController child, Vector2 targetPos)
    {
        if (child != null && Vector2.Distance(child.transform.position, targetPos) <= 0.1f)
        {
            return true;  
        }
        return false;
    }

    bool CheckSquadReachTarget(Vector2 targetPos)
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (CheckCellReachTarget(children[i], targetPos) == false)
            {
                return false;
            }
        }
        return true;
    }

    public void CommandAttack()
    {
        currentCommand = "Attack";
    }

    void Attack()
    {
        SetLeader();
        Vector2 targetPos = enemy.transform.position;
        if (leader != null)
        {
            Vector2 difference = new Vector2(targetPos.x - leader.transform.position.x, targetPos.y - leader.transform.position.y);
            Vector2 direction = difference.normalized;

            leader.transform.Translate(direction * speed);
        }
        
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null && CheckCellReachTarget(children[i], targetPos) == false && leader != null)
            {
                Vector2 diff = new Vector2(children[i].transform.position.x - leader.transform.position.x, children[i].transform.position.y - leader.transform.position.y);
                Vector2 toLeader = -diff.normalized;
                children[i].transform.Translate(toLeader * speed);
            }
            else if (children[i] != null)
            {
                Vector2 diff = new Vector2(children[i].transform.position.x - targetPos.x, children[i].transform.position.y - targetPos.y);
                Vector2 toTarget = -diff.normalized;
                children[i].transform.Translate(toTarget * speed);
            }
        }

        if (CheckChildrenEmpty())
        {
            currentCommand = "";
        }
    }

    void SetLeader()
    {
        leader = ReturnLeader();
    }

    public bool Empty()
    {
        if (children.Count <= 0)
        {
            return true;
        }
        return false;
    }
}
