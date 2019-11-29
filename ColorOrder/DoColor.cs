using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
[LabelText("颜色变化")]
public class DoColor : GradualOrder
{
    [HideIf("hideColor")]
    [BoxGroup("Color")]
    [PropertyOrder(100)]
    public Color color;
    [EnumToggleButtons]
    [BoxGroup("Color")]
    [PropertyOrder(100)]
    public ColorType colorType;
    [MinValue(0)]
    public float during;

    private bool hideColor { get { return colorType != ColorType.SingleColor; } }

    public override Tween GetOrder(ColorPoint point)
    {
        switch (colorType)
        {
            case ColorType.SingleColor:
                {
                    return point.mat.DOColor(color, during);
                }
            case ColorType.TextureMapping:
                {
                    return point.mat.DOColor(point.mappingColor, during);
                }
            case ColorType.Random:
                {
                    return point.mat.DOColor(point.randomColor, during);
                }
        }
        Debug.LogError("colorType未选择!");
        return null;
    }
}
// public class Reset : GradualOrder
// {
//     public override Tween GetOrder(ColorPoint point)
//     {
//         return point.mat.DOColor(point.mappingColor, during);
//     }
// }


// [LabelText("颜色,停顿,颜色")]
// public class C_P_C : GradualOrder
// {
//     [LabelText("亮起停顿时间")]
//     public float interval;
//     [MaxValue("during")]
//     public float timePoint;
//     [HideIf("hideResetColor")][BoxGroup("reset")][PropertyOrder(200)]
//     public Color ResetColor;
//     [BoxGroup("reset")][EnumToggleButtons][PropertyOrder(200)]
//     public ColorType resetColorType;

//     Color endColor;
//     Color resetColor;
//     public override Tween GetOrder(ColorPoint point)
//     {
//         switch (colorType)
//         {
//             case ColorType.SingleColor:
//                 {
//                     endColor=color;
//                     break;
//                 }
//             case ColorType.TextureMapping:
//                 {
//                     endColor=point.mappingColor;
//                     break;
//                 }
//             case ColorType.Random:
//                 {
//                     endColor=point.randomColor;
//                     break;
//                 }
//             case ColorType.FlowMapping:
//             {
//                 endColor=point.flowMappingColor;
//                 break;
//             }
//         }
//         switch (resetColorType)
//         {
//             case ColorType.SingleColor:
//                 {
//                     resetColor = ResetColor;
//                     break;
//                 }
//             case ColorType.TextureMapping:
//                 {
//                     resetColor = point.mappingColor;
//                     break;
//                 }
//             case ColorType.Random:
//                 {
//                     resetColor = point.randomColor;
//                     break;
//                 }
//             case ColorType.FlowMapping:
//                 {
//                     resetColor = point.flowMappingColor;
//                     break;
//                 }
//         }
//         Sequence sequence=DOTween.Sequence();
//         sequence.Append(point.mat.DOColor(endColor,dur))
//     }
// }
// [LabelText("颜色,颜色,停顿")]
// public class C_C_P : GradualOrder
// {
//     public float interval;
//     [HideIf("hideResetColor")][BoxGroup("reset")][PropertyOrder(200)]
//     public Color ResetColor;
//     [BoxGroup("reset")][EnumToggleButtons][PropertyOrder(200)]
//     public ColorType resetColorType;
//     Color endColor;
//     Color resetColor;
//     bool hideResetColor{get{return resetColorType!=ColorType.SingleColor;}}
//     public override Tween GetOrder(ColorPoint point)
//     {
//         switch (colorType)
//         {
//             case ColorType.SingleColor:
//                 {
//                     endColor = color;
//                     break;
//                 }
//             case ColorType.TextureMapping:
//                 {
//                     endColor = point.mappingColor;
//                     break;
//                 }
//             case ColorType.Random:
//                 {
//                     endColor = point.randomColor;
//                     break;
//                 }
//             case ColorType.FlowMapping:
//                 {
//                     endColor = point.flowMappingColor;
//                     break;
//                 }
//         }
//         switch (resetColorType)
//         {
//             case ColorType.SingleColor:
//                 {
//                     resetColor = ResetColor;
//                     break;
//                 }
//             case ColorType.TextureMapping:
//                 {
//                     resetColor = point.mappingColor;
//                     break;
//                 }
//             case ColorType.Random:
//                 {
//                     resetColor = point.randomColor;
//                     break;
//                 }
//             case ColorType.FlowMapping:
//                 {
//                     resetColor = point.flowMappingColor;
//                     break;
//                 }
//         }
//         Sequence sequence = DOTween.Sequence();
//         sequence.Append(point.mat.DOColor(endColor, during / 2));
//         sequence.Append(point.mat.DOColor(resetColor, during / 2));
//         sequence.AppendInterval(interval);
//         //Debug.LogError("colorType未选择!");
//         return sequence;
//     }

// }


