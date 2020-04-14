using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
public class ProjectManager : MonoBehaviour
{
    static MovementManager currentTarget;



    public  Quaternion RotationInfo;
    public  Vector3 PosInfo;
    public  int ChildCount;
    public static ProjectManager instance;



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
    public static void ResetAllColorAndTween()
    {
        DOTween.CompleteAll();
        currentTarget.ResetAllColor();   
    }

}
