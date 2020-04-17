using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class TriggerBase : SerializedMonoBehaviour
{
    public List<RecordData> recordGroup;
    public RecordData data;
    //记录第一个点的时间
    [HideInInspector]
    public float recordTimer;

    [HorizontalGroup("Tags")]
    public List<string> targetTags = new List<string>(); // 影响的飞机的标签
    [HorizontalGroup("Tags")]
    public List<string> ignoreTags = new List<string>();
    protected virtual void Awake()
    {

    }
    void Start()
    {
        data.Init();
        recordGroup = ProjectManager.Instance.RecordProject.RecordDic[data.parentName];
    }
    // Update is called once per frame
    void Update()
    {

    }
    [Button(ButtonSizes.Gigantic)]
    void Push()
    {
        ProjectManager.Instance.RecordProject.AddData(data.parentName,data);
        data.Clear();
    }

    // [Button(ButtonSizes.Gigantic)]
    // [PropertyOrder(-1)]
    // [FoldoutGroup("灯效文件读写模块")]
    // public void ReadOrderData(OrderData data)
    // {
    //     if (!data)
    //     {
    //         Debug.LogError("存储文件为空");
    //         return;
    //     }
    //     colorOrders.Clear();
    //     foreach (var order in data.colorOrders)
    //     {
    //         colorOrders.Add(order);
    //     }
    //     Debug.Log("读取成功");
    // }
    // [Button(ButtonSizes.Gigantic)]
    // [PropertyOrder(-2)]
    // [FoldoutGroup("灯效文件读写模块")]
    // public void WriteData(OrderData data)
    // {
    //     if (!data)
    //     {
    //         Debug.LogError("存储文件为空");
    //         return;
    //     }
    //     data.colorOrders.Clear();
    //     foreach (var order in colorOrders)
    //     {
    //         data.colorOrders.Add(order);
    //     }
    //     Debug.Log("写入成功");

    // }
}
