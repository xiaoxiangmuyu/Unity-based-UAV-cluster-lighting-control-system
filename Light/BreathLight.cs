using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathLight : ChangingColor
{
    public float fadeSpeed = 2f;
    public float changeMargin = 0.1f;
    public int playCount = 3;
    public float breathDelayTime;
    public bool defineColor; // 是否指定呼吸灯的颜色，“是”则targetColor属性生效；否则使用当前颜色
    public Color targetColor;
    public bool fadeIn = true; // 默认淡入，否则认为已上色且达到最大亮度，需要淡出

    private float h, s, v;
    private float tmpV;
    private float targetV;
    private float maxV = 1f;
    private float minV = 0f;
    private float breathDelayTimer = 0f;
    private bool isDone = false; // 是否取得了当前颜色

    protected override void Awake()
    {
        base.Awake();

        if (fadeIn)
        {
            targetV = maxV;
        }
        else
        {
            targetV = minV;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playCount <= 0)
        {
            return;
        }

        breathDelayTimer += Time.deltaTime;

        if (breathDelayTimer < breathDelayTime)
        {
            return;
        }

        if (!isDone)
        {
            if (defineColor)
            {
                mat.color = targetColor;
            }
            else // 从texture获取初始颜色
            {
                if (!colorMapping)
                {
                    Debug.LogErrorFormat("ColorMapping script is NOT found in parent, child name: {0}", name);
                    return;
                }

                Color color = colorMapping.GetColor(transform);

                if (color == Color.clear)
                {
                    return;
                }

                mat.color = color;
            }

            Color.RGBToHSV(mat.color, out h, out s, out v); // 呼吸前取当前颜色更准确，因为awake后还可能变色
            isDone = true;

            if (!fadeIn)
            {
                tmpV = v;
            }
        }

        tmpV = Mathf.Lerp(tmpV, targetV, fadeSpeed * Time.deltaTime);

        if (Mathf.Abs(targetV - tmpV) < changeMargin)
        {
            tmpV = targetV;

            if (targetV == maxV)
            {
                targetV = minV;
                playCount--;
            }
            else
            {
                targetV = maxV;
                //playCount--;
            }
        }

        SetColor(Color.HSVToRGB(h, s, tmpV));
    }
}
