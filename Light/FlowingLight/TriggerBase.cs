using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class TriggerBase : SerializedMonoBehaviour
{
    
    [SerializeField][ShowInInspector]
    private bool hideDataOperation=true;
    public List<string> filterTags=new List<string>(); // 影响的飞机的标签
    [LabelText("命令序列")][BoxGroup("MainArea")]
    public List<ColorOrderBase>colorOrders;

    protected virtual void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    [Button(ButtonSizes.Gigantic)][HideIf("hideDataOperation")]
    public void ReadOrderData(OrderData data)
    {
        if(!data)
        {
            Debug.LogError("存储文件为空");
            return ;
        }
        colorOrders.Clear();
        for(int i=0;i<data.colorOrders.Count;i++)
        {
            if(data.colorOrders[i] is DoColor)
            {
                DoColor temp=new DoColor();
                //temp=data.colorOrders[i] as DoColor;
                colorOrders.Add(temp);
            }
        }
        Debug.Log("读取成功");
    }
    [Button(ButtonSizes.Gigantic)][HideIf("hideDataOperation")]
    public void WriteData(OrderData data)
    {
        if(!data)
        {
            Debug.LogError("存储文件为空");
            return ;
        }
        data.colorOrders.Clear();
        foreach (var order in colorOrders)
        {
            data.colorOrders.Add(order);
        }
        Debug.Log("写入成功");

    }
}
