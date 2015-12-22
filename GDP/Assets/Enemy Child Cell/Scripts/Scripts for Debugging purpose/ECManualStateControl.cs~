using UnityEngine;
using System.Collections;

public class ECManualStateControl : MonoBehaviour {

	//A class that is used for debugging and testing whether the Enemy Child states is working.
	//Can toggle all of the enemy child cell on screen to change states by pressing 1/2/3/4/5

	void Update () 
	{
		//if this enemy child cell is not dead and 1/2/3/4/5 is pressed, transition this enemy child cell
		//to a specific state
		if(gameObject.GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
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
			if(Input.GetKeyDown(KeyCode.Alpha4))
			{
				MessageDispatcher.Instance.DispatchMessage(gameObject,gameObject,MessageType.Landmine,0);
			}
			if(Input.GetKeyDown(KeyCode.Alpha5))
			{
				MessageDispatcher.Instance.DispatchMessage(gameObject,gameObject,MessageType.TrickAttack,0);
			}
		}
	}
}
