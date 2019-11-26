using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMapping : ColorMapping
{
    protected override void MappingFunc()
    {
        SetColor(destTex);
        isFinished = true;
    }
}
