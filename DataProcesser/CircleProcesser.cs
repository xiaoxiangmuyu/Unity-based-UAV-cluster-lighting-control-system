using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class CircleProcesser : IDataProcesser
{
    [BoxGroup("Behavior Property")]
    [Range(0, 1)]
    public float center_X;
    [BoxGroup("Behavior Property")]
    [Range(0, 1)]
    public float center_Y;
    void IDataProcesser.Process(ref RecordData data, float animTime)
    {
        if (animTime == 0)
        {
            Debug.LogError("animTime为0");
            return;
        }
        Camera mainCamera = Camera.main;
        List<GameObject> objects = MyTools.FindObjs(data.ObjNames);

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
        Rect rect = new Rect(xMin.Value + (xMax.Value - xMax.Value) / 2, yMin.Value + (yMax.Value - yMin.Value) / 2, xMax.Value - xMin.Value, yMax.Value - yMin.Value);
        Vector2 anchorPoint = new Vector2(xMin.Value + (xMax.Value - xMin.Value) * center_X, yMin.Value + (yMax.Value - yMin.Value) * center_Y);
        float maxDistance = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            if (Vector2.Distance(anchorPoint, mainCamera.WorldToScreenPoint(objects[i].transform.position)) > maxDistance)
                maxDistance = Vector2.Distance(anchorPoint, mainCamera.WorldToScreenPoint(objects[i].transform.position));
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
                float tempDistance = Vector2.Distance(mainCamera.WorldToScreenPoint(objects[i].transform.position), anchorPoint);
                if (tempDistance <= distance)
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
    }
}
