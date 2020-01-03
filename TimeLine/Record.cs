using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[CreateAssetMenu(menuName = "创建记录容器", fileName = "新位置序列")]
public class Record : SerializedScriptableObject, IPlayableAsset
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
    [Range(0, 1)]
    public float beginPos = 0;
    [BoxGroup("Behavior Property")]
    public bool isflip;
    [BoxGroup("Behavior Property")]
    public bool forceMode;
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
    ScriptPlayable<OrderBehavior> scriptPlayable;
    [Button(ButtonSizes.Large)]
    public void Clear()
    {
        objs = new List<string>();
        times = new List<float>();
        objParent=string.Empty;
    }
    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        scriptPlayable = ScriptPlayable<OrderBehavior>.Create(graph);
        if (orderType == OrderType.OrderFile)
        {
            scriptPlayable.GetBehaviour().orders = orderData.colorOrders;
        }
        else
        {
            scriptPlayable.GetBehaviour().orders = colorOrders;
        }
        scriptPlayable.GetBehaviour().record = this;
        scriptPlayable.GetBehaviour().scriptPlayable=scriptPlayable;
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


