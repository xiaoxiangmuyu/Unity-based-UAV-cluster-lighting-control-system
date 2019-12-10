using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class ColorParent : SerializedMonoBehaviour
{
    public abstract Color GetMappingColor(Transform trans,int texIndex);
}
