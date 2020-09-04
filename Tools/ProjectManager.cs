using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEditor.Timeline;
public class ProjectManager : MonoBehaviour
{
    static GameObject currentTarget;
    const string ProjectParentPath = "Projects/";
    static ProjectManager instance;
    public static TxtForAnimation currentAnim;
    public static TxtAnimBehavior currentAnimBehavior;
    public static ProjectManager Instance
    {
        get
        {
            if (instance == null) instance = Camera.main.GetComponent<ProjectManager>();
            return instance;
        }
    }
    [SerializeField]
    public static string curAnimName;
    [SerializeField]
    public static int curAnimFrame;
    RecordProject recordProject;
    public RecordProject RecordProject
    {
        get
        {
            if (recordProject == null)
                recordProject = Resources.Load<RecordProject>(ProjectParentPath + ProjectManager.instance.projectName + "/RecordParent");
            return recordProject;
        }
    }
    public Quaternion RotationInfo;
    public Vector3 PosInfo;
    public int ChildCount;
    public string projectName;

    private void Awake()
    {
        instance = this;
    }
    public static void SetOperateTarget(GameObject mr)
    {
        currentTarget = mr;
    }
    public static GameObject GetPointsRoot()
    {
        if (ProjectManager.currentTarget == null)
        {
            MovementManager[] mm = GameObject.FindObjectsOfType<MovementManager>();
            foreach (var m in mm)
            {
                if (m.enabled)
                {
                    ProjectManager.currentTarget = m.gameObject;
                    break;
                }
            }
        }
        return ProjectManager.currentTarget;
    }
    public static void RefreshCurTarget()
    {
        MovementManager[] mm = GameObject.FindObjectsOfType<MovementManager>();
        foreach (var m in mm)
        {
            if (m.gameObject.activeSelf)
            {
                ProjectManager.currentTarget = m.gameObject;
                return;
            }
        }
    }
    public static void ResetAllColorAndTween()
    {
        DOTween.CompleteAll();
        DOTween.KillAll();
        //currentTarget.ResetAllColor();
    }
    public static void SetAnimProcess(string name,int curframe)
    {
        ProjectManager.curAnimName=name;
        ProjectManager.curAnimFrame=curframe;
    }
    public static TxtForAnimation FindAnimByName(string name)
    {
        var anims=GetPointsRoot().GetComponents<TxtForAnimation>();
        for(int i=0;i<anims.Length;i++)
        {
            if(anims[i].animName==name)
            return anims[i];
        }
        Debug.LogError(name+"   动画没有找到");
        return null;
    }


}
