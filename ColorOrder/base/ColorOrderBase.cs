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




