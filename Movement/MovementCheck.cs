using System.Collections.Generic;
using UnityEngine;
using System.Collections;
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
        lastPos = MyTools.TruncVector3(transform.position);
        posInfos = new List<Vector3>();
        colorInfos = new List<Color>();
        maxDistance = 0f;
        droneName = name;
    }
    private void LateUpdate() {
        if(ProjectManager.Instance.texture!=null)
        Record(ProjectManager.Instance.texture);
    }
    // Update is called once per frame
    public void Record(Texture2D texture)
    {
        if (!movementManager.isWorking)
            return;
        if (movementManager.usePostProcessing)
        {
            Vector2 screenPos = ProjectManager.MainCamera.WorldToScreenPoint(transform.position);
            Color temp = texture.GetPixel((int)screenPos.x, (int)screenPos.y);
            // if (temp.Equals(Color.green))
            //     return;
            // else
                colorInfos.Add(temp);
        }
        curPos = MyTools.TruncVector3(transform.position);
        if (!firstFrameIgnore)
        {
            distance = 0;
            VecticalDis = 0;
            firstFrameIgnore = true;
        }
        else
        {
            distance = Vector3.Distance(curPos, lastPos);
            VecticalDis = Mathf.Abs(lastPos.y - curPos.y);
        }
        lastPos = curPos;
        posInfos.Add(curPos);

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
    public string GetDroneName()
    {
        return droneName;
    }
}
