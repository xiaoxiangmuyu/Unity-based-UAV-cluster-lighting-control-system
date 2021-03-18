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
    public DoColor(ColorType colorType = ColorType.SingleColor, float during = 0f)
    {
        this.colorType = colorType;
        this.during = during;
        colors.Add(Color.white);
    }
    public DoColor()
    {
        colors.Add(Color.white);
    }
    [HideIf("hideColor")]
    [BoxGroup("Color")]
    [PropertyOrder(10)]
    public List<Color> colors = new List<Color>();

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
    //[HorizontalGroup("Color/ColorPro")]
    public bool isWithIndex;
    [ShowIf("showRandom")]
    [BoxGroup("Color")]
    //[HorizontalGroup("Color/ColorPro")]
    public bool isRandom;
    [ShowIf("isMapping")]
    [BoxGroup("Color")]
    [HorizontalGroup("Color/ColorPro")]
    [ValueDropdown("availableIndex")]
    [PropertyOrder(-10)]
    public string colorGroupName;
    [ShowIf("isWithIndex")]
    [BoxGroup("Color")]
    public int colorIndex;
    [ValueDropdown("availableData")]
    [ShowIf("isMappingData")]
    [PropertyOrder(11)]
    [BoxGroup("Color")]
    public string mappingDataName;
    MappingData mappingData;
    IEnumerable availableData
    {
        get
        {
            List<string> dataNames = new List<string>();
            if (colorType != ColorType.MappingData || ProjectManager.GetDataGroupByGroupName(colorGroupName) == null)
                return dataNames;
            dataNames.Add("UnSelect");
            var datalist = ProjectManager.GetDataGroupByGroupName(colorGroupName);
            for (int i = 0; i < datalist.mappingDatas.Count; i++)
            {
                dataNames.Add(datalist.mappingDatas[i].dataName);
            }
            return dataNames;
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
            return ProjectManager.availableGroups;
        }
    }
    MappingData GetMappingData(ColorPoint point)
    {
        if (mappingDataName != "UnSelect" && mappingDataName != null && mappingDataName != String.Empty)
            return ProjectManager.GetDataGroupByGroupName(colorGroupName).mappingDatas.Find((a) => a.dataName == mappingDataName);
        else
        {
            var data = ProjectManager.GetDataGroupByGroupName(colorGroupName);
            return data.mappingDatas.Find((a) => a.objNames.Contains(point.name));
        }
    }
    #region ColorMapper
    [ValueDropdown("availableMappingSource")]
    [ShowIf("isColorByMapper")]
    public string mapperName;
    void GetMapper()
    {
        colorMapper = ProjectManager.Instance.RecordProject.GetColorMapper(mapperName);
    }
    IEnumerable availableMappingSource
    {
        get
        {
            return ProjectManager.Instance.RecordProject.ColorMapperNames;
        }
    }
    ColorMapper colorMapper;
    #endregion
    bool hideColor { get { return colorType != ColorType.SingleColor; } }
    bool hideGradient { get { return colorType != ColorType.Gradient && colorType != ColorType.ColorByMapper; } }
    bool showHSVInfo { get { return colorType == ColorType.HSV; } }
    //bool showDarkInfo { get { return colorType == ColorType.Dark; } }
    //bool showTextureMappingInfo { get { return colorType == ColorType.TextureMapping; } }
    bool isMapping { get { return colorType == ColorType.MappingData; } }
    bool showRandom { get { return colorType == ColorType.MappingData || colorType == ColorType.SingleColor; } }
    bool isMappingData { get { return colorType == ColorType.MappingData; } }
    bool isColorByMapper { get { return colorType == ColorType.ColorByMapper; } }
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
                    if (isRandom)
                        targetColor = colors[UnityEngine.Random.Range(0, colors.Count)];
                    else
                    {
                        if (colors == null || colors.Count == 0)
                        {
                            colors = new List<Color>();
                            colors.Add(Color.white);
                        }
                        targetColor = colors[0];
                    }
                    break;
                }
            case ColorType.Black:
                {
                    targetColor = Color.black; break;
                }
            case ColorType.ColorByMapper:
                {
                    if (colorMapper == null)
                        GetMapper();
                    point.gradient = gradient;
                    point.colorMapper = colorMapper;
                    return point.mat.DOColor(point.MapperColor, during);
                }
            case ColorType.ShaderMode:
                {
                    targetColor = point.RenderColor;
                    break;
                }
            case ColorType.MappingData:
                {
                    mappingData = GetMappingData(point);
                    if (mappingData == null)
                    {
                        targetColor = Color.white;
                        //Debug.LogError(point.name + "找不到映射颜色");
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
            case ColorType.HSV:
                {
                    targetColor = point.GetColorByHSV(hsvValue); break;
                }
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


