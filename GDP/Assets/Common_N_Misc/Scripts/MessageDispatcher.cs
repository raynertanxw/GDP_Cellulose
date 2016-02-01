﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Telegram
{
    public GameObject Sender;
    public GameObject Receiver;
    public MessageType Msg;
    public double dDispatchTime;
}

public class MessageDispatcher {

    private List<Telegram> m_PriorityList;
    private static MessageDispatcher s_Instance;

    public MessageDispatcher()
    {
        m_PriorityList = new List<Telegram>();
    }

    public static MessageDispatcher Instance
    {
        get
        {
			if(s_Instance == null)
            {
				s_Instance = new MessageDispatcher();
            }
			return s_Instance;
        }
    }

    public void DispatchMessage(GameObject _Sender, GameObject _Receiver, MessageType _Msg, double _Delay)
    {
        Telegram telegram = new Telegram();
		telegram.Sender = _Sender;
		telegram.Receiver = _Receiver;
		telegram.Msg = _Msg;
		telegram.dDispatchTime = _Delay;

		if (_Delay <= 0.0)
        {
			Discharge(_Receiver, telegram);
        }
        else
        {
            double dCurrentTime = Time.time;
			telegram.dDispatchTime = dCurrentTime + _Delay;
			m_PriorityList.Add(telegram);
            SortListByDelay();
        }
    }

    public void DispatchDelayedMessages()
    {
        double dCurrentTime = Time.time;
		if (m_PriorityList[0].dDispatchTime < dCurrentTime && m_PriorityList[0].dDispatchTime > 0.0)
        {
			Telegram telegram = m_PriorityList[0];

			Discharge(telegram.Receiver, telegram);

			m_PriorityList.Remove(m_PriorityList[0]);
        }
    }

    public void Discharge(GameObject _receiver, Telegram _msg)
    {
		if (_receiver.HandleMessage(_msg) == false)
        {
            Debug.Log("Message cannot be send due to error !");
        }
    }

    private void SortListByDelay()
    {
		int size = m_PriorityList.Count;
        List<Telegram> sortResult = new List<Telegram>();

        while (sortResult.Count < size)
        {
			double shortestDelay = m_PriorityList[0].dDispatchTime;
            Telegram closest = new Telegram();

			for (int i = 0; i < m_PriorityList.Count; i++)
            {
				if (m_PriorityList[i].dDispatchTime < shortestDelay)
                {
					shortestDelay = m_PriorityList[i].dDispatchTime;
					closest = m_PriorityList[i];
                }
            }

            sortResult.Insert(0, closest);
        }

		m_PriorityList = sortResult;
    }
}
