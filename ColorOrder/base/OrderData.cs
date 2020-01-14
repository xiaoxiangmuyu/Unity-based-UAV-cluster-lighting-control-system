using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
[CreateAssetMenu(menuName = "创建灯效文件", fileName = "新灯效")]
public class OrderData : SerializedScriptableObject
{
    // [ShowInInspector][PropertyOrder(1)]
    // public double totalTime{get{return Tools.GetTotalTime(colorOrders);}}
    public bool forceMode;//执行时是否强制覆盖正在执行的颜色命令  
    public float totalTime{get{if(colorOrders!=null) return Tools.GetTotalTime(colorOrders);else return 0;}}
    [LabelText("灯效命令")][ShowInInspector][PropertyOrder(2)]
    public List<ColorOrderBase> colorOrders = new List<ColorOrderBase>();
   



}
