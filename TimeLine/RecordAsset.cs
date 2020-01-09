using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[CreateAssetMenu(menuName = "创建记录容器", fileName = "新位置序列")]
public class RecordAsset : SerializedScriptableObject, IPlayableAsset
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
    [MinMaxSlider(0, "ObjCount",true)]
    public Vector2 workRange;
    [BoxGroup("Behavior Property")]
    public bool isflip;
    [BoxGroup("Behavior Property")]
    public bool forceMode;
    [BoxGroup("Behavior Property")]
    public bool timeInit;
    public double myDuration;


    #region Record
    [ReadOnly]
    public string objParent;
    [ReadOnly]
    public List<string> objs;
    [ReadOnly]
    public List<float> times;
    #endregion

    [ShowInInspector]
    [PropertyOrder(1)]
    [LabelText("总用时")]
    public double totalTime { get { return GetTotalTime(); } }
    [ShowIf("useOrderFile")]
    [PropertyOrder(2)]
    [InlineEditor]
    public OrderData orderData;
    [HideIf("useOrderFile")]
    [PropertyOrder(2)]
    public List<ColorOrderBase> colorOrders;


    bool useOrderFile { get { return orderType == OrderType.OrderFile; } }
    int ObjCount { get { if (objs != null) return objs.Count - 1; else return 0; } }
    ScriptPlayable<RecordBehavior> scriptPlayable;
    [Button(ButtonSizes.Large)]
    public void Clear()
    {
        objs = new List<string>();
        times = new List<float>();
        objParent = string.Empty;
    }
    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        RecordBehavior behavior = new RecordBehavior();
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
        scriptPlayable = ScriptPlayable<RecordBehavior>.Create(graph, behavior);
        if(workRange==Vector2.zero)
        workRange=new Vector2(0,ObjCount);
        return scriptPlayable;

    }
    [Button(ButtonSizes.Large)]
    public void RefreshDuring()
    {
        double temp;
        if (useOrderFile)
        {
            temp = Tools.GetTotalTime(orderData.colorOrders);
        }
        else
        {
            temp = Tools.GetTotalTime(colorOrders);
        }
        scriptPlayable.SetDuration(temp);
    }
    [Button(ButtonSizes.Large)]
    void LogDuration()
    {
        Debug.Log(scriptPlayable.GetDuration());
        Debug.Log(scriptPlayable.GetHashCode());

    }
    [Button(ButtonSizes.Large)]
    void ReadOrderFile()
    {
        colorOrders.Clear();
        foreach (var order in orderData.colorOrders)
        {
            colorOrders.Add(order);
        }
        Debug.Log("!");
    }
    double GetTotalTime()
    {
        if (orderType == OrderType.Custom && colorOrders != null)
            return Tools.GetTotalTime(colorOrders);
        else if (orderType == OrderType.OrderFile && orderData != null)
            return Tools.GetTotalTime(orderData.colorOrders);
        else
            return 0;
    }
}


