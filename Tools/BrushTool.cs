using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class BrushTool : MonoBehaviour
{
    public Camera mainCamera;
    public List<string> pointList = new List<string>();
    public bool singleMode = true;
    public float unitTimer = 0.04f;
    [HideIf("singleMode")]
    public float brushSize = 3;
    static bool isUsing;
    Ray ray;
    RaycastHit hit;
    RaycastHit[] result;
    static List<System.Action<RaycastHit>> leftClickActions;
    static List<System.Action<RaycastHit>> rightClickActions;
    public RecordData tempData;
    private void Start()
    {
        result = new RaycastHit[30];
        leftClickActions = new List<System.Action<RaycastHit>>();
        rightClickActions = new List<System.Action<RaycastHit>>();

    }
    private void FixedUpdate()
    {
        if (!isUsing)
            return;
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (singleMode)
            SingleModeUpdate();
        else
            MulModeUpdate();
    }
    [Button]
    void PushData()
    {
        tempData = new RecordData();
        tempData.objNames = new List<string>(pointList);
        float timer = 0;
        for (int i = 0; i < tempData.objNames.Count; i++)
        {
            tempData.times.Add(timer);
            timer += unitTimer;
        }
        tempData.animTime = tempData.times[tempData.times.Count - 1];
        ProjectManager.Instance.RecordProject.AddData(tempData);
        pointList.Clear();
        Debug.Log("数据记录完成");
    }
    void SingleModeUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, int.MaxValue, 1 << LayerMask.NameToLayer("Point")))
            {
                DisPatchLeftClick();
                hit.transform.GetComponent<MeshRenderer>().material.color = Color.green;
                if (!pointList.Contains(hit.transform.name))
                {
                    pointList.Add(hit.transform.name);
                    Debug.Log("添加成功");
                }
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out hit, int.MaxValue, 1 << LayerMask.NameToLayer("Point")))
            {
                DisPatchRightClick();
                hit.transform.GetComponent<MeshRenderer>().material.color = Color.white;
                if (pointList.Contains(hit.transform.name))
                {
                    pointList.Remove(hit.transform.name);
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
        if (singleMode || !enabled)
            return;
        if (!mainCamera)
        {
            mainCamera = GameObject.Find("PointsPreview").GetComponent<Camera>();
        }
        Gizmos.DrawWireSphere(mainCamera.ScreenToWorldPoint(Input.mousePosition), brushSize);
    }
    [Button]
    public void ChangeSpeed()
    {
        tempData.times = new List<float>();
        float timer = 0;
        for (int i = 0; i < tempData.objNames.Count; i++)
        {
            tempData.times.Add(timer);
            timer += unitTimer;
        }
        tempData.animTime = tempData.times[tempData.times.Count - 1];
        ProjectManager.Instance.RecordProject.AddData(tempData);
        Debug.Log("速度调整完成");

    }
    [GUIColor("StateColor")]
    [Button(ButtonSizes.Gigantic)]
    public static void SwitchWorkState()
    {
        isUsing = !isUsing;
    }
    Color StateColor()
    {
        return isUsing ? Color.green : Color.red;
    }
    public static void RegisterLeftClick(System.Action<RaycastHit> action)
    {
        if (!leftClickActions.Contains(action))
            leftClickActions.Add(action);
    }
    public static void RegisterRightClick(System.Action<RaycastHit> action)
    {
        if (!rightClickActions.Contains(action))
            rightClickActions.Add(action);
    }
    public static void RemoveRayCast(System.Action<RaycastHit> action)
    {
        if (leftClickActions.Contains(action))
            leftClickActions.Remove(action);
        if (rightClickActions.Contains(action))
            rightClickActions.Remove(action);
    }
    void DisPatchLeftClick()
    {
        foreach (var action in leftClickActions)
        {
            action(hit);
        }
    }
    void DisPatchRightClick()
    {
        foreach (var action in rightClickActions)
        {
            action(hit);
        }
    }
}
