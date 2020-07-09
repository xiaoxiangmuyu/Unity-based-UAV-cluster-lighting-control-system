using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
[System.Serializable]
public struct MappingData
{
    // public MappingData(GameObject[]objs)
    // {
    //     this.objs=objs;
    //     dic=new Dictionary<string, Color>();
    // }
    public string dataName;
    [SerializeField]
    [HideInInspector]
    Dictionary<string, Color> dic;
    public Gradient gradient;
    [OnValueChanged("Caulate")]
    public DirType dirType;
    public Vector3 center;
    [HideInInspector]
    public List<string> names;
    [SerializeField][HideInInspector]
    private GameObject[] objs;
    public GameObject[] Objects
    {
        get
        {
            if (objs.Length == 0 || objs[0] == null)
                objs = MyTools.FindObjs(names).ToArray();
            return objs;
        }
        set{
            objs=value;
        }
    }
    public bool isNull()
    {
        return names==null;
    }

    [Button(ButtonSizes.Medium)]
    public void ShowObjects()
    {
        Selection.objects = Objects;
    }
    public Color GetMappingColor(string name)
    {
        if (!dic.ContainsKey(name))
        {
            Debug.LogError("键不存在:" + name);
            return Color.red;
        }
        return dic[name];
    }
    public bool ContainsPoint(string name)
    {
        return dic.ContainsKey(name);
    }
    [Button(ButtonSizes.Medium)]
    public void Caulate()
    {
        dic = new Dictionary<string, Color>();
        if (dirType == DirType.Ball)
        {
            float maxDistance = 0;
            float? minX = null, maxX = null, minY = null, maxY = null, minZ = null, maxZ = null;
            foreach (var point in Objects)
            {
                if (!minX.HasValue || point.transform.position.x < minX)
                    minX = point.transform.position.x;
                if (!maxX.HasValue || point.transform.position.x > maxX)
                    maxX = point.transform.position.x;
                if (!minY.HasValue || point.transform.position.y < minY)
                    minY = point.transform.position.y;
                if (!maxY.HasValue || point.transform.position.y > maxY)
                    maxY = point.transform.position.y;
                if (!minZ.HasValue || point.transform.position.z < minZ)
                    minZ = point.transform.position.z;
                if (!maxZ.HasValue || point.transform.position.z > maxZ)
                    maxZ = point.transform.position.z;
            }
            center = new Vector3((maxX.Value + minX.Value) / 2, (maxY.Value + minY.Value) / 2, (maxZ.Value + minZ.Value) / 2);
            foreach (var point in Objects)
            {
                float dis = Vector3.Distance(center, point.transform.position);
                if (dis > maxDistance)
                    maxDistance = dis;
            }
            foreach (var point in Objects)
            {
                Color color = gradient.Evaluate(Vector3.Distance(point.transform.position, center) / maxDistance);
                dic.Add(point.name, color);
            }
        }
        else
        {
            Camera mainCamera = Camera.main;
            float? xMin = null, xMax = null, yMin = null, yMax = null;
            foreach (var point in Objects)
            {
                Vector2 screenPos = mainCamera.WorldToScreenPoint(point.transform.position);
                if (!xMin.HasValue || screenPos.x < xMin)
                    xMin = screenPos.x;
                if (!xMax.HasValue || screenPos.x > xMax)
                    xMax = screenPos.x;
                if (!yMin.HasValue || screenPos.y < yMin)
                    yMin = screenPos.y;
                if (!yMax.HasValue || screenPos.y > yMax)
                    yMax = screenPos.y;
            }
            Vector2 pos;
            float maxDistance = 0;
            foreach (var point in Objects)
            {
                float dis = Vector2.Distance(mainCamera.WorldToScreenPoint(point.transform.position), new Vector2((xMax.Value + xMin.Value) / 2, (yMin.Value + yMax.Value) / 2));
                if (dis > maxDistance)
                    maxDistance = dis;

            }
            switch (dirType)
            {
                case DirType.Up_Down:
                    foreach (var point in Objects)
                    {
                        pos = mainCamera.WorldToScreenPoint(point.transform.position);
                        dic.Add(point.name, gradient.Evaluate(1 - ((pos.y - yMin.Value) / (yMax.Value - yMin.Value))));
                    }
                    break;
                case DirType.Down_UP:
                    foreach (var point in Objects)
                    {
                        pos = mainCamera.WorldToScreenPoint(point.transform.position);
                        dic.Add(point.name, gradient.Evaluate((pos.y - yMin.Value) / (yMax.Value - yMin.Value)));
                    }
                    break;
                case DirType.Left_Right:
                    foreach (var point in Objects)
                    {
                        pos = mainCamera.WorldToScreenPoint(point.transform.position);
                        dic.Add(point.name, gradient.Evaluate((pos.x - xMin.Value) / (xMax.Value - xMin.Value)));
                    }
                    break;
                case DirType.Right_Left:
                    foreach (var point in Objects)
                    {
                        pos = mainCamera.WorldToScreenPoint(point.transform.position);
                        dic.Add(point.name, gradient.Evaluate(1 - ((pos.x - xMin.Value) / (xMax.Value - xMin.Value))));
                    }
                    break;
                case DirType.In_Out:

                    foreach (var point in Objects)
                    {
                        float value = Vector2.Distance(mainCamera.WorldToScreenPoint(point.transform.position), new Vector2((xMax.Value + xMin.Value) / 2, (yMin.Value + yMax.Value) / 2));
                        dic.Add(point.name, gradient.Evaluate(value / maxDistance));
                    }
                    break;
                case DirType.Out_In:
                    foreach (var point in Objects)
                    {
                        float value = Vector2.Distance(mainCamera.WorldToScreenPoint(point.transform.position), new Vector2((xMax.Value + xMin.Value) / 2, (yMin.Value + yMax.Value) / 2));
                        dic.Add(point.name, gradient.Evaluate(1 - (value / maxDistance)));
                    }
                    break;
            }

        }
        Debug.Log("计算完成");
    }


}
