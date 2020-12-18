﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEditor.Timeline;
using UnityEditor;
public class ProjectManager : MonoBehaviour
{
    const string ProjectParentPath = "Projects/";
    static ProjectManager instance;
    public static ProjectManager Instance
    {
        get
        {
            if (instance == null)
                instance = Camera.main.GetComponent<ProjectManager>();
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


    public string projectName;
    public static List<string> availableGroups
    {
        get
        {
            var globalAnimNames = new List<string>();
            var temp = Instance.RecordProject.globalPosDic;
            foreach (var info in temp)
            {
                globalAnimNames.Add(info.groupName);
            }
            return globalAnimNames;
        }
    }


    public static List<string> AllMainAnimNames
    {
        get
        {

            var allAnimNames = new List<string>();
            foreach (var animation in GetPointsRoot().GetComponents<TxtForAnimation>())
            {
                if (animation.danceDB.animName != null && animation.danceDB.animName.StartsWith("G"))
                    allAnimNames.Add(animation.danceDB.animName);
            }
            return allAnimNames;
        }
    }

    public static GameObject GetPointsRoot()
    {
        return GameObject.FindObjectOfType<MovementManager>().gameObject;
    }


    public static void ResetAllColorAndTween()
    {
        DOTween.CompleteAll();
        DOTween.KillAll();
        //currentTarget.ResetAllColor();
    }


    public static TxtForAnimation FindAnimByName(string name)
    {
        var anims = GetPointsRoot().GetComponents<TxtForAnimation>();
        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i].danceDB.animName == name)
                return anims[i];
        }
        Debug.LogError(name + "   动画没有找到");
        return null;
    }


    // public static List<string> FindPointsByPos(PointIndexInfo info)
    // {
    //     var anims = GetPointsRoot().GetComponents<TxtForAnimation>();
    //     for (int i = 0; i < anims.Length; i++)
    //     {
    //         var tempNames=anims[i].FindPointNames(info.posList);
    //         if(!tempNames.Contains(null)&&tempNames.Count!=0)
    //         return tempNames;
    //     }
    //     Debug.LogError("所有动画都没找到这组坐标对应的点");
    //     return null;
    // }
    public static TxtForAnimation GetAnimationByName(string animName)
    {
        var anims = GetPointsRoot().GetComponents<TxtForAnimation>();
        foreach (var anim in anims)
        {
            if (anim.danceDB.animName.Equals(animName))
                return anim;
        }
        Debug.LogError("没有名为" + animName + "的动画");
        return null;
    }


    public static TxtForAnimation GetAnimationbyGroupIndex(int groupIndex)
    {
        var anims = GetPointsRoot().GetComponents<TxtForAnimation>();
        return anims[groupIndex - 1];
    }


    public static GlobalPosInfo GetGlobalPosInfoByGroup(string groupName)
    {
        var info = instance.RecordProject.globalPosDic.Find((a) => a.groupName.Equals(groupName));
        return info;
    }


    public static DataGroup GetDataGroupByGroupName(string groupName)
    {
        string path = "Projects/" + Instance.projectName + "/" + groupName;
        DataGroup result = Resources.Load<DataGroup>(path);
        //DataGroup result = instance.RecordProject.datas.Find((a) => a.groupName.Equals(groupName));
        if (result == null)
        {
            Debug.Log("没有找到名为" + groupName + "的数据,返回新数据组");
            result = ScriptableObject.CreateInstance<DataGroup>();
            result.groupName = groupName;
            AssetDatabase.CreateAsset(result, "Assets/Resources/Projects/" + Instance.projectName + "/" + groupName + ".asset");
            //instance.recordProject.datas.Add(result);
            return result;
        }
        return result;
    }


}
