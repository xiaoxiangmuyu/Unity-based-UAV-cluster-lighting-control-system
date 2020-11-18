using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Events;
[System.Serializable]
public class OrderGroup:GradualOrder
{
    [ListDrawerSettings(Expanded=true)]//[InlineProperty]
    public List<ColorOrderBase>colorOrders=new List<ColorOrderBase>();
    
}
[LabelText("入画:亮灭几次出现")]
public class In_1:OrderGroup
{
    public In_1()
    {
        OrderGroup orderGroup=new OrderGroup();
        orderGroup.colorOrders.Add(new DoColor(ColorType.MappingData,0.2f));
        orderGroup.colorOrders.Add(new DoColor(ColorType.Black,0.2f));
        orderGroup.colorOrders.Add(new Interval(false,0.5f));
        orderGroup.playCount=3;
        this.colorOrders.Add(orderGroup);
        this.colorOrders.Add(new DoColor(ColorType.SingleColor,0.2f));
        this.colorOrders.Add(new DoColor(ColorType.MappingData,0.5f));
    }
}
[LabelText("出画:随机闪消失")]
public class Out_RandomFade:OrderGroup
{
    public Out_RandomFade()
    {
        OrderGroup orderGroup=new OrderGroup();
        orderGroup.colorOrders.Add(new Interval(true,0.3f));
        orderGroup.colorOrders.Add(new DoColor(ColorType.Black,0.1f));
        orderGroup.colorOrders.Add(new DoColor(ColorType.MappingData,0.1f));
        orderGroup.playCount=1;
        this.colorOrders.Add(orderGroup);
        this.colorOrders.Add(new DoColor(ColorType.Black,0.5f));
    }
}
// [LabelText("路径灯光")]
// public class PathLight:OrderGroup
// {
//     public PathLight()
//     {
//         this.colorOrders.Add(new DoColor(ColorType.SingleColor,0.2f));
//         this.colorOrders.Add(new DoColor(ColorType.MappingData,0.3f));
//         this.colorOrders.Add(new DoColor(ColorType.Black,0.2f));
//     }
// }
[LabelText("普通流光")]
public class FlowLight:OrderGroup
{
    public FlowLight()
    {
        this.colorOrders.Add(new DoColor(ColorType.SingleColor,0.2f));
        this.colorOrders.Add(new DoColor(ColorType.MappingData,0.5f));
        this.colorOrders.Add(new Interval(false,1f));

    }
}
//[LabelText("呼吸灯")]

// public class BreathLight:OrderGroup
// {
//     public BreathLight()
//     {
//         this.colorOrders.Add(new DoColor(ColorType.MappingData,0.5f));
//         this.colorOrders.Add(new DoColor(ColorType.Black,0.5f));
//     }

// }
[LabelText("星星闪")]
public class StarLight:OrderGroup
{
    public StarLight()
    {
        this.colorOrders.Add(new Interval(true,1));
        this.colorOrders.Add(new DoColor(ColorType.MappingData,0.5f));
        this.colorOrders.Add(new DoColor(ColorType.Black,0.5f));
        this.playCount=10;
    }
}