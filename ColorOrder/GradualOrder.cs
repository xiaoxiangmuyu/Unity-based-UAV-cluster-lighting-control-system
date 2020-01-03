using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public abstract class GradualOrder : ColorOrderBase
{
    [MinValue(0)]
    [HorizontalGroup]
    [LabelText("播放次数")]
    public int playCount = 1;
}
[LabelText("颜色变化")]
public class DoColor : GradualOrder
{
    [LabelText("是否记录颜色"), ShowIf("hideGradient")]
    public bool recordColor;
    [HideIf("hideColor")]
    [BoxGroup("Color")]
    [PropertyOrder(100)]
    public Color color = Color.white;
    [HideIf("hideGradient"), BoxGroup("Color")]
    public Gradient gradient;
    [EnumToggleButtons, HideLabel]
    [BoxGroup("Color")]
    [PropertyOrder(100)]
    public ColorType colorType;
    [MinValue(0)]
    [HorizontalGroup]
    [LabelText("持续时间")]
    public float during;
    private bool hideColor { get { return colorType != ColorType.SingleColor; } }
    private bool hideGradient { get { return colorType != ColorType.Gradient; } }

    public override Tween GetOrder(ColorPoint point)
    {
        Color targetColor = Color.white;
        switch (colorType)
        {
            case ColorType.SingleColor:
                {
                    targetColor = color; break;
                }
            case ColorType.TextureMapping:
                {
                    targetColor = point.TextureColor; break;
                }
            case ColorType.ColorMapping:
                {
                    targetColor = point.mappingColor; break;
                }
            case ColorType.Random:
                {
                    targetColor = point.randomColor; break;
                }
            case ColorType.FlowMapping:
                {
                    targetColor = point.flowTextureColor; break;
                }
            case ColorType.HSV:
                {
                    targetColor = point.hsvColor; break;
                }
            case ColorType.Origin:
                {
                    targetColor = point.originalColor; break;
                }
            case ColorType.Dark:
                {
                    Color ori = point.mat.color;
                    float h, s, v;
                    Color.RGBToHSV(ori, out h, out s, out v);
                    s = s / 2;
                    targetColor = Color.HSVToRGB(h, s, v);
                    break;
                }

        }
        if (recordColor)
        {
            point.originalColor = targetColor;
            if (!hideGradient)
                Debug.LogError("暂不支持记录复合流光的颜色");
        }

        if (!hideGradient)
            return point.mat.DOGradientColor(gradient, during);
        else
            return point.mat.DOColor(targetColor, during);
        // Debug.LogError("colorType未选择!");
        // return null;
    }
}
// [System.Serializable][LabelText("命令组")]
// public class OrderGroup:GradualOrder
// {
//     public List<ColorOrderBase>colorOrders=new List<ColorOrderBase>();

//}

