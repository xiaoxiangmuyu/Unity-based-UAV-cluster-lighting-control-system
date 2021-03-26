using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
public class MyTools
{
    public static bool RandomTool(float value)
    {
        var result = Random.Range(0f, 1f);
        return result <= value;
    }
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
                if (!temp.Random)
                    totalTime += temp.during;
                else
                    totalTime += (temp.range.y - temp.range.x) * 0.8f;
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
            else if (order is Function.GlobalGradientColor)
            {
                var temp = (Function.GlobalGradientColor)order;
                totalTime += temp.time;
            }
        }
        return totalTime;
    }
    public static void UpdateClipDuring(GameObject obj)
    {
        PlayableDirector playableDirector = obj.GetComponentInParent<PlayableDirector>();
        // if(!playableDirector)
        // playableDirector=obj.GetComponentInParent<PlayableDirector>();
        var timeLineAsset = playableDirector.playableAsset as TimelineAsset;
        foreach (var track in timeLineAsset.GetOutputTracks())
        {
            if (track.muted)
                continue;
            foreach (var clip in track.GetClips())
            {
                var temp = clip.asset as ControlBlock;
                if (temp != null)
                {
                    temp.Register();
                    if (temp.GetDuring() == 0)
                        clip.duration = 3;
                    else if (!temp.isDynamicPos)
                        clip.duration = temp.GetDuring();
                    else
                        clip.duration = temp.GetDuring() * temp.processTimes;
                    if (temp.targetDataName != string.Empty)
                        clip.displayName = temp.targetDataName;
                    temp.SetColorIndex();
                }
                else
                {
                    var temp2 = clip.asset as TxtAnimAsset;
                    if (temp2 != null)
                    {
                        clip.duration = temp2.seconds + temp2.safeSeconds;
                        temp2.SetStartFrame(Mathf.RoundToInt((float)clip.start * 25));
                        clip.displayName = temp2.animName;
                    }
                    else
                    {
                        // var temp3 = clip.asset as OverallAsset;
                        // if (temp3 != null)
                        // {
                        //     clip.duration = temp3.GetDuring();
                        //     temp3.RefreshObjs();
                        // }
                    }
                }
            }

        }
    }
    // public static List<GameObject> FindObjs(PointIndexInfo pointsInfo)
    // {
    //     List<GameObject> objects = new List<GameObject>();
    //     Transform parent = ProjectManager.GetPointsRoot().transform;
    //     List<string> names = new List<string>(ProjectManager.FindPointsByPos(pointsInfo));
    //     foreach (var name in names)
    //     {
    //         MyTools.FindChild(parent, name);
    //         if (tempObj == null)
    //             Debug.LogError("没有找到" + name);
    //         objects.Add(tempObj.gameObject);
    //     }
    //     return objects;

    // }
    public static List<GameObject> FindObjs(List<string> names)
    {
        List<GameObject> objects = new List<GameObject>();
        Transform parent = ProjectManager.GetPointsRoot().transform;
        foreach (var name in names)
        {
            MyTools.FindChild(parent, name);
            if (tempObj == null)
                Debug.LogError("没有找到" + name);
            objects.Add(tempObj.gameObject);
        }
        return objects;

    }
    public static List<string> FindNamesByPosList(List<Vector3> posList, string animName)
    {
        var animation = ProjectManager.FindAnimByName(animName);
        var newNames = new List<string>(animation.FindPointNamesByPos(posList));
        return newNames;
    }
    // public static List<string> FindNamesByCurrentNames(List<string>curNames,int groupIndex)
    // {
    //     var newNames = new List<string>();
    //     var animation=ProjectManager.GetAnimationbyGroupIndex(groupIndex);
    //     foreach(var name in curNames)
    //     {
    //         int curIndex=int.Parse(name)-1;
    //         newNames.Add(animation.indexs[curIndex].ToString());
    //     }
    //     return newNames;
    // }
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
    static Color EvaluateColor(Color from, Color to, float percent, Ease ease = Ease.Linear)
    {
        return new Color(DOVirtual.EasedValue(from.r, to.r, percent, ease),
        DOVirtual.EasedValue(from.g, to.g, percent, ease),
        DOVirtual.EasedValue(from.b, to.b, percent, ease)
        );
    }
    static float Trunc(float num)
    {
        // var temp = num.ToString("f2");
        //return float.Parse(temp);
        float temp = num * 100;
        //int i=(int)temp;
        int i = Mathf.RoundToInt(temp);//根据泽鹏的建议改成四舍五入
        float result = i / 100f;
        return result;
    }
    public static Vector3 TruncVector3(Vector3 v)
    {
        float x = Trunc(v.x);
        float y = Trunc(v.y);
        float z = Trunc(v.z);

        return new Vector3(x, y, z);
    }
    public static bool VectorSimilar(Vector3 a, Vector3 b, float dis = 1.85f)
    {
        //if(Mathf.Abs(a.x-b.x)<1&&Mathf.Abs(a.y-b.y)<1&&Mathf.Abs(a.z-b.z)<1)
        if (Vector3.Distance(a, b) <= dis)
            return true;
        else
            return false;

    }
    public static void ResfrshTimeLine()
    {
        var obj = ProjectManager.GetPointsRoot();
        var asset = obj.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
        foreach (var track in asset.GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                var temp = clip.asset as ControlBlock;
                if (temp != null)
                {
                    temp.targetDataName = temp.data.dataName;
                    temp.RefreshData();
                }
            }
        }
    }
    public static bool IsSimilar(Vector3 a, Vector3 b)
    {
        var m = (a - b).sqrMagnitude;
        if (m <= 0.01f)
            return true;
        else
        {
            //Debug.LogError("模:" + m.ToString());
            return false;
        }
    }


}
