using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
public class OverallBehavior : PlayableBehaviour
{
    public GameObject GraphParent;
    public OverallAsset script;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {


    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if(Application.isPlaying)
        script.Begin();
    }
    public override void OnGraphStart(Playable playable)
    {      
        if(!GraphParent.activeSelf)
        return;
        MyTools.UpdateDuring(GraphParent);
        
    }

}
