using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 沿路径记录飞机编号，然后顺序通过texture上色
/// </summary>
public class PathLight : MonoBehaviour
{
    public float interval = 0.2f;
    public float showColorTime;
    public float delayTime;
    public int showCount;
    public Color targetColor;
    public string[] names;
    public bool loop;

    private ColorMapping colorMapping;
    private float timer = 0f;
    private float delayTimer = 0f;
    private int index;
    private Transform child;
    private List<ColorPoint>curChilds;

    private void Awake()
    {
        colorMapping = GetComponent<ColorMapping>();
        curChilds=new List<ColorPoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= names.Length)
        {
            if(loop)
            index=0;
            else
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
            curChilds.Clear();
            for(int i=0;i<showCount;i++)
            {
                curChilds.Add(transform.Find(names[index+i]).GetComponent<ColorPoint>());
            }
            if (curChilds.Count!=0)
            {
                foreach (var child in curChilds)
                {
                    child.GradualColor(child.mappingColor,showColorTime);
                    index++;
                }
                    //Debug.LogErrorFormat("ColorMapping script is NOT found on gameObject: {0}", name);
                //}
            }
            else
            {
                Debug.LogErrorFormat("Child is NOT found, name: {0}", names[index]);
            }
        }
    }
}
