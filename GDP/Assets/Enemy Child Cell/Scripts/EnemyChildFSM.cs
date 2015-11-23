using UnityEngine;
using System.Collections;

public class EnemyChildFSM : MonoBehaviour {

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
