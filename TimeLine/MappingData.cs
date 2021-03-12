using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
[System.Serializable]
public class MappingData
{
    struct CameraPosSetting
    {
        public Vector3 pos;
        public Vector3 eulerAngles;
        public void Set(Transform transform)
        {
            pos = transform.position;
            eulerAngles = transform.eulerAngles;
        }
        public void Show(Transform transform)
        {
            transform.position = pos;
            transform.eulerAngles = eulerAngles;
        }
        public void Clear()
        {
            pos = Vector3.zero;
            eulerAngles = Vector3.zero;
        }
        public bool isNull()
        {
            return pos == Vector3.zero && eulerAngles == Vector3.zero;
        }

    }
    public MappingData()
    {
        colors = new List<Gradient>();
        colors.Add(new Gradient());
    }
    [VerticalGroup("Main")]
    [HideLabel]
    public string dataName;
    [ValueDropdown("availableNames")]
    [VerticalGroup("Main")]
    [HideLabel]
    [GUIColor("GetGroupColor")]
    public string groupName;
    [SerializeField]
    [HideInInspector]
    List<StringColorDictionary> colorDics;
    [OnValueChanged("SetStateDirty")]
    [VerticalGroup("Properties")]
    public DirType dirType;
    [ShowIf("ShowXY")]
    [Range(0, 1)]
    [VerticalGroup("Properties")]
    public float anchorX = 0.5f, anchorY = 0.5f;
    [ShowIf("ShowZ")]
    [Range(0, 1)]
    [VerticalGroup("Properties")]
    public float anchorZ;
    [VerticalGroup("Properties")]
    [ListDrawerSettings(Expanded = false)]
    [OnValueChanged("SetStateDirty")]
    public List<Gradient> colors = new List<Gradient>();
    [HideInInspector]
    public List<string> objNames;
    [HideInInspector]
    public PointIndexInfo pointsInfo;

    IEnumerable availableNames
    {
        get
        {
            return ProjectManager.availableGroups;
        }
    }
    Vector3 center;
    bool ShowXY { get { return dirType == DirType.In_Out || dirType == DirType.Out_In || dirType == DirType.Ball; } }
    bool ShowZ { get { return dirType == DirType.Ball; } }
    bool NeedCau;
    [SerializeField]
    [HideInInspector]
    CameraPosSetting cameraPosSetting;

    [SerializeField]
    [HideInInspector]
    StringVector3Dictionary screenPosDic = new StringVector3Dictionary();



    [Button("显示", ButtonSizes.Medium)]
    [VerticalGroup("Buttons")]
    [HorizontalGroup("Buttons/view")]
    [GUIColor(0.7f, 1, 1)]
    void ShowObjects()
    {
        var objects = MyTools.FindObjs(objNames).ToArray();
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(true);
        }
        Selection.objects = objects;
    }
    [Button("隐藏", ButtonSizes.Medium)]
    [VerticalGroup("Buttons")]
    [HorizontalGroup("Buttons/view")]
    [GUIColor(0.5f, 1, 1)]
    void HideObjects()
    {
        var objects = MyTools.FindObjs(objNames);
        objects.ForEach((a) => a.SetActive(false));
        UnityEditor.Selection.objects = null;
    }
    [Button("更新", ButtonSizes.Medium)]
    [VerticalGroup("Buttons")]
    void UpdateContent()
    {
        if (UnityEditor.Selection.objects.Length == 0)
            return;
        objNames.Clear();
        pointsInfo.posList.Clear();
        foreach (var point in UnityEditor.Selection.gameObjects)
        {
            if (!int.TryParse(point.name, out int res))
                continue;
            pointsInfo.posList.Add(MyTools.TruncVector3(point.transform.position));
            objNames.Add(point.name);
        }
        SavePointScreenPos();
        NeedCau = true;
        Debug.Log(dataName + "内容更换完毕");
    }


    [Button("记录映射角度", ButtonSizes.Medium)]
    [VerticalGroup("Buttons")]
    void SavePointScreenPos()
    {
        var mainCamera = Camera.main;
        screenPosDic.Clear();
        foreach (var pointName in objNames)
        {
            var info = ProjectManager.Instance.RecordProject.globalPosDic.Find((a) => a.groupName.Equals(groupName));
            screenPosDic.Add(pointName, mainCamera.WorldToScreenPoint(info.posList[int.Parse(pointName) - 1]));
        }
        cameraPosSetting.Set(mainCamera.transform);
        SetStateDirty();
    }


    [VerticalGroup("Buttons")]
    [Button("添加数据", ButtonSizes.Medium)]
    void AddRecordData()
    {
        var recordData = ProjectManager.GetDataGroupByGroupName(groupName).recordDatas;
        foreach (var temp in recordData)
        {
            if (temp.dataName.Equals(dataName))
            {
                temp.groupName = groupName;
                temp.objNames = new List<string>(objNames);
                temp.times = new List<float>();
                foreach (var name in objNames)
                {
                    temp.times.Add(0);
                }
                temp.pointsInfo.posList = pointsInfo.posList;
                Debug.Log("数据更新完毕");
                return;
            }
        }
        RecordData data = new RecordData();
        data.pointsInfo.posList = pointsInfo.posList;
        data.objNames = new List<string>(objNames);
        data.times = new List<float>();
        foreach (var name in data.objNames)
        {
            data.times.Add(0);
        }
        data.dataName = dataName;
        data.groupName = groupName;
        ProjectManager.Instance.RecordProject.AddData(data);
        Debug.Log("添加数据成功");
    }


    CameraPosSetting temp;
    //[Button("切换显示映射角度", ButtonSizes.Medium)]
    [VerticalGroup("Buttons")]
    void ShowScreenPos()
    {
        var mainCamera = Camera.main;
        if (temp.isNull())
        {
            temp.Set(mainCamera.transform);
            cameraPosSetting.Show(mainCamera.transform);
        }
        else
        {
            temp.Show(mainCamera.transform);
            temp.Clear();
        }
    }


    public Color GetMappingColor(string name, int colorIndex = 0, bool random = false)
    {
        if (!colorDics[colorIndex].ContainsKey(name))
        {
            Debug.LogError(dataName + "没有:" + name + "的颜色信息");
            return Color.red;
        }
        if (random)
        {
            int result = Random.Range(0, colorDics.Count);
            return colorDics[result][name];
        }
        return colorDics[colorIndex][name];
    }
    // public bool ContainsPoint(string name)
    // {
    //     return dics.ContainsKey(name);
    // }


    [Button("计算", ButtonSizes.Medium)]
    [GUIColor("GetStateColor")]
    [VerticalGroup("Main")]
    void CaulateAll()
    {
        colorDics = new List<StringColorDictionary>();
        if (screenPosDic.Count == 0)
        {
            SavePointScreenPos();
        }
        for (int i = 0; i < colors.Count; i++)
        {
            Caulate(i);
        }
        NeedCau = false;
        Debug.Log("计算完成");
    }
    [VerticalGroup("Properties")]
    [Button("SaveColor", ButtonSizes.Medium)]
    void SaveCurColor()
    {
        colorDics = new List<StringColorDictionary>();
        colorDics.Add(new StringColorDictionary());
        //foreach (var obj in Selection.gameObjects)
        //{
        //    if (!int.TryParse(obj.name, out int res))
        //        return;
        //    colorDics[0].Add(obj.name, obj.GetComponent<ColorPoint>().mat.color);
        //}
        foreach (var point in MyTools.FindObjs(objNames))
        {
            colorDics[0].Add(point.name, point.GetComponent<ColorPoint>().mat.color);
        }
        Debug.Log("存储颜色完成");

    }


    public void Caulate(int index)
    {
        var tempPosDic = new StringVector3Dictionary();
        foreach (var pointName in objNames)
        {
            var info = ProjectManager.Instance.RecordProject.globalPosDic.Find((a) => a.groupName.Equals(groupName));
            var pos = info.posList[int.Parse(pointName) - 1];
            tempPosDic.Add(pointName, pos);
        }
        colorDics.Add(new StringColorDictionary());
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
                colorDics[index].Add(pointName, color);
            }
        }
        else
        {
            float? xMin = null, xMax = null, yMin = null, yMax = null;
            foreach (var screenPos in screenPosDic.Values)
            {
                if (!xMin.HasValue || screenPos.x < xMin)
                    xMin = screenPos.x;
                if (!xMax.HasValue || screenPos.x > xMax)
                    xMax = screenPos.x;
                if (!yMin.HasValue || screenPos.y < yMin)
                    yMin = screenPos.y;
                if (!yMax.HasValue || screenPos.y > yMax)
                    yMax = screenPos.y;
            }
            Vector2 tempScreenPos;
            float maxDistance = 0;
            foreach (var screenPos in screenPosDic.Values)
            {
                float dis = Vector2.Distance(screenPos, new Vector2((xMax.Value + xMin.Value) / 2, (yMin.Value + yMax.Value) / 2));
                if (dis > maxDistance)
                    maxDistance = dis;

            }
            switch (dirType)
            {
                case DirType.Up_Down:
                    foreach (var pointName in screenPosDic.Keys)
                    {
                        tempScreenPos = screenPosDic[pointName];
                        colorDics[index].Add(pointName, colors[index].Evaluate(1 - ((tempScreenPos.y - yMin.Value) / (yMax.Value - yMin.Value))));
                    }
                    break;
                case DirType.Down_UP:
                    foreach (var pointName in screenPosDic.Keys)
                    {
                        tempScreenPos = screenPosDic[pointName];
                        colorDics[index].Add(pointName, colors[index].Evaluate((tempScreenPos.y - yMin.Value) / (yMax.Value - yMin.Value)));
                    }
                    break;
                case DirType.Left_Right:
                    foreach (var pointName in screenPosDic.Keys)
                    {
                        tempScreenPos = screenPosDic[pointName];
                        colorDics[index].Add(pointName, colors[index].Evaluate((tempScreenPos.x - xMin.Value) / (xMax.Value - xMin.Value)));
                    }
                    break;
                case DirType.Right_Left:
                    foreach (var pointName in screenPosDic.Keys)
                    {
                        tempScreenPos = screenPosDic[pointName];
                        colorDics[index].Add(pointName, colors[index].Evaluate(1 - ((tempScreenPos.x - xMin.Value) / (xMax.Value - xMin.Value))));
                    }
                    break;
                case DirType.In_Out:

                    foreach (var pointName in screenPosDic.Keys)
                    {
                        float value = Vector2.Distance(screenPosDic[pointName], new Vector2(xMin.Value + (xMax.Value - xMin.Value) * anchorX, yMin.Value + (yMax.Value - yMin.Value) * anchorY));
                        colorDics[index].Add(pointName, colors[index].Evaluate(value / maxDistance));
                    }
                    break;
                case DirType.Out_In:
                    foreach (var pointName in screenPosDic.Keys)
                    {
                        float value = Vector2.Distance(screenPosDic[pointName], new Vector2(xMin.Value + (xMax.Value - xMin.Value) * anchorX, yMin.Value + (yMax.Value - yMin.Value) * anchorY));
                        colorDics[index].Add(pointName, colors[index].Evaluate(1 - (value / maxDistance)));
                    }
                    break;
            }
        }
    }
    Color GetStateColor()
    {
        if (colorDics == null || NeedCau || colorDics.Count == 0)
            return Color.red;
        else
            return Color.green;
    }
    void SetStateDirty()
    {
        NeedCau = true;
    }
    Color GetGroupColor()
    {
        if (groupName == null || groupName.Equals(""))
            return Color.red;
        int index = 0;
        foreach (var animName in ProjectManager.availableGroups)
        {
            if (animName.Equals(groupName))
                break;
            else
                index++;
        }
        float c = (float)1f / ProjectManager.availableGroups.Count * index;
        return Color.HSVToRGB(c, 0.4f, 1f);
    }
    public int GetOrder()
    {
        int index = 0;
        foreach (var animName in ProjectManager.availableGroups)
        {
            if (animName.Equals(groupName))
                return index;
            else
                index++;
        }
        return 0;
    }
    //[Button("校正")]
    public void CorrectIndex()
    {
        var animName = ProjectManager.GetGlobalPosInfoByGroup(groupName).animName;
        var newNames = new List<string>(MyTools.FindNamesByPosList(pointsInfo.posList, animName));
        if (newNames.Count != 0)
            objNames = new List<string>(newNames);
        // var newDic = new StringVector3Dictionary();
        // int index = 0;
        // foreach (var pair in screenPosDic)
        // {
        //     newDic.Add(newNames[index], pair.Value);
        //     index++;
        // }
        // screenPosDic.Clear();
        // screenPosDic = newDic;
        SavePointScreenPos();
        CaulateAll();
    }
    [VerticalGroup("Main")]
    [Button("预览", ButtonSizes.Medium)]
    public void ShowColor()
    {
        if (Application.isPlaying)
            foreach (var name in objNames)
            {
                GameObject obj = GameObject.Find(name);
                Color color = GetMappingColor(name);
                if (obj)
                    obj.GetComponent<ColorPoint>().mat.color = color;
            }
        else
            Debug.Log("只能在运行模式下执行颜色预览");
    }


}
