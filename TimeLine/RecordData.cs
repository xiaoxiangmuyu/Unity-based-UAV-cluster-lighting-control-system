using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[System.Serializable]
public class RecordData
{
    [SerializeField][OnValueChanged("EventDispatch")]
    public string dataName;
    [SerializeField][OnValueChanged("EventDispatch")]
    public float animTime;

    [SerializeField]
    public List<string>ObjNames;
    [SerializeField]
    public List<float>times;

    List<System.Action>Actions;
    public RecordData(string name="")
    {
        dataName=name;
        animTime=0;
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
    public bool IsEmpty()
    {
        if(ObjNames.Count!=0&&times.Count!=0)
        return false;
        else 
        return true;
    }
    public void CopyFrom(RecordData data)
    {
        Init();
        dataName=data.dataName;
        //parentName=data.parentName;
        ObjNames=new List<string>(data.ObjNames.ToArray());
        times=new List<float>(data.times.ToArray());
    }
    public void AddListener(System.Action action)
    {
        if(Actions==null)
        Actions=new List<System.Action>();

        if(Actions.Contains(action))
        return;

        Actions.Add(action);
    }
    void EventDispatch()
    {
        if(Actions==null)
        {
            Debug.Log("Action为空");
            return;
        }
        foreach(var action in Actions)
        {
            action();
        }
    }
     
}
