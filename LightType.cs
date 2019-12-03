using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    SingleColor,//指定显示单一颜色
    //LowBrightness,//现有颜色基础上亮度减半变暗
    TextureMapping,//贴图映射
    //ColorAndReset,//流光，渐渐接近目标颜色，完成后还原
    FlowMapping,
    Random,
    HSV//沿色环取色
}
