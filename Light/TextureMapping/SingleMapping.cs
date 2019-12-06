using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMapping : ColorMapping
{
    public Color color;
    // protected override void MappingFunc()
    // {
    //     SetColor(color);
    //     isFinished = true;
    // }
    protected override void Awake()
    {
        
    }
    public override Color GetMappingColor(Transform trans,int texIndex)
    {
        return color;
    }

}
