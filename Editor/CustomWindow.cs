using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class NodeEditor : EditorWindow
{
    string number="number";
    string animNumber="animNumber";
    [MenuItem("Window/项目初始化")]
    static void ShowEditor()
    {
        NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
        //editor.Init();
    }
    public void Init()
    {
        //window1 = new Rect(10, 10, 100, 100);
        //window2 = new Rect(210, 210, 100, 100);
    }
    void OnGUI()
    {
        GUILayout.Label("飞机数量");
        number=EditorGUILayout.TextField("飞机数量",number);
        GUILayout.Label("画面数量");
        animNumber=EditorGUILayout.TextField("画面数量",animNumber);
        if(GUI.Button(new Rect(10, 80, 100, 100),"初始化项目"))
        {
            var root=new GameObject("Main");
            int animCount=int.Parse(animNumber);
            //animCount=animCount*2-1;
            EfyTools.Init(new GameObject[1]{root});
            for(int i=0;i<animCount;i++)
            {
                root.AddComponent<TxtForAnimation>();
            }
            root.AddComponent<Helper>().GeneratePoint(int.Parse(number));

        }
    }
    void DrawNodeWindow(int id)
    {
        GUI.DragWindow();
    }
    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 3; i++) // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }
}

