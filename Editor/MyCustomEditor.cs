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
            menu.AddItem(new GUIContent("显示"), false, ShowAndHide, "menu_1");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("创建数据"), false, CreatGroup, "menu_4");
            menu.AddItem(new GUIContent("创建颜色"), false, CreatMapping, "menu_5");
            menu.AddItem(new GUIContent("颜色预览"), false, ColorPreview, "menu_6");
            menu.AddItem(new GUIContent("颜色重置"), false, ResetAllColor, "menu_8");
            //menu.AddItem(new GUIContent("创建旧映射组"), false, CreatOldMapping, "menu_5");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("调整摄像机"), false, SetCameraPos, "menu_3");
            menu.AddItem(new GUIContent("刷新时间轴"), false, Resfrsh, "menu_7");
            menu.AddItem(new GUIContent("创建全局位置数据"), false, CreatGlobalPosData, "test/menu_8");
            menu.AddItem(new GUIContent("激活全部点"), false, ActiveAll, "test/menu_9");
            //menu.AddItem(new GUIContent("校正所选效果"), false, CorrectIndex, "menu_9");
            menu.ShowAsContext();
        }
    }
    static void ShowAndHide(object userData)
    {
        //EditorUtility.DisplayDialog("Tip", "OnMenuClick"+ userData.ToString(), "Ok");
        var mat = Resources.Load<Material>("bai");
        if (mat.color.Equals(Color.white))
        {
            Color color = mat.color;
            color = Color.black;
            color.a = 0.01f;
            mat.color = color;
        }
        else
            mat.color = Color.white;
    }

    //移动摄像机角度使其与场景视角相同，不需要选中摄像机
    static void SetCameraPos(object userData)
    {
        GameObject obj = Camera.main.gameObject;
        Selection.activeGameObject = obj;
        SceneView.lastActiveSceneView.AlignWithView();
        //obj.transform.SetParent(null);
        GameObject preview = GameObject.Find("PointsPreview");
        Selection.activeGameObject = preview;
        SceneView.lastActiveSceneView.AlignWithView();

        //obj.transform.SetAsFirstSibling();

    }
    //创建全局数据组
    static void CreatGlobalPosData(object userData)
    {
        var tempDic = new StringVector3Dictionary();
        GameObject root = GameObject.Find("Main");
        for (int i = 0; i < root.transform.childCount; i++)
        {
            var child = root.transform.GetChild(i);
            if (child.GetComponent<ColorPoint>())
            {
                tempDic.Add(child.name, child.transform.position);
            }
        }
        var tempPointNames = new List<string>(tempDic.Keys);
        tempPointNames.Sort((a, b) => int.Parse(a) - int.Parse(b));
        var tempPos = new List<Vector3>();
        for (int i = 0; i < tempPointNames.Count; i++)
        {
            tempPos.Add(tempDic[tempPointNames[i]]);
        }
        var result = new GlobalPosInfo();
        result.groupName = "newAnim";
        result.posList = new List<Vector3>(tempPos);
        ProjectManager.Instance.RecordProject.globalPosDic.Add(result);

    }
    //创建数据组
    static void CreatGroup(object userData)
    {
        RecordData tempdata = new RecordData();
        tempdata.pointsInfo.posList = new List<Vector3>();
        foreach (var point in Selection.gameObjects)
        {
            if (!int.TryParse(point.name, out int res))
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
    static void CreatMapping(object userData)
    {
        MappingData tempdata = new MappingData();
        tempdata.pointsInfo.posList = new List<Vector3>();
        //tempdata.Objects = Selection.gameObjects;
        tempdata.objNames = new List<string>();
        foreach (var point in Selection.gameObjects)
        {
            if (!int.TryParse(point.name, out int res))
                continue;
            tempdata.objNames.Add(point.name);
            tempdata.pointsInfo.posList.Add(MyTools.TruncVector3(point.transform.position));
        }
        ProjectManager.Instance.RecordProject.AddMappingData(tempdata);
        Debug.Log("创建颜色组成功");
    }
    // [MenuItem("GameObject/工具/批量添加Tag", priority = 0)]
    // static void AddTag()
    // {
    //     GameObject obj = Selection.activeGameObject;
    //     foreach (var point in obj.GetComponentsInChildren<ColorPoint>())
    //     {
    //         point.AddTag(obj.name);
    //     }
    //     Debug.Log("添加Tag成功");
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
    static void ActiveAll(object userdata)
    {
        GameObject root = GameObject.Find("Main");
        for (int i = 0; i < root.transform.childCount; i++)
        {
            root.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    static void ResetAllColor(object userdata)
    {
        GameObject root = GameObject.Find("Main");
        foreach (var point in root.GetComponentsInChildren<ColorPoint>())
        {
            point.mat.color = Color.white;
        }
    }
    static void ColorPreview(object userdata)
    {
        ProjectManager.Instance.RecordProject.ColorPreview();
    }
    [MenuItem("GameObject/泽宇工具箱/准备渲染套件", priority = 0)]
    static void RenderMode()
    {
        var tool = Resources.Load<GameObject>("工具箱/CinemachineTools");
        Instantiate(tool);
        Debug.Log("渲染套件准备完成");
    }
    [MenuItem("GameObject/使用模版/圆形水波闪烁", priority = 0)]
    static void UseCrossTemplete()
    {
        var helper = GameObject.FindObjectOfType<Helper>();
        helper.targetTemplete = Resources.Load<PlayableAsset>("Templetes/" + "单向水波闪烁");
        helper.UseTemplete();
    }
    [MenuItem("GameObject/使用模版/圆形水波循环闪烁", priority = 0)]
    static void UseCrossTemplete1()
    {
        var helper = GameObject.FindObjectOfType<Helper>();
        helper.targetTemplete = Resources.Load<PlayableAsset>("Templetes/" + "循环水波闪烁");
        helper.UseTemplete();
    }


}

