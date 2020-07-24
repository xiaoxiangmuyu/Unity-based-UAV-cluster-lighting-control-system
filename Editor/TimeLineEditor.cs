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
        var cb = clip.asset as ControlBlock;
        ClipDrawOptions clipOptions = base.GetClipOptions(clip);
        if (cb.state==BlockState.Ready)
        {
            clipOptions.highlightColor = Color.green;
        }
        else if(cb.state==BlockState.NoData)
        {
            clipOptions.highlightColor = Color.red;
        }
        else if(cb.state==BlockState.NeedRefresh)
        {
            clipOptions.highlightColor = Color.yellow;
        }
        return clipOptions;
    }
}
