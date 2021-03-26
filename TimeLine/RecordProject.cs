using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;
[System.Serializable]
public class GlobalPosInfo
{
    [SerializeField]
    public string groupName;
    [SerializeField]
    [ValueDropdown("animNames")]
    public string animName;
    [SerializeField]
    public List<Vector3> posList = new List<Vector3>();
    IEnumerable animNames
    {
        get
        {
            return ProjectManager.AllMainAnimNames;
        }
    }

}
public class RecordProject : SerializedScriptableObject
{
    [ValueDropdown("available")]
    [OnValueChanged("ChangeView")]
    [TabGroup("数据视图")]
    public string currentGroup;

    [TabGroup("数据视图")]
    [InlineEditor]
    public DataGroup current;

    [TabGroup("全局位置")]
    public List<GlobalPosInfo> globalPosDic = new List<GlobalPosInfo>();

    public List<string> ColorMapperNames = new List<string>();

    private List<ColorMapper> colorMappers = new List<ColorMapper>();

    IEnumerable available
    {
        get
        {
            return ProjectManager.availableGroups;
        }
    }
    //[Button("RefreshCurrent")]
    void ChangeView()
    {
        current = ProjectManager.GetDataGroupByGroupName(currentGroup);
    }

    public void AddData(RecordData data)
    {
        if (current == null)
        {
            Debug.LogError("Current为空");
            return;
        }
        if (current.recordDatas.Exists((a) => a.dataName == data.dataName))
        {
            current.recordDatas.Find((a) => a.dataName == data.dataName).CopyFrom(data);
        }
        else
        {
            RecordData tempData = new RecordData();
            tempData.CopyFrom(data);
            if (tempData.dataName == null || tempData.dataName.Equals(""))
                tempData.dataName = (current.recordDatas.Count + 1).ToString();
            tempData.groupName = current.groupName;
            current.recordDatas.Add(tempData);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public void AddMappingData(MappingData data)
    {
        data.groupName = current.groupName;
        if (data.dataName == null || data.dataName.Equals(""))
            data.dataName = (current.mappingDatas.Count + 1).ToString();
        current.mappingDatas.Add(data);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
    public ColorMapper GetColorMapper(string name)
    {
        if (colorMappers.Exists((a) => a.name.Equals(name)))
            return colorMappers.Find((a) => a.name.Equals(name));
        ColorMapper result = GameObject.Find(name).GetComponent<ColorMapper>();
        //colorMappers.Add(result);
        return result;
    }

    public void AddMapper(ColorMapper mapper)
    {
        if (ColorMapperNames.Exists((a) => a.Equals(mapper.name)))
            return;
        ColorMapperNames.Add(mapper.name);
    }

    [Button("一键指派", ButtonSizes.Gigantic)]
    [FoldoutGroup("指派与校准")]
    //一键指派
    public void MappingAll()
    {
        var anims = ProjectManager.GetPointsRoot().GetComponents<TxtForAnimation>();
        List<Vector3> temp = new List<Vector3>();
        for (int i = 0; i < anims.Length; i++)
        {
            if (i == 0)
            {
                anims[0].indexs = new List<int>();
                for (int j = 0; j < anims[0].childCount; j++)
                {
                    anims[0].indexs.Add(j);
                }
                temp = new List<Vector3>(anims[i].GetEndPoitions());
            }
            else
            {
                anims[i].CorrectPointIndex(temp);
                temp = new List<Vector3>(anims[i].GetEndPoitions());
            }

        }
        Debug.Log("一键指派完成");
    }

    [Button("校准灯光", ButtonSizes.Gigantic)]
    [FoldoutGroup("指派与校准")]
    void CorrectAll()
    {
        UpdateGlobalPos();
        var dataList = new List<DataGroup>();
        foreach (var info in globalPosDic)
        {
            dataList.Add(Resources.Load<DataGroup>("Projects/" + ProjectManager.Instance.projectName + "/" + info.animName));
        }
        foreach (var data in dataList)
        {
            for (int i = 0; i < data.recordDatas.Count; i++)
            {
                data.recordDatas[i].CorrectIndex();
            }
            for (int i = 0; i < data.mappingDatas.Count; i++)
            {
                data.mappingDatas[i].CorrectIndex();
            }
        }
        //MyTools.ResfrshTimeLine();
        Debug.Log("全局数据校准完成");
    }
    [FilePath]
    [ShowInInspector]
    [FoldoutGroup("读取分组信息")]
    string path;
    [Button("读取", ButtonSizes.Gigantic)]
    [FoldoutGroup("读取分组信息")]
    void ReadGroupInfo()
    {
        using (var reader = new StreamReader(path))
        {
            string line = null;
            RecordData temp = new RecordData();
            while ((line = reader.ReadLine()) != null)
            {
                var data = line.Split('\t');
                if (data.Length <= 1)
                {
                    if (!temp.dataName.Equals(""))
                    {
                        AddData(temp);
                        temp = new RecordData();
                    }
                    temp.dataName = data[0];
                    continue;
                }
                if (!int.TryParse(data[0], out int result))
                    continue;
                temp.objNames.Add(data[0]);
                temp.pointsInfo.posList.Add(new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3])));
            }
            AddData(temp);
            reader.Close();
        }
        Debug.Log("读取分组信息成功");
    }

    //[Button]
    public void GenerateGlobalPos()
    {
        foreach (var anim in ProjectManager.AllMainAnimNames)
        {
            var txtForAnimation = ProjectManager.FindAnimByName(anim);
            GlobalPosInfo info = new GlobalPosInfo();
            info.animName = anim;
            info.groupName = anim;
            foreach (var pos in txtForAnimation.GetBeginPosition())
            {
                info.posList.Add(pos);
            }
            globalPosDic.Add(info);

        }
        for (int i = 0; i < globalPosDic.Count; i++)
        {
            if (AssetDatabase.FindAssets("Assets/Resources/Projects/" + ProjectManager.Instance.projectName + "/" + globalPosDic[i].groupName + ".asset").Length == 0)
            {
                var instance = ScriptableObject.CreateInstance<DataGroup>();
                instance.groupName = globalPosDic[i].groupName;
                AssetDatabase.CreateAsset(instance, "Assets/Resources/Projects/" + ProjectManager.Instance.projectName + "/" + globalPosDic[i].groupName + ".asset");
            }
            else
                Debug.Log("DataGroup已存在");
        }
        Debug.Log("生成全局位置数据和组数据完成");
    }


    public void UpdateGlobalPos()
    {
        for (int i = 0; i < globalPosDic.Count; i++)
        {
            globalPosDic[i].posList.Clear();
            var txtForAnimation = ProjectManager.FindAnimByName(globalPosDic[i].animName);
            foreach (var pos in txtForAnimation.GetBeginPosition())
            {
                globalPosDic[i].posList.Add(pos);
            }
        }
    }
    [Button("颜色预览", ButtonSizes.Gigantic)]
    public void ColorPreview()
    {
        foreach (var data in current.mappingDatas)
        {
            data.ShowColor();
        }
    }



}
