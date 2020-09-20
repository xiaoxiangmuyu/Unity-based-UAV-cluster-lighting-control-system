using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using DG.Tweening;
// A behaviour that is attached to a playable
public class ControlBehavior : PlayableBehaviour
{
    public ControlBlock record;
    public GameObject GraphParent;

    List<bool> hasProcess;
    float timer;
    List<float> times { get { return record.data.times; } }
    bool needResetState { get { return hasProcess.Exists((x) => x == true); } }
    Vector2 workRange { get { return record.workRange; } }
    bool trigger;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        //MyTools.UpdateDuring(GraphParent,System.DateTime.Now);
        if (!Application.isPlaying)
            return;
        // if (record.state != BlockState.Ready)
        //     return;
        if (hasProcess == null)
        {
            hasProcess = new List<bool>();
            for (int i = 0; i < record.data.objNames.Count; i++)
            {
                hasProcess.Add(false);
            }
        }
        // if (needResetState)
        //     ResetState();
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {

    }
    public override void OnPlayableCreate(Playable playable)
    {

    }
    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (!Application.isPlaying)
            return;
        // if (record.state != BlockState.Ready)
        //     return;
        if (needResetState)
            ResetState();
        // if(!Application.isPlaying)
        // return;
        // objs.RemoveAt(0);
        // times.RemoveAt(0);
        //objs[0].GetComponent<ColorPoint>().SetProcessType(orders);


    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {

    }
    public override void OnPlayableDestroy(Playable playable)
    {

        //Debug.Log(scriptPlayable.GetHashCode() + "被销毁");
    }
    // Called each frame while the state is set to Play
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // if (record.state != BlockState.Ready)
        //     return;
        if (!Application.isPlaying)
            return;
        //DOTween.ManualUpdate(0.04f, 0.04f);
        timer += Time.deltaTime * record.speed;
        Process(record.isflip);

    }
    void Process(bool isflip)
    {
        int counter = 0;
        int timeIndex = 0;
        if (!isflip)
        {
            for (int i = (int)workRange.x; i <= (int)workRange.y; i++)
            {
                if (i > record.objs.Count - 1)
                {
                    i = i - record.objs.Count;
                }
                if (record.timeInit)
                    timeIndex = counter;
                else
                    timeIndex = i;
                if (timer >= times[timeIndex] && hasProcess[i] == false)
                {
                    record.objs[i].GetComponent<ColorPoint>().SetProcessType(record.colorOrders, record.forceMode, record.possibility);
                    hasProcess[i] = true;
                }
                counter += 1;
            }
        }
        else
        {
            for (int i = (int)workRange.y; i >= (int)workRange.x; i--)
            {
                if (i < 0)
                {
                    i = i + record.objs.Count;
                }
                if (record.timeInit)
                    timeIndex = counter;
                else
                    timeIndex = i;
                if (timer >= times[timeIndex] && hasProcess[i] == false)
                {
                    record.objs[i].GetComponent<ColorPoint>().SetProcessType(record.colorOrders, record.forceMode, record.possibility);
                    hasProcess[i] = true;
                }
                counter += 1;
            }
        }
        if (!hasProcess.Exists((x) => x == false)&&!trigger)
            if (record.isDynamic)
            {
                trigger=true;
                float time = MyTools.GetTotalTime(record.colorOrders);
                DOVirtual.DelayedCall(time,(TweenCallback)PrcessAndReset);
            }
    }
    void PrcessAndReset()
    {
        trigger=false;
        record.ProcessData();
        timer = 0;
        for (int i = 0; i < hasProcess.Count; i++)
        {
            hasProcess[i] = false;
        }
    }
    void ResetState()
    {
        for (int i = 0; i < hasProcess.Count; i++)
        {
            hasProcess[i] = false;
        }
        timer = 0;
        ProjectManager.ResetAllColorAndTween();
        //Debug.Log("ResetState");
    }

}
