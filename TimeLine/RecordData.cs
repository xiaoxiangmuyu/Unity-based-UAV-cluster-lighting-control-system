using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[System.Serializable]
public class RecordData
{
    [SerializeField]
    public string dataName;
    [SerializeField]
    public float animTime;

    [SerializeField]
    public List<string>ObjNames;
    [SerializeField]
    public List<float>times;
    public RecordData(string name="")
    {
        dataName=name;
        animTime=1;
        ObjNames=new List<string>();
        times=new List<float>();
    }
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
        //parentName=data.parentName;
        ObjNames=new List<string>(data.ObjNames.ToArray());
        times=new List<float>(data.times.ToArray());
    }
}
