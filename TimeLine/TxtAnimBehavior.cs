using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
public class TxtAnimBehavior : PlayableBehaviour
{
    public TxtForAnimation script;
    public PlayableDirector director;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        DOTween.ManualUpdate(0.04f, 0.04f);
        UpdatePos();
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //Debug.Log("OnBehaviourPlay");
    }
    public override void OnGraphStart(Playable playable)
    {      
        //Debug.Log("OnGraphStart");
    }
    void UpdatePos()
    {
        if(!script)
        return;
        script.MyUpdate(Mathf.RoundToInt((float)director.time*25));
    }
}
