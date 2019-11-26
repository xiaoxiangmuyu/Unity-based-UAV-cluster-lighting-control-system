using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MovementCheck : MonoBehaviour
{
    private Vector3 lastPos;
    private List<string> infos;
    private float distance;
    private float maxDistance;
    private MovementManager movementManager;
    private Renderer curRenderer;
    private Material mat;
    private StringBuilder sb = new StringBuilder();
    private int r;
    private int g;
    private int b;

    private void Awake()
    {
        lastPos = TruncVector3(transform.position);
        infos = new List<string>();
        maxDistance = 0f;
        movementManager = GetComponentInParent<MovementManager>();
        curRenderer = GetComponent<Renderer>();

        if (curRenderer)
        {
            mat = curRenderer.material;
        }

        RecordInfo(lastPos);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (movementManager.GetIsFinished()) // 动画播放完毕或静态画面达到持续时间后则不再进行检测
        {
            return;
        }

        Vector3 curPos = TruncVector3(transform.position);
        distance = Vector3.Distance(curPos, lastPos);

        if (distance > maxDistance)
        {
            maxDistance = distance;
        }

        lastPos = curPos;

        if (distance <= movementManager.limitedSpeed)
        {
            RecordInfo(curPos);
        }
    }

    private void RecordInfo(Vector3 curPos)
    {
        r = Mathf.FloorToInt(mat.color.r * 255);
        g = Mathf.FloorToInt(mat.color.g * 255);
        b = Mathf.FloorToInt(mat.color.b * 255);

        sb.Clear();
        sb.Append(name);
        sb.Append("\t");
        sb.Append(curPos.x);
        sb.Append("\t");
        sb.Append(-curPos.z); // 统一取相反数
        sb.Append("\t");
        sb.Append(curPos.y);
        sb.Append("\t");
        sb.Append(r);
        sb.Append("\t");
        sb.Append(g);
        sb.Append("\t");
        sb.Append(b);
        infos.Add(sb.ToString());
    }

    public float GetMaxDistance()
    {
        return maxDistance;
    }

    public List<string> GetInfos()
    {
        return infos;
    }

    private float Trunc(float num)
    {
        string tmp = num.ToString("f2");
        float result = float.Parse(tmp);
        return result;
    }

    private Vector3 TruncVector3(Vector3 v)
    {
        float x = Trunc(v.x);
        float y = Trunc(v.y);
        float z = Trunc(v.z);

        return new Vector3(x, y, z);
    }
}
