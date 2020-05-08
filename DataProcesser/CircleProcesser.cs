using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class CircleProcesser : IDataProcesser
{
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_X;
    [OnValueChanged("EventDispatch")]
    [Range(0, 1)]
    public float center_Y;
    public override bool Process(ref RecordData data, float animTime)
    {
        if (animTime == 0)
        {
            Debug.LogError("animTime为0");
            return false;
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
            if (!xMax.HasValue||screenPos.x > xMax.Value)
                xMax = screenPos.x;
            if (!xMin.HasValue||screenPos.x < xMin.Value)
                xMin = screenPos.x;
            if (!yMin.HasValue||screenPos.y < yMin.Value)
                yMin = screenPos.y;
            if (!yMax.HasValue||screenPos.y > yMax.Value)
                yMax = screenPos.y;

        }
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
        return true;
    }
}
