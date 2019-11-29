// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CubeTrigger : TriggerBase
// {
//     public Vector3 destPos; // 播放流光时Box Collider的终点位置

//     protected override void Play(float speed)
//     {
//        if (transform.localPosition != destPos)
//        {
//            transform.localPosition = Vector3.Lerp(transform.localPosition, destPos, speed * Time.deltaTime);

//            if (Vector3.Distance(destPos, transform.localPosition) < changeMargin)
//            {
//                transform.localPosition = destPos;
//            }
//        }
//        else
//        {
//            playingTimer += Time.deltaTime;

//            if (playingTimer >= waitInterval)
//            {
//                playingTimer = 0f;
//                transform.localPosition = originalPos; // 重置触发器位置以便再次播放流光
//                playingCount--;
//            }
//        }
//     }
// }
