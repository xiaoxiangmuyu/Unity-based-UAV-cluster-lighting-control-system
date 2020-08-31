using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using Sirenix.OdinInspector;
public class TxtAnimBehavior : PlayableBehaviour
{
    public GameObject GraphParent;
    public PlayableDirector director;
    public MovementManager movementManager;
    public int startFrame;
    public int curframe = 0;
    bool isExportMode;
    public TxtForAnimation target;
    public bool isMappingIndex;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!isExportMode)
            UpdatePos();//编辑模式,会跳帧或者不连续播放
        else
            UpdatePosFrameByFrame();//导出模式，逐帧播放，不允许丢帧或者跳跃

    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        ProjectManager.currentAnim=target;
        ProjectManager.currentAnimBehavior=this;
    }
    public override void OnGraphStart(Playable playable)
    {
        if (!GraphParent.activeSelf)
            return;
        MyTools.UpdateDuring(GraphParent);
        isExportMode = movementManager.isWorking;
        //初始化点的位置，防止瞬间读取动画造成超速
        if (isExportMode)
            target.MyUpdate(0,isMappingIndex);
        else
            curframe = Mathf.FloorToInt((float)director.time * 25f) - startFrame;

    }
    //随时间轴进度条更新位置
    void UpdatePos()
    {
        if (target == null)
            return;
        if (curframe < 0)
            curframe = 0;
        if(curframe==0&&isMappingIndex)
        {
            target.CorrectPointIndex();
        }
        target.MyUpdate(curframe,isMappingIndex);
        if (Application.isPlaying)
            curframe += 1;
        else
            curframe = Mathf.FloorToInt((float)director.time * 25f) - startFrame;
        //Debug.LogFormat("startFrame:{0}",startFrame);
    }
    //s
    void UpdatePosFrameByFrame()
    {
        if (target == null)
            return;
        ConsoleProDebug.Watch("curframe:", curframe.ToString());
        target.MyUpdate(curframe,isMappingIndex);
        curframe += 1;
    }
}
