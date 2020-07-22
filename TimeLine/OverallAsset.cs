using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
[CreateAssetMenu(menuName = "创建OverAllControlAsset", fileName = "新动画序列")]
public class OverallAsset : SerializedScriptableObject, IPlayableAsset
{
    #region  IPlayableAsset
    public double duration { get; }
    public IEnumerable<PlayableBinding> outputs { get; }
    #endregion
    public string targetName;


    [LabelText("执行次数")]
    public int processTimes=100;
    [LabelText("执行间隔")]
    public float processInterval=0.01f;
    [LabelText("执行的子物体个数")]
    [PropertyRange(0, "childCount")]
    public int ChildCount=10;
    [ReadOnly]
    [ShowInInspector]
    List<Transform> childs = new List<Transform>();
    [EnumToggleButtons]
    public OrderType orderType;
    [ShowIf("useOrderFile")]
    public OrderData orderData;
    public List<ColorOrderBase> ColorOrders { get { if (useOrderFile) return orderData.colorOrders; else return colorOrders; } }
    [HideIf("useOrderFile")]
    public List<ColorOrderBase> colorOrders = new List<ColorOrderBase>();

    bool useOrderFile { get { return orderType == OrderType.OrderFile; } }
    float timer;
    int childCount { get { return childs.Count; } }
    int _processTimes;

    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var scriptPlayable = ScriptPlayable<OverallBehavior>.Create(graph);
        scriptPlayable.GetBehaviour().script = this;
        scriptPlayable.GetBehaviour().GraphParent = owner;
        // if(targetName!=string.Empty&&targetName!="")
        // if(childs==null||childs.Count==0)
        // GetChilds();
        return scriptPlayable;
    }
    public double GetDuring()
    {
        if (processTimes * processInterval + MyTools.GetTotalTime(ColorOrders) == 0)
            return 3;
        else
            return processTimes * processInterval + MyTools.GetTotalTime(ColorOrders);
    }
    public void RefreshObjs()
    {
        if(childs==null||childs.Count==0||childs.Exists((a)=>a==null))
        GetChilds();
    }
    public void Begin()
    {
        Reset();
        ProjectManager.Instance.GetComponent<MonoBehaviour>().StartCoroutine(WholeProcess());
    }
    void SetOrders(List<ColorOrderBase> orders)
    {
        if (ChildCount < childs.Count)
            RandomChild(orders);
        else
            ControlAll(orders);
    }
    void ControlAll(List<ColorOrderBase> orders)
    {
        for (int i = 0; i < childs.Count; i++)
        {
            var point = childs[i].GetComponent<ColorPoint>();
            point.SetProcessType(orders);
        }
    }
    void RandomChild(List<ColorOrderBase> orders)
    {
        for (int i = 0; i < ChildCount; i++)
        {
            int index = Random.Range(0, childs.Count);
            var point = childs[index].GetComponent<ColorPoint>();
            point.SetProcessType(orders);
        }
    }
    IEnumerator WholeProcess()
    {
        while (_processTimes < processTimes)
        {
            SetOrders(ColorOrders);
            _processTimes += 1;
            yield return new WaitForSeconds(processInterval);
        }
    }
    [Button(ButtonSizes.Large)]
    void GetChilds()
    {
        if (childs != null && childs.Count != 0)
            childs.Clear();
        AddChild(GameObject.Find(targetName).transform);
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
    void Reset()
    {
        _processTimes = 0;
        timer = 0;
    }
}
