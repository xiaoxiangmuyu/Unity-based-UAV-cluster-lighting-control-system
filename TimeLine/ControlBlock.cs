﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Events;
using System;
[CreateAssetMenu(menuName = "创建ControlBlock", fileName = "new_ControlBlock")]
public class ControlBlock : SerializedScriptableObject, IPlayableAsset
{
    #region  IPlayableAsset
    public double duration { get; }
    public IEnumerable<PlayableBinding> outputs { get; }
    #endregion
    [EnumToggleButtons]
    public OrderType orderType;
    [BoxGroup("控制块属性")]
    public float speed = 1;
    [BoxGroup("控制块属性")]
    [MinMaxSlider(0, "ObjMaxIndex", true)]
    public Vector2 workRange;
    [BoxGroup("控制块属性")]
    public bool isflip;
    [BoxGroup("控制块属性")]
    public bool forceMode = true;
    [BoxGroup("控制块属性")]
    public bool timeInit;



    #region Record
    bool needProcess;
    [BoxGroup("数据处理模块")]
    [LabelText("是否动态计算位置")]
    public bool isDynamicPos;
    [BoxGroup("数据处理模块")]
    [LabelText("是否动态执行命令")]
    public bool isDynamicBehavior;
    [LabelText("处理次数")]
    [BoxGroup("数据处理模块")]
    [ShowIf("isDynamicPos")]
    public int processTimes;
    [BoxGroup("数据处理模块")]
    [OnValueChanged("Register")]
    public RecordData data = new RecordData();
    [BoxGroup("数据处理模块")]
    [OnValueChanged("Register")]
    public IDataProcesser processer;
    [BoxGroup("数据处理模块")]
    public IListOrderProcesser listOrderProcesser;
    #endregion
    [ShowInInspector]
    [PropertyOrder(1)]
    [LabelText("总用时")]
    public double totalTime { get { return GetDuring(); } }
    [Range(0, 1)]
    [LabelText("执行可能性")]
    [PropertyOrder(2)]
    public float possibility = 1;
    [ListDrawerSettings(Expanded = true)]
    [PropertyOrder(3)]
    [OnValueChanged("SetColorIndex")]
    public List<ColorOrderBase> colorOrders = new List<ColorOrderBase>();
    [HideInInspector]
    public List<GameObject> objs;
    [ValueDropdown("availableFilter")]
    [BoxGroup("数据读取模块")]
    //[OnValueChanged("SetColor")]
    public string groupFilter;
    [ValueDropdown("availableData")]
    [OnValueChanged("RefreshData")]
    [BoxGroup("数据读取模块")]
    public string targetDataName;

    IEnumerable availableData
    {
        get
        {
            List<string> dataNames = new List<string>();
            if (groupFilter == null || groupFilter.Equals(""))
                return dataNames;
            var datalist = ProjectManager.GetDataGroupByGroupName(groupFilter);
            foreach (var data in datalist.recordDatas)
            {
                if (data.groupName == null || data.groupName == String.Empty)
                    continue;
                dataNames.Add(data.dataName);
            }
            return dataNames;
        }
    }
    IEnumerable availableFilter
    {
        get
        {
            return ProjectManager.availableGroups;
        }
    }
    // public BlockState state
    // {
    //     get
    //     {
    //         if (this.data == null)
    //             return BlockState.NoData;
    //         var data = ProjectManager.GetDataGroupByGroupName(this.data.groupName);
    //         if (data.recordDatas.Exists((a) => a.dataName.Equals(this.data.dataName)))
    //         {
    //             var objNames = data.recordDatas.Find((a) => a.dataName.Equals(this.data.dataName)).objNames;
    //             if (objNames.Count != this.data.objNames.Count || objs.Exists(a => a == null) || objs.Exists(a => !a.activeInHierarchy))
    //             {
    //                 return BlockState.NeedRefresh;
    //             }
    //         }
    //         else
    //         {
    //             return BlockState.NoData;
    //         }
    //         return BlockState.Ready;
    //     }
    // }
    public Color blockColor
    {
        get
        {

            // switch (state)
            // {
            //     case BlockState.NeedRefresh:
            //         return Color.yellow;
            //     case BlockState.NoData:
            //         return Color.red;
            //     case BlockState.Ready:
            //         return data.GetGroupColor();
            //     default:
            //         return Color.red;
            // }
            return data.GetGroupColor();
        }
    }
    int ObjMaxIndex
    {
        get
        {
            if (data == null)
            {
                return 0;

            }
            else if (data.objNames != null)
            {
                return data.objNames.Count - 1;
            }
            return 0;

        }
    }
    ControlBehavior behavior;
    public Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // if (!Application.isPlaying)
        //     return ScriptPlayable<ControlBehavior>.Create(graph);
        //recordGroup = ProjectManager.Instance.RecordProject.RecordDic[owner.name];
        behavior = new ControlBehavior();
        behavior.record = this;
        behavior.GraphParent = owner;
        var scriptPlayable = ScriptPlayable<ControlBehavior>.Create(graph, behavior);
        if (workRange == Vector2.zero)
            workRange = new Vector2(0, ObjMaxIndex);
        return scriptPlayable;

    }
    public double GetDuring()
    {
        //if (processer is VirusProcesser && data.objNames.Count != 0)
        //{
        if (data.times.Count != 0 && colorOrders != null)
            return MyTools.GetTotalTime(colorOrders) + data.times[data.times.Count - 1];
        return 3;
        //}
        //return MyTools.GetTotalTime(colorOrders) + data.animTime;
    }
    // [Button(ButtonSizes.Large)]
    // void ReadOrderFile(OrderData orderData)
    // {
    //     colorOrders.Clear();
    //     foreach (var order in orderData.colorOrders)
    //     {
    //         colorOrders.Add(order);
    //     }
    //     Debug.Log("!");
    // }

    [BoxGroup("数据处理模块")]
    [Button(ButtonSizes.Large), GUIColor("GetColor")]
    //[ShowIf("needProcess")]
    public void ProcessData()
    {
        if (processer == null)
            return;
        //if (processer is VirusProcesser)
        //{
        //    RefreshData();
        //    //return;
        //}
        if (processer.Process(ref data, data.animTime))
        {

            needProcess = false;
            if (listOrderProcesser != null)
                ProcessList();
        }
    }
    [BoxGroup("数据处理模块")]
    [Button(ButtonSizes.Large)]
    public void ProcessList()
    {
        listOrderProcesser.ProcessOrder(data, processer.easeType);
        FindPoints();
        SetWorkRangeMax();
    }
    Color GetColor()
    {
        if (needProcess)
            return Color.red;
        else
            return Color.green;
    }

    //刷新数据，重新从data建立索引
    public void RefreshData()
    {
        RecordData result = new RecordData();
        var dataList = ProjectManager.GetDataGroupByGroupName(groupFilter);
        if (dataList == null)
        {
            Debug.LogError("Group Filter不合法");
            return;
        }
        foreach (var data in dataList.recordDatas)
        {
            if (data.dataName.Equals(targetDataName))
            {
                if (groupFilter != null || groupFilter != string.Empty)
                {
                    if (ProjectManager.GetGlobalPosInfoByGroup(data.groupName).groupName.Equals(groupFilter))
                    {
                        result = data;
                        break;
                    }
                }
            }
        }
        if (result != null)
        {
            data.CopyFrom(result);
            objs = new List<GameObject>();
            Register();
            SetWorkRangeMax();
            //bool temp = processer is VirusProcesser;
            //if (!temp)
            ProcessData();
        }
        else
        {
            Debug.LogError("没有找到该数据:" + targetDataName);
        }
    }
    public void SetWorkRangeMax()
    {
        workRange.y = ObjMaxIndex;
    }
    public void Register()
    {
        if (data != null)
            data.AddListener(BtnSwitch);
        if (processer != null)
        {
            processer.AddValueChangeListener(BtnSwitch);
            processer.AddProcessCompleteListener(FindPoints);
        }
        if (objs == null || objs.Count == 0 || objs.Exists((a) => a == null) || objs.Exists((a) => !data.objNames.Contains(a.name)))
            FindPoints();
        //Debug.Log("注册完成");
    }
    void BtnSwitch()
    {
        if (processer != null)
            needProcess = true;
    }
    public void FindPoints()
    {
        if (data == null)
            return;
        objs = new List<GameObject>();
        GameObject parent = GameObject.Find(ProjectManager.GetPointsRoot().gameObject.name);
        if (parent == null)
        {
            Debug.LogError("没有找到父物体 " + ProjectManager.GetPointsRoot().gameObject.name);
            return;
        }
        if (data.objNames != null)
        {
            foreach (var name in data.objNames)
            {
                FindChild(parent.transform, name);
                if (!tempObj)
                    Debug.LogError("没有找到" + name);
                objs.Add(tempObj.gameObject);
            }
        }
        //Debug.Log("Find GameObjects");
    }
    Transform tempObj;
    void FindChild(Transform tran, string childName)
    {
        Transform target = tran.Find(childName);
        if (target)
        {
            tempObj = target;
            return;
        }
        else
        {
            for (int i = 0; i < tran.childCount; i++)
            {
                FindChild(tran.GetChild(i), childName);
            }
        }
    }
    //[ShowInInspector]
    //[ValueDropdown("availableNames")]
    //[HorizontalGroup("SetColorGroup")]
    //[LabelText("颜色分组")]
    //string colorGroupName;
    IEnumerable availableNames
    {
        get
        {
            return ProjectManager.availableGroups;
        }
    }
    //[Button("设置全部颜色序号")]
    //[HorizontalGroup("SetColorGroup")]
    public void SetColorIndex()
    {
        SetColorGroup(colorOrders);
        //ConsoleProDebug.LogToFilter("设置颜色序号成功", "Log");
    }
    public void SetDefaultGroup()
    {
        if (groupFilter == null)
        {
            groupFilter = ProjectManager.Instance.RecordProject.current.groupName;
        }
    }
    void SetColorGroup(List<ColorOrderBase> orders)
    {
        foreach (var order in orders)
        {
            if (order is DoColor)
            {
                var temp = order as DoColor;
                if (temp.colorType == ColorType.MappingData)
                {
                    //if (temp.colorGroupName == null || temp.colorGroupName == "MappingData")
                    if (!temp.isManualMode)
                        temp.colorGroupName = groupFilter;
                }
            }
            else if (order is OrderGroup)
            {
                var temp = order as OrderGroup;
                SetColorGroup(temp.colorOrders);
            }
        }

    }

}


