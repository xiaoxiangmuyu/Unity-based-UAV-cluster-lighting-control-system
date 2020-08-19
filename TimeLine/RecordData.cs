using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[System.Serializable]
public class RecordData
{
    [SerializeField]
    [OnValueChanged("EventDispatch")]
    [GUIColor("GetGroupColor")]
    public string dataName;
    [SerializeField]
    [OnValueChanged("EventDispatch")]
    [GUIColor("GetGroupColor")]

    public float animTime;
    [ValueDropdown("availableIndex")]
    [GUIColor("GetGroupColor")]

    public int groupIndex;

    [SerializeField]
    [HideInInspector]
    public List<string> ObjNames;
    [SerializeField]
    [HideInInspector]
    public List<float> times;
    IEnumerable availableIndex
    {
        get
        {
            int count = ProjectManager.Instance.RecordProject.globalPosDic.Count;
            var temp = new List<int>();
            for (int i = 0; i < count; i++)
            {
                temp.Add(i + 1);
            }
            return temp;
        }
    }

    List<System.Action> Actions;

    public RecordData(string name = "")
    {
        dataName = name;
        animTime = 0;
        ObjNames = new List<string>();
        times = new List<float>();
        //posDic=new StringVector3Dictionary();
    }
    public void Clear()
    {
        ObjNames.Clear();
        times.Clear();
        dataName = string.Empty;
        //posDic.Clear();
    }
    public void Init()
    {
        ObjNames = new List<string>();
        times = new List<float>();
        //posDic=new StringVector3Dictionary();
    }
    public bool IsEmpty()
    {
        if (ObjNames.Count != 0 && times.Count != 0)
            return false;
        else
            return true;
    }
    public void CopyFrom(RecordData data)
    {
        Init();
        dataName = data.dataName;
        //parentName=data.parentName;
        ObjNames = new List<string>(data.ObjNames.ToArray());
        times = new List<float>(data.times.ToArray());
        if (data.animTime != 0)
            animTime = data.animTime;
        groupIndex = data.groupIndex;
        //posDic=data.posDic;
    }
    public void AddListener(System.Action action)
    {
        if (Actions == null)
            Actions = new List<System.Action>();

        if (Actions.Contains(action))
            return;

        Actions.Add(action);
    }
    void EventDispatch()
    {
        if (Actions == null)
        {
            //Debug.Log("Action为空");
            return;
        }
        foreach (var action in Actions)
        {
            action();
        }
    }
    [Button("Add", ButtonSizes.Medium)]
    [HorizontalGroup("Buttons")]
    void AddMappingData()
    {
        MappingData data = new MappingData();
        data.names = new List<string>(ObjNames);
        //data.Objects=MyTools.FindObjs(ObjNames).ToArray();
        data.dataName = dataName;
        ProjectManager.Instance.RecordProject.AddMappingData(data);
    }
    [Button("Show", ButtonSizes.Medium)]
    [HorizontalGroup("Buttons")]
    [GUIColor(0.7f, 1, 1)]
    public void ShowObjects()
    {
        UnityEditor.Selection.objects = MyTools.FindObjs(ObjNames).ToArray();
    }
    Color GetGroupColor()
    {
        var temp = ProjectManager.Instance.RecordProject.globalPosDic.Count;
        float c = (float)1f / temp * groupIndex;
        return Color.HSVToRGB(c, 0.4f, 1f);
    }

}
