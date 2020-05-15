using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDataProcesser 
{
    public List<System.Action>Actions;
    public event System.Action OnProcessComplete; 
    public abstract bool Process(ref RecordData data,float animTime);
    public virtual void EventDispatch()
    {
        foreach(var action in Actions)
        {
            action();
        }
    }
    public virtual void AddListener(System.Action action)
    {
        if(Actions==null)
        Actions=new List<System.Action>();
        
        if(Actions.Contains(action))
        return;
        Actions.Add(action);
        
    }
    public virtual void ProcessComplete()
    {
        if(OnProcessComplete!=null)
        OnProcessComplete();
    }
}
