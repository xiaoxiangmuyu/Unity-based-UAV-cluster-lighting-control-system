using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Timeline;
[CustomTimelineEditor(typeof(ControlBlock))]
public class TransformTweenClipEditor : ClipEditor
{
    public override ClipDrawOptions GetClipOptions(TimelineClip clip)
    {
        ClipDrawOptions clipOptions = base.GetClipOptions(clip);
        var cb = clip.asset as ControlBlock;
        if(cb!=null)
        clipOptions.highlightColor=cb.blockColor;
        //var anim=clip.asset as TxtAnimAsset;
        //if(anim!=null)
        //{
        //    if (anim.totalFrameCount == 0)
        //        clipOptions.highlightColor = Color.green;
        //    else
        //        clipOptions.highlightColor = Color.yellow;
        //}
        //else
        //{
        //    Debug.Log("is null");
        //}
        // if (cb.state==BlockState.Ready)
        // {
        //     clipOptions.highlightColor = Color.green;
        // }
        // else if(cb.state==BlockState.NoData)
        // {
        //     clipOptions.highlightColor = Color.red;
        // }
        // else if(cb.state==BlockState.NeedRefresh)
        // {
        //     clipOptions.highlightColor = Color.yellow;
        // }
        return clipOptions;
    }
}
