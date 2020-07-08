using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
public class TxtAnimBehavior : PlayableBehaviour
{
    public GameObject GraphParent;
    public TxtForAnimation script;
    public PlayableDirector director;
    public MovementManager movementManager;
    public int startFrame;
    int curframe;
    bool isExportMode;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //DOTween.ManualUpdate(0.04f, 0.04f);
        if(!isExportMode)
        UpdatePos();//编辑模式,会跳帧或者不连续播放
        else
        UpdatePosFrameByFrame();//导出模式，逐帧播放，不允许丢帧或者跳跃

    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        
    }
    public override void OnGraphStart(Playable playable)
    {      
        MyTools.UpdateDuring(GraphParent);
        isExportMode=movementManager.isWorking;
        //初始化点的位置，防止瞬间读取动画造成超速
        if(isExportMode)
        script.MyUpdate(0);
    }
    //随时间轴进度条更新位置
    void UpdatePos()
    {
        if(!script)
        return;
        curframe=Mathf.FloorToInt((float)director.time*25f)-startFrame;
        //Debug.Log(curframe);
        script.MyUpdate(curframe);
        //Debug.LogFormat("startFrame:{0}",startFrame);
    }
    //逐帧更新位置
    void UpdatePosFrameByFrame()
    {
        if(!script)
        return;
        Debug.Log(curframe);
        script.MyUpdate(curframe);
        curframe+=1;
    }
}
