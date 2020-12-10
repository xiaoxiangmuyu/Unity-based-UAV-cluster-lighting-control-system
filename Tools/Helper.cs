using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEditor;
using UnityEditor.Timeline;

[ExecuteInEditMode]
public class Helper : MonoBehaviour
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
        var trackRoot = asset.CreateTrack<GroupTrack>(targetTemplete.name);
        var timelineLength = asset.duration;
        foreach (var track in templete.GetOutputTracks())
        {
            if (track.name == "Markers")
                continue;
            var tempTrack = asset.CreateTrack<PlayableTrack>(trackRoot, track.name);
            foreach (var clip in track.GetClips())
            {
                var tempClip = tempTrack.CreateClip<ControlBlock>();
                tempClip.start = clip.start;
                tempClip.duration = clip.duration;
                tempClip.displayName = clip.displayName;
                var from = JsonUtility.ToJson(clip.asset);
                JsonUtility.FromJsonOverwrite(from, tempClip.asset);
                //tempClip.start+=timelineLength;
            }
        }
        //Selection.activeGameObject = gameObject;
        TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

        Debug.Log("应用模版完成");
    }
    void SetCurrentObj()
    {
        Selection.activeGameObject = gameObject;
        Debug.Log("应用模版完成");

    }
    private void OnEnable()
    {
        ProjectManager.SetOperateTarget(gameObject);
    }
    private void OnDisable()
    {
        if (ProjectManager.GetPointsRoot() == GetComponent<MovementManager>())
        {
            ProjectManager.RefreshCurTarget();
        }
    }
    [Button("生成点", ButtonSizes.Gigantic)]
    public void GeneratePoint(int number)
    {
        if (transform.childCount != 0)
        {
            Debug.Log("已经有子物体了");
            return;
        }
        GameObject pointPrefab = Resources.Load<GameObject>("PointPrefab");
        GameObject temp;
        for (int i = 0; i < number; i++)
        {
            temp = Instantiate(pointPrefab);
            temp.transform.SetParent(transform);
            temp.name = (i + 1).ToString();
        }
    }
    [Button("创建所有动画", ButtonSizes.Gigantic)]
    void CreatAllAnimForTimeLine()
    {
        var anims = GetComponents<TxtForAnimation>();
        foreach (var anim in anims)
        {
            anim.Init();
        }
        var asset = GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
        var trackRoot = asset.CreateTrack<PlayableTrack>("Animation");
        float end = 0;
        for (int i = 0; i < anims.Length; i++)
        {
            var clip = trackRoot.CreateClip<TxtAnimAsset>();
            clip.start = end;
            var temp = clip.asset as TxtAnimAsset;
            temp.animName = anims[i].danceDB.animName;
            if (anims[i].danceDB.totalFrameCount == 0)
            {
                temp.safeSeconds = 20;
                end += 20;
            }
            else
            {
                //end += anims[i].totalFrameCount / 25f + 2;
                end += anims[i].danceDB.totalFrameCount / 25f;
            }
        }
        TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        Debug.Log("创建动画完成");
        ProjectManager.Instance.RecordProject.MappingAll();
        ProjectManager.Instance.RecordProject.GenerateGlobalPos();

    }


}
