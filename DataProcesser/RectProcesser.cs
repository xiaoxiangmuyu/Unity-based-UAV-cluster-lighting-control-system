using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class RectProcesser : IDataProcesser
{
    [OnValueChanged("EventDispatch")]
    public float K;
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float offset;

    float offset_Min;
    float offset_Max;
    float A { get { return K; } }
    float B { get { return -1; } }
    float C { get { return offset_Min + (offset_Max - offset_Min) * offset; } }

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
        float maxDistance = 0;
        float tempDistance = 0;

        float? xMax = null;
        float? xMin = null;
        float? yMax = null;
        float? yMin = null;
        foreach (var obj in objects)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(obj.transform.position);
            if (!xMax.HasValue)
                xMax = screenPos.x;
            if (!xMin.HasValue)
                xMin = screenPos.x;
            if (!yMin.HasValue)
                yMin = screenPos.y;
            if (!yMax.HasValue)
                yMax = screenPos.y;

            if (screenPos.x > xMax.Value)
                xMax = screenPos.x;
            if (screenPos.x < xMin.Value)
                xMin = screenPos.x;
            if (screenPos.y > yMax.Value)
                yMax = screenPos.y;
            if (screenPos.y < yMin.Value)
                yMin = screenPos.y;
        }

        List<Vector2> corners = new List<Vector2>();
        corners.Add(new Vector2(xMin.Value, yMin.Value));
        corners.Add(new Vector2(xMin.Value, yMax.Value));
        corners.Add(new Vector2(xMax.Value, yMin.Value));
        corners.Add(new Vector2(xMax.Value, yMax.Value));
        offset_Min = 5000;
        offset_Max = -5000;
        foreach (var corner in corners)
        {
            if (offset_Min > corner.y - K * corner.x)
                offset_Min = corner.y - K * corner.x;
            if (offset_Max < corner.y - K * corner.x)
                offset_Max = corner.y - K * corner.x;
        }
        foreach (var point in objects)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(point.transform.position);
            tempDistance = Mathf.Abs(A * screenPos.x + B * screenPos.y + C) / Mathf.Sqrt(Mathf.Pow(A, 2) + Mathf.Pow(B, 2));
            if (maxDistance < tempDistance)
                maxDistance = tempDistance;
        }
        timer = 0;
        tempNames = new List<string>();
        tempTimes = new List<float>();
        index = new List<int>();
        DOVirtual.Float(0, maxDistance, animTime, OnValueUpdate).SetEase(easeType);
        Debug.Log("处理中...");
        return true;
    }
    void OnValueUpdate(float value)
    {
        //Debug.Log(value);
        for (int i = 0; i < objects.Count; i++)
        {
            if (index.Contains(i))
                continue;
            Vector2 coordinate = mainCamera.WorldToScreenPoint(objects[i].transform.position);
            float tempDistance = Mathf.Abs(A * coordinate.x + B * coordinate.y + C) / Mathf.Sqrt(Mathf.Pow(A, 2) + Mathf.Pow(B, 2));
            if (tempDistance <= value)
            {
                //Debug.Log(value);
                tempTimes.Add(timer);
                tempNames.Add(objects[i].name);
                index.Add(i);
            }
        }
        timer += Time.deltaTime;
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
