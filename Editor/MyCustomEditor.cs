using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
public class MyCustomEditor : Editor
{
    [InitializeOnLoadMethod]
    static void Init()
    {
        //SceneView.onSceneGUIDelegate += OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
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
            //menu.AddItem(new GUIContent("创建旧映射组"), false, CreatOldMapping, "menu_5");
            menu.AddItem(new GUIContent("刷新时间轴"), false, Resfrsh, "menu_7");
            menu.AddItem(new GUIContent("创建全局位置数据"), false, CreatGlobalPosData, "menu_8");
            //menu.AddItem(new GUIContent("校正所选效果"), false, CorrectIndex, "menu_9");
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
    //创建全局数据组
    static void CreatGlobalPosData(object userData)
    {
        var tempDic = new StringVector3Dictionary();
        foreach (var point in Selection.gameObjects)
        {
            if (point.name == "Main Camera")
                continue;
            if (point.transform.childCount != 0)
            {
                foreach (var child in point.GetComponentsInChildren<ColorPoint>())
                {
                    tempDic.Add(child.name,child.transform.position);
                }
            }
            else
            {
                tempDic.Add(point.name,point.transform.position);
            }
        }
        var tempPointNames=new List<string>(tempDic.Keys);
        tempPointNames.Sort((a,b)=>int.Parse(a)-int.Parse(b));
        var tempPos=new List<Vector3>();
        for(int i=0;i<tempPointNames.Count;i++)
        {
            tempPos.Add(tempDic[tempPointNames[i]]);
        }
        var result=new GlobalPosInfo();
        result.groupName="newAnim";
        result.posList=new List<Vector3>(tempPos);
        ProjectManager.Instance.RecordProject.globalPosDic.Add(result);

    }
    //创建数据组
    static void CreatGroup(object userData)
    {
        RecordData tempdata = new RecordData();
        tempdata.pointsInfo.posList = new List<Vector3>();
        foreach (var point in Selection.gameObjects)
        {
            if (!int.TryParse(point.name,out int res))
                continue;
            if (point.transform.childCount != 0)
            {
                foreach (var child in point.GetComponentsInChildren<ColorPoint>())
                {
                    tempdata.objNames.Add(point.name);
                    tempdata.pointsInfo.posList.Add(MyTools.TruncVector3(child.transform.position));
                    tempdata.times.Add(0);
                    //tempdata.posDic.Add(child.name, child.transform.position);
                }
                tempdata.dataName = point.name;
            }
            else
            {
                tempdata.objNames.Add(point.name);
                tempdata.pointsInfo.posList.Add(MyTools.TruncVector3(point.transform.position));
                tempdata.times.Add(0);
                //tempdata.posDic.Add(point.name, point.transform.position);

            }
        }
        ProjectManager.Instance.RecordProject.AddData(tempdata);
        Debug.Log("创建数据组成功");
    }
    // //创建旧映射组
    // static void CreatOldMapping(object userData)
    // {
    //     GameObject parent = ProjectManager.GetPointsRoot().gameObject;
    //     GameObject temp = new GameObject();
    //     temp.AddComponent<ColorMapping>();
    //     temp.transform.SetParent(parent.transform);
    //     temp.transform.SetAsFirstSibling();
    //     foreach (var point in Selection.gameObjects)
    //     {
    //         if (point.name == "Main Camera")
    //             continue;
    //         point.transform.SetParent(temp.transform);
    //     }
    //     Debug.Log("创建旧映射组成功");
    //     Selection.activeGameObject = temp;
    // }
    // [MenuItem("GameObject/工具/创建数据组", priority = 0)]
    // static void CreatGroup()
    // {
    //     RecordData tempdata = new RecordData(Selection.activeGameObject.name);
    //     tempdata.pointsInfo.animName=ProjectManager.curAnimName;
    //     tempdata.pointsInfo.frame=ProjectManager.curAnimFrame;
    //     tempdata.pointsInfo.posList=new List<Vector3>();
    //     foreach (var point in Selection.activeGameObject.GetComponentsInChildren<ColorPoint>())
    //     {
    //         tempdata.pointsInfo.posList.Add(MyTools.TruncVector3(point.transform.position));
    //         tempdata.times.Add(0);
    //         //tempdata.posDic.Add(point.name, point.transform.position);
    //     }
    //     ProjectManager.Instance.RecordProject.AddData(ProjectManager.GetPointsRoot().name, tempdata);
    //     Debug.Log("创建数据组成功");

    // }
    static void CreatMapping(object userData)
    {
        MappingData tempdata = new MappingData();
        tempdata.pointsInfo.posList = new List<Vector3>();
        //tempdata.Objects = Selection.gameObjects;
        tempdata.ObjNames = new List<string>();
        foreach (var point in Selection.gameObjects)
        {
            if (!int.TryParse(point.name,out int res))
                continue;
            tempdata.ObjNames.Add(point.name);
            tempdata.pointsInfo.posList.Add(MyTools.TruncVector3(point.transform.position));
        }
        ProjectManager.Instance.RecordProject.AddMappingData(tempdata);
        Debug.Log("创建颜色组成功");
    }
    // [MenuItem("GameObject/工具/创建全局数据组", priority = 0)]
    // static void CreatGlobalPosData()
    // {
    //     var temp=new StringVector3Dictionary();
    //     foreach (var point in Selection.activeGameObject.GetComponentsInChildren<ColorPoint>())
    //     {
    //         temp.Add(point.name,point.transform.position);
    //     }
    //     ProjectManager.Instance.RecordProject.AddMappingData(tempdata);
    //     Debug.Log("创建映射组成功");
    // }


    // [MenuItem("GameObject/工具/创建映射组", priority = 0)]
    // static void CreatMapping()
    // {
    //     MappingData tempdata = new MappingData();
    //     //tempdata.Objects = Selection.gameObjects;
    //     tempdata.pointNames = new List<string>();
    //     foreach (var point in Selection.activeGameObject.GetComponentsInChildren<ColorPoint>())
    //     {
    //         tempdata.pointNames.Add(point.name);
    //     }
    //     tempdata.dataName = Selection.activeGameObject.name;
    //     ProjectManager.Instance.RecordProject.AddMappingData(tempdata);
    //     Debug.Log("创建映射组成功");
    // }
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
    // [MenuItem("GameObject/工具/应用模板", priority = 0)]
    // static void UseTemplate()
    // {
    //     GameObject obj = Selection.activeGameObject;
    //     if (obj.GetComponent<Helper>())
    //         obj.GetComponent<Helper>().UseTemplete();
    //     else
    //     {
    //         Debug.LogError("请添加TempleteHelper组件");
    //     }
    // }
    static void Resfrsh(object userData)
    {
        var clips = TimelineEditor.selectedClips;
        if (clips.Length == 0)
        {
            var obj = ProjectManager.GetPointsRoot();
            var asset = obj.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
            foreach (var track in asset.GetOutputTracks())
            {
                foreach (var clip in track.GetClips())
                {
                    var temp = clip.asset as ControlBlock;
                    if (temp != null)
                    {
                        temp.targetDataName = temp.data.dataName;
                        temp.RefreshData();
                    }
                }
            }
        }
        else
        {
            foreach (var clip in clips)
            {
                var temp = clip.asset as ControlBlock;
                if (temp != null)
                {
                    temp.targetDataName = temp.data.dataName;
                    temp.RefreshData();
                }
            }
        }
    }
    static void CorrectIndex(object userData)
    {
        var clips = TimelineEditor.selectedClips;
        if(clips.Length==0)
        {
            Debug.LogError("没有选中效果");
        }
        foreach(var clip in clips)
        {
            var cb=clip.asset as ControlBlock;
            var dataName=cb.data.dataName;
            ProjectManager.Instance.RecordProject.RecorDataList.Find((a)=>a.dataName.Equals(dataName)).CorrectIndex();
        }


    }
}

