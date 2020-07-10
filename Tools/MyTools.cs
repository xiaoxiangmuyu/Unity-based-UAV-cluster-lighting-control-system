using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
public class MyTools
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
                if(!temp.Random)
                totalTime += temp.during;
                else
                totalTime+=temp.range.y;
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
                var temp = clip.asset as ControlBlock;
                if (temp != null)
                {
                    temp.Register();
                    if (temp.GetDuring() == 0)
                        clip.duration = 3;
                    else
                        clip.duration = temp.GetDuring();
                    if(temp.targetDataName!=string.Empty)
                    clip.displayName=temp.targetDataName;
                }
                else
                {
                    var temp2 = clip.asset as TxtAnimAsset;
                    if (temp2 != null)
                    {
                        clip.duration = temp2.seconds;
                        temp2.SetStartFrame(Mathf.RoundToInt((float)clip.start * 25));
                    }
                    else
                    {
                        var temp3 = clip.asset as OverallAsset;
                        if (temp3 != null)
                        {
                            clip.duration=temp3.GetDuring();
                            temp3.RefreshObjs();
                        }
                    }
                }
            }

        }
    }
    public static List<GameObject> FindObjs(List<string> names)
    {
        List<GameObject> objects = new List<GameObject>();
        Transform parent = ProjectManager.GetCurrentMR().transform;
        foreach (var name in names)
        {
            MyTools.FindChild(parent, name);
            if (tempObj == null)
                Debug.LogError("没有找到" + name);
            objects.Add(tempObj.gameObject);
        }
        return objects;

    }
    static Transform tempObj;
    static void FindChild(Transform tran, string childName)
    {
        Transform target = tran.Find(childName);
        if (target)
        {
            tempObj = target;
            return;
        }
        else
        {
            for (int i = 0; i < tran.childCount; i++)
            {
                FindChild(tran.GetChild(i), childName);
            }
        }
    }

}
