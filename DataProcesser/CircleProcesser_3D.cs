using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class CircleProcesser_3D : IDataProcesser
{
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_X;
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_Y;
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_Z;
    Vector3 anchorPoint;
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
        float? zMax = null;
        float? zMin = null;
        foreach (var obj in objects)
        {
            if (!xMax.HasValue || obj.transform.position.x > xMax.Value)
                xMax = obj.transform.position.x;
            if (!xMin.HasValue || obj.transform.position.x < xMin.Value)
                xMin = obj.transform.position.x;
            if (!yMax.HasValue || obj.transform.position.y > yMax.Value)
                yMax = obj.transform.position.y;
            if (!yMin.HasValue || obj.transform.position.y < yMin.Value)
                yMin = obj.transform.position.y;
            if (!zMax.HasValue || obj.transform.position.z > zMax.Value)
                zMax = obj.transform.position.z;
            if (!zMin.HasValue || obj.transform.position.z < zMin.Value)
                zMin = obj.transform.position.z;
        }
        anchorPoint = new Vector3(xMin.Value + (xMax.Value - xMin.Value) * center_X, yMin.Value + (yMax.Value - yMin.Value) * center_Y,zMin.Value+(zMax.Value-zMin.Value)*center_Z);
        float maxDistance = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            if (Vector3.Distance(anchorPoint, objects[i].transform.position) > maxDistance)
                maxDistance = Vector3.Distance(anchorPoint, objects[i].transform.position);
        }
        timer = 0;
        tempNames = new List<string>();
        tempTimes = new List<float>();
        index = new List<int>();
        DOVirtual.Float(0, maxDistance, animTime, OnValueUpdate).SetEase(easeType);
        //temp.StartCoroutine(MyTools.Process(data.animTime));
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
            float tempDistance = Vector3.Distance(objects[i].transform.position, anchorPoint);
            if (tempDistance <= value)
            {
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
