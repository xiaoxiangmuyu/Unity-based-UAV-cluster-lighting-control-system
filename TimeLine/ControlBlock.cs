using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "创建ControlBlock", fileName = "new_ControlBlock")]
public class ControlBlock : SerializedScriptableObject, IPlayableAsset
{
    #region  IPlayableAsset
    public double duration { get; }
    public IEnumerable<PlayableBinding> outputs { get; }
    #endregion

    [EnumToggleButtons]
    public OrderType orderType;
    [BoxGroup("控制块属性")]
    public float speed = 1;
    [BoxGroup("控制块属性")]
    [MinMaxSlider(0, "ObjMaxIndex", true)]
    public Vector2 workRange;
    [BoxGroup("控制块属性")]
    public bool isflip;
    [BoxGroup("控制块属性")]
    public bool forceMode;
    [BoxGroup("控制块属性")]
    public bool timeInit;



    #region Record
    bool needProcess;
    [BoxGroup("数据处理模块")]
    [OnValueChanged("Register")]
    public RecordData data;
    [BoxGroup("数据处理模块")]
    [OnValueChanged("Register")]
    public IDataProcesser processer;
    #endregion
    [ShowInInspector]
    [PropertyOrder(1)]
    [LabelText("总用时")]
    public double totalTime { get { return GetDuring(); } }
    [Range(0,1)][LabelText("执行可能性")][PropertyOrder(2)]
    public float possibility=1;
    [PropertyOrder(3)]
    public List<ColorOrderBase> colorOrders = new List<ColorOrderBase>();
    public List<GameObject> objs;
    [ValueDropdown("availableData")][OnValueChanged("RefreshData")]
    [BoxGroup("数据读取模块")]
    public string targetDataName;

    IEnumerable availableData
    {
        get
        {
            var datalist= ProjectManager.Instance.RecordProject.RecordDic[ProjectManager.GetCurrentMR().name];
            List<string>names=new List<string>();
            foreach(var data in datalist)
            {
                names.Add(data.dataName);
            }
            return names;
        }
    }

    int ObjMaxIndex { get { if (data.ObjNames != null) return data.ObjNames.Count - 1; else return 0; } }
    ControlBehavior behavior;
    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // if (!Application.isPlaying)
        //     return ScriptPlayable<ControlBehavior>.Create(graph);
        //recordGroup = ProjectManager.Instance.RecordProject.RecordDic[owner.name];
        behavior = new ControlBehavior();
        behavior.record = this;
        behavior.GraphParent = owner;
        var scriptPlayable = ScriptPlayable<ControlBehavior>.Create(graph, behavior);
        if (workRange == Vector2.zero)
            workRange = new Vector2(0, ObjMaxIndex);
        return scriptPlayable;

    }
    public double GetDuring()
    {
        return MyTools.GetTotalTime(colorOrders) + data.animTime;
    }
    [Button(ButtonSizes.Large)]
    void ReadOrderFile(OrderData orderData)
    {
        colorOrders.Clear();
        foreach (var order in orderData.colorOrders)
        {
            colorOrders.Add(order);
        }
        Debug.Log("!");
    }

    [BoxGroup("数据处理模块")]
    [Button(ButtonSizes.Large), GUIColor(1, 0.2f, 0)]
    [ShowIf("needProcess")]
    public void ProcessData()
    {
        if (processer.Process(ref data, data.animTime))
        {

            needProcess = false;
        }
    }

    //刷新数据，重新从data建立索引
    public void RefreshData()
    {
        var result = ProjectManager.Instance.RecordProject.RecordDic[ProjectManager.GetCurrentMR().name].Find((a) => a.dataName == targetDataName);
        if (result != null)
        {
            data.CopyFrom(result);
            objs.Clear();
            Register();
            SetWorkRangeMax();
            if (processer != null)
                ProcessData();
        }
        else
        {
            Debug.LogError("没有找到该数据:" + targetDataName);
        }
    }
    public void SetWorkRangeMax()
    {
        workRange.y = ObjMaxIndex;
    }
    [Button]
    public void Register()
    {
        if (data != null)
            data.AddListener(BtnSwitch);
        if (processer != null)
        {
            processer.AddValueChangeListener(BtnSwitch);
            processer.AddProcessCompleteListener(Init);
        }
        if (objs == null || objs.Count == 0 || objs.Exists((a) => a == null))
            Init();
        //Debug.Log("注册完成");
    }
    void BtnSwitch()
    {
        needProcess = true;
    }
    public void Init()
    {
        objs = new List<GameObject>();
        GameObject parent = GameObject.Find(ProjectManager.GetCurrentMR().gameObject.name);
        if (parent == null)
            Debug.LogError("没有找到父物体 " + ProjectManager.GetCurrentMR().gameObject.name);
        foreach (var name in data.ObjNames)
        {
            FindChild(parent.transform, name);
            if (!tempObj)
                Debug.LogError("没有找到" + name);
            objs.Add(tempObj.gameObject);
        }
        //Debug.Log("Find GameObjects");
    }
    Transform tempObj;
    void FindChild(Transform tran, string childName)
    {
        Transform target = tran.Find(childName);
        if (target)
        {
            tempObj = target;
            return;
        }
        else
        {
            for (int i = 0; i < tran.childCount; i++)
            {
                FindChild(tran.GetChild(i), childName);
            }
        }
    }
}


