using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using System;
using UnityEditor;
public class TxtForAnimation : MonoBehaviour
{
    #region {Public field}
    public DanceDB danceDB;
    [HideInInspector]
    public bool useColor;
    public bool HasFinish { get { return hasFinish; } }
    [ShowInInspector]
    public int childCount { get { if (childs != null) return childs.Count; else return 0; } }
    [SerializeField]
    [HideInInspector]
    public List<int> indexs;
    #endregion


    #region {Private field}
    bool hasFinish;
    [SerializeField]
    [HideInInspector]
    List<ColorPoint> childs = new List<ColorPoint>();
    #endregion


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
                childs.Add(child.GetComponent<ColorPoint>());
            }
            else
                AddChild(tra.GetChild(i));
        }
    }
    [Button("读取动画模型数据", ButtonSizes.Gigantic)]
    [PropertyOrder(5)]
    public void Init()
    {
        danceDB.cords.Clear();
        danceDB.staticPositions.Clear();
        childs.Clear();
        GetChilds();
        if (Directory.Exists(danceDB.animFolderPath))
        {
            danceDB.ReadAnimTxtFile();
            Debug.Log("读取动画成功");
        }
        else
        {
            danceDB.ReadStaticTxtFile();
            Debug.Log("读取静态模型成功");
        }
        EditorUtility.SetDirty(danceDB);
    }

    // Update is called once per frame
    public void MyUpdatePos(int frame)
    {
        if (danceDB.staticPositions.Count != 0)
        {
            StaticUpdate();
        }
        else
        {
            AnimUpdate(frame);
        }
        //curFrameindex++;
    }
    void StaticUpdate()
    {
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].transform.position = danceDB.staticPositions[indexs[i]];
        }
        hasFinish = true;
    }
    void AnimUpdate(int frame)
    {//frame是索引，范围是0-totalFrameCount-1
        if (frame >= danceDB.totalFrameCount - 1)
        {
            if (hasFinish)
                return;
            ConsoleProDebug.LogToFilter(danceDB.animName + " 播放完成,共" + (frame + 1) + "帧", "Result");
            //Debug.Log("播放完成,共" + frame + "帧");
            hasFinish = true;
            return;
        }
        SetChildPos(frame);
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
    public void CorrectPointIndex(List<Vector3> pos)
    {
        indexs = new List<int>();
        bool failed = false;
        if (danceDB.totalFrameCount != 0)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                int index = danceDB.cords.FindIndex((a) => a.GetPos(0) == pos[i]);
                if (index == -1)
                {
                    failed = true;
                    Debug.LogError((i + 1).ToString() + "接不上上个动画最后一帧数");
                }
                indexs.Add(index);
            }
        }
        else
        {
            for (int i = 0; i < childs.Count; i++)
            {
                int index = danceDB.staticPositions.FindIndex((a) => a == pos[i]);
                indexs.Add(index);
            }
        }
        if (failed)
            Debug.LogError(danceDB.animName + "指派失败");
    }
    void SetChildPos(int frame)
    {
        for (int i = 0; i < childs.Count; i++)
        {
            Vector3 pos = danceDB.cords[indexs[i]].GetPos(frame);
            childs[i].transform.position = pos;
        }
        if (useColor && Application.isPlaying)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                Color color = danceDB.cords[indexs[i]].GetColor(frame);
                childs[i].mat.color = color;
            }
        }
    }
    public Vector3 GetPointPosByFrame(string pointName, int frame)
    {
        int index = int.Parse(pointName);
        return danceDB.cords[index - 1].GetPos(frame);
    }
    //播放最后一帧
    public void SetAnimEnd()
    {
        if (danceDB.totalFrameCount == 0)
        {
            StaticUpdate();
        }
        else
            SetChildPos(danceDB.totalFrameCount - 1);
    }
    //播放第一帧
    public void SetAnimBegin()
    {
        if (danceDB.totalFrameCount == 0)
        {
            StaticUpdate();
        }
        else
            SetChildPos(0);
    }
    string FindPointName(Vector3 pos, int frame, bool similar = false)
    {
        if (danceDB.totalFrameCount == 0)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                if (danceDB.staticPositions[indexs[i]] == pos)
                    return (i + 1).ToString();
                if (similar)
                {
                    if (MyTools.VectorSimilar(danceDB.staticPositions[indexs[i]], pos))
                        return (i + 1).ToString();
                }
            }
        }
        else
        {
            for (int i = 0; i < childs.Count; i++)
            {
                if (danceDB.cords[indexs[i]].GetPos(frame) == pos)
                    return (i + 1).ToString();
                if (similar)
                {
                    if (MyTools.VectorSimilar(danceDB.cords[indexs[i]].GetPos(frame), pos))
                        return (i + 1).ToString();
                }
            }
        }
        return null;
    }
    public List<string> FindPointNamesByPos(List<Vector3> posList)
    {
        //Debug.Log(posList.Count);
        List<string> temp = new List<string>();
        //先试一下第一帧能不能找到
        // foreach (var pos in posList)
        // {
        //     temp.Add(FindPointName(pos, 0));
        // }
        // if (!temp.Contains(null))
        // {
        //     Debug.Log("校正成功!");
        //     return temp;
        // }
        //temp.Clear();

        // //全动画帧遍历寻找精确对应位置
        // for (int i = 0; i < totalFrameCount; i++)
        // {
        //     foreach (var pos in posList)
        //     {
        //         var result = FindPointName(pos, i);
        //         if (result != null)
        //             temp.Add(result);
        //         else
        //             break;
        //     }
        //     //temp.Add(FindPointName(posList[0], i));
        //     if (temp.Count != posList.Count)
        //     {
        //         temp.Clear();
        //     }
        //     else
        //     {
        //         Debug.Log("校正成功");
        //         return temp;
        //     }
        // }

        // //在所有帧错帧寻找对应位置
        // int totalFrame = 0;
        // bool flag = false;
        // int failCount=0;
        // for (int i = 0; i < posList.Count; i++)
        // {
        //     flag=false;
        //     for (int j = 0; j < totalFrameCount; j++)
        //     {
        //         string findResult = FindPointName(posList[i], j);
        //         if (findResult != null&&!temp.Contains(findResult))
        //         {
        //             temp.Add(findResult);
        //             totalFrame += j;
        //             flag = true;
        //             break;
        //         }
        //     }
        //     if (!flag)
        //     {
        //         temp.Add(null);
        //         failCount++;
        //     }
        // }


        //在所有帧错帧寻找对应位置
        int totalFrame = 0;
        bool flag = false;
        for (int i = 0; i < posList.Count; i++)
        {
            if (flag)
            {
                temp.Add(null);
                continue;
            }
            for (int j = 0; j < danceDB.totalFrameCount; j++)
            {
                string findResult = FindPointName(posList[i], j);
                if (findResult != null)
                {
                    temp.Add(findResult);
                    totalFrame = j;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                temp.Add(null);
            }
        }
        Debug.Log("判断在 " + totalFrame + " 附近");
        //int avargeFrame = Mathf.RoundToInt(totalFrame / temp.Count);
        int avargeFrame = totalFrame;
        int beginSearchFrame = avargeFrame - 1000 < 0 ? 0 : avargeFrame - 1000;
        int endSearchFrame = avargeFrame + 1000 > danceDB.totalFrameCount - 1 ? danceDB.totalFrameCount - 1 : avargeFrame + 1000;
        List<string> resultList = new List<string>();
        totalFrame = 0;
        //没有完全匹配的在可能帧数附近模糊查找
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i] != null)
            {
                resultList.Add(temp[i]);
                continue;
            }
            for (int j = beginSearchFrame; j < endSearchFrame; j++)
            {
                //Debug.Log(i+""+(posList.Count-1));
                string findResult = FindPointName(posList[i], j);
                if (findResult != null)
                {
                    if (temp.Contains(findResult))
                        continue;
                    if (resultList.Contains(findResult))
                        continue;
                    totalFrame += j;
                    resultList.Add(findResult);
                    break;
                }
            }
        }
        Debug.Log(danceDB.animName + "校正结果: " + (posList.Count - resultList.Count) + " 个点没有找到位置");
        return resultList;
    }
    public List<Vector3> GetEndPoitions()
    {
        if (danceDB.animType == AnimType.Static)
        {
            List<Vector3> temp = new List<Vector3>();
            for (int i = 0; i < indexs.Count; i++)
            {
                temp.Add(danceDB.staticPositions[indexs[i]]);
            }
            return temp;
        }
        else
        {
            var temp = new List<Vector3>();
            for (int i = 0; i < indexs.Count; i++)
            {
                temp.Add(danceDB.cords[indexs[i]].GetPos(danceDB.totalFrameCount - 1));
            }
            return temp;
        }
    }
}
