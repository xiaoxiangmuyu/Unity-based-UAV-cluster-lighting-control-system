﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class RandomBlink : SerializedMonoBehaviour
{
    public int breathCount;
    public float interval = 1;
    public List<Transform> childs=new List<Transform>();
    public List<ColorOrderBase> colorOrders;
    void Awake()
    {
        if(childs.Count==0)
        AddChild(transform);
    }
    void Start()
    {
        StartCoroutine(RandomChild());
    }
    IEnumerator RandomChild()
    {
        while (true)
        {
            for (int i = 0; i < breathCount; i++)
            {
                int index = Random.Range(0, childs.Count);
                var point = childs[index].GetComponent<ColorPoint>();
                point.SetProcessType(colorOrders);
            }
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
                string name=tra.GetChild(i).name;
                if (!int.TryParse(name, out temp))
                    continue;
                childs.Add(tra.GetChild(i));
            }
            else
                AddChild(tra.GetChild(i));
        }
    }
}