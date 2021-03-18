using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(menuName = "创建图案数据存储", fileName = "new_DanceDB")]
public class DataGroup : ScriptableObject
{
    public DataGroup(string groupName)
    {
        this.groupName = groupName;
    }
    [ValueDropdown("availableNames")]
    [GUIColor("GetGroupColor")]
    [HorizontalGroup("BaseInfo")]
    //[ReadOnly]
    public string groupName;
    [SerializeField]
    [TableList(DrawScrollView = false)]
    [TabGroup("数据")]
    public List<RecordData> recordDatas = new List<RecordData>();
    [SerializeField]
    [TableList(DrawScrollView = false)]
    [TabGroup("颜色")]
    public List<MappingData> mappingDatas = new List<MappingData>();
    IEnumerable availableNames
    {
        get
        {
            return ProjectManager.availableGroups;
        }
    }
    Color GetGroupColor()
    {
        if (groupName == null || groupName.Equals(""))
            return Color.red;
        int index = 0;
        foreach (var animName in ProjectManager.availableGroups)
        {
            if (animName.Equals(groupName))
                break;
            else
                index++;
        }
        float c = (float)1f / ProjectManager.availableGroups.Count * index;
        return Color.HSVToRGB(c, 0.4f, 1f);
    }
}
