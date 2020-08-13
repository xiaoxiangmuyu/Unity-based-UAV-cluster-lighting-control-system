using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;
public abstract class IDataProcesser
{
    [OnValueChanged("EventDispatch")]
    public Ease easeType=Ease.OutQuad;
    //public AnimationCurve curve;
    public abstract bool Process(ref RecordData data, float animTime);


    protected List<System.Action> OnValueChangeActions;
    protected List<System.Action> OnProcessCompleteActions;
    protected bool isProcessed;
    protected List<string> tempNames;
    protected List<float> tempTimes;
    protected List<string> index;
    //protected List<GameObject> objects;
    protected Camera mainCamera;
    protected float timer;
    protected RecordData data;


    public virtual void EventDispatch()
    {
        foreach (var action in OnValueChangeActions)
        {
            action();
        }
    }
    public virtual void AddValueChangeListener(System.Action action)
    {
        if (OnValueChangeActions == null)
            OnValueChangeActions = new List<System.Action>();

        if (OnValueChangeActions.Contains(action))
            return;

        OnValueChangeActions.Add(action);

    }
    public virtual void AddProcessCompleteListener(System.Action action)
    {
        if (OnProcessCompleteActions == null)
            OnProcessCompleteActions = new List<System.Action>();

        if (OnProcessCompleteActions.Contains(action))
            return;

        OnProcessCompleteActions.Add(action);
    }
    public virtual void RemoveValueChangelistener(System.Action action)
    {
        if(OnValueChangeActions.Contains(action))
        OnValueChangeActions.Remove(action);
    }
    public virtual void ProcessComplete()
    {
        foreach(var action in OnProcessCompleteActions)
        {
            action();
        }
    }
}
