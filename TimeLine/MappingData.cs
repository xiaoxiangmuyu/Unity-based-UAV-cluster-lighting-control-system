using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
[System.Serializable]
public class MappingData
{
    // public MappingData(GameObject[]objs)
    // {
    //     this.objs=objs;
    //     dic=new Dictionary<string, Color>();
    // }
    public string dataName;
    [Range(0, 360)]
    [OnValueChanged("RotateCamera")]
    public float angle;
    [SerializeField]
    [HideInInspector]
    List<StringColorDictionary> dics;
    [ShowIf("ShowXY")]
    [Range(0, 1)]
    public float anchorX, anchorY;
    [ShowIf("ShowZ")]
    [Range(0, 1)]
    public float anchorZ;
    [OnValueChanged("CaulateAll")]
    public DirType dirType;
    public List<Gradient> colors;
    [HideInInspector]
    public List<string> names;
    Vector3 center;
    [SerializeField]
    [HideInInspector]
    GameObject[] objs;
    Camera mainCamera;
    Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            return mainCamera;
        }
    }
    public GameObject[] Objects
    {
        get
        {
            if (objs.Length == 0 || objs[0] == null || objs.Length == 1)
                objs = MyTools.FindObjs(names).ToArray();
            return objs;
        }
        set
        {
            objs = value;
        }
    }
    bool ShowXY { get { return dirType == DirType.In_Out || dirType == DirType.Out_In || dirType == DirType.Ball; } }
    bool ShowZ { get { return dirType == DirType.Ball; } }
    bool ShowAngle { get { return angle != 0; } }
    bool NeedCau;
    public bool isNull()
    {
        return names == null;
    }

    [Button(ButtonSizes.Medium)]
    [HorizontalGroup("Buttons")]
    public void ShowObjects()
    {
        Selection.objects = Objects;
    }
    public Color GetMappingColor(string name, int colorIndex = 0, bool random = false)
    {
        if (!dics[colorIndex].ContainsKey(name))
        {
            Debug.LogError("键不存在:" + name);
            return Color.red;
        }
        if (random)
        {
            int result = Random.Range(0, dics.Count);
            return dics[result][name];
        }
        return dics[colorIndex][name];
    }
    // public bool ContainsPoint(string name)
    // {
    //     return dics.ContainsKey(name);
    // }
    [Button(ButtonSizes.Medium)]
    [GUIColor("GetColor")]
    [HorizontalGroup("Buttons")]
    public void CaulateAll()
    {
        dics = new List<StringColorDictionary>();
        for (int i = 0; i < colors.Count; i++)
        {
            Caulate(i);
        }
    }
    public void Caulate(int index)
    {
        dics.Add(new StringColorDictionary());
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
            center = new Vector3(minX.Value + (maxX.Value - minX.Value) * anchorX, minY.Value + (maxY.Value - minY.Value) * anchorY, minZ.Value + (maxZ.Value - minZ.Value) * anchorZ);
            foreach (var point in Objects)
            {
                float dis = Vector3.Distance(center, point.transform.position);
                if (dis > maxDistance)
                    maxDistance = dis;
            }
            foreach (var point in Objects)
            {
                Color color = colors[index].Evaluate(Vector3.Distance(point.transform.position, center) / maxDistance);
                dics[index].Add(point.name, color);
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
                        dics[index].Add(point.name, colors[index].Evaluate(1 - ((pos.y - yMin.Value) / (yMax.Value - yMin.Value))));
                    }
                    break;
                case DirType.Down_UP:
                    foreach (var point in Objects)
                    {
                        pos = mainCamera.WorldToScreenPoint(point.transform.position);
                        dics[index].Add(point.name, colors[index].Evaluate((pos.y - yMin.Value) / (yMax.Value - yMin.Value)));
                    }
                    break;
                case DirType.Left_Right:
                    foreach (var point in Objects)
                    {
                        pos = mainCamera.WorldToScreenPoint(point.transform.position);
                        dics[index].Add(point.name, colors[index].Evaluate((pos.x - xMin.Value) / (xMax.Value - xMin.Value)));
                    }
                    break;
                case DirType.Right_Left:
                    foreach (var point in Objects)
                    {
                        pos = mainCamera.WorldToScreenPoint(point.transform.position);
                        dics[index].Add(point.name, colors[index].Evaluate(1 - ((pos.x - xMin.Value) / (xMax.Value - xMin.Value))));
                    }
                    break;
                case DirType.In_Out:

                    foreach (var point in Objects)
                    {
                        float value = Vector2.Distance(mainCamera.WorldToScreenPoint(point.transform.position), new Vector2(xMin.Value + (xMax.Value - xMin.Value) * anchorX, yMin.Value + (yMax.Value - yMin.Value) * anchorY));
                        dics[index].Add(point.name, colors[index].Evaluate(value / maxDistance));
                    }
                    break;
                case DirType.Out_In:
                    foreach (var point in Objects)
                    {
                        float value = Vector2.Distance(mainCamera.WorldToScreenPoint(point.transform.position), new Vector2(xMin.Value + (xMax.Value - xMin.Value) * anchorX, yMin.Value + (yMax.Value - yMin.Value) * anchorY));
                        dics[index].Add(point.name, colors[index].Evaluate(1 - (value / maxDistance)));
                    }
                    break;
            }
        }
        if (ShowAngle)
        {
            var temp = MainCamera.transform.rotation;
            temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, 0);
            mainCamera.transform.rotation = temp;
        }
        NeedCau = false;
        Debug.Log("计算完成");
    }
    Color GetColor()
    {
        if (dics == null || NeedCau || dics.Count == 0)
            return Color.red;
        else
            return Color.green;
    }
    void SetState()
    {
        NeedCau = true;
    }
    void RotateCamera()
    {
        var temp = MainCamera.transform.rotation;
        temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, angle);
        mainCamera.transform.rotation = temp;
        SetState();

    }


}
