using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
[CreateAssetMenu(menuName = "创建ControlBlock", fileName = "new_ControlBlock")]
public class ControlBlock : SerializedScriptableObject, IPlayableAsset
{
    #region  IPlayableAsset
    public double duration { get; }
    public IEnumerable<PlayableBinding> outputs { get; }
    #endregion

    [EnumToggleButtons]
    public OrderType orderType;
    [BoxGroup("Behavior Property")]
    public float speed = 1;
    [BoxGroup("Behavior Property")]
    [MinMaxSlider(0, "ObjMaxIndex", true)]
    public Vector2 workRange;
    [BoxGroup("Behavior Property")]
    public bool isflip;
    [BoxGroup("Behavior Property")]
    public bool forceMode;
    [BoxGroup("Behavior Property")]
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
    [ShowIf("useOrderFile")]
    [PropertyOrder(2)]
    [InlineEditor]
    public OrderData orderData;
    [HideIf("useOrderFile")]
    [PropertyOrder(2)]
    public List<ColorOrderBase> colorOrders = new List<ColorOrderBase>();

    bool useOrderFile { get { return orderType == OrderType.OrderFile; } }
    int ObjMaxIndex { get { if (data.ObjNames != null) return data.ObjNames.Count - 1; else return 0; } }
    ScriptPlayable<ControlBehavior> scriptPlayable;
    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // if (!Application.isPlaying)
        //     return ScriptPlayable<ControlBehavior>.Create(graph);


        //recordGroup = ProjectManager.Instance.RecordProject.RecordDic[owner.name];
        ControlBehavior behavior = new ControlBehavior();
        if (orderType == OrderType.OrderFile)
        {
            behavior.orders = orderData.colorOrders;
        }
        else
        {
            behavior.orders = colorOrders;
        }
        behavior.record = this;
        behavior.scriptPlayable = scriptPlayable;
        behavior.GraphParent = owner;
        scriptPlayable = ScriptPlayable<ControlBehavior>.Create(graph, behavior);
        if (workRange == Vector2.zero)
            workRange = new Vector2(0, ObjMaxIndex);
        return scriptPlayable;

    }
    public double GetDuring()
    {
        double temp;
        if (useOrderFile)
        {
            temp = MyTools.GetTotalTime(orderData.colorOrders);
        }
        else
        {
            temp = MyTools.GetTotalTime(colorOrders);
        }
        return temp + data.animTime;
    }
    // [Button(ButtonSizes.Large)]
    // void ReadRecordData()
    // {
    //     for(int i=0;i<recordGroup.Count;i++)
    //     {
    //         if (recordGroup[i].isSelect)
    //         {
    //             this.data.CopyFrom(recordGroup[i]);
    //             animTime=recordGroup[i].animTime;
    //             return;
    //         }
    //         Debug.LogError("没有选中要读取的数据");
    //     }
    // }



    // [Button(ButtonSizes.Large)]
    // void ReadOrderFile()
    // {
    //     colorOrders.Clear();
    //     foreach (var order in orderData.colorOrders)
    //     {
    //         colorOrders.Add(order);
    //     }
    //     Debug.Log("!");
    // }

    [BoxGroup("数据处理模块")]
    [Button(ButtonSizes.Large), GUIColor(1, 0.2f, 0)]
    [ShowIf("needProcess")]
    public void ProcessData()
    {
        if (processer.Process(ref data, data.animTime))
        {
            needProcess = false;
            Debug.Log("数据处理完成");
        }
    }

    [Button(ButtonSizes.Large)]
    public void FindData(string dataName)
    {
        var result = ProjectManager.Instance.RecordProject.RecordDic[ProjectManager.GetCurrentMR().name].Find((a) => a.dataName == dataName);
        if (result != null)
        {
            data.CopyFrom(result);
            if (processer != null)
                ProcessData();
            Register();
        }
        else
        {
            Debug.LogError("没有找到该数据");
        }
    }
    public void SetWorkRangeMax()
    {
        workRange.y=ObjMaxIndex;
    }
    [Button]
    void Register()
    {
        if (data != null)
            data.AddListener(BtnSwitch);
        if (processer != null)
            processer.AddListener(BtnSwitch);
        Debug.Log("注册完成");
    }
    void BtnSwitch()
    {
        needProcess = true;
    }
}


