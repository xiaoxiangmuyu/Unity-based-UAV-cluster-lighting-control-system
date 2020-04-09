using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
public static class MyTools
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
    public static void UpdateDuring(GameObject obj)
    {
        PlayableDirector playableDirector = obj.GetComponentInParent<PlayableDirector>();
        // if(!playableDirector)
        // playableDirector=obj.GetComponentInParent<PlayableDirector>();
        var timeLineAsset = playableDirector.playableAsset as TimelineAsset;
        foreach (var track in timeLineAsset.GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                var temp=clip.asset as RecordAsset;
                if(temp!=null)
                {
                    clip.duration=temp.GetDuring();
                }
                else
                {
                    var temp2=clip.asset as TxtAnimAsset;
                    if(temp2!=null)
                    clip.duration=temp2.totalFrameCount/25+temp2.safeSeconds;
                    else
                    {
                        var temp3=clip.asset as OverallAsset;
                        clip.duration=temp3.processTimes*temp3.processInterval+temp3.GetDuring();
                    }
                }
            }

        }
    }


}
