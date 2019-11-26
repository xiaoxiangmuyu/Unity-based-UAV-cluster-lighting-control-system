using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public float staticTime = 10f; // 静态表演时间
    public bool needExport = false; // 是否导出TXT
    public bool includeStartEndFrame = false; // 是否导出首末帧
    public string projectName = ""; // 导出路径中的项目文件夹的名字
    public bool forceStatic = false; // 是否强制指定表演时间，用于模型带动画但播放完毕后仍有表现内容的情况
    [HideInInspector]
    public float limitedSpeed = 0.12f; // 飞机速度限制为每秒不超过3米，1秒为25帧，所以每帧限速为 3 / 25 = 0.12

    private Animator animator;
    private AnimatorStateInfo info;
    private bool isFinished; // 动画是否播放完毕或静态画面达到持续时间
    private bool exportPathValid; // 导出路径是否有效
    private float maxDistance;
    private string exportPath;
    private List<MovementCheck> movementChecks;
    private float staticTimer = 0f; // 静态表演计时器
    private enum FrameType
    {
        start = 1,
        end = 2,
    }
    private string[] fileNames = { "d-m1-static", "f-m2-static", "h-m3-static", "j-m4-static", "l-m5-static",
        "n-m6-static","p-m7-static","r-m8-static","t-m9-static"};

    void Awake()
    {
        animator = GetComponent<Animator>();
        isFinished = false;
        maxDistance = 0f;
        exportPathValid = CheckExportPath();

        if (!exportPathValid)
        {
            return;
        }

        if (transform && transform.childCount > 0)
        {
            movementChecks = new List<MovementCheck>();
            MovementCheck movementCheck;
            MovementCheck[] movementChecksInChild;
            Transform child;

            for (int i = 0; i < transform.childCount; i++)
            {
                child = transform.GetChild(i);

                if (child)
                {
                    movementCheck = child.GetComponent<MovementCheck>();

                    // 检查每个飞机是否挂载了MovementCheck.cs
                    if (movementCheck)
                    {
                        movementChecks.Add(movementCheck);
                    }
                    else
                    {
                        movementChecksInChild = child.GetComponentsInChildren<MovementCheck>();

                        if (movementChecksInChild != null && movementChecksInChild.Length > 0)
                        {
                            for (int j = 0; j < movementChecksInChild.Length; j++)
                            {
                                movementChecks.Add(movementChecksInChild[j]);
                            }
                        }
                        else
                        {
                            Debug.LogErrorFormat("Child does NOT attach MovementCheck script, name: {0}", child.name);
                        }
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Child is null, index: {0}", i);
                }
            }
        }
        else
        {
            Debug.LogErrorFormat("There is NO child for this gameObject, name: {0}", name);
        }
    }

    void Update()
    {
        if (!exportPathValid || isFinished)
        {
            return;
        }

        if (!forceStatic)
        {
            if (animator) // 带动画的表演图形
            {
                info = animator.GetCurrentAnimatorStateInfo(0);

                if (info.normalizedTime >= 1.0f) // 动画播放完毕
                {
                    Debug.Log("动画播放完成");
                    isFinished = true;
                    MovementCheck();
                }
            }
            else // 静态的表演图形
            {
                staticTimer += Time.deltaTime;

                if (staticTimer >= staticTime)
                {
                    Debug.Log("静态图片");
                    isFinished = true;

                    if (!FrameCheck())
                    {
                        return;
                    }

                    if (needExport)
                    {
                        Export(); // 静态表演无位移，所以无需校验直接导出即可
                    }
                }
            }
        }
        else
        {
            staticTimer += Time.deltaTime;

            if (staticTimer >= staticTime)
            {
                Debug.LogError("Force static fin");
                isFinished = true;
                MovementCheck();
            }
        }
    }

    private bool CheckExportPath()
    {
        bool result = false;

        if (string.IsNullOrEmpty(projectName))
        {
            Debug.LogError("Please input project folder name for export.");
            return result;
        }

        string[] chars = name.Split('_');

        if (chars != null && chars.Length > 1)
        {
            string num = chars[1];
            int index;

            if (int.TryParse(num, out index))
            {
                index = index - 1; // 图案名字中的序号从1开始

                if (index < 0 || index >= fileNames.Length)
                {
                    Debug.LogErrorFormat("图案序号超出fileNames索引范围，请检查! index: {0}", index);
                    return result;
                }

                string folderName = fileNames[index];
                exportPath = string.Format("E:/ProjectDocs/Export/{0}/{1}", projectName, folderName);

                if (!Directory.Exists(exportPath))
                {
                    Directory.CreateDirectory(exportPath);
                }

                result = true;
                return result;
            }
            else
            {
                Debug.LogErrorFormat("图案序号转为int出错，请检查! num: {0}", num);
                return result;
            }
        }
        else
        {
            Debug.LogErrorFormat("图案名称解析失败，请检查！name: {0}", name);
            return result;
        }
    }

    private void MovementCheck()
    {
        maxDistance = GetMaxDistance();
        Debug.Log("maxDistance = " + maxDistance);

        if (maxDistance <= limitedSpeed) // 未超过移动阈值则导出，此时可根据需要继续调整播放速度然后再次导出
        {
            Debug.Log("移动距离ok");

            if (!FrameCheck())
            {
                return;
            }

            if (needExport)
            {
                Export();
            }

            if (includeStartEndFrame)
            {
                FrameExport("/StartFrame.txt", FrameType.start);
                FrameExport("/EndFrame.txt", FrameType.end);
            }
        }
        else
        {
            // 动画速度为 animSpeed，在1帧时间内 maxDistance / animSpeed = limitedSpeed / 建议速度
            float animSpeed = info.speed;
            float speed = limitedSpeed * animSpeed / maxDistance;
            Debug.LogErrorFormat("移动距离超出限制，建议将动画速度调整为 {0}", speed);
        }
    }

    private float GetMaxDistance()
    {
        float maxValue = 0f;
        float childMaxValue = 0f;
        MovementCheck movementCheck;

        if (movementChecks != null && movementChecks.Count > 0)
        {
            for (int i = 0; i < movementChecks.Count; i++)
            {
                movementCheck = movementChecks[i];

                if (movementCheck != null)
                {
                    childMaxValue = movementCheck.GetMaxDistance();
                }

                if (childMaxValue > maxValue)
                {
                    maxValue = childMaxValue;
                }
            }
        }

        return maxValue;
    }

    private void Export()
    {
        MovementCheck movementCheck;
        string path = "";
        List<string> infos = new List<string>();

        if (movementChecks != null && movementChecks.Count > 0)
        {
            for (int i = 0; i < movementChecks.Count; i++)
            {
                movementCheck = movementChecks[i];

                if (movementCheck != null)
                {
                    infos = movementCheck.GetInfos();

                    if (infos == null || infos.Count < 1)
                    {
                        continue;
                    }

                    path = exportPath + "/" + movementCheck.name + ".txt";

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    using (var writer = new StreamWriter(path))
                    {
                        for (int j = 0; j < infos.Count; j++)
                        {
                            writer.WriteLine(infos[j]);
                        }

                        writer.Flush();
                        writer.Close();
                    }
                }
            }

            Debug.Log("Export is finished");
            AssetDatabase.Refresh();
        }
    }

    public bool GetIsFinished()
    {
        return isFinished;
    }

    private void FrameExport(string fileName, FrameType frameType)
    {
        MovementCheck movementCheck;
        List<string> infos = new List<string>();

        if (movementChecks != null && movementChecks.Count > 0)
        {
            string path = exportPath + fileName;

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            List<string> orderInfo = new List<string>();

            for (int i = 0; i < movementChecks.Count; i++)
            {
                movementCheck = movementChecks[i];

                if (movementCheck != null)
                {
                    infos = movementCheck.GetInfos();

                    if (infos == null || infos.Count < 1)
                    {
                        continue;
                    }

                    if (frameType == FrameType.start)
                    {
                        orderInfo.Add(infos[0]);
                    }
                    else
                    {
                        orderInfo.Add(infos[infos.Count - 1]);
                    }
                }
            }

            orderInfo.Sort(SortFunc);

            using (var writer = new StreamWriter(path))
            {
                for (int j = 0; j < orderInfo.Count; j++)
                {
                    writer.WriteLine(orderInfo[j]);
                }

                writer.Flush();
                writer.Close();
                Debug.LogErrorFormat("Export is finished: {0}", path);
                //AssetDatabase.Refresh();
            }
        }
    }

    private int SortFunc(string a, string b)
    {
        var fieldsA = a.Split('\t');
        var fieldsB = b.Split('\t');
        int idA = int.Parse(fieldsA[0]);
        int idB = int.Parse(fieldsB[0]);
        return idA - idB;
    }

    /// <summary>
    /// 检查每个TXT中行数是否一致
    /// </summary>
    /// <returns></returns>
    private bool FrameCheck()
    {
        bool isValid = true;
        MovementCheck movementCheck;
        List<string> infos = new List<string>();
        Dictionary<int, List<string>> frameInfo = new Dictionary<int, List<string>>();
        List<string> names = new List<string>();

        if (movementChecks != null && movementChecks.Count > 0)
        {
            for (int i = 0; i < movementChecks.Count; i++)
            {
                movementCheck = movementChecks[i];

                if (movementCheck != null)
                {
                    infos = movementCheck.GetInfos();
                }

                if (infos == null)
                {
                    continue;
                }

                if (frameInfo.TryGetValue(infos.Count, out names))
                {
                    names.Add(movementCheck.name);
                }
                else
                {
                    frameInfo[infos.Count] = new List<string>();
                    frameInfo[infos.Count].Add(movementCheck.name);
                }
            }

            if (frameInfo.Count > 1)
            {
                Debug.LogError("Frame check failed.");

                foreach (var frameCount in frameInfo.Keys)
                {
                    Debug.LogErrorFormat("frameCount: {0}, fileCount: {1}", frameCount, frameInfo[frameCount].Count);
                }

                isValid = false;
            }
            else if (frameInfo.Count < 1)
            {
                Debug.LogError("frameInfo.Count < 1");
                isValid = false;
            }
        }

        return isValid;
    }
}
