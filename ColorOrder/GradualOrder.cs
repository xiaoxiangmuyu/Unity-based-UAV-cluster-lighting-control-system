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
    public DoColor(ColorType colorType=ColorType.ColorMapping,float during=0f)
    {
        this.colorType=colorType;
        this.during=during;
    }
    public DoColor()
    {
        
    }
    [LabelText("是否记录颜色"), ShowIf("hideGradient")]
    public bool recordColor;

    [HideIf("hideColor")]
    [BoxGroup("Color")]
    [PropertyOrder(10)]
    public Color color = Color.white;

    [HideIf("hideGradient"), BoxGroup("Color")]
    public Gradient gradient = new Gradient();

    [EnumToggleButtons, HideLabel]
    [BoxGroup("Color")]
    [PropertyOrder(10)]
    public ColorType colorType;

    [MinValue(0)]
    [HorizontalGroup]
    [LabelText("持续时间")]
    public float during;

    [MaxValue(1)]
    [ShowIf("showHSVInfo")]
    [PropertyOrder(1)]
    [BoxGroup("Color")]
    public Vector3 hsvValue;

    [MinValue(0)]
    [MaxValue(1)]
    [ShowIf("showDarkInfo")]
    [PropertyOrder(1)]
    [BoxGroup("Color")]
    public Vector2 darkValue;

    [ShowIf("isMapping")]
    [BoxGroup("Color")]
    public bool isWithIndex;
    [ShowIf("isWithIndex")]
    [BoxGroup("Color")]
    public int targetIndex;
    [ValueDropdown("availableData")]
    [ShowIf("isMappingData")]
    [OnValueChanged("GetMappingData")]
    public string dataName;
    [ShowIf("isMappingData")]
    public MappingData mappingData;
    IEnumerable availableData
    {
        get
        {
            var datalist = ProjectManager.Instance.RecordProject.mappingDatas;
            List<string>dataName=new List<string>();
            foreach(var data in datalist)
            dataName.Add(data.dataName);
            return dataName;
        }
    }
    void GetMappingData()
    {
        var datalist = ProjectManager.Instance.RecordProject.mappingDatas;
        mappingData=datalist.Find((a)=>a.dataName==dataName);
    }
    bool hideColor { get { return colorType != ColorType.SingleColor; } }
    bool hideGradient { get { return colorType != ColorType.Gradient; } }
    bool showHSVInfo { get { return colorType == ColorType.HSV; } }
    bool showDarkInfo { get { return colorType == ColorType.Dark; } }
    bool showColorMappingInfo { get { return colorType == ColorType.ColorMapping; } }
    bool showTextureMappingInfo { get { return colorType == ColorType.TextureMapping; } }
    bool isMapping { get { return colorType == ColorType.TextureMapping || colorType == ColorType.ColorMapping; } }
    bool isMappingData { get { return colorType == ColorType.MappingData; } }
    public override Tween GetOrder(ColorPoint point)
    {
        Color targetColor = Color.white;
        switch (colorType)
        {
            case ColorType.SingleColor:
                {
                    targetColor = color; break;
                }
            case ColorType.Black:
                {
                    targetColor = Color.black; break;
                }
            case ColorType.TextureMapping:
                {
                    if (isWithIndex)
                    {
                        targetColor = point.GetTextureColor(targetIndex); break;
                    }
                    else
                    {
                        targetColor = point.GetTextureColor(); break;
                    }
                }
            case ColorType.ColorMapping:
                {
                    if (isWithIndex)
                    {
                        targetColor = point.GetMappingColor(targetIndex); break;
                    }
                    else
                    {
                        targetColor = point.GetMappingColor(); break;
                    }
                }
            case ColorType.MappingData:
                {
                    if(!mappingData.isNull())
                    targetColor = mappingData.GetMappingColor(point.name);
                    else
                    {
                        foreach(var data in ProjectManager.Instance.RecordProject.mappingDatas)
                        {
                            if(data.ContainsPoint(point.name))
                            mappingData=data;
                            targetColor=mappingData.GetMappingColor(point.name);
                            break;
                        }
                    }
                    break;
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
                    targetColor = point.GetColorByHSV(hsvValue); break;
                }
            case ColorType.Origin:
                {
                    targetColor = point.originalColor; break;
                }
            case ColorType.Dark:
                {
                    targetColor = point.GetDarkColor(darkValue); break;
                }

        }
        if (recordColor)
        {
            point.originalColor = targetColor;
            if (!hideGradient)
                Debug.LogError("暂不支持记录复合流光的颜色");
        }

        if (!hideGradient)
        {
            if (during == 0)
                Debug.LogError("渐进颜色持续时间不能为0" + point.gameObject.name);
            return point.mat.DOGradientColor(gradient, during);
        }
        else
            return point.mat.DOColor(targetColor, during);
        //颜色混合在连续灯光命令中会发生内部融合导致灯光变化不正确
        //return point.mat.DOBlendableColor(targetColor, during);

    }
}


