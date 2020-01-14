using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public static float GetTotalTime(List<ColorOrderBase> orders)
    {
        double temp = ProcessOrder(orders);
        return float.Parse(temp.ToString("f2"));
    }
    static double ProcessOrder(List<ColorOrderBase> orders)
    {
        double totalTime = 0;
        if (orders == null)
        {
            Debug.LogError("命令列表为空");
            return 0;
        }
        foreach (var order in orders)
        {
            if (order == null)
            {
                Debug.LogError("命令为空!");
                return 0;
            }
            if (order is Interval)
            {
                Interval temp = order as Interval;
                totalTime += temp.during;
            }
            else if (order is OrderGroup)
            {
                var temp = (OrderGroup)order;
                double tempTime = 0;
                for (int i = 0; i < temp.playCount; i++)
                {
                    tempTime += (ProcessOrder(temp.colorOrders));
                }
                totalTime += tempTime;
            }
            else if (order is DoColor)
            {
                var temp = (DoColor)order;
                for (int i = 0; i < temp.playCount; i++)
                {
                    totalTime += temp.during;
                }
            }
        }
        return totalTime;
    }
}
