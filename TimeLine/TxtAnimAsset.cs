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
    [InfoBox("安全时间建议2秒，不然播放不完", InfoMessageType.Warning)]
    [LabelText("安全时间")]
    public float safeSeconds = 2;
    [ReadOnly]
    [LabelText("总帧数")]
    public int totalFrameCount;
    [ValueDropdown("animIndexs")]
    public string animName;

    [LabelText("总时长")]
    [ShowInInspector]
    public float seconds { get { return totalFrameCount / 25f; } }
    [LabelText("是否需要手动指派")]
    [InfoBox("除了第一段都勾上")]
    public bool NeedMappingIndex = true;

    TxtForAnimation[] scripts;
    ScriptPlayable<TxtAnimBehavior> scriptPlayable;

    IEnumerable animIndexs
    {
        get
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < scripts.Length; i++)
            {
                temp.Add(scripts[i].animName);
            }
            return temp;
        }
    }

    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        scriptPlayable = ScriptPlayable<TxtAnimBehavior>.Create(graph);
        scripts = owner.GetComponents<TxtForAnimation>();
        if (scripts != null)
        {
            var temp = new List<TxtForAnimation>(scripts);
            if (animName != null)
            {
                var anim = temp.Find((a) => a.animName == animName);
                totalFrameCount = anim.totalFrameCount;
                scriptPlayable.GetBehaviour().target = anim;
            }
            scriptPlayable.GetBehaviour().GraphParent = owner;
            scriptPlayable.GetBehaviour().isMappingIndex = NeedMappingIndex;
            //scriptPlayable.GetBehaviour().animIndex = animIndex;
        }
        scriptPlayable.GetBehaviour().director = owner.GetComponent<PlayableDirector>();
        scriptPlayable.GetBehaviour().movementManager = owner.GetComponent<MovementManager>();
        return scriptPlayable;
    }
    [Button(ButtonSizes.Gigantic)]
    void ResetTween()
    {
        DOTween.KillAll();
    }
    public void SetStartFrame(int frame)
    {
        scriptPlayable.GetBehaviour().startFrame = frame;
    }

}
