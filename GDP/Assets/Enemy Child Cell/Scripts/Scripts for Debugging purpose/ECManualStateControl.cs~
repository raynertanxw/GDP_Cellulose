using UnityEngine;
using System.Collections;

public class ECManualStateControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			MessageDispatcher.Instance.DispatchMessage(gameObject,gameObject,MessageType.Idle,0);
		}
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			MessageDispatcher.Instance.DispatchMessage(gameObject,gameObject,MessageType.Attack,0);
		}
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			MessageDispatcher.Instance.DispatchMessage(gameObject,gameObject,MessageType.Defend,0);
		}
	}
}
