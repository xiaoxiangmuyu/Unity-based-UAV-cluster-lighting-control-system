using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlink : ChangingColor
{
    public float duration = 40f; // 闪烁时间
    public Color[] blinkColors = { Color.black, new Color(0.96f, 0.30f, 0.14f) };
    public float blinkDelayTime = 0f; // 闪烁前的等待时间
    public float blinkInterval = 0.5f; // 颜色变换间隔时间

    private bool isFinished = false; // 闪烁是否结束
    private float blinkTimer = 0f;
    private float intervalTimer = 0f;
    private float blinkDelayTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        Blink();
    }

    private void Blink()
    {
        if (isFinished)
        {
            return;
        }

        blinkDelayTimer += Time.deltaTime;

        if (blinkDelayTimer < blinkDelayTime)
        {
            return;
        }

        blinkTimer += Time.deltaTime;

        if (blinkTimer < duration)
        {
            intervalTimer += Time.deltaTime;

            if (intervalTimer >= blinkInterval)
            {
                intervalTimer = 0f;
                SetColor(blinkColors[Random.Range(0, blinkColors.Length)]);
            }
        }
        else
        {
            SetColor(Color.black); // 闪烁完毕全部飞机变黑，以便和下一个画面形成节奏感
            isFinished = true;
        }
    }
}
