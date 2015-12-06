using UnityEngine;
using System.Collections;

public static class GameObjectExtension
{
    public static bool HandleMessage(this GameObject _GO, Telegram _Telegram)
    {
		//_GO being the reciever of the message and _Telegram being the message to be delivered to the reciever
		
		//If the sender and reciever of the message is not given, return false in the handling of message
		if (_Telegram.Sender == null && _Telegram.Receiver == null || _GO == null)
        {
            return false;
        }

        //Add additional "if" statements if other FSM require the usage of Telegram. However, 
        //you will need to add a Telegram variable into that FSM script so this function can assign 
        //sent messages to that FSM for them to process the message
        
		if (_GO.tag == "EnemyChild")
        {
			_GO.GetComponent<EnemyChildFSM>().Command = _Telegram.Msg;
        }

        return true;
    }
}
