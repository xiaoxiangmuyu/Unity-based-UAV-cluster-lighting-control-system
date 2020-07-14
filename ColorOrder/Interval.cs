using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[LabelText("停顿")]
public class Interval : ColorOrderBase
{
    public Interval(bool isRandom,float value)
    {
        this.Random=isRandom;
        if(isRandom)
        range.y=value;
        else
        during=value;
    }
    public Interval()
    {
        this.during=1f;
    }
    public bool Random;
    [ShowIf("Random")]
    public Vector2 range;
    [SerializeField][HideInInspector]
    public float _during;
    [ShowInInspector][HideIf("Random")]
    public float during{get{
        if(Random)
        return UnityEngine.Random.Range(range.x,range.y);
        else
        return _during;
    }
    set{
        _during=value;
    }}
}
