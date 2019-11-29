using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
[System.Serializable]
public abstract class ColorOrderBase
{
    public virtual Tween GetOrder(ColorPoint point) { Debug.LogError("没有实现命令！"); return null; }
}
public abstract class GradualOrder : ColorOrderBase
{

    [EnumToggleButtons]
    public ColorType colorType;
    [MinValue(0)]
    public float during;
 
    public int playCount=1;
    [HideIf("hideColor")]
    public Color color;


    private bool hideColor{get{return colorType==ColorType.TextureMapping||colorType==ColorType.ColorByHue;}}
}
public abstract class CallBack : ColorOrderBase
{

}


