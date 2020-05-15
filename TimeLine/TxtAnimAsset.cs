using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using DG.Tweening;
[CreateAssetMenu(menuName = "创建动画序列", fileName = "新动画序列")]
public class TxtAnimAsset : SerializedScriptableObject, IPlayableAsset
{
    #region  IPlayableAsset
    public double duration { get; }
    public IEnumerable<PlayableBinding> outputs { get; }
    #endregion
    [InfoBox("安全时间至少1秒，不然播放不完",InfoMessageType.Warning)]
    public float safeSeconds=1;
    [ReadOnly] [LabelText("总帧数")]
    public int totalFrameCount;
    [LabelText("总时长")]
    public float seconds{get{return totalFrameCount/25;}}
    TxtForAnimation script;
    ScriptPlayable<TxtAnimBehavior>scriptPlayable;
    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        scriptPlayable = ScriptPlayable<TxtAnimBehavior>.Create(graph);
        script = owner.GetComponent<TxtForAnimation>();
        if (script!=null)
        {
            totalFrameCount = script.totalFrameCount;
            scriptPlayable.GetBehaviour().script = script;
            scriptPlayable.GetBehaviour().GraphParent=owner;
        }
        scriptPlayable.GetBehaviour().director = owner.GetComponent<PlayableDirector>();
        scriptPlayable.GetBehaviour().movementManager=owner.GetComponent<MovementManager>();
        return scriptPlayable;
    }
    [Button(ButtonSizes.Gigantic)]
    void ResetTween()
    {
        DOTween.KillAll();
    }
    public void SetStartFrame(int frame)
    {
        scriptPlayable.GetBehaviour().startFrame=frame;
    }

}
