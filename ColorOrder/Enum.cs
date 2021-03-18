using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
public static class ColorManager
{
    public static readonly List<string> ColorTypes = new List<string> { "SingleColor", "Gradient", "ShaderMode", "MappingData", "HSV", "ColorByMapper", "Black" };
}
public enum ColorType
{
    SingleColor,//指定显示单一颜色
    //TextureMapping,//静态贴图映射
    //FlowMapping,//流动贴图，每次取的时候才会偏移贴图，不会随着时间自动偏移
    ShaderMode,
    MappingData,
    //Random,//随机取色，每次取颜色不一样，每个点不一样
    HSV,//沿色环取色，每次取的时候才会变化，与时间无关，每个点取到的颜色一致
    //Origin,//取光点的originColor，需要提前上色的时候勾选recordColor，不然就会取到默认的白色
    Gradient,//渐变色，可以提前指定一个渐变色，相当于复合颜色的流光，可以想象成一个多彩的SingleColor,非常好用
    //Dark,//变暗
    Black,//纯黑
    ColorByMapper

}
public enum OrderType
{
    Custom,
    OrderFile
}
public enum PointState
{
    Idle,//空闲状态
    Busy,//繁忙状态
}
public enum TriggerType
{
    Circle,
    Rect
}
//渐变的方向类型
public enum DirType
{
    Up_Down,//上下
    Down_UP,//下上
    Left_Right,//左右
    Right_Left,//右左
    In_Out,//内外
    Out_In,//外内
    List,//列表顺序
    Ball,//3D球形映射
}
public enum BlockState

{
    NoData,
    NeedRefresh,
    Ready

}



