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
public class Interval : ColorOrderBase
{
    public float during;
}
public class Breath : GradualOrder
{
    public override Tween GetOrder(ColorPoint point)
    {
        switch (colorType)
        {
            case ColorType.SingleColor:
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(point.mat.DOColor(point.randomColor, during / 2));
                    sequence.Append(point.mat.DOColor(Color.black, during / 2));
                    return sequence;
                }
            case ColorType.TextureMapping:
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(point.mat.DOColor(point.mappingColor, during / 2));
                    sequence.Append(point.mat.DOColor(Color.black, during / 2));
                    return sequence;
                }
        }
        Debug.LogError("colorType未选择!");
        return null;
    }
}
    public class Flowing:GradualOrder
    {
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
                    endColor=color;
                    break;
                }
            case ColorType.TextureMapping:
                {
                    endColor=point.mappingColor;
                    break;
                }
        }
        switch (resetColorType)
        {
            case ColorType.SingleColor:
                {
                    resetColor=ResetColor;
                    break;
                }
            case ColorType.TextureMapping:
                {
                    resetColor=point.mappingColor;
                    break;
                }
        }
        Sequence sequence=DOTween.Sequence();
        sequence.Append(point.mat.DOColor(endColor,during/2));
        sequence.Append(point.mat.DOColor(resetColor,during/2));
        //Debug.LogError("colorType未选择!");
        return sequence;
    }
    
}


