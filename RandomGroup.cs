using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class RandomGroup : MonoBehaviour
{
    public List<OverallControl>Groups;
    List<bool>isWork;
    public float beginTime;
    public float waitSeconds;
    float timer;
    bool isBegin;
    void Update()
    {
        if(!isBegin)
        {
            timer+=Time.deltaTime;
            if(timer>=beginTime)
            {
                isWork=new List<bool>();
                for(int i=0;i<Groups.Count;i++)
                {
                    isWork.Add(false);
                }
                isBegin=true;
                StartCoroutine(Process());
            }
        }

    }
    int preIndex;
    IEnumerator Process()
    {
        while(true)
        {
            int index=Random.Range(0,Groups.Count);
            if(!isWork[index]&&preIndex!=index)
            {
                Groups[index].BeginWithSelf();
                isWork[index]=true;
                yield return new WaitForSeconds(waitSeconds);
                isWork[index]=false;
                preIndex=index;
            }
        }
    }


}
