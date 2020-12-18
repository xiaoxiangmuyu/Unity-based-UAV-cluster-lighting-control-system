using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class EfyTools
{
    const string projectPath = "Assets/Resources/Projects/";
    static MeshRenderer _renderer;
    static MovementCheck movementCheck;
    [MenuItem("工具/EfyTools/Init", priority = 0)]
    public static void Init(GameObject[] objs)
    {
        bool isCountFinish = false;
        SetCamera();
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/bai.mat");
        //GameObject[] objs = Selection.gameObjects;

        if (objs.Length == 0)
        {
            Debug.LogError("没有选择模型父物体");
            return;
        }

        string projectName = SceneManager.GetActiveScene().name;
        ProjectManager.Instance.projectName = projectName;
        RecordProject recordProject = CreatRecordProject(projectName);
        int childName;
        foreach (var obj in objs)
        {
            int childCount = 0;

            if (obj.GetComponent<MovementManager>() == null)
            {
                obj.AddComponent<MovementManager>().projectName = projectName;
            }
            CreateTimeLine(obj, projectName);

            Transform[] children = obj.GetComponentsInChildren<Transform>();

            if (children == null || children.Length < 1)
            {
                Debug.LogError("没有子物体:" + obj.name);
                return;
            }

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].childCount > 0 || !int.TryParse(children[i].name, out childName))
                {
                    continue;
                }
                childCount += 1;
                HandleMovementCheck(children[i]);
                HandleRenderer(children[i], mat);
                HandleColorPoint(children[i]);
                children[i].gameObject.layer = LayerMask.NameToLayer("Point");
                // if (children[i].gameObject.layer != LayerMask.NameToLayer("TriggerIgnore"))
                // {
                //     recordProject.RecordDic[0].ObjNames.Add(children[i].name);
                //     recordProject.RecordDic[0].times.Add(0);
                // }
            }
            if (!isCountFinish)
            {
                isCountFinish = true;
                Debug.Log("本项目共" + childCount + "架飞机");
            }
        }
        EditorUtility.SetDirty(recordProject);
        AssetDatabase.SaveAssets();
        Debug.Log("初始化完成");
    }
    static void HandleColorPoint(Transform obj)
    {
        if (obj.GetComponent<ColorPoint>() == null)
        {
            Undo.AddComponent<ColorPoint>(obj.gameObject);
        }
        if (obj.GetComponent<Rigidbody2D>() == null)
        {
            Undo.AddComponent<Rigidbody2D>(obj.gameObject);
        }
        var col = obj.GetComponent<Rigidbody2D>();
        col.bodyType = RigidbodyType2D.Kinematic;
    }
    static void HandleMovementCheck(Transform obj)
    {
        movementCheck = obj.GetComponent<MovementCheck>();
        if (movementCheck == null)
        {
            movementCheck = Undo.AddComponent<MovementCheck>(obj.gameObject);
        }
        var col = obj.GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = obj.gameObject.AddComponent<CircleCollider2D>();
        }
    }
    static void HandleRenderer(Transform obj, Material mat)
    {
        _renderer = obj.GetComponent<MeshRenderer>();
        if (_renderer == null)
        {
            _renderer = Undo.AddComponent<MeshRenderer>(obj.gameObject);
        }
        //renderer.receiveShadows = false;
        _renderer.material = mat;

    }

    //初始化摄像机参数
    static void SetCamera()
    {
        var camera = Camera.main;
        if (!camera.gameObject.GetComponent<FrameRateManager>())
            camera.gameObject.AddComponent<FrameRateManager>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        camera.orthographic = true;
        if (camera.orthographicSize == 5)
            camera.orthographicSize = 100;
        if (!camera.GetComponent<ProjectManager>())
        {
            Undo.AddComponent<ProjectManager>(camera.gameObject);
        }
        if (!camera.GetComponent<MyDebugger>())
        {
            Undo.AddComponent<MyDebugger>(camera.gameObject);
        }
    }
    static RecordProject CreatRecordProject(string projectName)
    {
        RecordProject recordProject;
        if (!Directory.Exists(projectPath + projectName))
            Directory.CreateDirectory(projectPath + projectName);
        if (!File.Exists(projectPath + projectName + "/RecordParent.asset"))
        {
            recordProject = ScriptableObject.CreateInstance<RecordProject>();
            AssetDatabase.CreateAsset(recordProject, projectPath + projectName + "/RecordParent.asset");
        }
        else
        {
            recordProject = Resources.Load<RecordProject>("Projects/" + projectName + "/RecordParent");
        }
        return recordProject;

    }
    static void CreateTimeLine(GameObject obj, string projectName)
    {
        TimelineAsset asset;
        if (File.Exists(projectPath + projectName + "/" + obj.name + ".playable"))
        {
            Debug.Log(projectPath + projectName + "/" + obj.name + ".playable" + "已存在");
            asset = Resources.Load<TimelineAsset>("Projects/" + projectName + "/" + obj.name);
            if (!obj.GetComponent<PlayableDirector>())
            {
                obj.AddComponent<PlayableDirector>().playableAsset = asset;
                obj.GetComponent<PlayableDirector>().playOnAwake = false;
                obj.GetComponent<PlayableDirector>().extrapolationMode = DirectorWrapMode.Hold;
            }
            else
            {
                obj.GetComponent<PlayableDirector>().playableAsset = asset;
                obj.GetComponent<PlayableDirector>().playOnAwake = false;
                obj.GetComponent<PlayableDirector>().extrapolationMode = DirectorWrapMode.Hold;

            }

        }
        else
        {
            asset = TimelineAsset.CreateInstance<TimelineAsset>();
            asset.editorSettings.fps = 25;
            AssetDatabase.CreateAsset(asset, projectPath + projectName + "/" + obj.name + ".playable");
            if (!obj.GetComponent<PlayableDirector>())
            {
                obj.AddComponent<PlayableDirector>().playableAsset = asset;
                obj.GetComponent<PlayableDirector>().extrapolationMode = DirectorWrapMode.Hold;

            }
            else
                obj.GetComponent<PlayableDirector>().playableAsset = asset;
            obj.GetComponent<PlayableDirector>().extrapolationMode = DirectorWrapMode.Hold;

        }
    }
}
