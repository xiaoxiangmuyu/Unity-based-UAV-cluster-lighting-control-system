using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class CircleProcesser_3D : IDataProcesser
{
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_X = 0.5f;
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_Y = 0.5f;
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_Z = 0.5f;
    Vector3 anchorPoint;
    public override bool Process(ref RecordData data, float animTime = 1)
    {
        if (animTime == 0)
        {
            Debug.LogError("animTime is 0");
            return false;
        }
        isProcessed = false;
        this.data = data;
        mainCamera = Camera.main;
        float? xMax = null;
        float? xMin = null;
        float? yMax = null;
        float? yMin = null;
        float? zMax = null;
        float? zMin = null;
        tempPosDic = new StringVector3Dictionary();
        foreach (var pointName in data.objNames)
        {
            var info = ProjectManager.GetGlobalPosInfoByGroup(data.groupName);
            var pos = info.posList[int.Parse(pointName) - 1];
            tempPosDic.Add(pointName, pos);
        }
        foreach (var pos in tempPosDic.Values)
        {
            if (!xMax.HasValue || pos.x > xMax.Value)
                xMax = pos.x;
            if (!xMin.HasValue || pos.x < xMin.Value)
                xMin = pos.x;
            if (!yMax.HasValue || pos.y > yMax.Value)
                yMax = pos.y;
            if (!yMin.HasValue || pos.y < yMin.Value)
                yMin = pos.y;
            if (!zMax.HasValue || pos.z > zMax.Value)
                zMax = pos.z;
            if (!zMin.HasValue || pos.z < zMin.Value)
                zMin = pos.z;
        }
        anchorPoint = new Vector3(xMin.Value + (xMax.Value - xMin.Value) * center_X, yMin.Value + (yMax.Value - yMin.Value) * center_Y, zMin.Value + (zMax.Value - zMin.Value) * center_Z);
        float maxDistance = 0;
        foreach (var pos in tempPosDic.Values)
        {
            if (Vector3.Distance(anchorPoint, pos) > maxDistance)
                maxDistance = Vector3.Distance(anchorPoint, pos);
        }
        timer = 0;
        tempNames = new List<string>();
        tempTimes = new List<float>();
        index = new List<string>();
        float processPercent = 0;
        float value = 0;
        while (processPercent <= 1)
        {
            value = DOVirtual.EasedValue(0, maxDistance, processPercent, easeType);
            OnValueUpdate(value, animTime);
            processPercent += 0.04f / animTime;
            if (processPercent > 1)
            {
                OnValueUpdate(maxDistance, animTime);
                break;
            }
        }
        return true;
    }
    void OnValueUpdate(float value, float animTime)
    {
        //Debug.Log(value);
        foreach (var pointName in tempPosDic.Keys)
        {
            if (index.Contains(pointName))
                continue;
            float tempDistance = Vector3.Distance(tempPosDic[pointName], anchorPoint);
            if (tempDistance <= value)
            {
                tempTimes.Add(timer);
                tempNames.Add(pointName);
                index.Add(pointName);
            }
        }
        timer += 0.04f;
        timer = Mathf.Min(timer, animTime);
        if (index.Count == data.objNames.Count)
        {
            if (isProcessed)
                return;
            data.objNames = tempNames;
            data.times = tempTimes;
            ProcessComplete();
            isProcessed = true;
            tempPosDic.Clear();
            Debug.Log("处理完成");
        }
    }

}
