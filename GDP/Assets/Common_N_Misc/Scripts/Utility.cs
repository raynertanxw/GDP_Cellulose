using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
	//Utility is a class that is used for debugging purposes wherey any function that is used for debugging 
	//can be added here

	//A function that is used to check whether a variable is empty or not and print different messages 
	//respectively
    public static void CheckEmpty<varType>(varType _Variable)
    {
		if (_Variable == null)
        {
            Debug.Log("EmptyTest: Variable is empty");
        }
        else
        {
            Debug.Log("EmptyTest: Variable is not empty");
        }
    }
    
    //A function that is used to draw a cross on the screen based the position, color and size inputted through 
    //the perimeter
	public static void DrawCross(Vector2 _Pos, Color _Color, float _CrossSize)
	{
		//If there is no size given for the cross, a default size of 0.05f is given to the cross
		if(_CrossSize == null)
		{
			_CrossSize = 0.05f;
		}
		
		//The four corners of the cross are calculated based on the position given and the size of the cross
		Vector2 topLeft = new Vector2(_Pos.x - _CrossSize, _Pos.y + _CrossSize);
		Vector2 topRight = new Vector2(_Pos.x + _CrossSize, _Pos.y + _CrossSize);
		Vector2 botLeft = new Vector2(_Pos.x - _CrossSize, _Pos.y - _CrossSize);
		Vector2 botRight = new Vector2(_Pos.x + _CrossSize, _Pos.y - _CrossSize);
		
		//Draw two lines on the screen from top left to bottom right and from top right to bottom left
		//to form a cross
		Debug.DrawLine(topLeft,botRight,_Color,Mathf.Infinity,true);
		Debug.DrawLine(topRight,botLeft,_Color,Mathf.Infinity,true);
	}
	
	public static void DrawLine(Vector2 _Start, Vector2 _End)
	{
		Debug.DrawLine(_Start,_End,Color.red,Mathf.Infinity,true);
	}
	
	public static void DrawPath(List<Point> _Path, Color _Color, float _CrossSize)
	{
		foreach(Point point in _Path)
		{
			DrawCross(point.Position,_Color,_CrossSize);
		}
	}
	
	public static void DebugVector(Vector2 _Pos)
	{
		Debug.Log(_Pos.ToString("F4"));
	}
	
	public static void DrawCircleCross(Vector2 _Center, float _Radius, Color _Color)
	{
		Debug.DrawLine(new Vector2(_Center.x - _Radius, _Center.y),new Vector2(_Center.x + _Radius, _Center.y),_Color,Mathf.Infinity,true);
		Debug.DrawLine(new Vector2(_Center.x, _Center.y + _Radius),new Vector2(_Center.x, _Center.y - _Radius),_Color,Mathf.Infinity,true);
	}
}
