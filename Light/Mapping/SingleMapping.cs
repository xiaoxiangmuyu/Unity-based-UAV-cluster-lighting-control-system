using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMapping : ColorParent
{
    public Color color;

    public override Color GetMappingColor(Transform trans,int texIndex)
    {
        return color;
    }

}
