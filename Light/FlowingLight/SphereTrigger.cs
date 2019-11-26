using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTrigger : TriggerBase
{
    public float originalRadius = 0.1f;
    public float maxRadius = 3.4f;

    protected override void Play(float speed)
    {
        SphereCollider sphereCollider = curCollider as SphereCollider;

        if (sphereCollider.radius != maxRadius)
        {
            sphereCollider.radius = Mathf.Lerp(sphereCollider.radius, maxRadius, speed * Time.deltaTime);

            if (Mathf.Abs(sphereCollider.radius - maxRadius) < changeMargin)
            {
                sphereCollider.radius = maxRadius;
            }
        }
        else
        {
            playingTimer += Time.deltaTime;

            if (playingTimer >= waitInterval)
            {
                playingTimer = 0f;
                sphereCollider.radius = originalRadius; // 重置触发器大小以便再次播放流光
                playingCount--;
            }
        }
    }
}
