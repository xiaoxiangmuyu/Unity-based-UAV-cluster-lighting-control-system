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
    public List<Vector3>posList=new List<Vector3>();
    IEnumerable animNames
    {
        get
        {
            return ProjectManager.AllAnimNames;
        }
    }

}
public class RecordProject : SerializedScriptableObject
{
    [SerializeField]
    [TableList(DrawScrollView=false)]
    [TabGroup("Data")]
    [ListDrawerSettings(Expanded=true)]
    public List<RecordData> RecorDataList;
    [SerializeField]
    [TableList(DrawScrollView=false)]
    [TabGroup("Mapping")]
    [ListDrawerSettings(Expanded=true)]
    public List<MappingData> mappingDatas = new List<MappingData>();
    [TabGroup("GlobalPos")]
    [SerializeField]
    public List<GlobalPosInfo> globalPosDic = new List<GlobalPosInfo>();
    public List<string>ColorMapperNames=new List<string>();
    private List<ColorMapper>colorMappers=new List<ColorMapper>();

    public void AddData(RecordData data)
    {
        // if (data.pointsInfo.animName.Equals(""))
        // {
        //     Debug.LogError("要添加的数据动画名称为空，无法添加");
        //     return;
        // }
        if (RecorDataList.Exists((a) => a.dataName == data.dataName))
        {
            RecorDataList.Find((a) => a.dataName == data.dataName).CopyFrom(data);
        }
        else
        {
            RecordData tempData = new RecordData();
            tempData.CopyFrom(data);
            tempData.groupName=RecorDataList[RecorDataList.Count-1].groupName;
            RecorDataList.Add(tempData);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

    }
    public void AddMappingData(MappingData data)
    {
        // if (data.pointsInfo.animName.Equals(""))
        // {
        //     Debug.LogError("要添加的数据动画名称为空,无法添加");
        //     return;
        // }
        //data.groupName=RecorDataList[mappingDatas.Count-1].groupName;
        mappingDatas.Add(data);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
    [FoldoutGroup("buttons")]
    [Button("整理", ButtonSizes.Gigantic)]
    void Sort()
    {
        RecorDataList.Sort((a, b) => a.GetOrder() - b.GetOrder());
        mappingDatas.Sort((a, b) => a.GetOrder() - b.GetOrder());

    }
    public ColorMapper GetColorMapper(string name)
    {
        // if(colorMappers.Exists((a)=>a.name.Equals(name)))
        // return colorMappers.Find((a)=>a.name.Equals(name));
        ColorMapper result= GameObject.Find(name).GetComponent<ColorMapper>();
        //colorMappers.Add(result);
        return result;
    }
    public void AddMapper(ColorMapper mapper)
    {
        if(ColorMapperNames.Exists((a)=>a.Equals(mapper.name)))
        return;
        ColorMapperNames.Add(mapper.name);
    }
    [Button("一键指派",ButtonSizes.Gigantic)]
    [FoldoutGroup("buttons")]
    //一键指派
    void MappingAll()
    {
        var anims=ProjectManager.GetPointsRoot().GetComponents<TxtForAnimation>();
        List<Vector3>temp=new List<Vector3>();
        for(int i=0;i<anims.Length;i++)
        {
            if(i==0)
            {
                anims[i].useMapping=false;
                temp=new List<Vector3>(anims[i].GetEndPoitions());
            }
            else
            {
                anims[i].useMapping=true;
                anims[i].CorrectPointIndex(temp);
                temp=new List<Vector3>(anims[i].GetEndPoitions());
            }

        }
        Debug.Log("一键指派完成");
    }
    [Button("校准灯光",ButtonSizes.Gigantic)]
    [FoldoutGroup("buttons")]
    void CorrectAll()
    {
        //MappingAll();
        foreach(var data in RecorDataList)
        {
            data.CorrectIndex();
        }
        foreach(var data in mappingDatas)
        {
            data.CorrectIndex();
        }
        MyTools.ResfrshTimeLine();
        Debug.Log("全局数据校准完成");
    }
    [FilePath]
    [ShowInInspector]
    string path;
    public string animName;
    [Button(ButtonSizes.Gigantic)]
    [FoldoutGroup("buttons")]
    void ReadGroupInfo()
    {
        using (var reader = new StreamReader(path))
        {
            string line = null;
            RecordData temp=new RecordData();
            while ((line = reader.ReadLine()) != null)
            {
                var data = line.Split('\t');
                if(data.Length<=1)
                {
                    if(!temp.dataName.Equals(""))
                    {
                        AddData(temp);
                        temp=new RecordData();
                    }
                    temp.dataName=data[0];
                    continue;
                }
                if(!int.TryParse(data[0],out int result))
                continue;
                temp.objNames.Add(data[0]);
                temp.pointsInfo.posList.Add(new Vector3(float.Parse(data[1]),float.Parse(data[2]),float.Parse(data[3])));
            }
            AddData(temp);
            reader.Close();
        }
        Debug.Log("读取分组信息成功");


    }
    

}
