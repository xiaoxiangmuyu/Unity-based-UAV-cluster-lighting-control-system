﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using DG.Tweening;
public class TxtAnimAsset : SerializedScriptableObject, IPlayableAsset
{
    #region  IPlayableAsset
    public double duration { get; }
    public IEnumerable<PlayableBinding> outputs { get; }
    #endregion
    [LabelText("安全时间")]
    public double safeSeconds = 0;
    [ReadOnly]
    [LabelText("总帧数")]
    [PropertyOrder(-2)]
    public int totalFrameCount;
    [LabelText("总时长")]
    [ShowInInspector]
    [PropertyOrder(-1)]
    public double seconds { get { return totalFrameCount / 25f; } }
    [ValueDropdown("animIndexs")]
    public string animName;

    [LabelText("是否更新颜色")]
    [ShowInInspector]
    public bool useColor
    {
        get
        {
            if (!target)
                return false;
            return target.useColor;
        }
        set
        {
            if (!target)
                return;
            target.useColor = value;
        }
    }
    TxtForAnimation[] scripts;
    ScriptPlayable<TxtAnimBehavior> scriptPlayable;
    TxtForAnimation target;
    IEnumerable animIndexs
    {
        get
        {
            if (scripts == null)
                return null;
            List<string> temp = new List<string>();
            for (int i = 0; i < scripts.Length; i++)
            {
                temp.Add(scripts[i].danceDB.animName);
            }
            return temp;
        }
    }
    // [ShowInInspector]
    // bool available
    // {
    //     get
    //     {
    //         if (target == null)
    //             return false;
    //         return target.mappingSuccess;
    //     }
    // }
    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        scriptPlayable = ScriptPlayable<TxtAnimBehavior>.Create(graph);
        scripts = owner.GetComponents<TxtForAnimation>();
        if (scripts != null)
        {
            var temp = new List<TxtForAnimation>(scripts);
            if (animName != null)
            {
                var anim = temp.Find((a) => a.danceDB.animName == animName);
                totalFrameCount = anim.danceDB.totalFrameCount;
                scriptPlayable.GetBehaviour().target = anim;
                target = anim;
            }
            scriptPlayable.GetBehaviour().GraphParent = owner;
            //scriptPlayable.GetBehaviour().animIndex = animIndex;
        }
        scriptPlayable.GetBehaviour().director = owner.GetComponent<PlayableDirector>();
        scriptPlayable.GetBehaviour().movementManager = owner.GetComponent<MovementManager>();
        return scriptPlayable;
    }
    // [Button(ButtonSizes.Gigantic)]
    // void ResetTween()
    // {
    //     DOTween.KillAll();
    // }
    public void SetStartFrame(int frame)
    {
        scriptPlayable.GetBehaviour().startFrame = frame;
    }
    [Button("播放第一帧位置", ButtonSizes.Gigantic)]
    void SetAnimBegin()
    {
        target.SetAnimBegin();
    }
    [Button("播放最后一帧位置", ButtonSizes.Gigantic)]
    void SetAnimEnd()
    {
        target.SetAnimEnd();
    }

}
