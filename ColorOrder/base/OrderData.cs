using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
[CreateAssetMenu(menuName = "创建命令序列容器", fileName = "新命令序列")]
public class OrderData : SerializedScriptableObject
{
    // [ShowInInspector][PropertyOrder(1)]
    // public double totalTime{get{return Tools.GetTotalTime(colorOrders);}}
    public bool forceMode;//执行时是否强制覆盖正在执行的颜色命令  
    [LabelText("命令序列")][ShowInInspector][PropertyOrder(2)]
    public List<ColorOrderBase> colorOrders = new List<ColorOrderBase>();
   



}
