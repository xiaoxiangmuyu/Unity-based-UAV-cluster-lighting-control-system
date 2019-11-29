using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
[CreateAssetMenu(menuName="创建命令序列容器",fileName="新命令序列")]
public class OrderData : SerializedScriptableObject
{
    [LabelText("命令序列")]
    public List<ColorOrderBase>colorOrders=new List<ColorOrderBase>();
}
