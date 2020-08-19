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
        float maxDistance = 0;
        float tempDistance = 0;

        float? xMax = null;
        float? xMin = null;
        float? yMax = null;
        float? yMin = null;
        tempPosDic=new StringVector3Dictionary();
        foreach(var pointName in data.ObjNames)
        {
            var pos=ProjectManager.Instance.RecordProject.globalPosDic[data.groupIndex-1][pointName];
            tempPosDic.Add(pointName,pos);
        }
        foreach (var pos in tempPosDic.Values)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(pos);
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
        foreach (var pos in tempPosDic.Values)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(pos);
            tempDistance = Mathf.Abs(A * screenPos.x + B * screenPos.y + C) / Mathf.Sqrt(Mathf.Pow(A, 2) + Mathf.Pow(B, 2));
            if (maxDistance < tempDistance)
                maxDistance = tempDistance;
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
        if(MyDebugger.instance.IsDebugMode)
        DrawLine();
        return true;
    }
    void OnValueUpdate(float value,float animTime)
    {
        //Debug.Log(value);
        foreach (var pointName in tempPosDic.Keys)
        {
            if (index.Contains(pointName))
                continue;
            Vector2 coordinate = mainCamera.WorldToScreenPoint(tempPosDic[pointName]);
            float tempDistance = Mathf.Abs(A * coordinate.x + B * coordinate.y + C) / Mathf.Sqrt(Mathf.Pow(A, 2) + Mathf.Pow(B, 2));
            if (tempDistance <= value)
            {
                //Debug.Log(value);
                tempTimes.Add(timer);
                tempNames.Add(pointName);
                index.Add(pointName);
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
            tempPosDic.Clear();
            Debug.Log("处理完成");
        }
    }
    [Button]
    void BeginDrawLine()
    {
        AddValueChangeListener(DrawLine);
        DrawLine();
        MyDebugger.instance.BeginDebug();
    }
    [Button]
    void StopDrawLine()
    {
        RemoveValueChangelistener(DrawLine);
        MyDebugger.instance.StopDebug();
    }
    void DrawLine()
    {
        float ymax=1080;
        float ymin=0;
        float xmax=1920;
        float xmin=0;
        //Debug.Log("width:"+Screen.width+"height:"+Screen.height);
        List<Vector3>points=new List<Vector3>();
        float targetX=(ymax-C)/K;

        if(targetX>=xmin&&targetX<=xmax)
        points.Add(new Vector3(targetX,ymax,0));
        targetX=(ymin-C/K);
        if(targetX>=xmin&&targetX<=xmax)
        points.Add(new Vector3(targetX,ymin,0));

        float targetY=K*xmax+C;
        if(targetY<=ymax&&targetY>=ymin)
        points.Add(new Vector3(xmax,targetY,0));
        targetY=K*xmin+C;
        if(targetY<=ymax&&targetY>=ymin)
        points.Add(new Vector3(xmin,targetY,0));
        Camera mc=Camera.main;
        Vector3 start=mc.ScreenToWorldPoint(points[0]);
        Vector3 end=mc.ScreenToWorldPoint(points[1]);
        float offset=0;
        if(mc.transform.rotation.y<0.5f)
        {
            offset=10;
        }
        else
        {
            offset=-10;
        }
        start.z=mc.transform.position.z+offset;
        end.z=mc.transform.position.z+offset;
        MyDebugger.instance.DrawLine(start,end);
        //Debug.Log(points[0]+"  "+points[1]);


    }
}
