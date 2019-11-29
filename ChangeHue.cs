// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ChangeHue : ChangingColor
// {
//     public float interval = 0.1f;
//     public float delayTime;

//     private float timer;
//     private float delayTimer = 0f;

//     // Update is called once per frame
//     void Update()
//     {
//         delayTimer += Time.deltaTime;

//         if (delayTimer < delayTime)
//         {
//             return;
//         }

//         timer += Time.deltaTime;

//         if (timer >= interval)
//         {
//             timer = 0f;
//             SetColorByHue();
//         }
//     }

//     private void SetColorByHue()
//     {
//         float h, s, v;
//         Color.RGBToHSV(mat.color, out h, out s, out v);
//         float newHue = ((h * 360 + 10) % 360) / 360; // hue范围是[0,360]/360，这里每次累加10
//         mat.color = Color.HSVToRGB(newHue, s, v);
//     }
// }
