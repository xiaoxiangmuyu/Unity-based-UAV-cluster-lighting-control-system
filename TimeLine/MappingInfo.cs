using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(menuName = "创建序号映射表", fileName = "new_DanceDB")]
public class MappingInfo : ScriptableObject
{
    public DanceDB oldData;
    public DanceDB newData;
    [ShowInInspector]
    [SerializeField]
    public Dictionary<string, string> mappingIndex;
    [Button]
    void Cau()
    {
        if(oldData==null||oldData.cords.Count==0||newData==null||newData.cords.Count==0)
        {
            Debug.LogError("检查比对数据");
            return;
        }
        mappingIndex = new Dictionary<string, string>();
        for (int i = 0; i < oldData.cords.Count; i++)
        {
            for (int j = 0; j < newData.cords.Count; j++)
            {
                if (oldData.cords[i].GetPos(0) == newData.cords[j].GetPos(0))
                    mappingIndex.Add((i+1).ToString(), (j+1).ToString());
            }
        }
        Debug.Log("未成功数量: " + (oldData.cords.Count - mappingIndex.Count));
    }
}
