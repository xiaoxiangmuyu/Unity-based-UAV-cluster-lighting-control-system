using System.Collections.Generic;
using UnityEngine;

public class MovementCheck : MonoBehaviour
{
    Vector3 lastPos;
    Vector3 curPos;
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

        curPos = TruncVector3(transform.position);
        if (!firstFrameIgnore)
        {
            distance = 0;
            VecticalDis=0;
            firstFrameIgnore = true;
        }
        else
        {
            distance = Vector3.Distance(curPos, lastPos);
            VecticalDis = Mathf.Abs(lastPos.y - curPos.y);
        }
        //SpeedCheck();
        lastPos = curPos;
        RecordInfo(curPos, mat.color);
    }
    void SpeedCheck()//超速检测
    {
        if (distance > maxDistance)
        {
            maxDistance = distance;
            if (maxDistance > MovementManager.LimitedSpeed)
                Debug.LogError(gameObject.name + "合速度超速！ " + "最大距离:" + maxDistance);
        }
        if (VecticalDis > maxVecticalDis)
        {
            maxVecticalDis = VecticalDis;
            if (maxVecticalDis > MovementManager.LimitedVecticalSpeed)
                Debug.LogError(gameObject.name + "竖直方向超速！ " + "最大距离:" + maxVecticalDis);
        }
    }

    private void RecordInfo(Vector3 pos, Color color)//记录位置
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
