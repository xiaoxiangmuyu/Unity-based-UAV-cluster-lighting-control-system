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
    [VerticalGroup("Main")]
    [HideLabel]
    public string dataName;
    [Range(0, 360)]
    [OnValueChanged("RotateCamera")]
    [VerticalGroup("Properties")]

    public float angle;
    [ValueDropdown("availableIndex")]
    [VerticalGroup("Main")]
    [HideLabel]
    [GUIColor("GetGroupColor")]
    public int groupIndex;
    [SerializeField]
    [HideInInspector]
    List<StringColorDictionary> dics;
    [OnValueChanged("CaulateAll")]
    [VerticalGroup("Properties")]
    public DirType dirType;
    [ShowIf("ShowXY")]
    [Range(0, 1)]
    [VerticalGroup("Properties")]
    public float anchorX, anchorY;
    [ShowIf("ShowZ")]
    [Range(0, 1)]
    [VerticalGroup("Properties")]
    public float anchorZ;
    [VerticalGroup("Properties")]
    [ListDrawerSettings(Expanded = false)]
    public List<Gradient> colors;
    [HideInInspector]
    public List<string> names;

    IEnumerable availableIndex
    {
        get
        {
            int count = ProjectManager.Instance.RecordProject.globalPosDic.Count;
            var temp = new List<int>();
            for (int i = 0; i < count; i++)
            {
                temp.Add(i + 1);
            }
            return temp;
        }
    }
    Vector3 center;
    bool ShowXY { get { return dirType == DirType.In_Out || dirType == DirType.Out_In || dirType == DirType.Ball; } }
    bool ShowZ { get { return dirType == DirType.Ball; } }
    bool ShowAngle { get { return angle != 0; } }
    bool NeedCau;
    public bool isNull()
    {
        return names == null;
    }

    [Button("Show", ButtonSizes.Medium)]
    [VerticalGroup("Buttons")]
    [GUIColor(0.7f, 1, 1)]
    public void ShowObjects()
    {
        var objects = MyTools.FindObjs(names).ToArray();
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(true);
        }
        Selection.objects = objects;
    }
    [Button("Hide", ButtonSizes.Medium)]
    [VerticalGroup("Buttons")]
    [GUIColor(0.5f, 1, 1)]
    public void HideObjects()
    {
        var objects = MyTools.FindObjs(names);
        objects.ForEach((a) => a.SetActive(false));
    }
    [Button("Update", ButtonSizes.Medium)]
    [VerticalGroup("Buttons")]
    public void UpdateContent()
    {
        if(UnityEditor.Selection.objects.Length==0)
        return;
        names.Clear();
        foreach (var point in UnityEditor.Selection.objects)
        {
            if(point.name.Equals("Main Camera"))
            continue;
            names.Add(point.name);
        }
        NeedCau=true;
        Debug.Log(dataName + "内容更换完毕");
    }
    public Color GetMappingColor(string name, int colorIndex = 0, bool random = false)
    {
        if (!dics[colorIndex].ContainsKey(name))
        {
            Debug.LogError(dataName + "没有:" + name + "的颜色信息");
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
    [Button("计算", ButtonSizes.Medium)]
    [GUIColor("GetColor")]
    [VerticalGroup("Main")]
    public void CaulateAll()
    {
        dics = new List<StringColorDictionary>();
        if(ShowAngle)
        RotateCamera();
        for (int i = 0; i < colors.Count; i++)
        {
            Caulate(i);
        }
    }
    public void Caulate(int index)
    {
        var tempPosDic = new StringVector3Dictionary();
        foreach (var pointName in names)
        {
            var pos = ProjectManager.Instance.RecordProject.globalPosDic[groupIndex - 1][pointName];
            tempPosDic.Add(pointName, pos);
        }
        dics.Add(new StringColorDictionary());
        if (dirType == DirType.Ball)
        {
            float maxDistance = 0;
            float? minX = null, maxX = null, minY = null, maxY = null, minZ = null, maxZ = null;
            foreach (var pos in tempPosDic.Values)
            {
                if (!minX.HasValue || pos.x < minX)
                    minX = pos.x;
                if (!maxX.HasValue || pos.x > maxX)
                    maxX = pos.x;
                if (!minY.HasValue || pos.y < minY)
                    minY = pos.y;
                if (!maxY.HasValue || pos.y > maxY)
                    maxY = pos.y;
                if (!minZ.HasValue || pos.z < minZ)
                    minZ = pos.z;
                if (!maxZ.HasValue || pos.z > maxZ)
                    maxZ = pos.z;
            }
            center = new Vector3(minX.Value + (maxX.Value - minX.Value) * anchorX, minY.Value + (maxY.Value - minY.Value) * anchorY, minZ.Value + (maxZ.Value - minZ.Value) * anchorZ);
            foreach (var pos in tempPosDic.Values)
            {
                float dis = Vector3.Distance(center, pos);
                if (dis > maxDistance)
                    maxDistance = dis;
            }
            foreach (var pointName in tempPosDic.Keys)
            {
                Color color = colors[index].Evaluate(Vector3.Distance(tempPosDic[pointName], center) / maxDistance);
                dics[index].Add(pointName, color);
            }
        }
        else
        {
            Camera mainCamera = Camera.main;
            float? xMin = null, xMax = null, yMin = null, yMax = null;
            foreach (var tempPos in tempPosDic.Values)
            {
                Vector2 screenPos = mainCamera.WorldToScreenPoint(tempPos);
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
            foreach (var tempPos in tempPosDic.Values)
            {
                float dis = Vector2.Distance(mainCamera.WorldToScreenPoint(tempPos), new Vector2((xMax.Value + xMin.Value) / 2, (yMin.Value + yMax.Value) / 2));
                if (dis > maxDistance)
                    maxDistance = dis;

            }
            switch (dirType)
            {
                case DirType.Up_Down:
                    foreach (var pointName in tempPosDic.Keys)
                    {
                        pos = mainCamera.WorldToScreenPoint(tempPosDic[pointName]);
                        dics[index].Add(pointName, colors[index].Evaluate(1 - ((pos.y - yMin.Value) / (yMax.Value - yMin.Value))));
                    }
                    break;
                case DirType.Down_UP:
                    foreach (var pointName in tempPosDic.Keys)
                    {
                        pos = mainCamera.WorldToScreenPoint(tempPosDic[pointName]);
                        dics[index].Add(pointName, colors[index].Evaluate((pos.y - yMin.Value) / (yMax.Value - yMin.Value)));
                    }
                    break;
                case DirType.Left_Right:
                    foreach (var pointName in tempPosDic.Keys)
                    {
                        pos = mainCamera.WorldToScreenPoint(tempPosDic[pointName]);
                        dics[index].Add(pointName, colors[index].Evaluate((pos.x - xMin.Value) / (xMax.Value - xMin.Value)));
                    }
                    break;
                case DirType.Right_Left:
                    foreach (var pointName in tempPosDic.Keys)
                    {
                        pos = mainCamera.WorldToScreenPoint(tempPosDic[pointName]);
                        dics[index].Add(pointName, colors[index].Evaluate(1 - ((pos.x - xMin.Value) / (xMax.Value - xMin.Value))));
                    }
                    break;
                case DirType.In_Out:

                    foreach (var pointName in tempPosDic.Keys)
                    {
                        float value = Vector2.Distance(mainCamera.WorldToScreenPoint(tempPosDic[pointName]), new Vector2(xMin.Value + (xMax.Value - xMin.Value) * anchorX, yMin.Value + (yMax.Value - yMin.Value) * anchorY));
                        dics[index].Add(pointName, colors[index].Evaluate(value / maxDistance));
                    }
                    break;
                case DirType.Out_In:
                    foreach (var pointName in tempPosDic.Keys)
                    {
                        float value = Vector2.Distance(mainCamera.WorldToScreenPoint(tempPosDic[pointName]), new Vector2(xMin.Value + (xMax.Value - xMin.Value) * anchorX, yMin.Value + (yMax.Value - yMin.Value) * anchorY));
                        dics[index].Add(pointName, colors[index].Evaluate(1 - (value / maxDistance)));
                    }
                    break;
            }
        }
        if (ShowAngle)
        {
            var temp = Camera.main.transform.rotation;
            temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, 0);
            Camera.main.transform.rotation = temp;
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
    Camera mainCamera;
    void RotateCamera()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        var temp = mainCamera.transform.rotation;
        temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, angle);
        mainCamera.transform.rotation = temp;
        SetState();

    }
    Color GetGroupColor()
    {
        var temp = ProjectManager.Instance.RecordProject.globalPosDic.Count;
        float c = (float)1f / temp * groupIndex;
        return Color.HSVToRGB(c, 0.4f, 1f);
    }


}
