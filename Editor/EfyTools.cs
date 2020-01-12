using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class EfyTools 
{
    static MeshRenderer _renderer;
    static MovementCheck movementCheck;
    [MenuItem("工具/EfyTools/Init", priority = 0)]
    static void Init()
    {
        SetCamera();
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/bai.mat");
        GameObject[] objs = Selection.gameObjects;
        
        if (objs.Length == 0)
        {
            Debug.LogError("没有选择模型父物体");
            return;
        }

        int childName;
        foreach (var obj in objs)
        {
            int maxChildCount = 0;
            if (!obj)
            {
                return;
            }

            if (obj.GetComponent<MovementManager>() == null)
            {
                Undo.AddComponent<MovementManager>(obj).projectName = SceneManager.GetActiveScene().name;
            }

            Transform[] children = obj.GetComponentsInChildren<Transform>();

            if (children == null || children.Length < 1)
            {
                return;
            }

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] == null || children[i].childCount > 0 || !int.TryParse(children[i].name, out childName))
                {
                    continue;
                }
                maxChildCount += 1;
                HandleMovementCheck(children[i]);
                HandleRenderer(children[i], mat);
                if (children[i].GetComponent<ColorPoint>() == null)
                {
                    Undo.AddComponent<ColorPoint>(children[i].gameObject);
                }

            }

        }

        Debug.Log("Add speed check scripts complete.");
    }

    private static void HandleMovementCheck(Transform obj)
    {
        movementCheck = obj.GetComponent<MovementCheck>();
        if (movementCheck == null)
        {
            movementCheck = Undo.AddComponent<MovementCheck>(obj.gameObject);
        }
    }
    private static void HandleRenderer(Transform obj, Material mat)
    {
        _renderer = obj.GetComponent<MeshRenderer>();
        if (_renderer == null)
        {
            _renderer = Undo.AddComponent<MeshRenderer>(obj.gameObject);
        }
        //renderer.receiveShadows = false;
        _renderer.material=mat;

    }
    //初始化摄像机参数
    static void SetCamera()
    {
        var camera=Camera.main;
        if(!camera.gameObject.GetComponent<FrameRateManager>())
        camera.gameObject.AddComponent<FrameRateManager>();
        camera.clearFlags=CameraClearFlags.SolidColor;
        camera.backgroundColor=Color.black;
        camera.orthographic=true;
        if(camera.orthographicSize==5)
        camera.orthographicSize=100;    
    }
}
