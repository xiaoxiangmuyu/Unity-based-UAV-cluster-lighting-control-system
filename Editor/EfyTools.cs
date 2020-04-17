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
    static int maxChildCount;
    public static Quaternion RotationInfo;
    public static Vector3 PosInfo;

    [MenuItem("工具/EfyTools/Init", priority = 0)]
    static void Init()
    {
        RotationInfo = new Quaternion();
        PosInfo = new Vector3();
        bool isCountFinish = false;
        maxChildCount = 0;
        SetCamera();
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/bai.mat");
        GameObject[] objs = Selection.gameObjects;

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
                Undo.AddComponent<MovementManager>(obj).projectName = projectName;
                if (!recordProject.RecordDic.ContainsKey(obj.name))
                    recordProject.RecordDic.Add(obj.name, new List<RecordData>());
                // =recordProject.ProjectRecords[obj.name];
            }
            AddPlayable(obj,projectName);
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
            }
            if (!isCountFinish)
            {
                isCountFinish = true;
                RotationInfo = obj.transform.rotation;
                PosInfo = obj.transform.position;
                maxChildCount = childCount;
                //ProjectManager.instance.RotationInfo = RotationInfo;
                //ProjectManager.instance.PosInfo = PosInfo;
                ProjectManager.Instance.ChildCount = maxChildCount;
                Debug.Log("本项目共" + maxChildCount + "架飞机");
            }
            else
            {
                if (childCount != maxChildCount)
                    Debug.LogError(obj.name + "图案飞机数量与其他图案不一致" + childCount + "  " + maxChildCount);
                // if (obj.transform.position != PosInfo)
                //     Debug.LogError(obj.name + "图案位置信息与其他图案不一致");
                // if (obj.transform.rotation != RotationInfo)
                //     Debug.LogError(obj.name + "图案旋转信息与其他图案不一致");
            }

        }
        Debug.Log("初始化完成");
    }
    static void HandleColorPoint(Transform obj)
    {
        if (obj.GetComponent<ColorPoint>() == null)
        {
            Undo.AddComponent<ColorPoint>(obj.gameObject);
        }
    }
    static void HandleMovementCheck(Transform obj)
    {
        movementCheck = obj.GetComponent<MovementCheck>();
        if (movementCheck == null)
        {
            movementCheck = Undo.AddComponent<MovementCheck>(obj.gameObject);
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
    }
    static RecordProject CreatRecordProject(string projectName)
    {
        RecordProject recordProject = ScriptableObject.CreateInstance<RecordProject>();
        recordProject.RecordDic = new Dictionary<string, List<RecordData>>();
        if (!Directory.Exists(projectPath + projectName))
        //AssetDatabase.CreateFolder(projectPath, projectName);
        Directory.CreateDirectory(projectPath + projectName);
        AssetDatabase.CreateAsset(recordProject, projectPath + projectName + "/RecordParent.asset");
        return recordProject;

    }
    static void AddPlayable(GameObject obj, string projectName)
    {
        var asset = TimelineAsset.CreateInstance<TimelineAsset>();
        asset.editorSettings.fps=25;
        AssetDatabase.CreateAsset(asset, projectPath + projectName+"/"+obj.name+".playable");
        if (!obj.GetComponent<PlayableDirector>())
        {
            obj.AddComponent<PlayableDirector>().playableAsset=asset;
        }
        AssetDatabase.SaveAssets();
    }
}
