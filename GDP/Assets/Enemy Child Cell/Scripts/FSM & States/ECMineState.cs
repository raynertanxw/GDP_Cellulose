using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECMineState : IECState {

	//A vector 2 variable to store the target position for the landmine to move to
	private Vector2 m_TargetPosition;
	
	//2 booleans to track whether the enemy child cell had reach the target landmine position 
	// or the explosion of the enemy child cell had started
	private bool bReachPosition;
	private bool bExplosionStart;
	
	//a float variable that store the speed of movement for the enemy child cell in this state
	private float fSpeed;

	//Constructor
    public ECMineState(GameObject _childCell, EnemyChildFSM _ecFSM)
    {
		m_Child = _childCell;
		m_ecFSM = _ecFSM;
		m_Main = m_ecFSM.m_EMain;
    }

    public override void Enter()
    {
		//Refresh the pointDatabase with the latest information of the game environment
		PointDatabase.Instance.RefreshDatabase(m_Main.transform.position, m_ecFSM.m_PMain.transform.position ,GameObject.Find("Left Wall"));
		
		//Retrieve the best target position to move to based on the request for position type and other game information
		m_TargetPosition = PositionQuery.Instance.RequestLandminePos(DetermineType(), m_Main, m_ecFSM.m_PMain);
		
		fSpeed = 12f;
		bReachPosition = false;
		bExplosionStart = false;
    }

    public override void Execute()
    {
		//if the enemy child cell had not reach the target position, continue float towards it.
		//Once it reached, just float forward slowly
		if(!HasCellReachTarget(m_TargetPosition) && bReachPosition == false)
		{
			FloatTowards(m_TargetPosition);
		}
		else if(HasCellReachTarget(m_TargetPosition) && bReachPosition == false)
		{
			bReachPosition = true;
			FloatForward();
		}
		
		//if the enemy child cell is colliding with a player cell, start the explosion
		if(IsCollidingWithPlayerCell() && bExplosionStart == false)
		{
			m_ecFSM.StartChildCorountine(ExplodeCorountine());
			bExplosionStart = true;
		}
		
		//if the enemy child cell has collided with a player cell, explode the enemy child cell
		if(bExplosionStart == true && HasCollidedWithPlayerCells())
		{
			ExplodeDestroy();
		}
    }

    public override void Exit()
    {
		
    }
    
    //a function that return the type of position the landmine it should take based on the aggressiveness of 
    //the enemy main cell
    private PositionType DetermineType()
    {
		float fEMainAggressiveness = m_Main.GetComponent<EnemyMainFSM>().CurrentAggressiveness;

		if(fEMainAggressiveness >= 12)
		{
			Debug.Log("Aggressive");
			return PositionType.Aggressive;
		}
		else if(fEMainAggressiveness <= 6)
		{
			Debug.Log("Defensive");
			return PositionType.Defensive;
		}
		Debug.Log("Neutral");
		return PositionType.Neutral;
    }
    
	//a function that direct the enemy child cell towards a gameObject by changing its velocity through calculation
	private void FloatTowards(Vector2 _TargetPos)
	{
		Vector2 m_TargetPos = _TargetPos;
		Vector2 m_Difference = new Vector2(m_Child.transform.position.x- m_TargetPos.x, m_Child.transform.position.y - m_TargetPos.y);
		Vector2 m_Direction = -m_Difference.normalized;
		
		m_Child.GetComponent<Rigidbody2D>().velocity = m_Direction * fSpeed;
		fSpeed -= 0.2f;
		fSpeed = Mathf.Clamp(fSpeed,1f,6f);
	}
	
	//a function that direct the enemy child cell downward slowly 
	private void FloatForward()
	{
		m_Child.GetComponent<Rigidbody2D>().velocity = Vector2.down;
	}
	
	//A function that return a boolean that show whether the cell had reached the given position in the perimeter
	private bool HasCellReachTarget (Vector2 _TargetPos)
	{
		if (Vector2.Distance(m_Child.transform.position, _TargetPos) <= 0.1f)
		{
			return true;
		}
		return false;
	}
	
	//a function that return a boolean on whether the enemy child cell is collding with any player cell
	private bool IsCollidingWithPlayerCell()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, 1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		
		if(m_SurroundingObjects.Length <= 0)
		{
			return false;
		}
		
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			if(m_SurroundingObjects[i] != null && (m_SurroundingObjects[i].tag == Constants.s_strPlayerChildTag || m_SurroundingObjects[i].tag == Constants.s_strPlayerTag))
			{
				return true;
			}
		}
		
		return false;
	}
	
	//a function that return a boolean on whether the enemy child cell had collided with any player cell
	private bool HasCollidedWithPlayerCells()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position, 0.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		if(m_SurroundingObjects.Length <= 0)
		{
			return false;
		}
		
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			if(m_SurroundingObjects[i] != null && (m_SurroundingObjects[i].tag == Constants.s_strPlayerChildTag || m_SurroundingObjects[i].tag == Constants.s_strPlayerTag))
			{
				return true;
			}
		}
		
		return false;
	}
	
	//a function for any potential sound effects or visual effects to be played 
	//during explosions (to be implemented in the future)
	private void ExplodeSetup()
	{
		
	}
	
	//Go through all the surround cells, destroy any player child cells and damaing the player main cell if in range
	private void ExplodeDestroy()
	{
		Collider2D[] m_SurroundingObjects = Physics2D.OverlapCircleAll(m_Child.transform.position,1.5f * m_Child.GetComponent<SpriteRenderer>().bounds.size.x);
		for(int i = 0; i < m_SurroundingObjects.Length; i++)
		{
			//if the player child cell is within the exploding range, kill the player child
			if(m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerChildTag)
			{
				m_SurroundingObjects[i].GetComponent<PlayerChildFSM>().DeferredChangeState(PCState.Dead);
			}
			//if the player main cell is within the exploding range, damage the player main
			else if(m_SurroundingObjects[i].gameObject.tag == Constants.s_strPlayerTag)
			{
				m_SurroundingObjects[i].GetComponent<PlayerMain>().HurtPlayerMain();
			}
		}
		
		//After all the damaging and killing is done, transition the enemy child to a dead state
		MessageDispatcher.Instance.DispatchMessage(m_Child, m_Child,MessageType.Dead,0);
	}
	
	//a corountine for the cell explosion
	IEnumerator ExplodeCorountine()
	{
		//play explode sound/animation whatever
		ExplodeSetup();
		
		yield return new WaitForSeconds(1.5f);
		
		ExplodeDestroy();
	}
}
