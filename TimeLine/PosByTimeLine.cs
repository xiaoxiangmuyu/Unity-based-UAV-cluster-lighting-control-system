using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
[CreateAssetMenu(menuName = "创建位置序列", fileName = "新位置序列")]
public class PosByTimeLine : PlayableAsset
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var scriptPlayable= ScriptPlayable<PosBehavior>.Create(graph);
        scriptPlayable.GetBehaviour().script=owner.GetComponent<TxtForAnimation>();
        scriptPlayable.SetDuration(owner.GetComponent<TxtForAnimation>().totalFrameCount/25);
        return scriptPlayable;
    }
}
