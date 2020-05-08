using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class RectProcesser : IDataProcesser
{
    [OnValueChanged("EventDispatch")]
    public float K;
    [OnValueChanged("EventDispatch")][ProgressBar("offset_Min","offset_Max")]
    public float offset;

    float offset_Min;
    float offset_Max;
    float A { get { return K; } }
    float B { get { return -1; } }
    float C { get { return offset; } }

    public override bool Process(ref RecordData data, float animTime)
    {
        if (animTime == 0)
        {
            Debug.LogError("animTime为0");
            return false;
        }
        Camera mainCamera = Camera.main;
        List<GameObject> objects = MyTools.FindObjs(data.ObjNames);
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

            tempDistance = Mathf.Abs(A * screenPos.x + B * screenPos.y + C) / Mathf.Sqrt(Mathf.Pow(A, 2) + Mathf.Pow(B, 2));
            if (maxDistance < tempDistance)
                maxDistance = tempDistance;
        }

        List<Vector2>corners=new List<Vector2>();
        corners.Add(new Vector2(xMin.Value,yMin.Value));
        corners.Add(new Vector2(xMin.Value,yMax.Value));
        corners.Add(new Vector2(xMax.Value,yMin.Value));
        corners.Add(new Vector2(xMax.Value,yMax.Value));
        offset_Min=5000;
        offset_Max=-5000;
        foreach(var corner in corners)
        {
            if(offset_Min>corner.y-K*corner.x)
            offset_Min=corner.y-K*corner.x;
            if(offset_Max<corner.y-K*corner.x)
            offset_Max=corner.y-K*corner.x;
        }

        float distancePerFrame = maxDistance / animTime / 25;
        float distance = 0;
        float timer = 0;
        List<string> tempNames = new List<string>();
        List<float> tempTimes = new List<float>();
        List<int> index = new List<int>();
        while (tempNames.Count < objects.Count)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (index.Contains(i))
                    continue;
                Vector2 coordinate = mainCamera.WorldToScreenPoint(objects[i].transform.position);
                float temp = Mathf.Abs(A * coordinate.x + B * coordinate.y + C) / Mathf.Sqrt(Mathf.Pow(A, 2) + Mathf.Pow(B, 2));
                if (temp <= distance)
                {
                    if (timer > animTime)
                        tempTimes.Add(animTime);
                    else
                        tempTimes.Add(timer);

                    tempNames.Add(objects[i].name);
                    index.Add(i);
                }
            }
            timer += 0.04f;
            distance += distancePerFrame;
        }
        data.ObjNames = tempNames;
        data.times = tempTimes;
        return true;
    }
}
