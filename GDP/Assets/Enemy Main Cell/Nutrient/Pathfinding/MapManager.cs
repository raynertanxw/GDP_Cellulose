using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (CircleCollider2D))]
public class MapManager : MonoBehaviour
{
	// Single instance of the MapManager class
	private static MapManager instance = null;
	
	// Singleton of the MapManager instance
	public static MapManager Instance
	{
		get
		{
			if (instance == null)
			{
				// Get the MapManager object
				instance = FindObjectOfType(typeof(MapManager)) as MapManager;
				if (instance == null)
					Debug.LogError("MapManager cannot be found!");
			}
			return instance;
		}
	}
	
	// Destroy the instance when the game is stopped
	void OnApplicationQuit()
	{
		instance = null;
	}
	
	#region Fields
	public int nNumOfRows;
	public int nNumOfColumns;
	public float fUniversalnodeSize;
	public bool bShowGrid = true;
	public bool bShowObstacleBlocks = true;
	
	private Vector2 origin = new Vector2();
	private List<GameObject> obstacleList;
	public EnemyNutrientNode[,] nodes { get; set; }
	#endregion
	
	// Origin of the grid manager
	public Vector2 Origin
	{
		get { return (Vector2)this.gameObject.transform.position; }
	}

	void Awake()
	{
		// Get the list of obstacles objects tagged as "Obstacle"
		obstacleList = new List<GameObject>(GameObject.FindGameObjectsWithTag(Constants.s_strEnemyMainNutrient));
		GetObstacles();
	}

	void Update()
	{
		// Remove empty gameObject from the obstacleList when it is destroyed
		obstacleList.RemoveAll(item => item == null);
	}

	// Get the obstacles in the scene
	public void GetObstacles()
	{
		// Initialise the nodes
		nodes = new EnemyNutrientNode[nNumOfRows, nNumOfColumns];
		
		int index = 0;
		for (int i = 0; i < nNumOfRows; i++)
		{
			for (int j = 0; j < nNumOfColumns; j++)
			{
				Vector2 cellPos = GetNodeCenter(index);
				EnemyNutrientNode node = new EnemyNutrientNode(cellPos);
				nodes[i, j] = node;
				
				index++;
			}
		}
		
		// Set the position for each obstacle
		if (obstacleList != null && obstacleList.Count > 0)
		{
			foreach (GameObject obstacle in obstacleList)
			{
				if (obstacle != null)
				{
					if (IsInBounds (obstacle.transform.position))
					{
						int indexCell = GetGridIndex(obstacle.transform.position);
						int column = GetColumn(indexCell);
						int row = GetRow(indexCell);
						int range = (int)(obstacle.GetComponent<CircleCollider2D> ().bounds.size.x / fUniversalnodeSize);
						
						// Assign unaccessible area
						nodes[row, column].MarkAsUnaccessible();
						/*
						for (int i = row - range; i < row + range; i++)
						{
							for (int j = column - range; j < column + range; j++)
							{
								if (i >= 0 && i < nNumOfRows && j >= 0 && j < nNumOfColumns)
								{
									if (nodes[i, j].bAccessible)
										nodes[i, j].MarkAsUnaccessible();
								}
							}
						}
						*/
					}
				}
			}
		}
	}

	// Return the position of the center of the node
	public Vector2 GetNodeCenter(int index)
	{
		Vector2 cellPosition = GetNodePosition(index);
		cellPosition.x += (fUniversalnodeSize / 2.0f);
		cellPosition.y += (fUniversalnodeSize / 2.0f);
		
		return cellPosition;
	}

	// Return the position of the node
	public Vector2 GetNodePosition(int index)
	{
		int row = GetRow(index);
		int col = GetColumn(index);
		float xPosInGrid = col * fUniversalnodeSize;
		float yPosInGrid = row * fUniversalnodeSize;
		
		return Origin + new Vector2(xPosInGrid, yPosInGrid);
	}

	// Return the grid index of the position passed in 
	public int GetGridIndex(Vector2 pos)
	{
		if (!IsInBounds(pos))
		{
			return -1;
		}
		
		pos -= Origin;
		
		int col = (int)(pos.x / fUniversalnodeSize);
		int row = (int)(pos.y / fUniversalnodeSize);
		
		return (row * nNumOfColumns + col);
	}

	// Return the row number of the index
	public int GetRow(int index)
	{
		int row = index / nNumOfColumns;
		return row;
	}

	// Return the column number of the index
	public int GetColumn(int index)
	{
		int column = index % nNumOfColumns;
		return column;
	}

	// Check whether the current position is inside the grid or not
	public bool IsInBounds(Vector2 position)
	{
		float width = nNumOfColumns * fUniversalnodeSize;
		float height = nNumOfRows * fUniversalnodeSize;
		
		return (position.x >= Origin.x && position.x <= Origin.x + width && position.y <= Origin.y + height && position.y >= Origin.y);
	}

	// Get the  neighouring nodes in 4 cardinal directions
	public void GetNeighbours(EnemyNutrientNode node, ArrayList neighbours)
	{
		Vector2 neighbourPos = node.position;
		int neighborIndex = GetGridIndex(neighbourPos);
		
		int row = GetRow(neighborIndex);
		int column = GetColumn(neighborIndex);

		// Top
		int leftNodeRow = row + 1;
		int leftNodeColumn = column;
		AssignNeighbour(leftNodeRow, leftNodeColumn, neighbours);

		// Bottom
		leftNodeRow = row - 1;
		leftNodeColumn = column;
		AssignNeighbour(leftNodeRow, leftNodeColumn, neighbours);

		// Left
		leftNodeRow = row;
		leftNodeColumn = column - 1;
		AssignNeighbour(leftNodeRow, leftNodeColumn, neighbours);

		// Right
		leftNodeRow = row;
		leftNodeColumn = column + 1;
		AssignNeighbour(leftNodeRow, leftNodeColumn, neighbours);

		// Top left
		leftNodeRow = row + 1;
		leftNodeColumn = column - 1;
		AssignNeighbour(leftNodeRow, leftNodeColumn, neighbours);

		// Top right
		leftNodeRow = row + 1;
		leftNodeColumn = column + 1;
		AssignNeighbour(leftNodeRow, leftNodeColumn, neighbours);

		// Bottom left
		leftNodeRow = row - 1;
		leftNodeColumn = column - 1;
		AssignNeighbour(leftNodeRow, leftNodeColumn, neighbours);

		// Bottom right
		leftNodeRow = row - 1;
		leftNodeColumn = column + 1;
		AssignNeighbour(leftNodeRow, leftNodeColumn, neighbours);
	}

	// Check the neighbouring node. If it is accessible, add the neighbouring node to the neighbour list.
	void AssignNeighbour(int row, int column, ArrayList neighbors)
	{
		if (row >= 0 && column >= 0 && row < nNumOfRows && column < nNumOfColumns)
		{
			EnemyNutrientNode nodeToAdd = nodes[row, column];
			if (nodeToAdd.bAccessible)
			{
				neighbors.Add(nodeToAdd);
			}
		} 
	}

	// Draw debug grid map and unaccessible areas
	void OnDrawGizmos()
	{
		// Draw Grid
		if (bShowGrid)
		{
			DebugDrawGrid(transform.position, nNumOfRows, nNumOfColumns, fUniversalnodeSize, Color.blue);
		}
		
		// Grid Start Position
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.3f);
		
		// Draw Obstacle obstruction
		Gizmos.color = Color.white;
		if (bShowObstacleBlocks)
		{
			Vector2 cellSize = new Vector2(fUniversalnodeSize, fUniversalnodeSize);
			
			if (obstacleList != null && obstacleList.Count > 0)
			{
				/*
				foreach (GameObject data in obstacleList)
				{
					Gizmos.DrawCube(GetNodeCenter(GetGridIndex(data.transform.position)), cellSize);
				}
				*/
				for (int i = 0; i < nNumOfRows; i++)
				{
					for (int j = 0; j < nNumOfColumns; j++)
					{
						if (!nodes[i, j].bAccessible)
							Gizmos.DrawCube(GetNodeCenter(GetGridIndex(nodes[i, j].position)), cellSize);
					}
				}
			}
		}
	}

	// Draw the grid map
	public void DebugDrawGrid(Vector2 origin, int numRows, int numCols, float cellSize, Color color)
	{
		float width = (numCols * cellSize);
		float height = (numRows * cellSize);
		
		// Draw the horizontal lines
		for (int i = 0; i < numRows + 1; i++)
		{
			Vector2 startPos = origin + i * cellSize * new Vector2(0.0f, 1.0f);
			Vector2 endPos = startPos + width * new Vector2(1.0f, 0.0f);
			Debug.DrawLine(startPos, endPos, color);
		}
		
		// Draw the vertial lines
		for (int i = 0; i < numCols + 1; i++)
		{
			Vector2 startPos = origin + i * cellSize * new Vector2(1.0f, 0.0f);
			Vector2 endPos = startPos + height * new Vector2(0.0f, 1.0f);
			Debug.DrawLine(startPos, endPos, color);
		}
	}

	public static void ResetStatics()
	{
		instance = null;
	}
}