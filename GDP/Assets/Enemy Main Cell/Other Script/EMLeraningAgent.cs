using UnityEngine;
using System.Collections;

public class EMLeraningAgent : MonoBehaviour 
{
	EnemyMainFSM m_EMFSM;

	void Start ()
	{
		m_EMFSM = GetComponent<EnemyMainFSM> ();
	}

	void Update ()
	{

	}

	private void LearningElement ()
	{

	}

	private void Critic ()
	{

	}
}