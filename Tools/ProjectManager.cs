using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEditor;
public class ProjectManager : MonoBehaviour
{
    static MovementManager currentTarget;
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

    void Start()
    {

    }
    void Update()
    {

    }
    public static void SetOperateTarget(MovementManager mr)
    {
        currentTarget = mr;
    }
    public static MovementManager GetCurrentMR()
    {
        if (ProjectManager.currentTarget == null)
        {
            MovementManager[] mm = GameObject.FindObjectsOfType<MovementManager>();
            foreach (var m in mm)
            {
                if (m.enabled)
                {
                    ProjectManager.currentTarget = m;
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
                ProjectManager.currentTarget = m;
                return;
            }
        }
    }
    public static void ResetAllColorAndTween()
    {
        DOTween.CompleteAll();
        DOTween.KillAll();
        currentTarget.ResetAllColor();
    }


}
