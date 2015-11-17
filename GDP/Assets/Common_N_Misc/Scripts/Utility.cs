using UnityEngine;
using System.Collections;

public static class Utility
{
    public static void CheckEmpty<varType>(varType variable)
    {
        if (variable == null)
        {
            Debug.Log("EmptyTest: Variable is empty");
        }
        else
        {
            Debug.Log("EmptyTest: Variable is not empty");
        }
    }
}
