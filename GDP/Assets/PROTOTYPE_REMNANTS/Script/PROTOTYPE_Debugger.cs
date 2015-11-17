using UnityEngine;
using System.Collections;

public static class PROTOTYPE_Debugger{

    public static void CheckEmpty<type>(type t)
    {
        if (t == null)
        {
            Debug.Log("CheckEmpty: It's empty");
            return;
        }
        Debug.Log("CheckEmpty: It's not empty");
    }

}
