using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public struct RecordData
{
    
    public string dataName;
    public string parentName;
    public bool isSelect;
    [ReadOnly]
    public float animTime;
    public List<string>ObjNames;
    public List<float>times;
    public void Clear()
    {
        ObjNames.Clear();
        times.Clear();
        dataName=string.Empty;
    }
    public void Init()
    {
        ObjNames=new List<string>();
        times=new List<float>();
    }
    public void CopyFrom(RecordData data)
    {
        Init();
        dataName=data.dataName;
        parentName=data.parentName;
        animTime=data.animTime;
        ObjNames=new List<string>(data.ObjNames.ToArray());
        times=new List<float>(data.times.ToArray());
    }
}
