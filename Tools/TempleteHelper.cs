using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEditor;
public class TempleteHelper : MonoBehaviour
{
    const string templetePath="Templetes/";
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
    public void UseTemplete()
    {
        if(!Application.isPlaying)
        {
            Debug.Log("请在运行时调用该命令");
            return;
        }
        if(targetTemplete==null)
        {
            Debug.LogError("没有选择模版");
            return;
        }
        Selection.activeGameObject=null;
        AssetDatabase.CopyAsset("Assets/Resources/Templetes/"+targetTemplete.name+".playable", "Assets/Resources/Projects/"+ProjectManager.Instance.projectName+"/" + gameObject.name + ".playable");
        var asset=Resources.Load<TimelineAsset>("Projects/" + ProjectManager.Instance.projectName + "/" + gameObject.name);
        GetComponent<PlayableDirector>().playableAsset=asset;
        float maxTime=0;
        foreach (var track in asset.GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                var temp=clip.asset as ControlBlock;
                if(temp!=null)
                {   
                    temp.targetDataName=temp.data.dataName;
                    temp.RefreshData();
                    temp.SetWorkRangeMax();
                    if(temp.processer!=null)
                    {
                        if(temp.data.animTime>maxTime)
                        maxTime=temp.data.animTime;
                    }
                }
            }
        }
        Invoke("SetCurrentObj",maxTime);
    }
    void SetCurrentObj()
    {
        Selection.activeGameObject=gameObject;
        Debug.Log("应用模版完成");

    }

}
