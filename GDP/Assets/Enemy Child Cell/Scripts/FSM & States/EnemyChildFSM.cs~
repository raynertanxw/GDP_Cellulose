using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyChildFSM : MonoBehaviour
{
	//3 gameobjects variables to store the player main cell, enemy main cell and the target for this child
	//cell to charge towards
	public GameObject m_PMain;
	public GameObject m_EMain;
	private static Node_Manager NodeLeft;
	private static Node_Manager NodeRight;
	public GameObject m_ChargeTarget;
	private static EnemyMainFSM EMFSM;
	private static EMController EMControl;

	//3 variables to store the current state, the enumeration of the current state and the current command for
	//the child cell
	private IECState m_CurrentState;
	private ECState m_CurrentEnum;
	private MessageType m_CurrentCommand;
	
	public bool bHitWall;

	//declare a dictoary to store various IECstate with the key being ECState
	private Dictionary<ECState,IECState> m_StatesDictionary;

	private float fRotationTarget;
	private bool bRotateCW;
	private bool bRotateACW;

	void Start()
	{
		//Initialize the variables and data structure
		fRotationTarget = Random.Range(0f,360f);
		bRotateCW = false;
		bRotateACW = false;
		m_PMain = GameObject.Find("Player_Cell");
		m_EMain = GameObject.Find("Enemy_Cell");
		EMFSM = m_EMain.GetComponent<EnemyMainFSM>();
		EMControl = m_EMain.GetComponent<EMController>();
		NodeLeft = Node_Manager.GetNode(Node.LeftNode);
		NodeRight = Node_Manager.GetNode(Node.RightNode);
		
		m_ChargeTarget = null;
		m_StatesDictionary = new Dictionary<ECState,IECState>();

		//Initialize the various states for the enemy child cell and added them into the dictionary
		m_StatesDictionary.Add(ECState.Idle, new ECIdleState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Defend, new ECDefendState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Avoid, new ECAvoidState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Attack, new ECAttackState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.ChargeMain, new ECChargeMState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.ChargeChild, new ECChargeCState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.TrickAttack, new ECTrickAttackState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Landmine, new ECMineState(this.gameObject,this));
		m_StatesDictionary.Add(ECState.Dead, new ECDeadState(this.gameObject,this));

		//initialize the current state for the enemy child cell
		m_CurrentState = m_StatesDictionary[ECState.Dead];
		m_CurrentEnum = ECState.Dead;
		m_CurrentCommand = MessageType.Empty;
		CurrentState.Enter();
	}

	void Update()
	{
		m_CurrentState.Execute();
		UpdateState();

		if((m_CurrentEnum == ECState.Idle || m_CurrentEnum == ECState.Defend) && EMControl.bIsMainBeingAttacked && EMControl.bShouldMainTank)
		{
			AutoAvoid();
		}
		else if(m_CurrentEnum == ECState.Idle && EMControl.bIsMainBeingAttacked && !IsEMTanking() && !IsThereEnoughDefence())
		{
			AutoDefend();
		}
		else if(m_CurrentEnum == ECState.Idle && !ECDefendState.ReturningToMain && ECDefendState.bThereIsDefenders)
		{
			ReinforceDefence();
		}
	}

	void FixedUpdate()
	{
		m_CurrentState.FixedExecute();
	}

	//Various getter functionss
	public Dictionary<ECState,IECState> StateDictionary
	{
		get { return m_StatesDictionary; }
	}

	public GameObject Target
	{
		get { return m_ChargeTarget; }
		set { m_ChargeTarget = value; }
	}

	public MessageType Command
	{
		get { return m_CurrentCommand; }
		set { m_CurrentCommand = value; }
	}

	public IECState CurrentState
	{
		get { return m_CurrentState; }

	}

	public ECState CurrentStateEnum
	{
		get { return m_CurrentEnum; }
	}

	//a function to change the enemy child state and make the appropriate changes to the enemy child cell
	public void ChangeState(ECState _state)
	{
		m_CurrentState.Exit();
		m_CurrentState = m_StatesDictionary[_state];
		m_CurrentEnum = _state;
		m_CurrentState.Enter();
	}

	//a function to update the enemy child state based on the currentcommand variable in the enemy child FSM
	private void UpdateState()
	{
		if(m_CurrentCommand == MessageType.Empty)
		{
			return;
		}

		if (m_CurrentCommand == MessageType.Avoid)
		{
			ChangeState(ECState.Avoid);
		}
		else if (m_CurrentCommand == MessageType.Attack)
		{
			ChangeState(ECState.Attack);
		}
		else if(m_CurrentCommand == MessageType.ChargeChild)
		{
			ChangeState(ECState.ChargeChild);
		}
		else if(m_CurrentCommand == MessageType.ChargeMain)
		{
			ChangeState(ECState.ChargeMain);
		}
		else if(m_CurrentCommand == MessageType.TrickAttack)
		{
			ChangeState(ECState.TrickAttack);
		}
		else if (m_CurrentCommand == MessageType.Dead)
		{
			ChangeState(ECState.Dead);
		}
		else if (m_CurrentCommand == MessageType.Defend)
		{
			ChangeState(ECState.Defend);
		}
		else if (m_CurrentCommand == MessageType.Idle)
		{
			ChangeState(ECState.Idle);
		}
		else if (m_CurrentCommand == MessageType.Landmine)
		{
			ChangeState(ECState.Landmine);
		}

		m_CurrentCommand = MessageType.Empty;
	}

	//a function for player cells to kill this child cell by changing it to the dead state
	public void KillChildCell()
	{
		ChangeState(ECState.Dead);
	}

	/*private bool IsMainBeingAttacked()
	{
		Collider2D[] IncomingObjects;
		PCState PCCurrentState = PCState.Dead;
		
		//Incoming objects to main
		IncomingObjects = Physics2D.OverlapCircleAll(m_EMain.transform.position, 100f * m_EMain.GetComponent<SpriteRenderer>().bounds.size.x,Constants.s_onlyPlayerChildLayer);
		if(IncomingObjects.Length <= 0){return false;}
		
		for(int i = 0; i < IncomingObjects.Length; i++)
		{
			PCCurrentState = IncomingObjects[i].GetComponent<PlayerChildFSM>().GetCurrentState();
			if(PCCurrentState == PCState.ChargeChild || PCCurrentState == PCState.ChargeMain)
			{
				return true;
			}
		}
		return false;
	}*/

	private bool IsThereEnoughDefence()
	{
		List<PlayerChildFSM> Attackers = NodeLeft.DEPRECIATED_GetNodeChildList();
		Attackers.AddRange(NodeRight.DEPRECIATED_GetNodeChildList());
		List<EnemyChildFSM> Defenders = EMFSM.ECList;
		PCState PCCurrentState = PCState.Idle;
		int AttackerAmount = 0;
		int DefenderAmount = 0;

		for(int i = 0; i < Attackers.Count; i++)
		{
			PCCurrentState = Attackers[i].GetCurrentState();
			if(PCCurrentState == PCState.ChargeChild || PCCurrentState == PCState.ChargeMain)
			{
				AttackerAmount++;
			}
		}

		for(int i = 0; i < Defenders.Count; i++)
		{
			if(Defenders[i].CurrentStateEnum == ECState.Defend)
			{
				DefenderAmount++;
			}
		}

		return (DefenderAmount > AttackerAmount) ? true : false;
	}
	
	private bool IsThereDefenders()
	{
		List<EnemyChildFSM> ECList = EMFSM.ECList;
		for(int i = 0; i < ECList.Count; i++)
		{
			if(ECList[i].CurrentStateEnum == ECState.Defend)
			{
				return true;
			}
		}
		return false;
		
		/*foreach(EnemyChildFSM EC in EMFSM.ECList)
		{
			if(EC.CurrentStateEnum == ECState.Defend)
			{
				return true;
			}
		}
		return false;*/
	}

	private void ReinforceDefence()
	{
		//Change this cell to defend state
		MessageDispatcher.Instance.DispatchMessage(gameObject,gameObject,MessageType.Defend,0);
		
		//Add it into the formation as an reinforcement
		FormationDatabase.Instance.AddNewDefenderToCurrentFormation(gameObject);
	}

	private void AutoDefend()
	{
		Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(gameObject.transform.position, 3 * GetComponent<SpriteRenderer>().bounds.size.x/2, Constants.s_onlyEnemeyChildLayer);

		//Dispatch a message to all nearby enemy child cells that are idling to defend the main cell
		for(int i = 0; i < NearbyObjects.Length; i++)
		{
			if(NearbyObjects[i].GetComponent<EnemyChildFSM>().CurrentStateEnum == ECState.Idle)
			{
				MessageDispatcher.Instance.DispatchMessage(gameObject,NearbyObjects[i].gameObject,MessageType.Defend,0);
			}
		}
	}

	private void AutoAvoid()
	{
		Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(gameObject.transform.position, 3 * GetComponent<SpriteRenderer>().bounds.size.x/2, Constants.s_onlyEnemeyChildLayer);
		ECState ECCurrentState = ECState.Idle;

		//Dispatch a message to all nearby enemy child cells that are idling to defend the main cell
		for(int i = 0; i < NearbyObjects.Length; i++)
		{
			ECCurrentState = NearbyObjects[i].GetComponent<EnemyChildFSM>().CurrentStateEnum;
			if(ECCurrentState == ECState.Idle || ECCurrentState == ECState.Defend)
			{
				MessageDispatcher.Instance.DispatchMessage(gameObject,NearbyObjects[i].gameObject,MessageType.Avoid,0);
			}
		}
	}

	//two functions to start and stop corountines that are called from the child states
	public void StartChildCorountine(IEnumerator _childCorountine)
	{
		StartCoroutine(_childCorountine);
	}

	public void StopChildCorountine(IEnumerator _childCorountine)
	{
		StopCoroutine(_childCorountine);
	}

	public IEnumerator PassThroughDeath(float _Time)
	{
		GetComponent<Rigidbody2D>().drag = 0f;
		GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * 0.75f,GetComponent<Rigidbody2D>().velocity.y);

		yield return new WaitForSeconds(_Time);
		MessageDispatcher.Instance.DispatchMessage(this.gameObject,this.gameObject,MessageType.Dead,0);
	}

	public void RotateToHeading()
	{
		Vector2 Heading = gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
		float Rotation = -Mathf.Atan2(Heading.x, Heading.y) * Mathf.Rad2Deg;
		gameObject.GetComponent<Rigidbody2D>().MoveRotation(Rotation);
	}

	public void RandomRotation(float _RotateSpeed)
	{
		if(bRotateCW == false && bRotateACW == false)
		{
			if(fRotationTarget > gameObject.transform.eulerAngles.z)
			{
				bRotateCW = true;
			}
			else
			{
				bRotateACW = true;
			}
		}

		//Debug.Log(gameObject.name + "'s info: " + bRotateCW + " , " + bRotateACW + " , " + gameObject.transform.eulerAngles.z + " , " + fRotationTarget);

		if(bRotateCW && !bRotateACW && gameObject.transform.eulerAngles.z >= fRotationTarget || !bRotateCW && bRotateACW && gameObject.transform.eulerAngles.z <= fRotationTarget)
		{
			bRotateCW = !bRotateCW;
			bRotateACW = !bRotateACW;
			fRotationTarget = Random.Range(0f,360f);
		}

		if(bRotateCW && !bRotateACW && gameObject.transform.eulerAngles.z < fRotationTarget)
		{
			//Debug.Log("Rotate CW: " + gameObject.name);
			gameObject.transform.eulerAngles += new Vector3(0f,0f,_RotateSpeed);
		}
		else if(!bRotateCW && bRotateACW && gameObject.transform.eulerAngles.z > fRotationTarget)
		{
			//Debug.Log("Rotate ACW: " + gameObject.name);
			gameObject.transform.eulerAngles -= new Vector3(0f,0f,_RotateSpeed);
		}
	}

	private bool IsEMTanking()
	{
		List<EnemyChildFSM> Children = EMFSM.ECList;
		for(int i = 0; i < Children.Count; i++)
		{
			if(Children[i].CurrentStateEnum == ECState.Avoid)
			{
				return true;
			}
		}
		return false;
	}

	public bool OutOfBound()
	{
		Vector2 ScreenBottom = new Vector2(0f, -Screen.height);
		if(gameObject.transform.position.x < -4.5f || gameObject.transform.position.x > 4.5f || gameObject.transform.position.y <= ScreenBottom.y)
		{
			return true;
		}

		return false;
	}

	public bool HitBottomOfScreen()
	{
		BoxCollider2D[] Walls = GameObject.Find("Wall").GetComponents<BoxCollider2D>();
		BoxCollider2D BotWall = null;
		float LowestY = Mathf.Infinity;

		for(int i = 0; i < Walls.Length; i++)
		{
			Vector2 WallOrigin = Walls[i].transform.position;
			WallOrigin += Walls[i].offset;
			if(WallOrigin.y < LowestY)
			{
				BotWall = Walls[i];
				LowestY = WallOrigin.y;
			}
		}

		Vector2 Bottom = new Vector2(0f, BotWall.transform.position.y + BotWall.offset.y + BotWall.bounds.size.y/2);
		if(transform.position.y <= Bottom.y + 0.4f)
		{
			return true;
		}
		return false;
	}

	public bool IsHittingSideWalls()
	{
		if(transform.position.x <= -4.4f || transform.position.x >= 4.4f)
		{
			return true;
		}
		return false;
	}
}
