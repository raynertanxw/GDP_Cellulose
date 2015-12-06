using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// IExecuteOnceable.cs: Acts as a identifier for single-execution methods
public class IExecuteOnceable 
{
    public static IExecuteOnceable EndOfMethod;
}

// MethodHandler.cs: Aims to provide more controlled method execution
public class MethodHandler : MonoBehaviour 
{
    // Uneditable Fields
    private static List<Func<IExecuteOnceable>> s_list_ZeroArgumentMethod = new List<Func<IExecuteOnceable>>();
    private static List<OneArgumentMethod> s_list_OneArgumentMethod = new List<OneArgumentMethod>();
    private static List<TwoArgumentMethod> s_list_TwoArgumentMethod = new List<TwoArgumentMethod>();
    private static List<ThreeArgumentMethod> s_list_ThreeArgumentMethod = new List<ThreeArgumentMethod>();

    // Private Functions
    // Update(): is called every frame
    void Update()
    {
        // while: Runs all the methods with zero arguments
        while (s_list_OneArgumentMethod.Count > 0)
        {
            s_list_ZeroArgumentMethod[0]();
            s_list_ZeroArgumentMethod.RemoveAt(0);
        }
        // while: Runs all the methods with one argument
        while (s_list_OneArgumentMethod.Count > 0)
        {
            s_list_OneArgumentMethod[0].Method(s_list_OneArgumentMethod[0].Argument);
            s_list_OneArgumentMethod.RemoveAt(0);
        }
        // while: Runs all the method with two arguments
        while (s_list_TwoArgumentMethod.Count > 0)
        {
            s_list_TwoArgumentMethod[0].Method(s_list_TwoArgumentMethod[0].Argument1, s_list_TwoArgumentMethod[0].Argument2);
            s_list_TwoArgumentMethod.RemoveAt(0);
        }
        // while: Runs all the method with three arguments
        while (s_list_ThreeArgumentMethod.Count > 0)
        {
            s_list_ThreeArgumentMethod[0].Method(s_list_ThreeArgumentMethod[0].Argument1, s_list_ThreeArgumentMethod[0].Argument2, s_list_ThreeArgumentMethod[0].Argument3);
            s_list_ThreeArgumentMethod.RemoveAt(0);
        }
    }

    // Public Static Functions
    /// <summary>
    /// Limits the method to only be able to execute once in the current frame
    /// </summary>
    /// <param name="_method"> The method to be executed once </param>
    /// <returns></returns>
    public static bool ExecuteOnce(Func<IExecuteOnceable> _method)
    {
        for (int i = 0; i < s_list_ZeroArgumentMethod.Count; i++)
        {
            if (s_list_ZeroArgumentMethod[i] == _method)
                return false;
        }
        s_list_ZeroArgumentMethod.Add(_method);
        return true;
    }

    /// <summary>
    /// Limits the method to only be able to execute once in the current frame
    /// </summary>
    /// <param name="_method"> The method to be executed once </param>
    /// <param name="_argument"> The parameter to be passed in </param>
    /// <returns></returns>
    public static bool ExecuteOnce(Func<object, IExecuteOnceable> _method, object _argument)
    {
        for (int i = 0; i < s_list_OneArgumentMethod.Count; i++)
        {
            if (s_list_OneArgumentMethod[i].Method == _method)
                return false;
        }
        s_list_OneArgumentMethod.Add(new OneArgumentMethod(_method, _argument));
        return true;
    }

    /// <summary>
    /// Limits the method to only be able to execute once in the current frame
    /// </summary>
    /// <param name="_method"> The method to be executed once </param>
    /// <param name="_argument1"> The first parameter to be passed in </param>
    /// <param name="_argument2"> The second parameter to be passed in </param>
    /// <returns></returns>
    public static bool ExecuteOnce(Func<object, object, IExecuteOnceable> _method, object _argument1, object _argument2)
    {
        for (int i = 0; i < s_list_TwoArgumentMethod.Count; i++)
        {
            if (s_list_TwoArgumentMethod[i].Method == _method)
                return false;
        }
        s_list_TwoArgumentMethod.Add(new TwoArgumentMethod(_method, _argument1, _argument2));
        return true;
    }

    /// <summary>
    /// Limits the method to only be able to execute once in the current frame
    /// </summary>
    /// <param name="_method"> The method to be executed once </param>
    /// <param name="_argument1"> The first parameter to be passed in </param>
    /// <param name="_argument2"> The second parameter to be passed in </param>
    /// <param name="_argument3"> The third parameter to be passed in </param>
    /// <returns></returns>
    public static bool ExecuteOnce(Func<object, object, object, IExecuteOnceable> _method, object _argument1, object _argument2, object _argument3)
    {
        for (int i = 0; i < s_list_ThreeArgumentMethod.Count; i++)
        {
            if (s_list_ThreeArgumentMethod[i].Method == _method)
                return false;
        }
        s_list_ThreeArgumentMethod.Add(new ThreeArgumentMethod(_method, _argument1, _argument2, _argument3));
        return true;
    }
}

// OneArgumentMethod.cs: Handles one argument methods
public class OneArgumentMethod
{
    public OneArgumentMethod(Func<object, IExecuteOnceable> _method, object _argument)
    {
        Method = _method;
        Argument = _argument;
    }

    public Func<object, IExecuteOnceable> Method;
    public object Argument;
}

// TwoArgumentMethod.cs: Handles two arguments methods
public class TwoArgumentMethod
{
    public TwoArgumentMethod(Func<object, object, IExecuteOnceable> _method, object _argument1, object _argument2)
    {
        Method = _method;
        Argument1 = _argument1;
        Argument2 = _argument2;
    }

    public Func<object, object, IExecuteOnceable> Method;
    public object Argument1;
    public object Argument2;
}

// ThreeArgumentMethod.cs: Handles three arguments methods
public class ThreeArgumentMethod
{
    public ThreeArgumentMethod(Func<object, object, object, IExecuteOnceable> _method, object _argument1, object _argument2, object _argument3)
    {
        Method = _method;
        Argument1 = _argument1;
        Argument2 = _argument2;
        Argument3 = _argument3;
    }

    public Func<object, object, object, IExecuteOnceable> Method;
    public object Argument1;
    public object Argument2;
    public object Argument3;
}
