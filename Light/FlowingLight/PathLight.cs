using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 沿路径记录飞机编号，然后顺序通过texture上色
/// </summary>
public class PathLight : MonoBehaviour
{
    public float interval = 0.2f;
    public float delayTime;
    public string[] names = { "26", "27", "28", "91", "92", "29", "30", "31", "32", "93", "94" };

    private ColorMapping colorMapping;
    private float timer = 0f;
    private float delayTimer = 0f;
    private int index;
    private Transform child;

    private void Awake()
    {
        colorMapping = GetComponent<ColorMapping>();
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= names.Length)
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
            child = transform.Find(names[index]);

            if (child)
            {
                if (colorMapping)
                {
                    colorMapping.SetColor(child);
                    index++;
                }
                else
                {
                    Debug.LogErrorFormat("ColorMapping script is NOT found on gameObject: {0}", name);
                }
            }
            else
            {
                Debug.LogErrorFormat("Child is NOT found, name: {0}", names[index]);
            }
        }
    }
}
