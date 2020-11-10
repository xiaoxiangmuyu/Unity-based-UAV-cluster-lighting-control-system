using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System;
using System.IO;
public class ProjectInitWindow : EditorWindow
{
    int number;
    [ShowInInspector]
    public List<string> paths = new List<string>();
    [MenuItem("Window/项目初始化")]
    static void ShowEditor()
    {
        ProjectInitWindow editor = EditorWindow.GetWindow<ProjectInitWindow>();
        //editor.Init();
    }
    public void Init()
    {
        //window1 = new Rect(10, 10, 100, 100);
        //window2 = new Rect(210, 210, 100, 100);
    }
    void OnGUI()
    {
        GUILayout.Label("路径列表");
        //EditorGUILayout.IntField(number);
        if (GUI.Button(new Rect(10, 200, 300, 100), "排序（点不点都行）"))
        {
            paths.Sort(Sort);
        }
        if (GUI.Button(new Rect(10,300, 300, 100), "初始化项目"))
        {
            paths.Sort(Sort);
            for(int i=0;i<paths.Count;i++)
            {
                if(paths[i].StartsWith("f"))
                continue;
                if(Directory.Exists(paths[i]))
                {
                    number=Directory.GetFiles(paths[i]).Length;
                    break;
                }
            }
            var root = new GameObject("Main");
            root.AddComponent<Helper>().GeneratePoint(number);
            EfyTools.Init(new GameObject[1] { root });
            for (int i = 0; i < paths.Count; i++)
            {
                var anim = root.AddComponent<TxtForAnimation>();
                if (File.Exists(paths[i]))
                    anim.staticFilePath = paths[i];
                else if (Directory.Exists(paths[i]))
                {
                    string dirName=Path.GetFileNameWithoutExtension(paths[i]);
                    string childDir=paths[i]+"/"+dirName+".txt";
                    if(Directory.Exists(childDir))
                    anim.animFolderPath=childDir;
                    else
                    anim.animFolderPath = paths[i];
                }
                else
                    Debug.LogError("路径有问题，不是文件也不是文件夹");
            }
        }
        if (paths != null)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                GUILayout.TextArea(paths[i]);
            }
        }
        if (mouseOverWindow == this)
        {//鼠标位于当前窗口
            if (Event.current.type == EventType.DragUpdated)
            {//拖入窗口未松开鼠标
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;//改变鼠标外观
            }
            else if (Event.current.type == EventType.DragExited)
            {//拖入窗口并松开鼠标
             //Focus();//获取焦点，使unity置顶(在其他窗口的前面)
             //Rect rect=EditorGUILayout.GetControlRect();
             //rect.Contains(Event.current.mousePosition);//可以使用鼠标位置判断进入指定区域
                if (DragAndDrop.paths != null)
                {
                    int len = DragAndDrop.paths.Length;
                    for (int i = 0; i < len; i++)
                    {
                        paths.Add(DragAndDrop.paths[i]);

                    }
                }
            }
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
    int Sort(string a, string b)
    {
        float resA = StringParser(a);
        float resB = StringParser(b);
        return resA > resB ? 1 : -1;
    }
    float StringParser(string a)
    {
        string name = Path.GetFileNameWithoutExtension(a);
        int A;
        if (!name.StartsWith("f"))
        {
            A = int.Parse(name.Split('_')[1]);
            return A;
        }
        else
        {
            string[] chars = name.Split('_');
            return int.Parse(chars[2]) - 0.5f;
        }
    }

}

