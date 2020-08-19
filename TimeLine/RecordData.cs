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

    public string group;

    [SerializeField][HideInInspector]
    public List<string>ObjNames;
    [SerializeField][HideInInspector]
    public List<float>times;
    [SerializeField][HideInInspector]
    public StringVector3Dictionary posDic;
 
    List<System.Action>Actions;
    public RecordData(string name="")
    {
        dataName=name;
        animTime=0;
        ObjNames=new List<string>();
        times=new List<float>();
        posDic=new StringVector3Dictionary();
    }
    public void Clear()
    {
        ObjNames.Clear();
        times.Clear();
        dataName=string.Empty;
        posDic.Clear();
    }
    public void Init()
    {
        ObjNames=new List<string>();
        times=new List<float>();
        posDic=new StringVector3Dictionary();
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
        if(data.animTime!=0)
        animTime=data.animTime;
        posDic=data.posDic;
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
            //Debug.Log("Action为空");
            return;
        }
        foreach(var action in Actions)
        {
            action();
        }
    }
    [Button(ButtonSizes.Medium)]
    [HorizontalGroup("Buttons")]
    void AddMappingData()
    {
        MappingData data=new MappingData ();
        data.names=new List<string>(ObjNames);
        //data.Objects=MyTools.FindObjs(ObjNames).ToArray();
        data.dataName=dataName;
        ProjectManager.Instance.RecordProject.AddMappingData(data);
    }
    [Button(ButtonSizes.Medium)]
    [HorizontalGroup("Buttons")]
    public void ShowObjects()
    {
        UnityEditor.Selection.objects = MyTools.FindObjs(ObjNames).ToArray();
    }
     
}
