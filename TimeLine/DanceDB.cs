using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Sirenix.OdinInspector;
public enum AnimType
{
    Animation,
    Static
}
[Serializable]
public class PointInfo
{
    [SerializeField]
    List<Vector3> posList;
    [SerializeField]
    List<Color> colorList;
    public PointInfo()
    {
        posList = new List<Vector3>();
        colorList = new List<Color>();
    }
    public void AddPos(Vector3 pos)
    {
        posList.Add(pos);
    }
    public void AddColor(Color color)
    {
        colorList.Add(color);
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
    public Color GetColor(int frameIndex)
    {
        if (frameIndex < 0 || frameIndex > colorList.Count - 1)
        {
            Debug.LogError("frameIndex error!" + frameIndex.ToString());
            return Color.white;
        }
        return colorList[frameIndex];
    }
}
[CreateAssetMenu(menuName = "创建舞步存储", fileName = "new_DanceDB")]
public class DanceDB : ScriptableObject
{
    public string animName;
    [ShowInInspector]
    public float time { get { return (float)totalFrameCount / 25; } }
    [ReadOnly]
    public int totalFrameCount;
    public AnimType animType
    {
        get
        {
            if (totalFrameCount == 0)
                return AnimType.Static;
            else
                return AnimType.Animation;
        }
    }
    [FolderPath(AbsolutePath = false)]
    public string animFolderPath;
    [FilePath(AbsolutePath = false)]
    public string staticFilePath;
    [SerializeField]
    [HideInInspector]
    public List<PointInfo> cords = new List<PointInfo>();
    [SerializeField]
    [HideInInspector]
    public List<Vector3> staticPositions = new List<Vector3>();
    bool hasCount;
    public void ReadAnimTxtFile()
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
                        Color tempColor = new Color(float.Parse(Pos[4]) / 255, float.Parse(Pos[5]) / 255, float.Parse(Pos[6]) / 255);
                        tempList.AddColor(tempColor);
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
    public void ReadStaticTxtFile()
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

}
