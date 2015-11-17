using UnityEngine;
using System.Collections;

public class PROTOTYPE_ChildController : MonoBehaviour {

    [SerializeField]
    private bool isLeader;
	private PROTOTYPE_Squad_Manager squadManager;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setSquadManager(PROTOTYPE_Squad_Manager SM)
    {
        squadManager = SM;
    }

	public PROTOTYPE_Squad_Manager ReturnSquadManager()
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
