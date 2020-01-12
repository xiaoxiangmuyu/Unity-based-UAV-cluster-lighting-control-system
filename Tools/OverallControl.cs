using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class OverallControl : SerializedMonoBehaviour
{
    [LabelText("开始时间")]
    [HideIf("eventDrive")]
    public float beginTime;
    public bool eventDrive;
    public int processTimes;
    public float processInterval;
    [LabelText("执行的子物体个数")]
    [PropertyRange(0, "childCount")]
    public int breathChildCount;
    [SerializeField][ReadOnly]
    List<Transform> childs = new List<Transform>();
    [EnumToggleButtons]
    public OrderType orderType;
    [ShowIf("useOrderFile")]
    public OrderData orderData;
    public List<ColorOrderBase> ColorOrders{get{if(useOrderFile)return orderData.colorOrders;else return colorOrders;}}
    [HideIf("useOrderFile")]
    public List<ColorOrderBase>colorOrders;

    bool useOrderFile{get{return orderType==OrderType.OrderFile;}}
    float timer;
    bool isBegin;
    int childCount { get { return childs.Count; } }
    int _processTimes;
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
        if (isBegin || eventDrive)
            return;
        if (timer < beginTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            isBegin = true;
            Debug.Log("overallControl begin");
            BeginCoroutine();
            //SetOrders(colorOrders);
        }
    }
    IEnumerator WholeProcess()
    {
        while(_processTimes<processTimes)
        {
            SetOrders(ColorOrders);
            _processTimes+=1;
            yield return new WaitForSeconds(processInterval);
        }
    }
    public void BeginWithOrder(List<ColorOrderBase> orders)
    {
        SetOrders(orders);
    }
    public void BeginWithFile(OrderData file)
    {
        SetOrders(file.colorOrders);
    }
    public void BeginWithSelf()
    {
        SetOrders(ColorOrders);
    }
    public void BeginCoroutine()
    {
        StartCoroutine(WholeProcess());
    }
    public void Test()
    {
        Debug.Log("!!");
    }
    void SetOrders(List<ColorOrderBase> orders)
    {
        if (breathChildCount < childs.Count)
            RandomChild(orders);
        else
            ControlAll(orders);
    }
    void RandomChild(List<ColorOrderBase> orders)
    {
        for (int i = 0; i < breathChildCount; i++)
        {
            int index = Random.Range(0, childs.Count);
            var point = childs[index].GetComponent<ColorPoint>();
            point.SetProcessType(orders);
        }
    }
    void ControlAll(List<ColorOrderBase> orders)
    {
        for (int i = 0; i < childs.Count; i++)
        {
            var point = childs[i].GetComponent<ColorPoint>();
            point.SetProcessType(orders);
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
