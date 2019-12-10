using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于路径中飞机编号连续且有序时，沿路径上色
/// </summary>
public class LineLight : MonoBehaviour
{
    public float interval = 0.2f;
    public int startIndex;
    public int endIndex;
    public float delayTime;
    public bool singleColor = false; // 是否使用单一颜色上色
    public Color targetColor = Color.white; // 指定使用的单色

    private TextureMapping colorMapping;
    private float timer = 0f;
    private float delayTimer = 0f;
    private int index;
    private Transform child;

    private void Awake()
    {
        if (!singleColor)
        {
            colorMapping = GetComponent<TextureMapping>();
        }

        index = startIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (index > endIndex)
        {
            return;
        }

        delayTimer += Time.deltaTime;

        if (delayTimer < delayTime)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            child = transform.Find(index.ToString());

            if (child)
            {
                if (singleColor)
                {
                    child.GetComponent<Renderer>().material.color = targetColor;
                }
                else
                {
                    if (colorMapping)
                    {
                        //colorMapping.SetColor(child,1);
                    }
                }

                index++;
            }
            else
            {
                Debug.LogErrorFormat("Child is NOT found, name: {0}", index);
            }
        }
    }
}
