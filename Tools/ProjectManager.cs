using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
public class ProjectManager : MonoBehaviour
{
    static MovementManager currentTarget;

    const string ProjectParentPath="Projects/";
    static ProjectManager instance;
    public static ProjectManager Instance{get{
        if(instance==null)instance=Camera.main.GetComponent<ProjectManager>();
        return instance;
        }}
    RecordProject recordProject;
    public RecordProject RecordProject{get{
        if(recordProject==null)
        recordProject=Resources.Load<RecordProject>(ProjectParentPath+ProjectManager.instance.projectName+"/RecordParent");
        return recordProject;
    }}



    public  Quaternion RotationInfo;
    public  Vector3 PosInfo;
    public  int ChildCount;
    public string projectName;
    



    bool needUpdateTween;
    private void Awake() {
        instance=this;
    }

    void Start()
    {
        needUpdateTween=!currentTarget.GetComponent<PlayableDirector>().enabled;
    }
    void Update()
    {
        if(needUpdateTween)
        {
            DOTween.ManualUpdate(0.04f, 0.04f);
        }
    }
    public static void SetOperateTarget(MovementManager mr)
    {
        currentTarget=mr;
    }
    public static MovementManager GetCurrentMR()
    {
        return ProjectManager.currentTarget;
    }
    public static void ResetAllColorAndTween()
    {
        DOTween.CompleteAll();
        currentTarget.ResetAllColor();   
    }
    [Button(ButtonSizes.Gigantic)]
    void Load()
    {
        recordProject=Resources.Load<RecordProject>(ProjectParentPath+ProjectManager.instance.projectName+"/RecordParent");
    }

}
