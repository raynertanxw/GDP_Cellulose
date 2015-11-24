using UnityEngine;
using System.Collections;

public static class GameObjectExtension
{
    public static bool HandleMessage(this GameObject GO, Telegram telegram)
    {
        if (telegram.sender == null && telegram.receiver == null || GO == null)
        {
            return false;
        }

        //Add additional if statements if other FSM require the usage of Telegram. However, 
        //you will need to add a Telegram variable into that FSM script so this function can assign the command given to that FSM
        if (GO.tag == "EnemyChild")
        {
            GO.GetComponent<EnemyChildFSM>().Command = telegram.msg;
            Debug.Log(GO.GetComponent<EnemyChildFSM>().Command.ToString());
        }

        return true;
    }
}
