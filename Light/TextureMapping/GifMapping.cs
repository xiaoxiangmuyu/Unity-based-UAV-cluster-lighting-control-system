// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class GifMapping : ContinuousMapping
// {
//     public Texture2D[] texs;

//     private List<Texture2D> destTexs = new List<Texture2D>();
//     private int index = 0;

//     protected override void Awake()
//     {
//         base.Awake();

//         // GIF图片转为png数组，以此为源图片生成目标图片数组
//         if (texs != null && texs.Length > 0)
//         {
//             Texture2D tmp;

//             for (int i = 0; i < texs.Length; i++)
//             {
//                 tmp = ScaleTexture(texs[i], intMaxX, intMaxY);
//                 destTexs.Add(tmp);
//             }
//         }
//     }

//     protected override void HandleTexture()
//     {
//         if (texs == null || texs.Length < 1)
//         {
//             return;
//         }

//         if (destTexs != null && destTexs.Count > 0)
//         {
//             if (index == destTexs.Count)
//             {
//                 index = 0;
//             }

//             SetColor(destTexs[index]);
//             index++;
//         }
//     }
// }
