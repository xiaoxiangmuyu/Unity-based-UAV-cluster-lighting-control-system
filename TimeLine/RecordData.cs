using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[System.Serializable]
public struct PointIndexInfo
{
    public string animName;
    public int frame;
    [SerializeField]
    public List<Vector3> posList;
}
[System.Serializable]
public class RecordData
{
    [SerializeField]
    [OnValueChanged("EventDispatch")]
    [GUIColor("GetGroupColor")]
    public string dataName;
    [SerializeField]
    [OnValueChanged("EventDispatch")]
    [GUIColor("GetGroupColor")]

    public float animTime;
    [ValueDropdown("availableIndex")]
    [GUIColor("GetGroupColor")]

    public int groupIndex;
    [HideInInspector]
    public PointIndexInfo pointsInfo;

    [SerializeField]
    [HideInInspector]
    public List<string> ObjNames;
    [SerializeField]
    [HideInInspector]
    public List<float> times;
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

    List<System.Action> Actions;

    public RecordData(string name = "")
    {
        dataName = name;
        animTime = 0;
        ObjNames = new List<string>();
        times = new List<float>();
        //posDic=new StringVector3Dictionary();
    }
    public void Clear()
    {
        ObjNames.Clear();
        times.Clear();
        dataName = string.Empty;
        //posDic.Clear();
    }
    public void Init()
    {
        ObjNames = new List<string>();
        times = new List<float>();
        //posDic=new StringVector3Dictionary();
    }
    public bool IsEmpty()
    {
        if (ObjNames.Count != 0 && times.Count != 0)
            return false;
        else
            return true;
    }
    public void CopyFrom(RecordData data)
    {
        Init();
        dataName = data.dataName;
        //parentName=data.parentName;
        ObjNames = new List<string>(data.ObjNames.ToArray());
        times = new List<float>(data.times.ToArray());
        if (data.animTime != 0)
            animTime = data.animTime;
        if (!dataName.Equals("All") && !dataName.Equals("all"))
            groupIndex = data.groupIndex;
        pointsInfo.animName=data.pointsInfo.animName;
        pointsInfo.frame=data.pointsInfo.frame;
        pointsInfo.posList=new List<Vector3>(data.pointsInfo.posList);
        //posDic=data.posDic;
    }
    public void AddListener(System.Action action)
    {
        if (Actions == null)
            Actions = new List<System.Action>();

        if (Actions.Contains(action))
            return;

        Actions.Add(action);
    }
    void EventDispatch()
    {
        if (Actions == null)
        {
            //Debug.Log("Action为空");
            return;
        }
        foreach (var action in Actions)
        {
            action();
        }
    }
    [Button("Add", ButtonSizes.Medium)]
    [HorizontalGroup("添加映射")]
    void AddMappingData()
    {
        if (!ProjectManager.Instance.RecordProject.mappingDatas.Exists((a) => a.dataName == dataName))
        {
            MappingData data = new MappingData();
            data.pointsInfo.animName=pointsInfo.animName;
            data.pointsInfo.frame=pointsInfo.frame;
            data.pointsInfo.posList=pointsInfo.posList;
            data.pointNames = new List<string>(ObjNames);
            data.dataName = dataName;
            data.groupIndex = groupIndex;
            ProjectManager.Instance.RecordProject.AddMappingData(data);
            Debug.Log("添加映射成功");
        }
        else
        {
            var targetData = ProjectManager.Instance.RecordProject.mappingDatas.Find((a) => a.dataName == dataName);
            targetData.pointNames.Clear();
            targetData.groupIndex=groupIndex;
            targetData.pointNames=new List<string>(ObjNames);
            targetData.pointsInfo.animName=pointsInfo.animName;
            targetData.pointsInfo.frame=pointsInfo.frame;
            targetData.pointsInfo.posList=pointsInfo.posList;
            Debug.Log("数据更新完毕");
        }
    }
    [Button("Show", ButtonSizes.Medium)]
    [HorizontalGroup("View")]
    [GUIColor(0.7f, 1, 1)]
    public void ShowObjects()
    {
        var objects = MyTools.FindObjs(ObjNames).ToArray();
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(true);
        }
        UnityEditor.Selection.objects = objects;
    }
    [Button("Hide", ButtonSizes.Medium)]
    [HorizontalGroup("View")]
    [GUIColor(0.5f, 1, 1)]
    public void HideObjects()
    {
        var objects = MyTools.FindObjs(ObjNames);
        objects.ForEach((a) => a.SetActive(false));
    }
    [Button("Update", ButtonSizes.Medium)]
    [HorizontalGroup("View")]
    public void UpdateContent()
    {
        if (UnityEditor.Selection.objects.Length == 0)
            return;
        ObjNames.Clear();
        pointsInfo.animName=ProjectManager.curAnimName;
        pointsInfo.frame=ProjectManager.curAnimFrame;
        pointsInfo.posList.Clear();
        times.Clear();
        foreach (var point in UnityEditor.Selection.gameObjects)
        {
            if (point.name.Equals("Main Camera"))
                continue;
            pointsInfo.posList.Add(MyTools.TruncVector3(point.transform.position));
            ObjNames.Add(point.name);
            times.Add(0);
        }
        Debug.Log(dataName + "内容更换完毕");
    }
    public Color GetGroupColor()
    {
        if (groupIndex == 0)
            return Color.red;
        var temp = ProjectManager.Instance.RecordProject.globalPosDic.Count;
        float c = (float)1f / temp * groupIndex;
        return Color.HSVToRGB(c, 0.4f, 1f);
    }
   public void CorrectIndex()
   {
        ObjNames=new List<string>(MyTools.FindNamesByPointsInfo(pointsInfo));
   }

}
