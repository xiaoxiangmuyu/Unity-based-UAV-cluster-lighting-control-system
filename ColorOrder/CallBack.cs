// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// using System;
// public abstract class CallBack : ColorOrderBase
// {
//     public virtual void GetCallBack()
//     {
//         counter += 1;
//         if (counter == TriggerCount)
//         {
//             Action();
//             counter = 0;
//         }
//     }
//     protected abstract void Action();
//     public Transform doGroup;
//     int counter;
//     int triggerCount;
//     int TriggerCount
//     {
//         get
//         {
//             if (triggerCount == 0)
//             {
//                 triggerCount = doGroup.GetComponentsInChildren<ColorPoint>().Length;
//                 return triggerCount;
//             }
//             else
//                 return triggerCount;
//         }
//     }
//     public class SetUpGroupWithOrder : CallBack
//     {
//         public System.Action<List<ColorOrderBase>> action;
//         public List<ColorOrderBase> orders;

//         protected override void Action()
//         {
//             if (action != null)
//             {
//                 if (orders == null)
//                 {
//                     Debug.LogError("没有设置回调的颜色命令");
//                     return;
//                 }
//                 action(orders);
//                 Debug.Log("action!");
//             }
//             else
//             {
//                 Debug.LogError("回调方法为空");
//             }
//         }

//     }
//     public class SetUpGroupWithFile : CallBack
//     {
//         public System.Action<OrderData> action;
//         public OrderData orderFile;
//         protected override void Action()
//         {
//             if (action == null)
//             {
//                 Debug.LogError("回调方法没有设置");
//                 return;
//             }
//             if (orderFile == null || orderFile.colorOrders == null || orderFile.colorOrders.Count == 0)
//             {
//                 Debug.LogError("命令文件有问题");
//                 return;
//             }
//             action(orderFile);

//         }

//     }
//     public class SetUpGroup : CallBack
//     {
//         public System.Action action;
//         protected override void Action()
//         {
//             if (action == null)
//             {
//                 Debug.LogError("回调方法没有设置");
//                 return;
//             }
//             action();
//         }
//     }
//     public class LogTime:CallBack
//     {
//         public string logInfo;
//         protected override void Action()
//         {
//             Debug.Log(logInfo+"  "+Time.time);
//         }
//     }
// }


