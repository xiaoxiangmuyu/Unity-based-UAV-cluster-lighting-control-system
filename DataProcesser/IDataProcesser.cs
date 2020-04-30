using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDataProcesser 
{
    public List<System.Action>Actions;
    public abstract void Process(ref RecordData data,float animTime);
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
}
