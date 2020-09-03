using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using System;
public class TxtForAnimation : MonoBehaviour
{
    [Serializable]
    class PointInfo
    {
        [SerializeField]
        List<Vector3> posList;
        public PointInfo()
        {
            posList = new List<Vector3>();
        }
        public void AddPos(Vector3 pos)
        {
            posList.Add(pos);
        }
        public Vector3 GetPos(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex > posList.Count - 1)
            {
                Debug.LogError("frameIndex error!" + frameIndex.ToString());
                return Vector3.zero;
            }
            return posList[frameIndex];
        }
    }


    #region {Public field}
    public string animName;
    [ShowInInspector]
    public float time { get { return (float)totalFrameCount / 25; } }
    [FolderPath(AbsolutePath = false)]
    public string animFolderPath;
    [FilePath(AbsolutePath = false)]
    public string staticFilePath;
    [ReadOnly]
    public int totalFrameCount;
    public bool HasFinish { get { return hasFinish; } }
    [ShowInInspector]
    public int childCount { get { if (childs != null) return childs.Count; else return 0; } }
    #endregion


    #region {Private field}
    private int curFrameindex;
    private bool hasCount;
    private bool hasFinish;
    [SerializeField]
    [HideInInspector]
    private List<PointInfo> cords = new List<PointInfo>();
    [SerializeField]
    [HideInInspector]
    private List<Vector3> staticPositions = new List<Vector3>();
    [SerializeField]
    [HideInInspector]
    private List<Transform> childs = new List<Transform>();
    [SerializeField]
    List<int> indexs;
    [ShowInInspector]
    public bool isMappingFinish
    {
        get
        {
            return (!indexs.Exists((a) => a == -1)) && indexs.Count != 0;
        }
    }
    float timer;
    bool hasBegin;
    #endregion
    private void Awake()
    {
        AnimatorCheck();
    }
    void Start()
    {

    }
    void AnimatorCheck()
    {
        if (GetComponent<Animator>())
        {
            if (GetComponent<Animator>().enabled)
                Debug.LogError(gameObject.name + "动画组件没关");
        }
    }
    void ReadAnimTxtFile()
    {
        if (cords != null && cords.Count != 0)
            cords.Clear();
        hasCount = false;
        totalFrameCount = 0;
        animName = Path.GetFileNameWithoutExtension(animFolderPath);
        if (Directory.Exists(animFolderPath))
        {
            int fileIndex = 0;
            cords = new List<PointInfo>();
            String[] fileArray = Directory.GetFiles(animFolderPath, "*.txt");
            List<String> files = new List<string>(fileArray);
            files.Sort((a, b) => int.Parse(Path.GetFileNameWithoutExtension(a)) - int.Parse(Path.GetFileNameWithoutExtension(b)));
            foreach (string file in files)
            {
                PointInfo tempList = new PointInfo();
                using (var reader = new StreamReader(file))
                {
                    string line = null;
                    int lineIndex = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == string.Empty)
                        {
                            Debug.Log("line is null");
                            continue;
                        }
                        var Pos = line.Split('\t');
                        Vector3 tempPos = new Vector3(float.Parse(Pos[1]), float.Parse(Pos[3]), -float.Parse(Pos[2]));
                        tempList.AddPos(tempPos);
                        lineIndex++;
                        if (!hasCount)
                            totalFrameCount++;
                    }
                    if (!hasCount)
                        hasCount = true;

                    reader.Close();
                }
                cords.Add(tempList);
                fileIndex++;
            }
        }
        else
        {
            Debug.LogError("Path not exist");
        }

    }
    void ReadStaticTxtFile()
    {
        animName = Path.GetFileNameWithoutExtension(staticFilePath);
        using (var reader = new StreamReader(staticFilePath))
        {
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                var Pos = line.Split('\t');
                Vector3 tempPos = new Vector3(float.Parse(Pos[1]), float.Parse(Pos[3]), -float.Parse(Pos[2]));
                staticPositions.Add(tempPos);
            }

            reader.Close();
        }
    }
    void GetChilds()
    {
        if (childs != null && childs.Count != 0)
            childs.Clear();
        AddChild(transform);
        childs.Sort((a, b) => int.Parse(a.name) - int.Parse(b.name));
    }
    void AddChild(Transform tra)
    {
        for (int i = 0; i < tra.childCount; i++)
        {
            if (tra.GetChild(i).childCount == 0)
            {
                int temp;
                if (!int.TryParse(tra.GetChild(i).name, out temp))
                    continue;
                Transform child = tra.GetChild(i);
                childs.Add(child);
            }
            else
                AddChild(tra.GetChild(i));
        }
    }
    [Button("读取动画", ButtonSizes.Gigantic)]
    public void InitForAnim()
    {
        cords.Clear();
        staticPositions.Clear();
        childs.Clear();
        ReadAnimTxtFile();
        GetChilds();
        Debug.Log("读取动画成功");
    }
    [Button("读取静态模型", ButtonSizes.Gigantic)]
    public void InitForStatic()
    {
        cords.Clear();
        staticPositions.Clear();
        childs.Clear();
        ReadStaticTxtFile();
        GetChilds();
        Debug.Log("读取静态模型成功");
    }

    // Update is called once per frame
    public void MyUpdate(int frame, bool mappingIndex)
    {
        if (staticPositions.Count != 0)
        {
            StaticUpdate(mappingIndex);
        }
        else
        {
            AnimUpdate(frame, mappingIndex);
        }
        //curFrameindex++;
    }
    void StaticUpdate(bool mappingIndex)
    {
        if (mappingIndex)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].transform.position = staticPositions[indexs[i]];
            }
        }
        else
        {
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].transform.position = staticPositions[i];
            }
        }
        hasFinish = true;
    }
    void AnimUpdate(int frame, bool mappingIndex)
    {
        if (frame >= totalFrameCount)
        {
            if (hasFinish)
                return;
            ConsoleProDebug.LogToFilter("播放完成,共" + frame + "帧", "Result");
            //Debug.Log("播放完成,共" + frame + "帧");
            hasFinish = true;
            return;
        }
        SetChildPos(frame, mappingIndex);
    }
    // void Update()
    // {
    //     if(!hasBegin)
    //     {
    //         timer+=Time.deltaTime;
    //         if(timer>=animBeginTime)
    //         {
    //             hasBegin=true;
    //         }
    //         else
    //         return;
    //     }
    //     if (curFrameindex >= totalFrameCount)
    //     {
    //         if(hasFinish)
    //         return;
    //         Debug.Log("播放完成,共" + curFrameindex + "帧");
    //         hasFinish = true;
    //         return;
    //     }
    //     SetChildPos(curFrameindex);
    //     curFrameindex++;
    // }
    public void CorrectPointIndex()
    {
        if (isMappingFinish)
            return;
        indexs = new List<int>();
        if (totalFrameCount != 0)
        {
            foreach (var point in childs)
            {
                int index = cords.FindIndex((a) => a.GetPos(0) == MyTools.TruncVector3(point.transform.position));
                indexs.Add(index);
            }
        }
        else
        {
            foreach (var point in childs)
            {
                int index = staticPositions.FindIndex((a) => a == MyTools.TruncVector3(point.transform.position));
                indexs.Add(index);
            }
        }
    }
    void SetChildPos(int frame, bool mappingIndex)
    {
        if (mappingIndex)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                Vector3 pos = cords[indexs[i]].GetPos(frame);
                childs[i].transform.position = pos;
            }
        }
        else
        {
            for (int i = 0; i < childs.Count; i++)
            {
                Vector3 pos = cords[i].GetPos(frame);
                childs[i].transform.position = pos;
            }
        }
    }
    public Vector3 GetPointPosByFrame(string pointName, int frame)
    {
        int index = int.Parse(pointName);
        return cords[index - 1].GetPos(frame);
    }
    public void SetAnimEnd(bool mappingIndex)
    {
        if (totalFrameCount == 0)
        {
            StaticUpdate(mappingIndex);
        }
        else
            SetChildPos(totalFrameCount - 1, mappingIndex);
    }
    public void SetAnimBegin(bool mappingIndex)
    {
        if (totalFrameCount == 0)
        {
            StaticUpdate(mappingIndex);
        }
        else
        SetChildPos(0, mappingIndex);

    }
}
