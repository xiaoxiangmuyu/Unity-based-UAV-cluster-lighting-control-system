using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;
using DG.Tweening;
// A behaviour that is attached to a playable
public class LightControlBehavior : PlayableBehaviour
{
    public List<ColorOrderBase> orders;
    public LightControlAsset record;
    public ScriptPlayable<LightControlBehavior> scriptPlayable;
    public GameObject GraphParent;

    List<GameObject> objs;
    List<float> times;
    List<bool> hasProcess;
    float timer;
    bool hasInit;
    bool needResetState { get { return hasProcess.Exists((x) => x == true); } }
    Vector2 workRange { get { return record.workRange; } }

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        Debug.Log("OnGraphStart");
        MyTools.UpdateDuring(GraphParent);
        if (!hasInit)
            Init();
        // if (needResetState)
        //     ResetState();
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {

    }
    public override void OnPlayableCreate(Playable playable)
    {

    }
    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if(!Application.isPlaying)
        return;
        Debug.Log("OnBehaviourPlay");
        if (needResetState)
            ResetState();
        // if(!Application.isPlaying)
        // return;
        // objs.RemoveAt(0);
        // times.RemoveAt(0);
        //objs[0].GetComponent<ColorPoint>().SetProcessType(orders);


    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {

    }
    public override void OnPlayableDestroy(Playable playable)
    {

        //Debug.Log(scriptPlayable.GetHashCode() + "被销毁");
    }
    // Called each frame while the state is set to Play
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!Application.isPlaying)
            return;
        DOTween.ManualUpdate(0.04f, 0.04f);
        timer += Time.deltaTime * record.speed;
        if (objs.Count == 0 || times.Count == 0)
        {
            Debug.Log("执行物体为空");
            return;
        }
        Process(record.isflip);

    }
    void Process(bool isflip)
    {
        int counter=0;
        int timeIndex=0;
        if (!isflip)
        {
            for (int i = (int)workRange.x; i <= (int)workRange.y; i++)
            {
                if (i > objs.Count - 1)
                {
                    i = i - objs.Count;
                }
                if (record.timeInit)
                    timeIndex = counter;
                else
                    timeIndex = i;
                if (timer >= times[timeIndex] && hasProcess[i] == false)
                {
                    objs[i].GetComponent<ColorPoint>().SetProcessType(orders, record.forceMode);
                    hasProcess[i] = true;
                }
                counter += 1;
            }
        }
        else
        {
            for (int i = (int)workRange.y; i >= (int)workRange.x; i--)
            {
                if (i < 0)
                {
                    i = i + objs.Count;
                }
                if (record.timeInit)
                    timeIndex = counter;
                else
                    timeIndex = i;
                if (timer >= times[timeIndex] && hasProcess[i] == false)
                {
                    objs[i].GetComponent<ColorPoint>().SetProcessType(orders, record.forceMode);
                    hasProcess[i] = true;
                }
                counter += 1;
            }
        }
        // if (!hasProcess.Exists((x) => x == false))
        //     isFinish = true;
    }
    void Init()
    {
        if(record.data.ObjNames==null)
        return;
        objs = new List<GameObject>();
        times = new List<float>();
        hasProcess = new List<bool>();
        GameObject parent = GameObject.Find(record.data.parentName);
        if(parent==null)
        Debug.LogError("没有找到父物体 "+record.data.parentName);
        foreach (var name in record.data.ObjNames)
        {
            FindChild(parent.transform, name);
            if (!tempObj)
                Debug.LogError("没有找到" + name);
            objs.Add(tempObj.gameObject);
            hasProcess.Add(false);
        }
        foreach (var time in record.data.times)
            times.Add(time);
        hasInit = true;
    }
    void ResetState()
    {
        for (int i = 0; i < hasProcess.Count; i++)
        {
            hasProcess[i] = false;
        }
        timer = 0;
        ProjectManager.ResetAllColorAndTween();
        Debug.Log("ResetState");
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
}
