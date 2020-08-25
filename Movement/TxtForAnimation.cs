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
    [FolderPath(AbsolutePath = true)]
    public string path;
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
    private List<Transform> childs = new List<Transform>();
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
    void ReadTxtFile()
    {
        if (cords != null && cords.Count != 0)
            cords.Clear();
        hasCount = false;
        totalFrameCount = 0;
        animName = Path.GetFileNameWithoutExtension(path);
        if (Directory.Exists(path))
        {
            int fileIndex = 0;
            cords = new List<PointInfo>();
            String[] fileArray = Directory.GetFiles(path, "*.txt");
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
    [Button(ButtonSizes.Gigantic)]
    public void Init()
    {

        cords.Clear();
        childs.Clear();
        ReadTxtFile();
        GetChilds();
        Debug.Log("Init Success");
    }
    // Update is called once per frame
    public void MyUpdate(int frame)
    {
        // if (hasFinish&&isExportMode)
        //     return;
        if (frame >= totalFrameCount)
        {
            if (hasFinish)
                return;
            ConsoleProDebug.LogToFilter("播放完成,共" + frame + "帧", "Result");
            //Debug.Log("播放完成,共" + frame + "帧");
            hasFinish = true;
            return;
        }
        SetChildPos(frame);
        //curFrameindex++;
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
    void SetChildPos(int frame)
    {
        for (int i = 0; i < childs.Count; i++)
        {
            // if(i>childs.Count-1)
            // Debug.LogError("i超出范围:"+i.ToString());
            // if(curFrameindex>cords[0].Count-1)
            // Debug.LogError("curFrameindex超出范围:"+curFrameindex.ToString());
            Vector3 pos = cords[i].GetPos(frame);
            //Debug.Log(pos);
            childs[i].transform.position = pos;
            //Debug.Log(childs[i].transform.position);
        }
    }
}
