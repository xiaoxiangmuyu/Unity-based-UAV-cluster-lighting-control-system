using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class VirusProcesser : IDataProcesser
{
    public List<string> beginPoints = new List<string>();
    public bool IsSingleDir;
    [HideIf("IsSingleDir")]
    [ReadOnly]
    public float searchRadius;
    public float findInterval=0.05f;
    // List<string> tempNames;
    // List<float> tempTimes;
    public override bool Process(ref RecordData data, float animTime)
    {
        searchRadius=0;
        mainCamera = Camera.main;
        isProcessed = false;
        tempNames = new List<string>();
        tempTimes = new List<float>();
        for (int i = 0; i < beginPoints.Count; i++)
        {
            if (!data.objNames.Contains(beginPoints[i]))
            {
                Debug.LogError(beginPoints[i] + "不存在于这个数据组");
                return false;
            }
        }
        while (searchRadius <= 20)
        {
            tempNames.Clear();
            tempTimes.Clear();
            this.data=new RecordData();
            this.data.CopyFrom(data);
            FindNext(beginPoints, 0);
            if (tempNames.Count != data.objNames.Count)
            {
                searchRadius += 0.1f;
            }
            else
                break;
        }
        // while(tempNames.Count!=data.ObjNames.Count)
        // {
        //     searchRadius+=1;
        //     FindNext(beginPoints,0);
        //     if(searchRadius>=20)
        //     break;
        // }
        data.objNames = tempNames;
        data.times = tempTimes;
        ProcessComplete();
        isProcessed = true;
        Debug.Log("处理完成");
        return true;
    }
    void FindNext(List<string> pointNames, float time)
    {
        for (int i = 0; i < pointNames.Count; i++)
        {
            // if (!data.ObjNames.Contains(pointNames[i]))
            // {
            //     Debug.LogError(pointNames[i] + "不存在于这个数据组");
            //     return;
            // }
            if (!tempNames.Contains(pointNames[i]))
            {
                data.objNames.Remove(pointNames[i]);
                tempNames.Add(pointNames[i]);
                tempTimes.Add(time);
            }
        }
        time += findInterval;
        var allNames = new List<string>();
        for (int j = 0; j < pointNames.Count; j++)
        {
            var info = ProjectManager.GetGlobalPosInfoByGroup(data.groupName);
            Vector3 worldPos = info.posList[int.Parse(pointNames[j]) - 1];
            //Vector2 screenPos=mainCamera.WorldToScreenPoint(worldPos);
            Vector3 tempWorldPos;
            //Vector2 tempScreenPos;
            Dictionary<string, float> dis = new Dictionary<string, float>();
            for (int i = 0; i < data.objNames.Count; i++)
            {
                if (pointNames.Contains(data.objNames[i]))
                    continue;
                //tempScreenPos=mainCamera.WorldToScreenPoint(tempWorldPos);
                tempWorldPos = info.posList[int.Parse(data.objNames[i]) - 1];
                dis.Add(data.objNames[i], Vector3.Distance(worldPos, tempWorldPos));
            }
            if (dis.Count == 0)
                return;
            var names = GetNearPoints(dis);
            foreach (var name in names)
            {
                if (!allNames.Contains(name))
                    allNames.Add(name);
            }
        }
        if (allNames.Count != 0)
            FindNext(allNames, time);
    }
    List<string> GetNearPoints(Dictionary<string, float> dis)
    {
        var temp = new List<string>();
        float minDistance = 1000;
        string minName = "";
        if (!IsSingleDir)
        {
            foreach (var pair in dis)
            {
                if (pair.Value <= searchRadius)
                {
                    temp.Add(pair.Key);
                }
            }

        }
        else
        {
            foreach (var pair in dis)
            {
                if (pair.Value <= minDistance)
                {
                    minDistance = pair.Value;
                    minName = pair.Key;
                }
            }
            temp.Add(minName);
        }
        return temp;
    }
}


