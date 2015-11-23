using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Telegram
{
    public GameObject sender;
    public GameObject receiver;
    public MessageType msg;
    public double dDispatchTime;
}

public class MessageDispatcher {

    private List<Telegram> priorityList;
    private static MessageDispatcher instance;

    public MessageDispatcher()
    {
        priorityList = new List<Telegram>();
    }

    public static MessageDispatcher Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new MessageDispatcher();
            }
            return instance;
        }
    }

    public void DispatchMessage(GameObject sender, GameObject receiver, MessageType msg, double dDelay)
    {
        Telegram telegram = new Telegram();
        telegram.sender = sender;
        telegram.receiver = receiver;
        telegram.msg = msg;
        telegram.dDispatchTime = dDelay;

        if (dDelay <= 0.0)
        {
            Discharge(receiver, telegram);
        }
        else
        {
            double currentTime = Time.time;
            telegram.dDispatchTime = currentTime + dDelay;
            priorityList.Add(telegram);
            SortListByDelay();
        }
    }

    public void DispatchDelayedMessages()
    {
        double currentTime = Time.time;
        if (priorityList[0].dDispatchTime < currentTime && priorityList[0].dDispatchTime > 0.0)
        {
            Telegram telegram = priorityList[0];

            Discharge(telegram.receiver, telegram);

            priorityList.Remove(priorityList[0]);
        }
    }

    public void Discharge(GameObject receiver, Telegram msg)
    {
        if (receiver.HandleMessage(msg) == false)
        {
            Debug.Log("Message cannot be send due to error !");
        }
    }

    private void SortListByDelay()
    {
        int size = priorityList.Count;
        List<Telegram> sortResult = new List<Telegram>();

        while (sortResult.Count < size)
        {
            double shortestDelay = priorityList[0].dDispatchTime;
            Telegram closest = new Telegram();

            for (int i = 0; i < priorityList.Count; i++)
            {
                if (priorityList[i].dDispatchTime < shortestDelay)
                {
                    shortestDelay = priorityList[i].dDispatchTime;
                    closest = priorityList[i];
                }
            }

            sortResult.Insert(0, closest);
        }

        priorityList = sortResult;
    }
}
