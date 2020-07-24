using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class CircleProcesser : IDataProcesser
{
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_X;
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_Y;

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
        objects = MyTools.FindObjs(data.ObjNames);
        float? xMax = null;
        float? xMin = null;
        float? yMax = null;
        float? yMin = null;
        foreach (var obj in objects)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(obj.transform.position);
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
        for (int i = 0; i < objects.Count; i++)
        {
            if (Vector2.Distance(anchorPoint, mainCamera.WorldToScreenPoint(objects[i].transform.position)) > maxDistance)
                maxDistance = Vector2.Distance(anchorPoint, mainCamera.WorldToScreenPoint(objects[i].transform.position));
        }
        timer = 0;
        tempNames = new List<string>();
        tempTimes = new List<float>();
        index = new List<int>();
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
        for (int i = 0; i < objects.Count; i++)
        {
            if (index.Contains(i))
                continue;
            float tempDistance = Vector2.Distance(mainCamera.WorldToScreenPoint(objects[i].transform.position), anchorPoint);
            if (tempDistance <= value)
            {
                tempTimes.Add(timer);
                tempNames.Add(objects[i].name);
                index.Add(i);
            }
        }
        timer += 0.04f;
        timer=Mathf.Min(timer,animTime);
        if (index.Count == data.ObjNames.Count)
        {
            if (isProcessed)
                return;
            data.ObjNames = tempNames;
            data.times = tempTimes;
            ProcessComplete();
            isProcessed = true;
            Debug.Log("处理完成");
        }
    }

}
