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
    
	public static void DrawCross(Vector2 pos, Color color, float crossSize)
	{
		if(crossSize == null)
		{
			crossSize = 0.05f;
		}
		
		Vector2 topLeft = new Vector2(pos.x - crossSize, pos.y + crossSize);
		Vector2 topRight = new Vector2(pos.x + crossSize, pos.y + crossSize);
		Vector2 botLeft = new Vector2(pos.x - crossSize, pos.y - crossSize);
		Vector2 botRight = new Vector2(pos.x + crossSize, pos.y - crossSize);
		
		//topleft to botright
		Debug.DrawLine(topLeft,botRight,color,Mathf.Infinity,true);
		Debug.DrawLine(topRight,botLeft,color,Mathf.Infinity,true);
	}
}
