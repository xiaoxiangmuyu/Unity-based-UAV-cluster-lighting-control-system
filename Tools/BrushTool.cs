using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class BrushTool : MonoBehaviour
{
    public Camera mainCamera;
    public List<string> pointList = new List<string>();
    public bool singleMode;
    public float unitTimer = 0.04f;
    [HideIf("singleMode")]
    public float brushSize = 3;
    Ray ray;
    RaycastHit hit;
    RaycastHit[] result;

    private void Start()
    {
        result = new RaycastHit[30];
    }
    private void FixedUpdate()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (singleMode)
            SingleModeUpdate();
        else
            MulModeUpdate();
    }
    [Button]
    void PushData()
    {
        RecordData tempdata = new RecordData();
        tempdata.objNames = new List<string>(pointList);
        float timer = 0;
        for (int i = 0; i < tempdata.objNames.Count; i++)
        {
            tempdata.times.Add(timer);
            timer += unitTimer;
        }
        tempdata.animTime = tempdata.times[tempdata.times.Count - 1];
        ProjectManager.Instance.RecordProject.AddData(tempdata);
        pointList.Clear();
        Debug.Log("数据记录完成");
    }
    void SingleModeUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, int.MaxValue, 1 << LayerMask.NameToLayer("Point")))
            {
                if (!pointList.Contains(hit.transform.name))
                {
                    pointList.Add(hit.transform.name);
                    hit.transform.GetComponent<MeshRenderer>().material.color = Color.green;
                    Debug.Log("添加成功");
                }
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit, int.MaxValue, 1 << LayerMask.NameToLayer("Point")))
            {
                if (pointList.Contains(hit.transform.name))
                {
                    pointList.Remove(hit.transform.name);
                    hit.transform.GetComponent<MeshRenderer>().material.color = Color.white;
                    Debug.Log("取消成功");
                }
            }
        }
    }
    void MulModeUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            result = Physics.SphereCastAll(ray.origin, brushSize, ray.direction);
            if (result.Length != 0)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    if (!pointList.Contains(result[i].collider.name))
                    {
                        pointList.Add(result[i].collider.name);
                        result[i].collider.GetComponent<MeshRenderer>().material.color = Color.green;
                        Debug.Log("添加成功");
                    }
                }

            }
        }
        else if (Input.GetMouseButton(1))
        {
            result = Physics.SphereCastAll(ray.origin, brushSize, ray.direction);
            if (result.Length != 0)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    if (pointList.Contains(result[i].collider.name))
                    {
                        pointList.Remove(result[i].collider.name);
                        result[i].collider.GetComponent<MeshRenderer>().material.color = Color.white;
                        Debug.Log("取消成功");
                    }
                }
            }
        }

    }
    private void OnDrawGizmos()
    {
        if (singleMode||!enabled)
            return;
        if (!mainCamera)
        {
            mainCamera = GameObject.Find("PointsPreview").GetComponent<Camera>();
        }
        Gizmos.DrawWireSphere(mainCamera.ScreenToWorldPoint(Input.mousePosition), brushSize);
    }

}
