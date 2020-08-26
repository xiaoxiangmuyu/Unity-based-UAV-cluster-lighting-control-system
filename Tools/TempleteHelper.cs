using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEditor;
using UnityEditor.Timeline;
[ExecuteInEditMode]
public class TempleteHelper : MonoBehaviour
{
    const string templetePath = "Templetes/";
    [ValueDropdown("available")]//[OnValueChanged("UseTemplete")]
    public PlayableAsset targetTemplete;

    IEnumerable available
    {
        get
        {
            return Resources.LoadAll<PlayableAsset>(templetePath);
        }
    }
    [Button(ButtonSizes.Gigantic)]
    [LabelText("添加模版")]
    public void UseTemplete()
    {
        // if (!Application.isPlaying)
        // {
        //     Debug.Log("请在运行时调用该命令");
        //     return;
        // }
        if (targetTemplete == null)
        {
            Debug.LogError("没有选择模版");
            return;
        }
        var templete = Resources.Load<TimelineAsset>("Templetes/" + targetTemplete.name);
        //var asset = Resources.Load<TimelineAsset>("Projects/" + ProjectManager.Instance.projectName + "/" + gameObject.name);
        var asset = GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
        var trackRoot=asset.CreateTrack<GroupTrack>(targetTemplete.name);
        var timelineLength=asset.duration;
        foreach (var track in templete.GetOutputTracks())
        {
            if(track.name=="Markers")
            continue;
            var tempTrack= asset.CreateTrack<PlayableTrack>(trackRoot,track.name);
            foreach(var clip in track.GetClips())
            {
                var tempClip=tempTrack.CreateClip<ControlBlock>();
                tempClip.start=clip.start;
                tempClip.duration=clip.duration;
                tempClip.displayName=clip.displayName;
                tempClip.asset=clip.asset;
                tempClip.start+=timelineLength;
            }
        }
        //Selection.activeGameObject = gameObject;
        TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        
        Debug.Log("应用模版完成");
    }
    // [Button(ButtonSizes.Gigantic)]
    // void Resfrsh()
    // {
    //     Selection.activeGameObject = null;
    //     var asset = GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
    //     foreach (var track in asset.GetOutputTracks())
    //     {
    //         foreach (var clip in track.GetClips())
    //         {
    //             var temp = clip.asset as ControlBlock;
    //             if (temp != null)
    //             {
    //                 temp.targetDataName = temp.data.dataName;
    //                 temp.RefreshData();
    //                 temp.SetWorkRangeMax();
    //                 if (temp.processer != null)
    //                 {
    //                     temp.ProcessData();
    //                 }
    //             }
    //         }
    //     }
    // }
    void SetCurrentObj()
    {
        Selection.activeGameObject = gameObject;
        Debug.Log("应用模版完成");

    }
    private void OnEnable()
    {
        ProjectManager.SetOperateTarget(GetComponent<MovementManager>());
    }
    private void OnDisable()
    {
        if (ProjectManager.GetCurrentMR() == GetComponent<MovementManager>())
        {
            ProjectManager.RefreshCurTarget();
        }
    }

}
