using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
public class MovementManager : MonoBehaviour
{
    public enum ExportType { Time, Frame }
    public bool isWorking;
    [EnumToggleButtons]
    public ExportType exportType = ExportType.Time;
    [LabelText("导出时间")][ShowIf("IsExportByTime")]
    public float ExportTime = 10f; // 静态表演时间
    [LabelText("导出帧数")][HideIf("IsExportByTime")]
    public int ExportFrame = 0;
    public const float LimitedVecticalSpeed = 0.08f;//0.08f
    public const float LimitedSpeed = 0.12f; // 飞机速度限制为每秒不超过3米，1秒为25帧，所以每帧限速为 3 / 25 = 0.12
    public string projectName;

    bool IsExportByTime{get{return exportType==ExportType.Time;}}
    Animator animator;
    AnimatorStateInfo info;
    string exportPath;
    List<MovementCheck> movementChecks;
    float timer = 0f; // 静态表演计时器
    int frame = 0;
    enum FrameType
    {
        start = 1,
        end = 2,
    }
    StringBuilder sb = new StringBuilder(50, 50);
    int r, g, b;
    [Button]
    void SetCurrent()
    {
        ProjectManager.SetOperateTarget(this);
    }
    void OnEnable()
    {
        ProjectManager.SetOperateTarget(this);
    }
    void Start()
    {
        // if (!isWorking)
        //     return;

        animator = GetComponent<Animator>();

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
        PreCheck();
    }
    void Update()
    {
        if (!isWorking)
            return;

        if (animator && animator.enabled) //测速模式
        {
            info = animator.GetCurrentAnimatorStateInfo(0);

            if (info.normalizedTime >= 1.0f) // 动画播放完毕
            {
                Debug.Log("动画播放完成");
                isWorking = false;
                if (ExportCheck())
                    Export();
            }
        }
        else // 输出模式
        {
            if (exportType == ExportType.Time)
            {
                timer += Time.deltaTime;

                if (timer >= ExportTime)
                {
                    isWorking = false;
                    if (ExportCheck())
                        Export();
                }
            }
            else
            {
                frame += 1;
                if (frame == ExportFrame)
                {
                    isWorking = false;
                    if (ExportCheck())
                        Export();
                }
            }
        }
    }
    #region 检查

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
    void PreCheck()
    {
        if (!CheckExportPath())
        {
            Debug.LogError("导出路径不合法");
        }
        // if (transform.position != ProjectManager.Instance.PosInfo)
        // {
        //     Debug.LogError(gameObject.name + "位置与其他图案不一致");
        // }
        // if (transform.rotation != ProjectManager.Instance.RotationInfo)
        // {
        //     Debug.LogError(gameObject.name + "旋转信息与其他图案不一致");
        // }
        if (movementChecks.Count != ProjectManager.Instance.ChildCount)
        {
            Debug.LogError(gameObject.name + "子物体数量与其他图案不一致" + movementChecks.Count);
        }
        if (movementChecks.Count % 10 != 0)
        {
            Debug.LogError("飞机数量有问题：" + movementChecks.Count);
        }

    }
    private bool ExportCheck()
    {
        if (!FrameCheck())
        {
            Debug.LogError("txt行数不一致");
            return false;
        }
        if (!CheckExportPath())
        {
            Debug.LogError("导出路径不合法");
            return false;
        }
        if (GetComponent<TxtForAnimation>())
        {
            if (!GetComponent<TxtForAnimation>().HasFinish)
            {
                Debug.LogError("导出时间过短，TxT没播放完");
                return false;
            }
        }
        // if (transform.position != ProjectManager.Instance.PosInfo)
        // {
        //     Debug.LogError(gameObject.name + "位置与其他图案不一致");
        //     return false;
        // }
        // if (transform.rotation != ProjectManager.Instance.RotationInfo)
        // {
        //     Debug.LogError(gameObject.name + "旋转信息与其他图案不一致");
        //     return false;
        // }
        if (movementChecks.Count != ProjectManager.Instance.ChildCount)
        {
            Debug.LogError(gameObject.name + "子物体数量与其他图案不一致" + movementChecks.Count);
        }

        return true;
    }
    /// <summary>
    /// 检查每个TXT中行数是否一致
    /// </summary>
    private bool FrameCheck()
    {
        if (movementChecks.Exists((a) => a.GetPosInfos() == null))
        {
            string name = movementChecks.Find((a) => a.GetPosInfos() == null).gameObject.name;
            Debug.LogError(name + "没有激活");
            return false;
        }
        int temp = movementChecks[0].GetPosInfos().Count;
        if (movementChecks.Exists((a) => a.GetPosInfos().Count != temp))
        {
            string name = movementChecks.Find((a) => a.GetPosInfos().Count != temp).gameObject.name;
            Debug.LogError(name + "导出的txt行数有问题,是否中途被关闭?");
            return false;
        }
        return true;
    }
    #endregion
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
            // if (GetComponent<TxtForAnimation>())
            // {
            //     FrameExport("/StartFrame.txt", FrameType.start);
            //     FrameExport("/EndFrame.txt", FrameType.end);
            // }
            Debug.Log("<<<<<    导出完成    >>>>>");
            AssetDatabase.Refresh();
        }
    }
    /// <summary>
    /// 导出首末帧
    /// </summary>
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
                //Debug.LogErrorFormat("Export is finished: {0}", path);
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
    public void SetProjectName(string name)
    {
        projectName = name;
    }
    public void ResetAllColor()
    {
        foreach (var point in movementChecks)
        {
            point.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }
}
