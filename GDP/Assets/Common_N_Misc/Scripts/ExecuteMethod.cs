using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ExecuteMethod : MonoBehaviour
{
    /* ExecuteMethod.cs API - Everything you need to know about SquadCaptain
 * ------------------------------------------------------------------------------------------------------------------------------
 * Static Functions:
 * - bool OnceInUpdate: Control the execution of a method to be called only once every update, handles multiple calling of the same method
 * ------------------------------------------------------------------------------------------------------------------------------
 */

    private static List<MethodDetails> s_list_methodDetails = new List<MethodDetails>();
	
	// Update(): is called once per frame
	void Update ()
    {
        while (s_list_methodDetails.Count > 0)
        {
            MethodDetails methodDetails = s_list_methodDetails[0];
            methodDetails.MethodName.Invoke(methodDetails.Instance, methodDetails.Parameters);
            s_list_methodDetails.RemoveAt(0);
        }
    }

    // Public-Static Functions
    /// <summary>
    /// Execute the method once every frame, prevents method to be called multiple times
    /// </summary>
    /// <param name="_methodString"> The method to be executed in string format </param>
    /// <param name="_parameters"> The array of parameters to executed with the method </param>
    /// <param name="_instance"> The instance of the object that the method is called upon, if the method is static, this will be ignored </param>
    public static bool OnceInUpdate(string _methodString, object[] _parameters, object _instance)
    {
        MethodDetails methodDetails = new MethodDetails(_methodString, _parameters, _instance);
        for (int i = 0; i < s_list_methodDetails.Count; i++)
        {
            if (s_list_methodDetails.Contains(methodDetails))
                return false;
        }
        s_list_methodDetails.Add(methodDetails);
        return true;
    }
}

public struct MethodDetails
{
    private Type methodType;
    private MethodInfo methodName;
    private object[] parameters;
    private object instance;

    public MethodDetails(string _str_methodString, object[] _parameters, object _instance)
    {
        // for: Gets the type from the method string
        for (int i = 0; i < _str_methodString.Length; i++)
        {
            if (_str_methodString[i] == '.')
            {
                char[] c = new char[i];
                _str_methodString.CopyTo(0, c, 0, i);
                methodType = Type.GetType(new string(c));
                char[] d = new char[_str_methodString.Length - i];
                _str_methodString.CopyTo(i + 1, d, 0, _str_methodString.Length - i - 1);
                methodName = methodType.GetMethod(new string(d));

                parameters = _parameters;
                instance = _instance;
                return;
            }
        }

        methodType = null;
        methodName = null;
        parameters = null;
        instance = null;
        Debug.LogWarning("MethodDetails.MethodDetails(): Cannot get method type. methodType = " + _str_methodString + "(Possibly missing a '.'?)");
    }

    public Type MethodType { get { return methodType; } }
    public MethodInfo MethodName { get { return methodName; } }
    public object[] Parameters { get { return parameters; } }
    public object Instance { get { return instance; } }
}
