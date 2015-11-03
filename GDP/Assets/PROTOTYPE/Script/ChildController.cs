using UnityEngine;
using System.Collections;

public class ChildController : MonoBehaviour {

    [SerializeField]
    private bool isLeader;
    private Squad_Manager squadManager;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setSquadManager(Squad_Manager SM)
    {
        squadManager = SM;
    }

    public Squad_Manager ReturnSquadManager()
    {
        return squadManager;
    }

    public void SetLeader()
    {
        isLeader = true;
    }

    public bool ReturnIsLeader()
    {
        return isLeader;
    }
}
