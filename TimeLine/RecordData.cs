using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[System.Serializable]
public struct PointIndexInfo
{
    [SerializeField]//所有点的坐标信息
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
    [ValueDropdown("availableNames")]
    [GUIColor("GetGroupColor")]
    public string groupName;
    [HideInInspector]
    public PointIndexInfo pointsInfo;

    [SerializeField]
    [HideInInspector]
    public List<string> objNames;
    [SerializeField]
    [HideInInspector]
    public List<float> times;
    // [SerializeField]
    // public List<string> originNames;
    IEnumerable availableNames
    {
        get
        {
            return ProjectManager.availableGroups;
        }
    }
    List<System.Action> Actions;

    public RecordData(string name = "")
    {
        dataName = name;
        animTime = 0;
        objNames = new List<string>();
        times = new List<float>();
        pointsInfo.posList = new List<Vector3>();
        //posDic=new StringVector3Dictionary();
    }
    public void Clear()
    {
        objNames.Clear();
        times.Clear();
        dataName = string.Empty;
        //posDic.Clear();
    }
    public void Init()
    {
        objNames = new List<string>();
        times = new List<float>();
        //posDic=new StringVector3Dictionary();
    }
    public bool IsEmpty()
    {
        if (objNames.Count != 0 && times.Count != 0)
            return false;
        else
            return true;
    }
    public void CopyFrom(RecordData data)
    {
        Init();
        dataName = data.dataName;
        //parentName=data.parentName;
        objNames = new List<string>(data.objNames.ToArray());
        times = new List<float>(data.times.ToArray());
        if (data.animTime != 0)
            animTime = data.animTime;
        //if (data.groupName!=null||!data.groupName.Equals(""))
        groupName = data.groupName;
        pointsInfo.posList = new List<Vector3>(data.pointsInfo.posList);
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
    [HorizontalGroup("添加颜色")]
    void AddMappingData()
    {
        var mappingDatas = ProjectManager.GetDataGroupByGroupName(groupName).mappingDatas;
        MappingData targetData = new MappingData();
        foreach (var temp in mappingDatas)
        {
            if (temp.dataName.Equals(dataName) && temp.groupName.Equals(groupName))
            {
                targetData = temp;
                targetData.groupName = groupName;
                targetData.objNames = new List<string>(objNames);
                targetData.pointsInfo.posList = pointsInfo.posList;
                Debug.Log("数据更新完毕");
                return;
            }
        }
        MappingData data = new MappingData();
        data.pointsInfo.posList = pointsInfo.posList;
        data.objNames = new List<string>(objNames);
        data.dataName = dataName;
        data.groupName = groupName;
        ProjectManager.Instance.RecordProject.AddMappingData(data);
        Debug.Log("添加颜色成功");
    }
    [Button("Show", ButtonSizes.Medium)]
    [HorizontalGroup("View")]
    [GUIColor(0.7f, 1, 1)]
    public void ShowObjects()
    {
        var objects = MyTools.FindObjs(objNames).ToArray();
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
        var objects = MyTools.FindObjs(objNames);
        objects.ForEach((a) => a.SetActive(false));
        UnityEditor.Selection.objects = null;
    }
    [Button("Update", ButtonSizes.Medium)]
    [HorizontalGroup("View")]
    public void UpdateContent()
    {
        if (UnityEditor.Selection.objects.Length == 0)
            return;
        objNames.Clear();
        pointsInfo.posList.Clear();
        times.Clear();
        foreach (var point in UnityEditor.Selection.gameObjects)
        {
            if (point.name.Equals("Main Camera"))
                continue;
            pointsInfo.posList.Add(MyTools.TruncVector3(point.transform.position));
            objNames.Add(point.name);
            times.Add(0);
        }
        Debug.Log(dataName + "内容更换完毕");
    }
    public Color GetGroupColor()
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
        // var animName = ProjectManager.GetGlobalPosInfoByGroup(groupName).animName;
        // var temp = new List<string>(MyTools.FindNamesByPosList(pointsInfo.posList, animName));
        // if (temp.Count != 0)
        //     objNames = new List<string>(temp);
        CorrectByMappingIndex();
    }
    //动画数据不变(坐标和序号都不能变)，仅顺序调整时使用
    public void CorrectByMappingIndex()
    {
        string animName = ProjectManager.GetGlobalPosInfoByGroup(groupName).animName;
        TxtForAnimation animation = ProjectManager.GetAnimationByName(animName);
        List<int> mappingList = animation.indexs;
        List<string> result = new List<string>();
        foreach (var name in objNames)
        {
            for (int i = 0; i < mappingList.Count; i++)
            {
                if (mappingList[i] == int.Parse(name))
                {
                    result.Add((i + 1).ToString());
                    break;
                }
            }
        }
        if (result.Count != objNames.Count)
        {
            Debug.LogError("校正失败，少了" + (objNames.Count - result.Count) + "个点");
            return;
        }
        else
        {
            Debug.Log("校正成功");
            objNames = new List<string>(result);
        }


    }
    //[Button("校正")]
    //    public void CorrectIndex(string animName)
    //    {
    //        var anims=ProjectManager.GetPointsRoot().GetComponents<TxtForAnimation>();
    //        TxtForAnimation anim=new TxtForAnimation();
    //        foreach(var a in anims)
    //        {
    //             if(a.animName.Equals(animName))
    //             anim=a;
    //        }
    //        var names=anim.FindPointNames(pointsInfo.posList);
    //        if(names.Count!=0&&!names.Contains(null))
    //        {
    //            ObjNames=names;
    //            Debug.Log("校正成功");
    //        }
    //        else
    //        {
    //            Debug.LogError("校正失败");
    //        }
    //    }

}
