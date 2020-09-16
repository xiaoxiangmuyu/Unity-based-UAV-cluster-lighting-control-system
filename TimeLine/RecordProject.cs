using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;
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
    public List<StringVector3Dictionary> globalPosDic = new List<StringVector3Dictionary>();
    public List<string>ColorMapperNames=new List<string>();
    private List<ColorMapper>colorMappers=new List<ColorMapper>();

    public void AddData(RecordData data)
    {
        if (data.pointsInfo.animName.Equals(""))
        {
            Debug.LogError("要添加的数据动画名称为空，无法添加");
            return;
        }
        if (RecorDataList.Exists((a) => a.dataName == data.dataName))
        {
            RecorDataList.Find((a) => a.dataName == data.dataName).CopyFrom(data);
        }
        else
        {
            RecordData tempData = new RecordData();
            tempData.CopyFrom(data);
            RecorDataList.Add(tempData);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

    }
    public void AddMappingData(MappingData data)
    {
        if (data.pointsInfo.animName.Equals(""))
        {
            Debug.LogError("要添加的数据动画名称为空,无法添加");
            return;
        }
        mappingDatas.Add(data);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
    [Button("整理", ButtonSizes.Gigantic)]
    void Sort()
    {
        RecorDataList.Sort((a, b) => a.groupIndex - b.groupIndex);
        mappingDatas.Sort((a, b) => a.groupIndex - b.groupIndex);

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
    [Button("全局校准",ButtonSizes.Gigantic)]
    void CorrectAll()
    {
        MappingAll();
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
    [Button(ButtonSizes.Gigantic)]
    void ReadGroupInfo()
    {
        using (var reader = new StreamReader(path))
        {
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                RecordData temp=new RecordData();
                var data = line.Split(' ');
                temp.pointsInfo.animName=data[0];
                for(int i=1;i<data.Length;i++)
                {
                    temp.ObjNames.Add(data[i]);
                }
                ProjectManager.Instance.RecordProject.AddData(temp);
            }
            reader.Close();
        }

    }
    

}
