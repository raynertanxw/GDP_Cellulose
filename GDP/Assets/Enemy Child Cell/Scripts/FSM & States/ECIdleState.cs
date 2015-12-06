using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECIdleState : IECState
{
	//various float variables for flocking purposes to direct the enemy child cell
	private float fAlignStrength;
	private float fCohesionStrength;
	private float fSeperateStrength;
	private float fWanderStrength;
	private float fWanderRadius;
	private float fWanderDistance;
	private float fWanderJitter;
	
	//Booleans to track whether the enemy child cell is wondering or reach the initial position
	private bool bIsWondering;
	private bool bReachInitialPos;
	private Vector2 m_InitialTarget;
	
	//Constructor
	public ECIdleState(GameObject _childCell, EnemyChildFSM _ecFSM)
	{
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
	}
	
	//Initialize all of the variables and generate the initial postion for the wandering behavior of the enemy
	//child cell. It will then start the corountine for the enemy child cell to wandering around the enemy main
	//cell.
	public override void Enter()
	{
		fAlignStrength = 0.0f;
		fCohesionStrength = 0.0f;
		fSeperateStrength = 0.0f;
		fWanderStrength = 0.0f;
		fWanderRadius = 2.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x / 2;
		fWanderDistance = 2f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x;
		fWanderJitter = 2f;
		bReachInitialPos = false;
		
		m_InitialTarget = GenerateRandomPos();
		bIsWondering = true;
		m_ecFSM.StartChildCorountine(Wandering());
	}
	
	public override void Execute()
	{
		if (bIsWondering == true && (HittingWall(m_Child.transform.position) || LeavingIdleRange(m_Child.transform.position)))
		{
			m_Child.GetComponent<Rigidbody2D>().velocity = GenerateInverseVelo(m_Child.GetComponent<Rigidbody2D>().velocity);
		}
		
		/*
        //Implementation of different group movement behaviour (Further testing is needed to implement)//
        
        //if this child cell is the only child cell in the main cell it is not wondering now, start the wandering process
        if (m_Main.GetComponent<EnemyMainFSM>().ECList.Count <= 1 && bIsWondering == false)
        {
            bIsWondering = true;
            m_ecFSM.StartChildCorountine(Wandering());
        }
        else if (m_Main.GetComponent<EnemyMainFSM>().ECList.Count > 1)
        {
            if (bIsWondering == true)
            {
                bIsWondering = false;
            }

            float veloX = Alignment().x * fAlignStrength + Cohesion().x * fCohesionStrength + Seperation().x * fSeperateStrength + Wander().x * fWanderStrength;
			float veloY = Alignment().y * fAlignStrength + Cohesion().y * fCohesionStrength + Seperation().y * fSeperateStrength + Wander().y * fWanderStrength;

            Vector2 velocity = new Vector2(veloX, veloY);
            m_Child.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y + m_ecFSM.eMain.GetComponent<Rigidbody2D>().velocity.y);
        }

        //if the child cell is not wandering but it is going to leave the idle range or hit the wall, reverse its velocity
        if (bIsWondering == false && (HittingWall(m_Child.transform.position) || LeavingIdleRange(m_Child.transform.position)))
        {
			m_Child.GetComponent<Rigidbody2D>().velocity = GenerateInverseVelo(m_Child.GetComponent<Rigidbody2D>().velocity);
        }
        */
		
	}
	
	public override void Exit()
	{
		//When exiting from idle state, exit the corountine of wandering
		m_ecFSM.StopChildCorountine(this.Wandering());
		bIsWondering = false;
	}
	
	//a function to find all the nearby enemy child cells around this enemy child cell and return an array of
	//enemy child cells for it.
	private GameObject[] TagNeighbours()
	{
		Collider2D[] GOaround = Physics2D.OverlapCircleAll(m_Child.transform.position, 5 * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		GameObject[] neighbours = new GameObject[GOaround.Length];
		int count = 0;
		for (int i = 0; i < GOaround.Length; i++)
		{
			if (GOaround[i].gameObject.tag == "EnemyChild")
			{
				neighbours[count] = GOaround[i].gameObject;
				count++;
			}
		}
		return neighbours;
	}
	
	//A function to generate a random vector position based on the game environment
	private Vector2 GenerateRandomPos()
	{
		float minX = GameObject.Find("Left Wall").transform.position.x + GameObject.Find("Left Wall").GetComponent<SpriteRenderer>().bounds.size.x / 2 + m_Child.GetComponent<SpriteRenderer>().bounds.size.x / 2;
		float maxX = GameObject.Find("Right Wall").transform.position.x - GameObject.Find("Right Wall").GetComponent<SpriteRenderer>().bounds.size.x / 2 - m_Child.GetComponent<SpriteRenderer>().bounds.size.x / 2;
		float minY = m_Main.transform.position.y - 1.5f * m_Main.GetComponent<SpriteRenderer>().bounds.size.x;
		float maxY = m_Main.transform.position.y + 1.5f * m_Main.GetComponent<SpriteRenderer>().bounds.size.x;
		
		return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
	}
	
	//a function to generate an inverse version of the inputted velocity through the perimeter
	private Vector2 GenerateInverseVelo(Vector2 velo)
	{
		Vector2 inverse = velo;
		
		Vector2 diff = new Vector2(m_Child.transform.position.x - m_Main.transform.position.x, m_Child.transform.position.y - m_Main.transform.position.y);
		inverse = -diff.normalized;
		
		return inverse;
	}
	
	//a function that direct the enemy child cell towards a gameObject by changing its velocity through calculation
	private void MoveTowards(Vector2 target)
	{
		Vector2 targetPos = m_Child.transform.position;
		Vector2 difference = new Vector2(target.x - m_Child.transform.position.x, target.y - m_Child.transform.position.y);
		Vector2 direction = difference.normalized;
		Vector2 towards = new Vector2(m_Child.transform.position.x + direction.x * m_ecFSM.fSpeed, m_Child.transform.position.y + direction.y * m_ecFSM.fSpeed);
		m_Child.GetComponent<Rigidbody2D>().MovePosition(towards);
	}
	
	//a function that return a boolean to show whether the current position of the enemy child agent is getting
	//closer to the wall
	private bool HittingWall(Vector2 pos)
	{
		GameObject LWall = GameObject.Find("Left Wall");
		GameObject RWall = GameObject.Find("Right Wall");
		float fWallWidth = LWall.GetComponent<SpriteRenderer>().bounds.size.x/2;
		float fCellWidth = m_Child.GetComponent<SpriteRenderer>().bounds.size.x/2;
		
		if(m_Child.transform.position.x <= LWall.transform.position.x + fWallWidth + fCellWidth || m_Child.transform.position.x >= RWall.transform.position.x - fWallWidth - fCellWidth)
		{
			return true;
		}
		return false;
	}
	
	//a function that return a boolean to show whether the current position of the enemy child agent is getting
	//out of the idle range
	private bool LeavingIdleRange(Vector2 pos)
	{
		float maxY = m_Main.transform.position.y + m_Main.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 2.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x;
		float minY = m_Main.transform.position.y - m_Main.GetComponent<SpriteRenderer>().bounds.size.x / 2 - 2.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x;
		float maxX = m_Main.transform.position.x + m_Main.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x;
		float minX = m_Main.transform.position.x - m_Main.GetComponent<SpriteRenderer>().bounds.size.x / 2 - 5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x;
		
		if (pos.y > maxY || pos.y < minY || pos.x > maxX || pos.x < minX)
		{
			return true;
		}
		return false;
	}
	
	//a function that return a velocity to drive the agent back towards the idle range
	private Vector2 ReturnToIdleRange()
	{
		Vector2 target = GenerateRandomPos();
		Vector2 diff = new Vector2(target.x - m_Child.transform.position.x, target.y - m_Child.transform.position.y);
		diff.Normalize();
		return new Vector2(diff.x, diff.y + m_Main.GetComponent<Rigidbody2D>().velocity.y);
	}
	
	//A corountine to drive the wandering movement of the enemy child cells
	public IEnumerator Wandering()
	{
		bool bRecover = false;
		
		//while the enemy child cell is still wandering and no other group movement method is affect the enemy child cell,
		//repeat the following:
		// - check if the enemy child cell is hitting the wall or leaving the idle range
		// - if it is, start the recovering process whereby the enemy child cell will generate an inverse version of
		//   it's velocity to drive the enemy child cell back towards the enemy main cell
		// - else, continue the wander process by generating the velocity to drive the enemy child cell using the
		//   "Wander" function
		
		while (bIsWondering == true && fCohesionStrength == 0.0f && fSeperateStrength == 0.0f && fAlignStrength == 0.0f)
		{
			if (HittingWall(m_Child.transform.position) || LeavingIdleRange(m_Child.transform.position))
			{
				bRecover = true;
			}
			
			if (bRecover == true)
			{
				if (Vector2.Distance(m_Child.transform.position, m_Main.transform.position) <= m_Child.GetComponent<SpriteRenderer>().bounds.size.x + m_Main.GetComponent<SpriteRenderer>().bounds.size.x / 2)
				{
					bRecover = false;
				}
				
				Vector2 current = m_Child.GetComponent<Rigidbody2D>().velocity;
				Vector2 inverse = GenerateInverseVelo(current);
				inverse.y *= 1.2f;
				m_Child.GetComponent<Rigidbody2D>().velocity = inverse;
				yield return new WaitForSeconds(0.1f);
			}
			else if (bRecover == false)
			{
				Vector2 velo = Wander();
				velo.y += m_ecFSM.m_EMain.GetComponent<Rigidbody2D>().velocity.y;
				m_Child.GetComponent<Rigidbody2D>().velocity = velo;
				float duration = Random.Range(0.5f, 1f);
				yield return new WaitForSeconds(duration);
			}
			
		}
		bIsWondering = false;
		yield break;
	}
	
	//a function that return a velocity to direct the enemy child cell to wander around the enemy main cell
	private Vector2 Wander()
	{
		Vector2 m_target = new Vector2(m_Child.transform.position.x + Random.Range(-6f, 6f), m_Child.transform.position.y + Random.Range(-1f, 1f));
		Vector2 projection = m_target.normalized * fWanderRadius;
		projection += new Vector2(0f, fWanderDistance);
		projection *= Random.Range(-1f, 1f);
		projection.x += Random.Range(-0.3f, 0.3f);
		return projection.normalized;
	}
	
	//a function to return a steering force to direct the enemy child cell to align with the average rotation
	//of all the nearby enemy child cells
	private Vector2 Alignment()
	{
		GameObject[] neighbours = TagNeighbours();
		int neighbourCount = 0;
		Vector2 steering = new Vector2(0f, 0f);
		
		foreach (GameObject cell in neighbours)
		{
			if (cell != null && cell != m_Child)
			{
				steering.x += cell.GetComponent<Rigidbody2D>().velocity.x;
				steering.y += cell.GetComponent<Rigidbody2D>().velocity.y;
				neighbourCount++;
			}
		}
		
		if (neighbourCount <= 0)
		{
			return steering;
		}
		else
		{
			steering /= neighbourCount;
			steering.Normalize();
			return steering;
		}
	}
	
	//a function that return a velocity to direct the enemy child cell to seperate from other enemy child cell
	private Vector2 Seperation()
	{
		GameObject[] neighbours = TagNeighbours();
		int neighbourCount = 0;
		Vector2 steering = new Vector2(0f, 0f);
		
		foreach (GameObject cell in neighbours)
		{
			if (cell != null && cell != m_Child)
			{
				steering.x += cell.transform.position.x - m_Child.transform.position.x;
				steering.y += cell.transform.position.y - m_Child.transform.position.y;
				neighbourCount++;
			}
		}
		
		if (neighbourCount <= 0)
		{
			return steering;
		}
		else
		{
			steering /= neighbourCount;
			steering *= -1f;
			steering.Normalize();
			return steering;
		}
	}
	
	//a function that return a velocity to direct the enemy child cell to gather all the other enemy child cell
	private Vector2 Cohesion()
	{
		GameObject[] neighbours = TagNeighbours();
		int neighbourCount = 0;
		Vector2 steering = new Vector2(0f, 0f);
		
		foreach (GameObject cell in neighbours)
		{
			if (cell != null && cell != m_Child)
			{
				steering.x += cell.transform.position.x;
				steering.y += cell.transform.position.y;
				neighbourCount++;
			}
		}
		
		if (neighbourCount <= 0)
		{
			return steering;
		}
		else
		{
			steering /= neighbourCount;
			steering = new Vector2(steering.x - m_Child.transform.position.x, steering.y - m_Child.transform.position.y);
			steering.Normalize();
			return steering;
		}
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachPosition(Vector2 pos)
	{
		if (Vector2.Distance(m_Child.transform.position, pos) < 0.01f)
		{
			return true;
		}
		return false;
	}
}