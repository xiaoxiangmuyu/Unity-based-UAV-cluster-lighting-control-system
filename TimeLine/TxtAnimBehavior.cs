using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
public class TxtAnimBehavior : PlayableBehaviour
{
    public TxtForAnimation script;
    public PlayableDirector director;
    public MovementManager movementManager;
    int curframe;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //DOTween.ManualUpdate(0.04f, 0.04f);
        if(!movementManager.needExport)
        UpdatePos();
        else
        UpdatePosFrameByFrame();

    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //Debug.Log("OnBehaviourPlay");
    }
    public override void OnGraphStart(Playable playable)
    {      
        //DOTween.KillAll();
        
    }
    void UpdatePos()
    {
        if(!script)
        return;
        curframe=Mathf.RoundToInt((float)director.time*25);
        //Debug.Log(curframe);
        script.MyUpdate(curframe);
    }
    void UpdatePosFrameByFrame()
    {
        if(!script)
        return;
        Debug.Log(curframe);
        script.MyUpdate(curframe);
        curframe+=1;
    }
}
