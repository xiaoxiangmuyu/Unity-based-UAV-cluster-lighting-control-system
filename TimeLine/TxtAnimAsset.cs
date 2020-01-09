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
    [ReadOnly]
    public int totalFrameCount;
    TxtForAnimation script;
    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var scriptPlayable = ScriptPlayable<TxtAnimBehavior>.Create(graph);
        script = owner.GetComponent<TxtForAnimation>();
        if (script!=null)
        {
            totalFrameCount = script.totalFrameCount;
            scriptPlayable.GetBehaviour().script = script;
        }
        scriptPlayable.GetBehaviour().director = owner.GetComponent<PlayableDirector>();
        scriptPlayable.GetBehaviour().movementManager=owner.GetComponent<MovementManager>();
        //scriptPlayable.SetDuration(owner.GetComponent<TxtForAnimation>().totalFrameCount / 25);
        return scriptPlayable;
    }
    [Button(ButtonSizes.Gigantic)]
    void ResetTween()
    {
        DOTween.KillAll();
    }
}
