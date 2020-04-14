using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
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
            menu.AddItem(new GUIContent("取消所有动画"),false,CancelTween,"menu_4");
           // menu.AddItem(new GUIContent("Dotween手动模式"), false, DotweenManualUpdate, "menu_2");
            //menu.AddItem(new GUIContent("Dotween普通模式"), false, DotweenNormalUpdate, "menu_2");
            //menu.AddItem(new GUIContent("显示名字"), false, delegate{ShowSceneObjName.Show=true;}, "menu_2");
            //menu.AddItem(new GUIContent("隐藏名字"), false, delegate{ShowSceneObjName.Show=false;}, "menu_2");

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
        GameObject obj=Camera.main.gameObject;
        Selection.activeGameObject=obj;
        SceneView.lastActiveSceneView.AlignWithView();
        obj.transform.SetParent(null);
        obj.transform.SetAsFirstSibling();

    }
    static void CancelTween(object userData)
    {
        ProjectManager.ResetAllColorAndTween();
    }
    static void DotweenManualUpdate(object userData)
    {
        DOTween.defaultUpdateType=UpdateType.Manual;
    }
    static void DotweenNormalUpdate(object userData)
    {
        DOTween.defaultUpdateType=UpdateType.Normal;
    }
    public class ShowSceneObjName : Editor
    {
        public static bool Show;
        [DrawGizmo(GizmoType.NonSelected)]
        static void DrawGameObjectName(Transform transform, GizmoType gizmoType)
        {
            int temp;
            if (ShowSceneObjName.Show)
            {
                if (int.TryParse(transform.gameObject.name, out temp))
                    Handles.Label(transform.position, transform.gameObject.name);
            }

        }
        [MenuItem("GameObject/工具/添加Tag", priority = 0)]
        static void Init()
        {
            GameObject obj=Selection.activeGameObject;
            foreach(var point in obj.GetComponentsInChildren<ColorPoint>())
            {
                point.AddTag(obj.name);
            }
            Debug.Log("添加Tag成功");
        }
    }

}