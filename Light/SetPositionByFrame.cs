using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 读取某一帧TXT文本中所有无人机的位置信息，在开始运行时设置到每个代表无人机的gameObject上
/// </summary>
public class SetPositionByFrame : MonoBehaviour
{
    private string path = "E:/ProjectDocs/Workspace";
    private Dictionary<string, Vector3> dic = new Dictionary<string, Vector3>();
    private Dictionary<string, Color> dicColor = new Dictionary<string, Color>();

    private void Awake()
    {
        if (Directory.Exists(path))
        {
            Vector3 axis = Vector3.zero;
            Color color = Color.clear;

            foreach (string file in Directory.GetFiles(path, "*.txt"))
            {
                using (var reader = new StreamReader(file))
                {
                    string line = null;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == string.Empty)
                        {
                            continue;
                        }

                        var fields = line.Split('\t');
                        // 注意y和z颠倒，且z取相反数
                        axis = new Vector3(float.Parse(fields[1]), float.Parse(fields[3]), -float.Parse(fields[2]));
                        dic[fields[0]] = axis;

                        color = new Color(float.Parse(fields[4]), float.Parse(fields[5]), float.Parse(fields[6]));
                        dicColor[fields[0]] = color;
                    }

                    reader.Close();
                }
            }
        }
        else
        {
            Debug.LogErrorFormat("Import path does NOT exists, path: {0}", path);
            return;
        }

        Transform child;

        for (int i = 0; i < transform.childCount; i++)
        {
            child = transform.GetChild(i);

            if (child && dic.ContainsKey(child.name))
            {
                child.position = dic[child.name];
                child.GetComponent<Renderer>().material.color = dicColor[child.name];
            }
        }
    }
}
