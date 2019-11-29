using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
[LabelText("颜色变化")]
public class DoColor : GradualOrder
{
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
public class Reset : GradualOrder
{
    public override Tween GetOrder(ColorPoint point)
    {
        return point.mat.DOColor(point.mappingColor, during);
    }
}
[LabelText("停顿")]
public class Interval : ColorOrderBase
{
    public float during;
}
[LabelText("呼吸灯")]
public class Breath : GradualOrder
{
    public float interval;
    [MaxValue("during")]
    public float timePoint;
    public override Tween GetOrder(ColorPoint point)
    {
        switch (colorType)
        {
            case ColorType.SingleColor:
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(point.mat.DOColor(color, during / 2));
                    sequence.Append(point.mat.DOColor(Color.black, during / 2));
                    sequence.AppendInterval(interval);
                    return sequence;
                }
            case ColorType.TextureMapping:
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(point.mat.DOColor(point.mappingColor, during / 2));
                    sequence.Append(point.mat.DOColor(Color.black, during / 2));
                    sequence.AppendInterval(interval);
                    return sequence;
                }
            case ColorType.Random:
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(point.mat.DOColor(point.randomColor, during / 2));
                    sequence.Append(point.mat.DOColor(Color.black, during / 2));
                    sequence.AppendInterval(interval);
                    return sequence;
                }
        }
        Debug.LogError("colorType未选择!");
        return null;
    }
}
[LabelText("流光")]
public class Flowing : GradualOrder
{
    public float interval;
    public Color ResetColor;
    public ColorType resetColorType;
    Color endColor;
    Color resetColor;
    public override Tween GetOrder(ColorPoint point)
    {
        switch (colorType)
        {
            case ColorType.SingleColor:
                {
                    endColor = color;
                    break;
                }
            case ColorType.TextureMapping:
                {
                    endColor = point.mappingColor;
                    break;
                }
            case ColorType.Random:
                {
                    endColor = point.randomColor;
                    break;
                }
            case ColorType.FlowMapping:
                {
                    endColor = point.flowMappingColor;
                    break;
                }
        }
        switch (resetColorType)
        {
            case ColorType.SingleColor:
                {
                    resetColor = ResetColor;
                    break;
                }
            case ColorType.TextureMapping:
                {
                    resetColor = point.mappingColor;
                    break;
                }
            case ColorType.Random:
                {
                    resetColor = point.randomColor;
                    break;
                }
            case ColorType.FlowMapping:
                {
                    resetColor = point.flowMappingColor;
                    break;
                }
        }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(point.mat.DOColor(endColor, during / 2));
        sequence.Append(point.mat.DOColor(resetColor, during / 2));
        sequence.AppendInterval(interval);
        //Debug.LogError("colorType未选择!");
        return sequence;
    }

}


