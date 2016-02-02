using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyChildFSM : MonoBehaviour
{
	public GameObject m_PMain;
	public GameObject m_EMain;
	public GameObject m_ChargeTarget;
	public GameObject m_AttackTarget;
	private Rigidbody2D m_Rigidbody2D;
	private static EnemyMainFSM m_EMFSM;
	private static EMController m_EMControl;
	
	private IECState m_CurrentState;
	private ECState m_CurrentEnum;
	private MessageType m_CurrentCommand;
	private AudioSource m_AudioSource;

	private Dictionary<ECState,IECState> m_StatesDictionary;

	private float m_fRotationTarget;
	private bool m_bRotateCW;
	private bool m_bRotateACW;
	public bool m_bHitWall;
	

	void Start()
	{
		//Initialize the variables and data structure
		m_fRotationTarget = Random.Range(0f,360f);
		m_bRotateCW = false;
		m_bRotateACW = false;
		m_PMain = GameObject.Find("Player_Cell");
		m_EMain = GameObject.Find("Enemy_Cell");
		m_EMFSM = m_EMain.GetComponent<EnemyMainFSM>();
		m_EMControl = m_EMain.GetComponent<EMController>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		
		m_ChargeTarget = null;
		m_StatesDictionary = new Dictionary<ECState,IECState>();
		
		m_AudioSource = GetComponent<AudioSource>();

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

		if((m_CurrentEnum == ECState.Idle || m_CurrentEnum == ECState.Defend) && m_EMControl.bIsMainBeingAttacked && m_EMControl.bShouldMainTank)
		{
			AutoAvoid();
		}
		else if(m_CurrentEnum == ECState.Idle && m_EMControl.bIsMainBeingAttacked && !IsEMTanking() && !IsThereEnoughDefence())
		{
			AutoDefend();
		}
		else if(m_CurrentEnum == ECState.Idle && !ECDefendState.ReturningToMain && ECDefendState.m_bThereIsDefenders)
		{
			ReinforceDefence();
		}
		
		if(m_EMFSM.CurrentStateIndex == EMState.Die && CurrentStateEnum != ECState.TrickAttack && CurrentStateEnum != ECState.Dead)
		{
			MessageDispatcher.Instance.DispatchMessage(gameObject,gameObject,MessageType.TrickAttack,0f);
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
	
	public Rigidbody2D rigidbody2D
	{
		get{ return m_Rigidbody2D;}
	}
	
	public AudioSource Audio
	{
		get{ return m_AudioSource;}
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

	private bool IsThereEnoughDefence()
	{
		int AttackerAmount = 0;
		int DefenderAmount = ECTracker.s_Instance.DefendCells.Count;

		for (int i = 0; i < Settings.s_nEnemyChildCountCap; i++)
		{
			if (PlayerChildFSM.s_playerChildStatus[i] == pcStatus.Attacking)
			{
				AttackerAmount++;
			}
		}
		return (DefenderAmount > AttackerAmount) ? true : false;
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
		rigidbody2D.drag = 0f;
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x * 0.75f,rigidbody2D.velocity.y);

		yield return new WaitForSeconds(_Time);
		MessageDispatcher.Instance.DispatchMessage(this.gameObject,this.gameObject,MessageType.Dead,0);
	}

	public void RotateToHeading()
	{
		Vector2 Heading = m_Rigidbody2D.velocity.normalized;
		float Rotation = -Mathf.Atan2(Heading.x, Heading.y) * Mathf.Rad2Deg;
		m_Rigidbody2D.MoveRotation(Rotation);
	}

	public void RandomRotation(float _RotateSpeed)
	{
		if(m_bRotateCW == false && m_bRotateACW == false)
		{
			if(m_fRotationTarget > gameObject.transform.eulerAngles.z)
			{
				m_bRotateCW = true;
			}
			else
			{
				m_bRotateACW = true;
			}
		}

		//Debug.Log(gameObject.name + "'s info: " + bRotateCW + " , " + bRotateACW + " , " + gameObject.transform.eulerAngles.z + " , " + fRotationTarget);

		if(m_bRotateCW && !m_bRotateACW && gameObject.transform.eulerAngles.z >= m_fRotationTarget || !m_bRotateCW && m_bRotateACW && gameObject.transform.eulerAngles.z <= m_fRotationTarget)
		{
			m_bRotateCW = !m_bRotateCW;
			m_bRotateACW = !m_bRotateACW;
			m_fRotationTarget = Random.Range(0f,360f);
		}

		if(m_bRotateCW && !m_bRotateACW && gameObject.transform.eulerAngles.z < m_fRotationTarget)
		{
			gameObject.transform.eulerAngles += new Vector3(0f,0f,_RotateSpeed);
		}
		else if(!m_bRotateCW && m_bRotateACW && gameObject.transform.eulerAngles.z > m_fRotationTarget)
		{
			gameObject.transform.eulerAngles -= new Vector3(0f,0f,_RotateSpeed);
		}
	}

	private bool IsEMTanking()
	{
		List<EnemyChildFSM> Children = m_EMFSM.ECList;
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
