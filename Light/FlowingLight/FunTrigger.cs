// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class FunTrigger : TriggerBase
// {
//     private Quaternion targetRotation;
//     private Quaternion originalRotation;

//     protected override void Awake()
//     {
//         base.Awake();
//         targetRotation = Quaternion.Euler(transform.localRotation.x, 180f, transform.localRotation.z);
//         originalRotation = transform.localRotation;
//     }

//     protected override void Play(float speed)
//     {
//         if (transform.localRotation != targetRotation)
//         {
//             transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, speed * Time.deltaTime);

//             if (Quaternion.Angle(targetRotation, transform.localRotation) < changeMargin)
//             {
//                 transform.localRotation = targetRotation;
//             }
//         }
//         else
//         {
//             playingTimer += Time.deltaTime;

//             if (playingTimer >= waitInterval)
//             {
//                 playingTimer = 0f;
//                 transform.localRotation = originalRotation; // 重置触发器位置以便再次播放流光
//                 playingCount--;
//             }
//         }
//     }
// }
