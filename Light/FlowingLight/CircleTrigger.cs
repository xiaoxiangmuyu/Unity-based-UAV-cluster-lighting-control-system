using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTrigger : TriggerBase
{
    // Start is called before the first frame update
    SphereCollider col;
    public float targetRaidus;
    public float during;

    private float originRadius;
    protected override void Awake()
    {
        base.Awake();
        col=GetComponent<SphereCollider>();
        originRadius=col.radius;

    }
    protected override void Play(float speed)
    {
        if(targetRaidus-col.radius<=changeMargin)
        {
            playingCount-=1;
            if(playingCount==0)
            return;
            else
            {
                col.radius=originRadius;
                isWait=true;
                waitTimer=0;
            }
        }
        if(isWait)
        {
            waitTimer+=Time.deltaTime;
            if(waitTimer>=waitInterval+colorChangingTime*2)
            {
                isWait=false;
            }
            else
            return;
        }
        col.radius+=(targetRaidus-col.radius)/during/25;

    }
}
