using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
[System.Serializable][LabelText("命令组")]
public class OrderGroup:GradualOrder
{
    public List<ColorOrderBase>colorOrders=new List<ColorOrderBase>();
    
}
