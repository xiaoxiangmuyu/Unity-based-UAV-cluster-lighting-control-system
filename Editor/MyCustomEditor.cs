﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class MyCustomEditor : Editor
{
    [InitializeOnLoadMethod]
    static void Init()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e != null && e.button == 1 && e.type == EventType.MouseDown)
        {
            //右键单击啦，在这里显示菜单
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("显示"), false, Show, "menu_1");
            menu.AddItem(new GUIContent("隐藏"), false, Hide, "menu_2");
            menu.AddItem(new GUIContent("MoveToView"), false, SetCameraPos, "menu_3");
            menu.AddItem(new GUIContent("创建数据组"), false, CreatGroup, "menu_4");
            menu.AddItem(new GUIContent("创建映射组"), false, CreatMapping, "menu_5");

            //menu.AddItem(new GUIContent("取消所有动画"),false,CancelTween,"menu_4");


            menu.ShowAsContext();
        }
    }
    static void Show(object userData)
    {
        //EditorUtility.DisplayDialog("Tip", "OnMenuClick"+ userData.ToString(), "Ok");
        var mat = Resources.Load<Material>("bai");
        mat.color = Color.white;
    }
    static void Hide(object userData)
    {
        //EditorUtility.DisplayDialog("Tip", "OnMenuClick"+ userData.ToString(), "Ok");
        var mat = Resources.Load<Material>("bai");
        Color color = mat.color;
        color = Color.black;
        color.a = 0.01f;
        mat.color = color;
    }
    //移动摄像机角度使其与场景视角相同，不需要选中摄像机
    static void SetCameraPos(object userData)
    {
        GameObject obj = Camera.main.gameObject;
        Selection.activeGameObject = obj;
        SceneView.lastActiveSceneView.AlignWithView();
        obj.transform.SetParent(null);
        obj.transform.SetAsFirstSibling();

    }
    static void CancelTween(object userData)
    {
        ProjectManager.ResetAllColorAndTween();
    }
    static void CreatGroup(object userData)
    {
        RecordData tempdata = new RecordData();
        foreach (var point in Selection.objects)
        {
            tempdata.ObjNames.Add(point.name);
            tempdata.times.Add(0);
        }
        ProjectManager.Instance.RecordProject.AddData(ProjectManager.GetCurrentMR().name, tempdata);
        Debug.Log("创建组成功");
    }
    static void CreatMapping(object userData)
    {
        GameObject parent=ProjectManager.GetCurrentMR().gameObject;
        GameObject temp=new GameObject();
        temp.AddComponent<ColorMapping>();
        temp.transform.SetParent(parent.transform);
        foreach(var point in Selection.gameObjects)
        {
            point.transform.SetParent(temp.transform);
        }

    }
    [MenuItem("GameObject/工具/创建数据组", priority = 0)]
    static void CreatGroup()
    {
        RecordData tempdata = new RecordData(Selection.activeGameObject.name);
        foreach (var point in Selection.activeGameObject.GetComponentsInChildren<ColorPoint>())
        {
            tempdata.ObjNames.Add(point.gameObject.name);
            tempdata.times.Add(0);
        }
        ProjectManager.Instance.RecordProject.AddData(ProjectManager.GetCurrentMR().name, tempdata);
        Debug.Log("创建组成功");

    }
    
    [MenuItem("GameObject/工具/批量添加Tag", priority = 0)]
    static void AddTag()
    {
        GameObject obj = Selection.activeGameObject;
        foreach (var point in obj.GetComponentsInChildren<ColorPoint>())
        {
            point.AddTag(obj.name);
        }
        Debug.Log("添加Tag成功");
    }
    [MenuItem("GameObject/工具/应用模板", priority = 0)]
    static void UseTemplate()
    {
        GameObject obj = Selection.activeGameObject;
        PlayableDirector playableDirector = obj.GetComponentInParent<PlayableDirector>();
        // if(!playableDirector)
        // playableDirector=obj.GetComponentInParent<PlayableDirector>();
        var timeLineAsset = playableDirector.playableAsset as TimelineAsset;
        foreach (var track in timeLineAsset.GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                var temp=clip.asset as ControlBlock;
                if(temp!=null)
                {   
                    temp.FindData(temp.data.dataName);
                    temp.SetWorkRangeMax();
                }
            }
        }
        Debug.Log("应用模板完成");

    }

}

