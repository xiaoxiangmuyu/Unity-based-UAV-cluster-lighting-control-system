using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
public class RecordProject : SerializedScriptableObject
{
    [SerializeField]
    public Dictionary<string, List<RecordData>> RecordDic;
    [SerializeField]
    public List<MappingData> mappingDatas = new List<MappingData>();

    public void AddData(string ImageName, RecordData data)
    {
        if (!RecordDic.ContainsKey(ImageName))
        {
            Debug.LogError("没有找到这个父物体" + ImageName);
            return;
        }
        if (RecordDic[ImageName].Exists((a) => a.dataName == data.dataName))
        {
            RecordDic[ImageName].Find((a) => a.dataName == data.dataName).CopyFrom(data);
        }
        else
        {
            RecordData tempData = new RecordData();
            tempData.CopyFrom(data);
            RecordDic[ImageName].Add(tempData);
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

}
