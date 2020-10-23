using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class CircleProcesser : IDataProcesser
{
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_X=0.5f;
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_Y=0.5f;

    Vector2 anchorPoint;


    public override bool Process(ref RecordData data, float animTime)
    {
        if (animTime == 0)
        {
            Debug.LogError("animTime为0");
            return false;
        }
        isProcessed = false;
        this.data = data;
        mainCamera = Camera.main;
        float? xMax = null;
        float? xMin = null;
        float? yMax = null;
        float? yMin = null;
        tempPosDic=new StringVector3Dictionary();
        foreach(var pointName in data.objNames)
        {
            var info=ProjectManager.GetGlobalPosInfoByGroup(data.groupName);
            var pos=info.posList[int.Parse(pointName)-1];
            tempPosDic.Add(pointName,pos);
        }
        foreach (var pos in tempPosDic.Values)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(pos);
            if (!xMax.HasValue || screenPos.x > xMax.Value)
                xMax = screenPos.x;
            if (!xMin.HasValue || screenPos.x < xMin.Value)
                xMin = screenPos.x;
            if (!yMin.HasValue || screenPos.y < yMin.Value)
                yMin = screenPos.y;
            if (!yMax.HasValue || screenPos.y > yMax.Value)
                yMax = screenPos.y;
        }
        anchorPoint = new Vector2(xMin.Value + (xMax.Value - xMin.Value) * center_X, yMin.Value + (yMax.Value - yMin.Value) * center_Y);
        float maxDistance = 0;
        foreach (var pos in tempPosDic.Values)
        {
            if (Vector2.Distance(anchorPoint, mainCamera.WorldToScreenPoint(pos)) > maxDistance)
                maxDistance = Vector2.Distance(anchorPoint, mainCamera.WorldToScreenPoint(pos));
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
            OnValueUpdate(value,animTime);
            processPercent += 0.04f / animTime;
            if (processPercent > 1)
            {
                OnValueUpdate(maxDistance,animTime);
                break;
            }
        }
        //DOVirtual.Float(0, maxDistance, animTime, OnValueUpdate).SetEase(easeType);
        return true;
    }
    void OnValueUpdate(float value,float animTime)
    {
        //Debug.Log(value);
        foreach (var pointName in tempPosDic.Keys)
        {
            if (index.Contains(pointName))
                continue;
            float tempDistance = Vector2.Distance(mainCamera.WorldToScreenPoint(tempPosDic[pointName]), anchorPoint);
            if (tempDistance <= value)
            {
                tempTimes.Add(timer);
                tempNames.Add(pointName);
                index.Add(pointName);
            }
        }
        timer += 0.04f;
        timer=Mathf.Min(timer,animTime);
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
