using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SceneEditor:Editor
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
            menu.AddItem(new GUIContent("显示名字"), false, delegate{ShowSceneObjName.Show=true;}, "menu_2");
            menu.AddItem(new GUIContent("隐藏名字"), false, delegate{ShowSceneObjName.Show=false;}, "menu_2");

            menu.ShowAsContext();
        }
    }
    static void Show(object userData)
    {
        //EditorUtility.DisplayDialog("Tip", "OnMenuClick"+ userData.ToString(), "Ok");
        var mat=Resources.Load<Material>("bai");
        mat.color=Color.white;
    }
    static void Hide(object userData)
    {
        //EditorUtility.DisplayDialog("Tip", "OnMenuClick"+ userData.ToString(), "Ok");
        var mat=Resources.Load<Material>("bai");
        Color color=mat.color;
        color=Color.black;
        color.a=0.01f;
        mat.color=color;
    }
    public class ShowSceneObjName : Editor
{
    public static bool Show;
    [DrawGizmo(GizmoType.NonSelected)]
    static void DrawGameObjectName(Transform transform, GizmoType gizmoType)
    {
        int temp;
        if(ShowSceneObjName.Show)
        {
            if(int.TryParse(transform.gameObject.name,out temp))
            Handles.Label(transform.position, transform.gameObject.name);
        }

    }
}

}