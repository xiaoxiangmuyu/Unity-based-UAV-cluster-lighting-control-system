using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
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

    public void AddData(string ImageName, RecordData data)
    {
        // if (!RecordDic.ContainsKey(ImageName))
        // {
        //     Debug.LogError("没有找到这个父物体" + ImageName);
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
            RecorDataList.Add(tempData);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

    }
    public void AddMappingData(MappingData data)
    {
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
    

}
