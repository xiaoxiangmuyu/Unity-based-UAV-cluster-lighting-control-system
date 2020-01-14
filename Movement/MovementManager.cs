using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
public class MovementManager : MonoBehaviour
{
    public bool isSpeedTest { get { return GetComponent<Animator>().enabled == true; } }
    public bool isWorking;
    public float staticTime = 10f; // 静态表演时间
    public bool needExport = false; // 是否导出TXT
    public bool includeStartEndFrame = false; // 是否导出首末帧
    public string projectName = ""; // 导出路径中的项目文件夹的名字
    public bool forceStatic = false; // 是否强制指定表演时间，用于模型带动画但播放完毕后仍有表现内容的情况
    private float limitedSpeed = 0.12f; // 飞机速度限制为每秒不超过3米，1秒为25帧，所以每帧限速为 3 / 25 = 0.12

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

    private StringBuilder sb = new StringBuilder(50, 50);
    private int r, g, b;

    void Awake()
    {
        if (!isWorking)
            return;

        animator = GetComponent<Animator>();
        isFinished = false;
        maxDistance = 0f;
        exportPathValid = CheckExportPath();

        if (!exportPathValid)
        {
            return;
        }

        if (transform.childCount > 0)
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
        PointCountCheck();
    }
    void PointCountCheck()
    {
        if (movementChecks.Count % 50 != 0)
        {
            Debug.LogError("飞机数量有问题：" + movementChecks.Count);
            movementChecks.Sort((a,b)=>int.Parse(a.name)-int.Parse(b.name));
            foreach(var a in movementChecks)
            {
                Debug.Log(a.name);
            }
        }
    }
    void Update()
    {
        if (!isWorking)
            return;
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
                Debug.Log("Force static fin");
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

        exportPath = string.Format("E:/ProjectDocs/Export/{0}/{1}", projectName, name);

        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }

        result = true;
        return result;
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
            if (GetComponent<TxtForAnimation>())
            {
                if (!GetComponent<TxtForAnimation>().HasFinish)
                {
                    Debug.LogError("导出时间过短，动画没播放完");
                    return;
                }
            }
            if (needExport)
            {
                Export();
            }

            if (includeStartEndFrame && needExport)
            {
                FrameExport("/StartFrame.txt", FrameType.start);
                FrameExport("/EndFrame.txt", FrameType.end);
            }
        }
        else
        {
            // 动画速度为 animSpeed，在1帧时间内 maxDistance / animSpeed = limitedSpeed / 建议速度
            // float animSpeed = info.speed;
            // float speed = limitedSpeed * animSpeed / maxDistance;
            // Debug.LogErrorFormat("移动距离超出限制，建议将动画速度调整为 {0}", speed);
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
        string droneName = "";
        string exportInfo = "";
        List<Vector3> posInfos = new List<Vector3>();
        List<Color> colorInfos = new List<Color>();

        if (movementChecks != null && movementChecks.Count > 0)
        {
            for (int i = 0; i < movementChecks.Count; i++)
            {
                movementCheck = movementChecks[i];

                if (movementCheck != null)
                {
                    posInfos = movementCheck.GetPosInfos();
                    colorInfos = movementCheck.GetColorInfos();

                    if (posInfos == null || posInfos.Count < 1 || colorInfos == null || colorInfos.Count < 1)
                    {
                        continue;
                    }

                    droneName = movementCheck.GetDroneName();
                    path = exportPath + "/" + droneName + ".txt";

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    using (var writer = new StreamWriter(path))
                    {
                        for (int j = 0; j < posInfos.Count; j++)
                        {
                            exportInfo = GetExportInfo(posInfos[j], colorInfos[j], droneName);
                            writer.WriteLine(exportInfo);
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
        string droneName = "";
        string exportInfo = "";
        List<Vector3> posInfos = new List<Vector3>();
        List<Color> colorInfos = new List<Color>();

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
                    droneName = movementCheck.GetDroneName();
                    posInfos = movementCheck.GetPosInfos();
                    colorInfos = movementCheck.GetColorInfos();

                    if (posInfos == null || posInfos.Count < 1 || colorInfos == null || colorInfos.Count < 1)
                    {
                        continue;
                    }

                    if (frameType == FrameType.start)
                    {
                        exportInfo = GetExportInfo(posInfos[0], colorInfos[0], droneName);
                    }
                    else
                    {
                        exportInfo = GetExportInfo(posInfos[posInfos.Count - 1], colorInfos[colorInfos.Count - 1], droneName);
                    }

                    orderInfo.Add(exportInfo);
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
        List<Vector3> posInfos = new List<Vector3>();
        Dictionary<int, List<string>> frameInfo = new Dictionary<int, List<string>>();
        List<string> names = new List<string>();
        string droneName = "";

        if (movementChecks != null && movementChecks.Count > 0)
        {
            for (int i = 0; i < movementChecks.Count; i++)
            {
                movementCheck = movementChecks[i];

                if (movementCheck != null)
                {
                    posInfos = movementCheck.GetPosInfos();
                    droneName = movementCheck.GetDroneName();
                }

                if (posInfos == null)
                {
                    continue;
                }

                if (frameInfo.TryGetValue(posInfos.Count, out names))
                {
                    names.Add(droneName);
                }
                else
                {
                    frameInfo[posInfos.Count] = new List<string>();
                    frameInfo[posInfos.Count].Add(droneName);
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

    public float GetLimitedSpeed()
    {
        return limitedSpeed;
    }
    public float GetLimitedVecticalSpeed()
    {
        return 0.08f;
    }

    private string GetExportInfo(Vector3 curPos, Color color, string droneName)
    {
        r = Mathf.FloorToInt(color.r * 255);
        g = Mathf.FloorToInt(color.g * 255);
        b = Mathf.FloorToInt(color.b * 255);

        sb.Clear();
        sb.Append(droneName);
        sb.Append("\t");
        sb.Append(curPos.x);
        sb.Append("\t");
        sb.Append(-curPos.z); // 统一取相反数
        sb.Append("\t");
        sb.Append(curPos.y);
        sb.Append("\t");
        sb.Append(r);
        sb.Append("\t");
        sb.Append(g);
        sb.Append("\t");
        sb.Append(b);
        return sb.ToString();
    }
}
