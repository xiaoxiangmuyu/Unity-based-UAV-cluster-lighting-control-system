using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class OverallControl : SerializedMonoBehaviour
{
    [LabelText("开始时间")]
    public float beginTime;
    [LabelText("呼吸次数")][Tooltip("执行一整组命令算一次")]
    public int breathTimes;
    [LabelText("执行的子物体个数")][PropertyRange(0,"childCount")]
    public int breathChildCount;
    public float interval = 1;
    public List<Transform> childs = new List<Transform>();
    public List<ColorOrderBase> colorOrders;


    
    float timer;
    bool isBegin;
    int breathTime;
    int childCount{get{return childs.Count;}}
    void Awake()
    {
        if (childs.Count == 0)
            AddChild(transform);
    }
    void Start()
    {
        //StartCoroutine(RandomChild());
    }
    void Update()
    {
        if (isBegin)
            return;
        if (timer < beginTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            isBegin = true;
            Debug.Log("overallControl begin");
            if(breathChildCount<childs.Count)
            StartCoroutine(RandomChild());
            else
            StartCoroutine(ControlAll());
        }
    }
    IEnumerator RandomChild()
    {
        while (breathTime < breathTimes)
        {
            for (int i = 0; i < breathChildCount; i++)
            {
                int index = Random.Range(0, childs.Count);
                var point = childs[index].GetComponent<ColorPoint>();
                point.SetProcessType(colorOrders);
            }
            breathTime += 1;
            yield return new WaitForSeconds(interval);
        }
    }
    IEnumerator ControlAll()
    {
        while (breathTime < breathTimes)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                var point = childs[i].GetComponent<ColorPoint>();
                point.SetProcessType(colorOrders);
            }
            breathTime += 1;
            yield return new WaitForSeconds(interval);
        }
    }
    [Button(ButtonSizes.Large)]
    void GetChilds()
    {
        if (childs != null && childs.Count != 0)
            childs.Clear();
        AddChild(transform);
        childs.Sort((a, b) => int.Parse(a.name) - int.Parse(b.name));
    }
    void AddChild(Transform tra)
    {
        for (int i = 0; i < tra.childCount; i++)
        {
            if (tra.GetChild(i).childCount == 0)
            {
                int temp;
                string name = tra.GetChild(i).name;
                if (!int.TryParse(name, out temp))
                    continue;
                childs.Add(tra.GetChild(i));
            }
            else
                AddChild(tra.GetChild(i));
        }
    }
}
