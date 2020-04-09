using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
public class TxtAnimBehavior : PlayableBehaviour
{
    public TxtForAnimation script;
    public PlayableDirector director;
    public MovementManager movementManager;
    int curframe;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //DOTween.ManualUpdate(0.04f, 0.04f);
        if(!movementManager.isWorking)
        UpdatePos();//编辑模式,会跳帧或者不连续播放
        else
        UpdatePosFrameByFrame();//导出模式，逐帧播放，不允许丢帧或者跳跃

    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //Debug.Log("OnBehaviourPlay");
    }
    public override void OnGraphStart(Playable playable)
    {      
        //DOTween.KillAll();
        
    }
    //随时间轴进度条更新位置
    void UpdatePos()
    {
        if(!script)
        return;
        curframe=Mathf.RoundToInt((float)director.time*25);
        //Debug.Log(curframe);
        script.MyUpdate(curframe);
    }
    //逐帧更新位置
    void UpdatePosFrameByFrame()
    {
        if(!script)
        return;
        //Debug.Log(curframe);
        script.MyUpdate(curframe);
        curframe+=1;
    }
}
