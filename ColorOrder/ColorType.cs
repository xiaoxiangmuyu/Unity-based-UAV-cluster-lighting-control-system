using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    SingleColor,//指定显示单一颜色
    TextureMapping,//静态贴图映射
    FlowMapping,//流动贴图，每次取的时候才会偏移贴图，不会随着时间自动偏移
    Random,//随机取色，每次取颜色不一样，每个点不一样
    HSV,//沿色环取色，每次取的时候才会变化，与时间无关，每个点取到的颜色一致
    Origin,//取光点的originColor，需要提前上色的时候勾选recordColor，不然就会取到默认的白色
    Gradient//渐变色，可以提前指定一个渐变色，相当于复合颜色的流光，可以想象成一个多彩的SingleColor,非常好用

}
