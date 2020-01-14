﻿using System.Collections.Generic;
using UnityEngine;

public class MovementCheck : MonoBehaviour
{
    private Vector3 lastPos;
    private List<Vector3> posInfos;
    private List<Color> colorInfos;
    private float distance;
    private float VecticalDis;
    private float maxDistance;
    private float maxVecticalDis;
    private MovementManager movementManager;
    private Renderer curRenderer;
    private Material mat;
    private string droneName;
    bool firstFrameIgnore;

    private void Awake()
    {
        movementManager = GetComponentInParent<MovementManager>();
        if (!movementManager.isWorking)
            return;
        curRenderer = GetComponent<Renderer>();
        if (curRenderer)
        {
            mat = curRenderer.material;
        }
        lastPos = TruncVector3(transform.position);
        posInfos = new List<Vector3>();
        colorInfos = new List<Color>();
        maxDistance = 0f;
        //RecordInfo(lastPos, mat.color);
        droneName = name;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!movementManager.isWorking)
            return;
        if (movementManager.GetIsFinished()) // 动画播放完毕或静态画面达到持续时间后则不再进行检测
        {
            return;
        }

        Vector3 curPos = TruncVector3(transform.position);
        if (!firstFrameIgnore)
        {
            distance = 0;
            firstFrameIgnore = true;
        }
        else
            distance = Vector3.Distance(curPos, lastPos);

        if (distance > maxDistance)
        {
            maxDistance = distance;
            SpeedCheck();
        }
        VecticalDis = Mathf.Abs(lastPos.y - curPos.y);
        if (VecticalDis > maxVecticalDis)
        {
            maxVecticalDis = VecticalDis;
            VecticalSpeedCheck();
        }
        lastPos = curPos;

        //if (distance <= movementManager.GetLimitedSpeed())
        //{
        RecordInfo(curPos, mat.color);
        //}

    }
    void SpeedCheck()
    {
        if (maxDistance > movementManager.GetLimitedSpeed() && movementManager.isSpeedTest)
            Debug.LogError(gameObject.name + "超速！ " + "最大距离:" + maxDistance);
    }
    void VecticalSpeedCheck()
    {
        if (maxVecticalDis > movementManager.GetLimitedVecticalSpeed() && movementManager.isSpeedTest)
            Debug.LogError(gameObject.name + "竖直方向超速！ " + "最大距离:" + maxVecticalDis);
    }
    private void RecordInfo(Vector3 pos, Color color)
    {
        posInfos.Add(pos);
        colorInfos.Add(color);
    }

    public float GetMaxDistance()
    {
        return maxDistance;
    }

    public List<Vector3> GetPosInfos()
    {
        return posInfos;
    }

    public List<Color> GetColorInfos()
    {
        return colorInfos;
    }
    string tmp;
    float result;
    private float Trunc(float num)
    {
        tmp = num.ToString("f2");
        result = float.Parse(tmp);
        return result;
    }

    private Vector3 TruncVector3(Vector3 v)
    {
        float x = Trunc(v.x);
        float y = Trunc(v.y);
        float z = Trunc(v.z);

        return new Vector3(x, y, z);
    }

    public string GetDroneName()
    {
        return droneName;
    }
}
