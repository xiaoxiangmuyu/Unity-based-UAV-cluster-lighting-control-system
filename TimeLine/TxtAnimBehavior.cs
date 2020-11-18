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
    bool isExportMode { get { return movementManager.isWorking; } }
    public TxtForAnimation target;
    bool isFirstAnim
    {
        get
        {
            if (!target)
                return false;
            return ProjectManager.GetPointsRoot().GetComponents<TxtForAnimation>()[0].animName.Equals(target.animName);
        }
    }
    //int counter;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Debug.Log("Test:" + (counter + 1).ToString());
        // counter += 1;
        if (!isExportMode)
        {
            UpdatePos();//编辑模式,会跳帧或者不连续播放
        }
        else
            UpdatePosFrameByFrame();//导出模式，逐帧播放，不允许丢帧或者跳跃

    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {

    }
    public override void OnGraphStart(Playable playable)
    {
        if (!GraphParent.activeSelf)
            return;
        if (isFirstAnim)
        {
            MyTools.UpdateClipDuring(GraphParent);
            if (isFirstAnim && isExportMode)
            {
                target.MyUpdatePos(0);
                ConsoleProDebug.LogToFilter(target.animName + "第一帧位置已校正", "Result");
            }
        }

    }
    //随时间轴进度条更新位置
    void UpdatePos()
    {
        if (target == null)
            return;
        if (curframe < 0)
            curframe = 0;
        target.MyUpdatePos(curframe);
        //if (Application.isPlaying)
        //    curframe += 1;
        //else
            curframe = Mathf.FloorToInt((float)director.time * 25f) - startFrame;
    }
    //s
    void UpdatePosFrameByFrame()
    {
        if (target == null)
        {
            Debug.LogError("anim is null");
            return;
        }
        ConsoleProDebug.Watch("curframe:", (curframe + 1).ToString());
        target.MyUpdatePos(curframe);
        curframe += 1;
    }
}
