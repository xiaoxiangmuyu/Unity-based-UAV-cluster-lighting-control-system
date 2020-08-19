using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;
public abstract class GradualOrder : ColorOrderBase
{
    [MinValue(0)]
    [LabelText("播放次数")]
    [HorizontalGroup]
    [PropertyOrder(-15)]
    public int playCount = 1;
}
[LabelText("颜色变化")]
public class DoColor : GradualOrder
{
    public DoColor(ColorType colorType = ColorType.ColorMapping, float during = 0f)
    {
        this.colorType = colorType;
        this.during = during;
    }
    public DoColor()
    {

    }
    [HideIf("hideColor")]
    [BoxGroup("Color")]
    [PropertyOrder(10)]
    public Color color = Color.white;

    [HideIf("hideGradient"), BoxGroup("Color")]
    public Gradient gradient = new Gradient();

    [EnumToggleButtons, HideLabel]
    [BoxGroup("Color")]
    [PropertyOrder(10)]
    [HideInInspector]
    public ColorType colorType;
    [BoxGroup("Color")]
    [PropertyOrder(-10)]
    [ValueDropdown("availableColorTypes")]
    [ShowInInspector]
    public string ColorTypeName
    {
        get
        {
            if (colorTypeName == null)
            {
                //if(colorType.ToString()=="6"||colorType.ToString()=="7")
                colorTypeName = colorType.ToString();
            }
            return colorTypeName;
        }
        set
        {
            colorTypeName = value;
            ColorType d = (ColorType)Enum.Parse(typeof(ColorType), colorTypeName);
            colorType = d;
        }
    }
    [SerializeField]
    [HideInInspector]
    private string colorTypeName;

    [MinValue(0)]
    [HorizontalGroup]
    [LabelText("持续时间")]
    [PropertyOrder(-15)]
    public float during;

    [MaxValue(1)]
    [ShowIf("showHSVInfo")]
    [PropertyOrder(1)]
    [BoxGroup("Color")]
    public Vector3 hsvValue;

    // [MinValue(0)]
    // [MaxValue(1)]
    // [ShowIf("showDarkInfo")]
    // [PropertyOrder(1)]
    // [BoxGroup("Color")]
    // public Vector2 darkValue;

    [ShowIf("isMapping")]
    [BoxGroup("Color")]
    [HorizontalGroup("Color/ColorPro")]
    public bool isWithIndex;
    [ShowIf("isMapping")]
    [BoxGroup("Color")]
    [HorizontalGroup("Color/ColorPro")]
    public bool isRandom;
    [ShowIf("isMapping")]
    [BoxGroup("Color")]
    [HorizontalGroup("Color/ColorPro")]
    [ValueDropdown("availableIndex")]
    [PropertyOrder(-10)]
    public int groupIndex;
    [ShowIf("isWithIndex")]
    [BoxGroup("Color")]
    public int colorIndex;
    [ValueDropdown("availableData")]
    [ShowIf("isMappingData")]
    [PropertyOrder(11)]
    [BoxGroup("Color")]
    public string mappingDataName;
    // [ShowIf("isMappingData")]
    // [SerializeField]
    // public MappingData mappingData;
    MappingData mappingData;
    IEnumerable availableData
    {
        get
        {
            List<string> dataNames = new List<string>();
            dataNames.Add("UnSelect");
            if (groupIndex == 0)
            {
                var datalist = ProjectManager.Instance.RecordProject.mappingDatas;
                foreach (var data in datalist)
                    dataNames.Add(data.dataName);
                return dataNames;
            }
            else
            {
                var datalist = ProjectManager.Instance.RecordProject.mappingDatas;
                datalist.ForEach((a)=>
                {
                    if(a.groupIndex==groupIndex)
                    dataNames.Add(a.dataName);
                });
                return dataNames;
            }
        }
    }
    IEnumerable availableColorTypes
    {
        get
        {
            return ColorManager.ColorTypes;
        }
    }
    IEnumerable availableIndex
    {
        get
        {
            int count = ProjectManager.Instance.RecordProject.globalPosDic.Count;
            var temp = new List<int>();
            for (int i = 0; i < count; i++)
            {
                temp.Add(i + 1);
            }
            return temp;
        }
    }
    MappingData GetMappingData(ColorPoint point)
    {
        if (mappingDataName!="UnSelect"&&mappingDataName!=null&&mappingDataName!=String.Empty)
            return ProjectManager.Instance.RecordProject.mappingDatas.Find((a) => a.dataName == mappingDataName);
        else
        {
            var data = ProjectManager.Instance.RecordProject.mappingDatas;
            List<MappingData> temp = new List<MappingData>(data.FindAll((a) => a.groupIndex == groupIndex));
            return temp.Find((a) => a.names.Contains(point.name));
        }
    }
    bool hideColor { get { return colorType != ColorType.SingleColor; } }
    bool hideGradient { get { return colorType != ColorType.Gradient; } }
    bool showHSVInfo { get { return colorType == ColorType.HSV; } }
    //bool showDarkInfo { get { return colorType == ColorType.Dark; } }
    bool showColorMappingInfo { get { return colorType == ColorType.ColorMapping; } }
    //bool showTextureMappingInfo { get { return colorType == ColorType.TextureMapping; } }
    bool isMapping { get { return colorType == ColorType.ColorMapping || colorType == ColorType.MappingData; } }
    bool isMappingData { get { return colorType == ColorType.MappingData; } }
    public override Tween GetOrder(ColorPoint point)
    {
        if (ColorTypeName.Equals("6"))
            ColorTypeName = "Gradient";
        else if (ColorTypeName.Equals("7"))
            ColorTypeName = "Black";
        else if (ColorTypeName.Equals("ColorMapping"))
            ColorTypeName = "MappingData";
        else
        {
            ColorType d = (ColorType)Enum.Parse(typeof(ColorType), colorTypeName);
            colorType = d;
        }
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
            // case ColorType.GlobalGradient:
            //     {
            //         point.gradient=gradient2;
            //         point.mappingSource=MappingSource;
            //         targetColor = point.globalColor;
            //         break;
            //     }
            case ColorType.ColorMapping:
                {
                    if (isWithIndex)
                    {
                        targetColor = point.GetMappingColor(colorIndex); break;
                    }
                    else
                    {
                        targetColor = point.GetMappingColor(); break;
                    }
                }
            case ColorType.MappingData:
                {
                    mappingData = GetMappingData(point);
                    if (mappingData == null)
                    {
                        Debug.LogError(point.name + "找不到映射颜色");
                        break;
                    }
                    if (isWithIndex)
                    {
                        targetColor = mappingData.GetMappingColor(point.name, colorIndex);
                    }
                    else
                    {
                        if (isRandom)
                            targetColor = mappingData.GetMappingColor(point.name, 0, true);
                        else
                            targetColor = mappingData.GetMappingColor(point.name);
                    }

                    break;
                }
            // case ColorType.Random:
            //     {
            //         targetColor = point.randomColor; break;
            //     }
            // case ColorType.FlowMapping:
            //     {
            //         targetColor = point.flowTextureColor; break;
            //     }
            case ColorType.HSV:
                {
                    targetColor = point.GetColorByHSV(hsvValue); break;
                }
                // case ColorType.Origin:
                //     {
                //         targetColor = point.originalColor; break;
                //     }
                // case ColorType.Dark:
                //     {
                //         targetColor = point.GetDarkColor(darkValue); break;
                //     }

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


