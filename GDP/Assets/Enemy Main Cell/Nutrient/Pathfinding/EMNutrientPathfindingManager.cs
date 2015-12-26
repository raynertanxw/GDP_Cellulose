using UnityEngine;
using System.Collections;

public class EMNutrientPathfindingManager : MonoBehaviour 
{
	private Transform startPos, endPos;
	public EnemyNutrientNode startNode { get; set; }
	public EnemyNutrientNode goalNode { get; set; }
	
	public ArrayList pathArray;						// ArrayList stores the path
	
	GameObject startObject, goalObject;
	
	private float elapsedTime = 0.0f;				// Timer to pause refind the path
	public float intervalTime = 1.0f; 				// Interval time between path finding

	void Start () 
	{
		// Start object is the current object and the goal object is the enemy main cell
		startObject = this.gameObject;
		goalObject = GameObject.FindGameObjectWithTag(Constants.s_strEnemyTag);
		// Find a path by default
		pathArray = new ArrayList();
		FindPath();
	}

	void Update () 
	{
		#region Find path every few seconds depending on the interval time
		elapsedTime += Time.deltaTime;
		
		if(elapsedTime >= intervalTime)
		{
			elapsedTime = 0.0f;
			FindPath();
		}
		#endregion
	}
	
	void FindPath()
	{
		startPos = startObject.transform;
		endPos = goalObject.transform;
		
		// Initialize start and goal node
		startNode = new EnemyNutrientNode(MapManager.instance.GetNodeCenter(MapManager.instance.GetGridIndex((Vector2)startPos.position)));
		goalNode = new EnemyNutrientNode(MapManager.instance.GetNodeCenter(MapManager.instance.GetGridIndex((Vector2)endPos.position)));
		// Update the size and position of all obstacles in map
		MapManager.instance.GetObstacles ();
		// Find a new path between the start and goal node
		pathArray = EnemyNutrientAStar.FindPath(startNode, goalNode);
	}

	// Draw the path found 
	void OnDrawGizmos()
	{
		if (pathArray == null) {
			return;
		}

		if (pathArray.Count > 0)
		{
			int index = 1;
			foreach (EnemyNutrientNode node in pathArray)
			{
				if (index < pathArray.Count)
				{
					EnemyNutrientNode nextNode = (EnemyNutrientNode)pathArray[index];
					Debug.DrawLine(node.position, nextNode.position, Color.green);
					index++;
				}
			};
		}
	}
}