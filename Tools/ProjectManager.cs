using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEditor.Timeline;
using UnityEngine.Rendering;
using UnityEditor;
public class ProjectManager : MonoBehaviour
{
    const string ProjectParentPath = "Projects/";
    static ProjectManager instance;
    public static ProjectManager Instance
    {
        get
        {
            if (instance == null)
                instance = Camera.main.GetComponent<ProjectManager>();
            return instance;
        }
    }
    static Camera mainCamera;
    public static Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
                mainCamera = Instance.GetComponent<Camera>();
            return mainCamera;
        }
    }
    RecordProject recordProject;
    public RecordProject RecordProject
    {
        get
        {
            if (recordProject == null)
                recordProject = Resources.Load<RecordProject>(ProjectParentPath + ProjectManager.instance.projectName + "/RecordParent");
            return recordProject;
        }
    }


    public string projectName;
    public static List<string> availableGroups
    {
        get
        {
            var globalAnimNames = new List<string>();
            var temp = Instance.RecordProject.globalPosDic;
            foreach (var info in temp)
            {
                globalAnimNames.Add(info.groupName);
            }
            return globalAnimNames;
        }
    }


    public static List<string> AllMainAnimNames
    {
        get
        {

            var allAnimNames = new List<string>();
            foreach (var animation in GetPointsRoot().GetComponents<TxtForAnimation>())
            {
                if (animation.danceDB.animName != null && animation.danceDB.animName.StartsWith("G"))
                    allAnimNames.Add(animation.danceDB.animName);
            }
            return allAnimNames;
        }
    }

    void OnEnable()
    {
        RenderPipelineManager.endFrameRendering += RenderPipelineManager_endCameraRendering;
    }
    void OnDisable()
    {
        RenderPipelineManager.endFrameRendering -= RenderPipelineManager_endCameraRendering;
    }
    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera[] camera)
    {
        RecordScreenColor();
    }
    private void Awake()
    {
        instance = this;
    }
    public static GameObject GetPointsRoot()
    {
        return GameObject.FindObjectOfType<MovementManager>().gameObject;
    }


    public static void ResetAllColorAndTween()
    {
        DOTween.CompleteAll();
        DOTween.KillAll();
        //currentTarget.ResetAllColor();
    }


    public static TxtForAnimation FindAnimByName(string name)
    {
        var anims = GetPointsRoot().GetComponents<TxtForAnimation>();
        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i].danceDB.animName == name)
                return anims[i];
        }
        Debug.LogError(name + "   动画没有找到");
        return null;
    }


    // public static List<string> FindPointsByPos(PointIndexInfo info)
    // {
    //     var anims = GetPointsRoot().GetComponents<TxtForAnimation>();
    //     for (int i = 0; i < anims.Length; i++)
    //     {
    //         var tempNames=anims[i].FindPointNames(info.posList);
    //         if(!tempNames.Contains(null)&&tempNames.Count!=0)
    //         return tempNames;
    //     }
    //     Debug.LogError("所有动画都没找到这组坐标对应的点");
    //     return null;
    // }
    public static TxtForAnimation GetAnimationByName(string animName)
    {
        var anims = GetPointsRoot().GetComponents<TxtForAnimation>();
        foreach (var anim in anims)
        {
            if (anim.danceDB.animName.Equals(animName))
                return anim;
        }
        Debug.LogError("没有名为" + animName + "的动画");
        return null;
    }


    public static TxtForAnimation GetAnimationbyGroupIndex(int groupIndex)
    {
        var anims = GetPointsRoot().GetComponents<TxtForAnimation>();
        return anims[groupIndex - 1];
    }


    public static GlobalPosInfo GetGlobalPosInfoByGroup(string groupName)
    {
        var info = instance.RecordProject.globalPosDic.Find((a) => a.groupName.Equals(groupName));
        return info;
    }
    public Texture2D texture;
    int width;
    int height;
    MovementManager movementManager;
    void RecordScreenColor()
    {
        // if (texture == null)
        // {
        //     width = RenderTexture.active.width;
        //     height = RenderTexture.active.height;
        //     texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        // }
        if (RenderTexture.active == null)
            RenderTexture.active = MainCamera.targetTexture;
        width = RenderTexture.active.width;
        height = RenderTexture.active.height;
        if (texture == null)
            texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        // if (movementManager == null)
        //     movementManager = ProjectManager.GetPointsRoot().GetComponent<MovementManager>();
        // movementManager.RecordAllPoint(texture);
    }
    public static Color GetRenderColor(Vector3 worldPos)
    {
        //instance.RecordScreenColor();
        Vector2 screenPos = MainCamera.WorldToScreenPoint(worldPos);
        Color temp = instance.texture.GetPixel((int)screenPos.x, (int)screenPos.y);
        return temp;
    }


    public static DataGroup GetDataGroupByGroupName(string groupName)
    {
        if (groupName == null || groupName.Equals(""))
            return null;
        string path = "Projects/" + Instance.projectName + "/" + groupName;
        DataGroup result = Resources.Load<DataGroup>(path);
        //DataGroup result = instance.RecordProject.datas.Find((a) => a.groupName.Equals(groupName));
        if (result == null)
        {
            Debug.Log("没有找到名为" + groupName + "的数据,返回新数据组");
            result = ScriptableObject.CreateInstance<DataGroup>();
            result.groupName = groupName;
            AssetDatabase.CreateAsset(result, "Assets/Resources/Projects/" + Instance.projectName + "/" + groupName + ".asset");
            //instance.recordProject.datas.Add(result);
            return result;
        }
        return result;
    }


}
