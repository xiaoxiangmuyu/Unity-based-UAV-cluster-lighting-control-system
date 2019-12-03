using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public abstract class GradualOrder : ColorOrderBase
{  
    public int playCount=1;
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
            case ColorType.FlowMapping:
                {
                    return point.mat.DOColor(point.flowMappingColor, during);
                }
            case ColorType.HSV:
                {
                    return point.mat.DOColor(point.hsvColor, during);
                }
        }
        Debug.LogError("colorType未选择!");
        return null;
    }
}
}
