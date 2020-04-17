using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class RecordProject : SerializedScriptableObject
{
    public Dictionary<string, List<RecordData>> RecordDic;
    public void AddData(string key, RecordData data)
    {
        if (!RecordDic.ContainsKey(key))
        {
            Debug.LogError("没有找到这个父物体" + key);
            return;
        }
        RecordData tempData=new RecordData();
        tempData.CopyFrom(data);
        if(RecordDic[key].Exists((a)=>a.dataName==data.dataName))
        {
            int index=RecordDic[key].FindIndex((a)=>a.dataName==data.dataName);
            RecordDic[key].RemoveAt(index);
        }
        RecordDic[key].Add(tempData);

    }
}
