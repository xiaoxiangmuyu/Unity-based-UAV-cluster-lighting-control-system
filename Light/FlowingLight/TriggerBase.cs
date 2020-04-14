using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class TriggerBase : SerializedMonoBehaviour
{
    // [ShowInInspector][BoxGroup("Time")][PropertyOrder(-1)]
    // public float TotalTime { get { return animTime + OrderTime; } }
    // [ShowInInspector][BoxGroup("Time")][PropertyOrder(-1)]
    // public float animTime { get {if(GetComponent<DOTweenAnimation>()) return GetComponent<DOTweenAnimation>().duration;else return GetComponent<DOTweenPath>().duration;} }
    // [ShowInInspector][BoxGroup("Time")][PropertyOrder(-1)]
    // public float OrderTime { get { if (orderFile != null) return orderFile.totalTime; else return MyTools.GetTotalTime(colorOrders); } }
    [Header("Property")]
    public bool forceMode;
    [ShowIf("useExitOrder")]
    public bool exitForceMode;
    public bool useExitOrder;
    public RecordAsset record;
    [HideInInspector]
    public float recordTimer;


    [InlineEditor]
    public OrderData orderFile;
    [HorizontalGroup("Tags")]
    public List<string> targetTags = new List<string>(); // 影响的飞机的标签
    [HorizontalGroup("Tags")]
    public List<string> ignoreTags = new List<string>();
    [LabelText("命令序列")]
    public List<ColorOrderBase> colorOrders;
    [ShowIf("useExitOrder")]
    [LabelText("退出命令序列")]
    public List<ColorOrderBase> exitOrders;

    protected virtual void Awake()
    {
        if(record!=null)
        record.animTime=GetComponent<DOTweenAnimation>()?GetComponent<DOTweenAnimation>().duration:0;
    }
    void Start()
    {
        if (record != null)
        {
            record.Clear();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
    [Button(ButtonSizes.Gigantic)]
    [PropertyOrder(-1)]
    [FoldoutGroup("灯效文件读写模块")]
    public void ReadOrderData(OrderData data)
    {
        if (!data)
        {
            Debug.LogError("存储文件为空");
            return;
        }
        colorOrders.Clear();
        foreach (var order in data.colorOrders)
        {
            colorOrders.Add(order);
        }
        Debug.Log("读取成功");
    }
    [Button(ButtonSizes.Gigantic)]
    [PropertyOrder(-2)]
    [FoldoutGroup("灯效文件读写模块")]
    public void WriteData(OrderData data)
    {
        if (!data)
        {
            Debug.LogError("存储文件为空");
            return;
        }
        data.colorOrders.Clear();
        foreach (var order in colorOrders)
        {
            data.colorOrders.Add(order);
        }
        Debug.Log("写入成功");

    }
    DOTweenAnimation animation;
    public void SpeedAnimation()
    {
        if (animation == null)
        {
            animation = GetComponent<DOTweenAnimation>();
        }
        animation.duration += 50f;
        Debug.Log("trigger");
    }
}
