using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
public class PosBehavior : PlayableBehaviour
{
    public TxtForAnimation script;
    ulong firstFrame;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        DOTween.ManualUpdate(0.04f, 0.04f);
        UpdatePos(info);
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        firstFrame = 0;
        Debug.Log("OnBehaviourPlay");
    }
    public override void OnGraphStart(Playable playable)
    {
        
        Debug.Log("OnGraphStart");
    }
    void UpdatePos(FrameData info)
    {
        if(!script)
        return;
        if (firstFrame == 0)
            firstFrame = info.frameId;
        ulong currentFrame = info.frameId - firstFrame;
        script.MyUpdate((int)currentFrame);
        //Debug.Log(currentFrame);
    }
}
