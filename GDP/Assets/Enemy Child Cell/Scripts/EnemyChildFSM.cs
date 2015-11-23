using UnityEngine;
using System.Collections;

public class EnemyChildFSM : MonoBehaviour {

    private int nIndex;
    public float fSpeed;
    private bool bIsMine;
    public GameObject pMain;
    public GameObject eMain;
    private IECState currentState;
    private ECIdleState idleState;
    private ECDefendState defendState;
    private ECAvoidState avoidState;
    private ECChargeCState chargeCState;
    private ECChargeMState chargeMState;
    private ECDeadState deadState;
    private ECMineState mineState;

    private MessageType currentCommand;

    public MessageType Command
    {
        get { return currentCommand; }
        set { currentCommand = value; }
    }

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {

	}
}
